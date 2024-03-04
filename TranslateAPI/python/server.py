# TODO: remove references to offline services in production version of this file

from fastapi import FastAPI, HTTPException, Form, File, UploadFile
from typing import List, Optional, Annotated
from pydantic import BaseModel
import whisper
from dotenv import load_dotenv
import httpx
import os
import subprocess

load_dotenv()

==================================================================================

def offline_transcribe(audio_file, input_language="", whisper_model=""):
    if whisper_model == "" and input_language == "en":
        whisper_model = "small.en"
    elif whisper_model == "":
        whisper_model = "medium"
    model = whisper.load_model(whisper_model)
    result = model.transcribe(audio_file)
    transcribed_text = result["text"]
    return transcribed_text

def online_transcribe(audio_file, input_language="", whisper_model="whisper-1"):
    from openai import OpenAI
    client = OpenAI() # automatically loads 'OPENAI_API_KEY' from environment
    #audio = open(audio_file, "rb")
    audio = audio_file
    transcription = client.audio.transcriptions.create(
            language=input_language,
            model=whisper_model,
            file=audio_file,
            response_format="text"
            )
    transcribed_text = transcription.text
    return transcribed_text

def transcribe(audio_file, input_language="", model="", online=False):
    transcribed_text = ""
    if online:
        transcribed_text = online_transcribe(audio_file, input_language ,"whisper-1")
    else:
        transcribed_text = offline_transcribe(audio_file, input_language, model)
    return transcribed_text

==================================================================================

def offline_translate(text, input_language, output_language, language_model='opus-mt'):
    if output_language in ["en-us", "en-gb"]:
        print('WARN: English variant "{}" is unsupported by EasyNMT; using "{}" instead'.format(output_language, "en"))
        output_language = "en"
    model = EasyNMT(language_model)
    result = model.translate(text, source_lang=input_language, target_lang=output_language)
    #result = model.translate(text, source_lang="ja", target_lang="en")
    return text

def online_translate(text, input_language, output_language):
    translator = deepl.Translator(os.environ.get('DEEPL_API_KEY'))
    result = translator.translate_text(text, target_lang=output_language)
    return result.text

def translate(text, input_language, output_language, online=False):
    result = ""
    if online:
        result = online_translate(text, input_language, output_language)
    else:
        result = offline_translate(text, input_language, output_language)
    return result

==================================================================================

app = FastAPI()

# TODO: rename all this
transcription_api_url = os.environ.get('API_PROXY_TRANSCRIPTION_URL')
translation_api_url = os.environ.get('API_PROXY_TRANSLATION_URL')

@app.post("/translate")
#async def proxy_api(form: RequestForm):
async def proxy_api(
    audio_file: UploadFile,
    source_lang: Annotated[str, Form()],
    target_lang: Annotated[str, Form()],
    ):
    try:
        # Proxying transcription API
        #async with httpx.AsyncClient() as client:
        #    transcription_response = await client.post(transcription_api_url, json={"text": text})
        #    transcription_result = transcription_response.json()

        # Proxying translation API
        #async with httpx.AsyncClient() as client:
        #    translation_response = await client.post(translation_api_url, json={"text": transcription_result["transcription"]})
        #    translation_result = translation_response.json()

        # TODO: replace this with http call to another remote server (see comment examples above/below as well as live example below)
        unique_filename = f"uploaded_{audio_file.filename}"
        with open(unique_filename, "wb") as f:
            f.write(audio_file.file.read())
        transcription_result = transcribe(unique_filename, source_lang)
         
        # Proxying translation API
        async with httpx.AsyncClient() as client:
            translation_response = await client.post(translation_api_url, json={"text": transcription_result, "source_lang": source_lang, "target_lang":target_lang})
            translation_result = translation_response.json()

        #async with httpx.AsyncClient() as client:
        #    transcription_response = await client.post(transcription_api_url, json={"text": text})
        #    transcription_result = transcription_response.json()
        #    translation_response = await client.post(translation_api_url, json={"text": transcription_result["transcription"]})
        #    translation_result = translation_response.json()

        #return {"transcription": transcription_result["transcription"], "translation": translation_result["translation"]}
        return {"transcription": transcription_result, "translation": translation_result["translated"]}

    except httpx.HTTPError as e:
        # Handle HTTP errors from remote servers
        raise HTTPException(status_code=e.response.status_code, detail=f"Error from remote server: {e}")

if __name__ == "__main__":
    import uvicorn

    uvicorn.run(app, host="0.0.0.0", port=8000)
