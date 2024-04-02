# Installing and Running
## Requirements
### Docker Install:
- Docker Container Engine or Docker Desktop (not tested)

### Local Install:
- Python 3.9 or later (earlier versons may work but they were not tested)

## Docker
Pull this repository and `cd` to `TranslateAPI/python`. Configure the environment as instructed below, then run `docker compose up -d`.

## From Source
Pull this repository and `cd` to `TranslateAPI/python`.
It is recommended that you create a virtual environment.
Create the virtual environment by running `python -m venv .venv`.
Activating the virtual environment is slightly different on Windows and Linux:
- Windows: `.\.venv\Scripts\activate` or `.\.venv\Scripts\activate.ps1`
- Linux: `. .venv/bin/activate`

Install the packages from `requirements-prod.txt`: `pip install -r requirements-prod.txt`
Install [PyTorch](https://pytorch.org/get-started/locally/)

Configure the environment as instructed below. then run `uvicorn server_prod:app --host 0.0.0.0`

# Configuring
Configure the relevant environment variables from the list below.

If you are running this via Docker, create a .env file or edit `docker-compose.yml` and configure the environment there.

If you are running this from source, create a .env vile or set the environment variables on your local system.

If you are using a cloud VPS, configure the environment from the cloud provider's dashboard.

Here is a summary of all the environment variables:
- `CLOUD_TRANSLATION_API`: The service used to for translation. You can choose between Google, OpenAI Whisper (for translating to English only), DeepL, and EasyNMT. Note that you will need to host your own EasyNMT instance.
- `CLOUD_TRANSCRIPTION_API`: The service used for transcription (speech-to-text). You can choose between Google and OpenAI Whisper
- `OPENAI_API_KEY`: Your OpenAI API key. You can get one from [here](https://platform.openai.com/docs/quickstart).
- `DEEPL_API_KEY`: Your DeepL API key. You can get one from [here](https://www.deepl.com/docs-api/api-access/authentication).
- `GOOGLE_APPLICATION_CREDENTIALS`: The path to your `client_secret.json` (or whatever you named it). You can generate this file [here](https://console.cloud.google.com/).
- `GOOGLE_PROJECT_ID`: Your Google Project ID. You can get this from your `client_secret.json` (hopefully this will automatically be extracted from `client_secret.json` later)
- `EASYNMT_TRANSLATION_URL`: If you are hosting an EasyNMT instance, you can supply the URL here. Be sure to point the translation endpoint (e.g. https://example.com/translate).
- `TRANSLATXR_TRUSTED_DOMAINS`: If you are hosting this over the Internet or through a reverse proxy, place the domain you intend to use here. Multiple domains are separated by commas.
