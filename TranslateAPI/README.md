# Installing
## Docker
Under construction...
## From Source
Under construction...

# Configuring
Configure the relevant environment variables from the list below.
Do this either by creating a .env file or by setting the environment on your local system or,
if you're using a cloud VPS, the cloud provider's dashboard.

Here is a summary of all the environment variables:
- `CLOUD_TRANSLATION_API`: The service used to for translation. You can choose between Google, OpenAI Whisper (for translating to English only), DeepL, and EasyNMT. Note that you will need to host your own EasyNMT instance.
- `CLOUD_TRANSCRIPTION_API`: The service used for transcription (speech-to-text). You can choose between Google and OpenAI Whisper
- `OPENAI_API_KEY`: Your OpenAI API key. You can get one from [here](https://platform.openai.com/docs/quickstart).
- `DEEPL_API_KEY`: Your DeepL API key. You can get one from [here](https://www.deepl.com/docs-api/api-access/authentication).
- `GOOGLE_APPLICATION_CREDENTIALS`: The path to your `client_secret.json`. You can generate this file [here](https://console.cloud.google.com/).
- `EASYNMT_TRANSLATION_URL`: If you are hosting an EasyNMT instance, you can supply the URL here. Be sure to point the translation endpoint (e.g. https://example.com/translate).
- `TRANSLATXR_TRUSTED_DOMAINS`: If you are hosting this over the Internet or through a reverse proxy, place the domain you intend to use here.
