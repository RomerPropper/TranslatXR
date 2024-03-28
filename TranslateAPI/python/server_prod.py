# TODO: test Google API

from fastapi import FastAPI, HTTPException, Form, File, UploadFile
from fastapi.middleware.cors import CORSMiddleware
from typing import List, Optional, Annotated, Dict
from pydantic import BaseModel
from openai import OpenAI
from google.cloud import speech
from google.cloud import translate_v2 as translate
import deepl
from dotenv import load_dotenv
from transformers import pipeline
import httpx
import os
import subprocess
import secrets

load_dotenv()

class TranslateTextBody(BaseModel):
    text: str
    target_lang: str
    source_lang: str | None = None

# ==================================================================================

# Google APIs
# https://cloud.google.com/translate/docs/basic/translating-text
async def google_translate(text, source_lang, target_lang):
    translate_client = translate.Client()
    result = translate_client.translate(values=text, format_="text", source_language=source_lang, target_language=target_lang)
    return result["translatedText"]

# https://cloud.google.com/speech-to-text/docs/sync-recognize#speech-sync-recognize-python
async def google_transcribe(audio_file, source_lang):
    client = speech.SpeechClient()

    with open(audio_file, "rb") as file:
        content = file.read()

    audio = speech.RecognitionAudio(content=content)
    config = speech.RecognitionConfig(
        encoding=speech.RecognitionConfig.AudioEncoding.LINEAR16,
        sample_rate_hertz=16000,
        language_code="en-US",
    )

    response = client.recognize(config=config, audio=audio)
    final_result = ""
    # Each result is for a consecutive portion of the audio. Iterate through
    # them to get the transcripts for the entire audio file.
    for result in response.results:
        # The first alternative is the most likely one for this portion.
        final_result += result.alternatives[0].transcript

    return final_result

# Whisper APIs
async def whisper_transcribe(audio_file, source_lang):
    client = OpenAI() # automatically loads 'OPENAI_API_KEY' from environment
    audio = open(audio_file, "rb")
    transcription = client.audio.transcriptions.create(
            language=source_lang,
            model="whisper-1",
            file=audio,
            response_format="text"
            )
    transcribed_text = transcription.text
    return transcribed_text

async def whisper_translate(audio_file, source_lang, target_lang):
    client = OpenAI() # automatically loads 'OPENAI_API_KEY' from environment
    audio = open(audio_file, "rb")
    transcription = client.audio.transcriptions.create(
            #language=source_lang, # NOTE: this doesn't exist on the translation endpoint
            model="whisper-1",
            file=audio,
            response_format="text"
            )
    transcribed_text = transcription.text
    return transcribed_text

# DeepL APIs
async def deepl_translate(text, source_lang, target_lang):
    translator = deepl.Translator(os.environ.get('DEEPL_API_KEY'))
    result = translator.translate_text(text, target_lang=target_lang)
    return result.text

# EasyNMT APIs
async def easynmt_translate(text, source_lang, target_lang):
    try:
        if source_lang == "en-us":
            source_lang = "en"
        async with httpx.AsyncClient() as client:
            translation_api_url = os.environ.get('EASYNMT_TRANSLATION_URL')
            translation_response = ""
            if not source_lang:
                translation_response = await client.post(translation_api_url, json={"text": text, "target_lang": target_lang})
            else:
                translation_response = await client.post(translation_api_url, json={"text": text, "source_lang": source_lang, "target_lang": target_lang})
            result_json = translation_response.json()
            result = result_json["translated"]
            return result
    except httpx.HTTPError as e:
        raise # just rethrow

translate_function_dict = {
#    'google': google_translate,``
#    'whisper': whisper_translate,
#    'openai': whisper_translate,
    'deepl': deepl_translate,
    'easynmt': easynmt_translate
}

transcribe_function_dict = {
#    'google': google_transcribe,
#    'whisper': whisper_transcribe,
#    'openai': whisper_transcribe
}

# ==================================================================================

async def transcribe(key, *args, **kwargs):
    result = ""
    if key in transcribe_function_dict:
        result = await transcribe_function_dict[key](*args, **kwargs)
    else:
        raise KeyError(f"Function key '{key}' not found")
    return result

# ==================================================================================

async def translate(key, *args, **kwargs):
    result = ""
    if key in translate_function_dict:
        result = await translate_function_dict[key](*args, **kwargs)
    else:
        raise KeyError(f"Function key '{key}' not found")
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

# TODO: do not allow all origins; perhaps configure it from environment
# For testing, may need to update in the future when publishing
app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],  # Allows all origins
    allow_credentials=True,
    allow_methods=["*"],  # Allows all methods
    allow_headers=["*"],  # Allows all headers
)

classifier = pipeline("sentiment-analysis", model="michellejieli/emotion_text_classifier")

# TODO: prevent extraneous API calls on serives that directly support audio translation
@app.post("/translate/audio", tags=["translate"])
async def translate_audio(
    target_lang: Annotated[str, Form(description="Language to translate to")],
    source_lang: Annotated[Optional[str], Form(description="Language to translate from")] = None,
    audio_file: UploadFile = File(description="Audio file to translate"),
    ):
    try:
        #==================================================
        transcription_result = ""
        key = os.environ.get("CLOUD_TRANSCRIPTION_API")
        unique_filename = secrets.token_hex(nbytes=16)
        # TODO: surely we can just do this in memory...
        with open(unique_filename, "wb") as f:
            f.write(audio_file.file.read())
        transcription_result = await transcribe(key=key, audio_file=unique_filename, source_lang=source_lang)

        # delete the file we just created
        try:
            os.remove(unique_filename)
            print(f"File {unique_filename} deleted successfully.")
        except FileNotFoundError:
            print(f"File {unique_filename} not found.")
        except Exception as e:
            print(f"An error occurred: {e}")
        #==================================================

        key = os.environ.get("CLOUD_TRANSLATION_API")
        translation_result = await translate(key=key, text=transcription_result, source_lang=source_lang, target_lang=target_lang)

        return {"transcription": transcription_result, "translation": translation_result}

    except httpx.HTTPError as e:
        # Handle HTTP errors from remote servers
        raise HTTPException(status_code=e.response.status_code, detail=f"Error from remote server: {e}")

@app.post("/translate/text", tags=["translate"])
async def translate_text(body: TranslateTextBody):
    try:
        translation_result = ""
        key = os.environ.get("CLOUD_TRANSLATION_API")
        translation_result = await translate(key=key, text=body.text, source_lang=body.source_lang, target_lang=body.target_lang)
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
        transcription_result = ""
        key = os.environ.get("CLOUD_TRANSCRIPTION_API")
        unique_filename = secrets.token_hex(nbytes=16)
        with open(unique_filename, "wb") as f:
            f.write(audio_file.file.read())
        transcription_result = await transcribe(key=key, audio_file=unique_filename, source_lang=source_lang)

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


@app.post("/sentiment")
async def analyze_sentiment(request_data: Dict[str, str]):
    try:
        # Pull text from body
        text = request_data.get("text", "")
        if not text:
            raise ValueError("Text for analysis must be provided.")
        
        # Perform sentiment analysis
        result = classifier(text)
        return {"result": result}
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))
    
# Endpoint for quick set up test
@app.get("/test")
async def read_test():
    return {"message": "Test endpoint is working"}

if __name__ == "__main__":
    import uvicorn

    uvicorn.run(app, host="0.0.0.0", port=8000)
