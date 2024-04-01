# TODO: remove references to offline services (localhost, EasyNMT, whisper) in production version of this file
# TODO: remove all is_online parameters and just use the environment variables directly

from fastapi import FastAPI, HTTPException, Form, File, UploadFile
from fastapi.middleware.cors import CORSMiddleware
from typing import List, Optional, Annotated, Dict
from pydantic import BaseModel
import whisper
import deepl
from easynmt import EasyNMT
from dotenv import load_dotenv
from transformers import pipeline
import httpx
import os
import subprocess

load_dotenv()

class TranslateTextBody(BaseModel):
    text: str
    target_lang: str
    source_lang: str | None = None

class SentimentRequest(BaseModel):
    text: str

# ==================================================================================

def offline_transcribe(audio_file, source_lang="", whisper_model=""):
    if not whisper_model and source_lang == "en":
        whisper_model = "small.en"
    elif whisper_model == "":
        whisper_model = "medium"
    model = whisper.load_model(whisper_model)
    result = model.transcribe(audio_file)
    transcribed_text = result["text"]
    return transcribed_text

def online_transcribe(audio_file, source_lang="", whisper_model="whisper-1"):
    from openai import OpenAI
    client = OpenAI() # automatically loads 'OPENAI_API_KEY' from environment
    audio = open(audio_file, "rb")
    #audio = audio_file
    transcription = client.audio.transcriptions.create(
            language=source_lang,
            model=whisper_model,
            file=audio,
            response_format="text"
            )
    transcribed_text = transcription.text
    return transcribed_text

def transcribe(audio_file, source_lang="", model="", online=False):
    transcribed_text = ""
    if online:
        transcribed_text = online_transcribe(audio_file, source_lang ,"whisper-1")
    else:
        transcribed_text = offline_transcribe(audio_file, source_lang, model)
    return transcribed_text

# ==================================================================================

async def offline_translate(text, source_lang, target_lang, language_model='opus-mt', is_localhost=True):
    result = ""
    if is_localhost:
        if target_lang in ["en-us", "en-gb"]:
            print('WARN: English variant "{}" is unsupported by EasyNMT; using "{}" instead'.format(target_lang, "en"))
            target_lang = "en"
        model = EasyNMT(language_model)

        # TODO: figure out if this conditional is even needed
        if source_lang:
            result = model.translate(text, source_lang=source_lang, target_lang=target_lang)
        else:
            result = model.translate(text, target_lang=target_lang)
    else:
        try:
            async with httpx.AsyncClient() as client:
                translation_api_url = os.environ.get('API_PROXY_TRANSLATION_URL')
                translation_response = ""
                if not source_lang:
                    translation_response = await client.post(translation_api_url, json={"text": text, "target_lang": target_lang})
                else:
                    translation_response = await client.post(translation_api_url, json={"text": text, "source_lang": source_lang, "target_lang": target_lang})
                result_json = translation_response.json()
                result = result_json["translated"]
                #result = translation_response.json()
        except httpx.HTTPError as e:
            raise # just rethrow

    return result

def online_translate(text, source_lang, target_lang):
    translator = deepl.Translator(os.environ.get('DEEPL_API_KEY'))
    result = translator.translate_text(text, target_lang=target_lang)
    return result.text

async def translate(text, source_lang, target_lang, online=False, is_localhost=True):
    result = ""
    if online:
        result = online_translate(text, source_lang, target_lang)
    else:
        result = await offline_translate(text, source_lang, target_lang, is_localhost=is_localhost)
    return result

# ==================================================================================

tags_metadata = [
    {
        "name": "translate",
        "description": "Translate to target language",
    },
    {
        "name": "transcribe",
        "description": "Transcribe audio file",
    },
]

app = FastAPI(openapi_tags=tags_metadata)

# For testing, may need to update in the future when publishing
app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],  # Allows all origins
    allow_credentials=True,
    allow_methods=["*"],  # Allows all methods
    allow_headers=["*"],  # Allows all headers
)

classifier = pipeline("sentiment-analysis", model="michellejieli/emotion_text_classifier")
is_online = os.environ.get("API_MODE").lower() == "online"

@app.post("/translate/audio", tags=["translate"])
async def translate_audio(
    target_lang: Annotated[str, Form(description="Language to translate to")],
    source_lang: Annotated[Optional[str], Form(description="Language to translate from")] = None,
    audio_file: UploadFile = File(description="Audio file to translate"),
    ):
    try:
        #==================================================
        # TODO: hash the file/filename to avoid collisions
        transcription_result = ""
        unique_filename = f"uploaded_{audio_file.filename}"
        with open(unique_filename, "wb") as f:
            f.write(audio_file.file.read())
        transcription_result = transcribe(unique_filename, source_lang, online=is_online)

        
        # delete the file we just created
        try:
            os.remove(unique_filename)
            print(f"File {unique_filename} deleted successfully.")
        except FileNotFoundError:
            print(f"File {unique_filename} not found.")
        except Exception as e:
            print(f"An error occurred: {e}")
        #==================================================

        translation_result = await translate(transcription_result, source_lang, target_lang, online=is_online)

        return {"transcription": transcription_result, "translation": translation_result}

    except httpx.HTTPError as e:
        # Handle HTTP errors from remote servers
        raise HTTPException(status_code=e.response.status_code, detail=f"Error from remote server: {e}")

@app.post("/translate/text", tags=["translate"])
async def translate_text(body: TranslateTextBody):
    try:
        translation_result = ""
        translation_result = await translate(body.text, body.source_lang, body.target_lang, online=is_online)
        return {"translation": translation_result}
    except httpx.HTTPError as e:
        raise HTTPException(status_code=e.response.status_code, detail=f"Error from remote server: {e}")

@app.post("/transcribe", tags=["transcribe"])
async def transcribe_audio(
    source_lang: Annotated[Optional[str], Form(description="Language to transcribe")] = None,
    audio_file: UploadFile = File(description="Audio file to transcribe"),
    ):
    try:
        #==================================================
        # TODO: hash the file/filename to avoid collisions
        transcription_result = ""
        unique_filename = f"{audio_file.filename}"
        with open(unique_filename, "wb") as f:
            f.write(audio_file.file.read())
        transcription_result = transcribe(unique_filename, source_lang, online=is_online)

        
        # delete the file we just created
        try:
            os.remove(unique_filename)
            print(f"File {unique_filename} deleted successfully.")
        except FileNotFoundError:
            print(f"File {unique_filename} not found.")
        except Exception as e:
            print(f"An error occurred: {e}")
        #==================================================

        return {"transcription": transcription_result}
    
    except httpx.HTTPError as e:
        raise HTTPException(status_code=e.response.status_code, detail=f"Error from remote server: {e}")


@app.post("/sentiment", tags=["/sentiment"])
async def analyze_sentiment(request_data: Dict[str, str]):
    try:
        # Pull text from body
        text = request_data.text
        if not text:
            raise ValueError("Text for analysis must be provided.")
        
        # Perform sentiment analysis
        result = classifier(text)
        return {"result": result}
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))
    
# Endpoint for quick set up test
@app.get("/test", tags=["test"])
async def read_test():
    return {"message": "Test endpoint is working"}

# Endpoint for online checking
@app.get("/", tags=["root"])
async def read_test():
    return {"message": "API is online"}

if __name__ == "__main__":
    import uvicorn

    uvicorn.run(app, host="0.0.0.0", port=8000)
