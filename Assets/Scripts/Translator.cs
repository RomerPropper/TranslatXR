using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OVRSimpleJSON;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class Translator
{
    public static string Transcribe(string filePath, string targetLang) {
        return "";
    }

    public static async Task<string> Transcribe(AudioClip recordedClip, string targetLang) {
        //Temporary workaround;
        ConvertToWav(recordedClip);
        string filePath = Path.Combine(Application.persistentDataPath, "tempRecordedAudio.wav");

        using (var client = new HttpClient())
        {
            // Prepare the multipart form data
            var formContent = new MultipartFormDataContent();
            var audioContent = new ByteArrayContent(File.ReadAllBytes(filePath));
            audioContent.Headers.Add("Content-Type", "audio/wav");
            formContent.Add(audioContent, "audio_file", "tempRecordedAudio.wav");
            formContent.Add(new StringContent(targetLang), "source_lang");

            // Send the request
            var response = await client.PostAsync("https://translatxr.presidentialcorn.com/transcribe", formContent);
            Debug.Log(response);
            // Read and return the response content
            string responseString = await response.Content.ReadAsStringAsync();
            JObject jsonObject = JObject.Parse(responseString);
            return jsonObject["transcription"].ToString();
        }
    }

    public static async Task<string> Translate(string message, string srcLang, string targetLang) {
        using (var client = new HttpClient())
        {
            // Prepare the JSON data
            var requestData = new
            {
                text = message,
                source_lang = srcLang,
                target_lang = targetLang
            };

            // Serialize the data to JSON
            var jsonContent = JsonConvert.SerializeObject(requestData);
            var stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Send the request
            var response = await client.PostAsync("https://translatxr.presidentialcorn.com/translate/text", stringContent);

            // Check if the request was successful
            response.EnsureSuccessStatusCode();

            // Read and return the response content
            string responseString = await response.Content.ReadAsStringAsync();

            // TODO: Add emotion here
            var jsonObject = JObject.Parse(responseString);
            return jsonObject["translation"].ToString();
        }
    }

    public static void ConvertToWav(AudioClip audioClip)
    {

        string filePath = Path.Combine(Application.persistentDataPath, "tempRecordedAudio.wav");
        // Create a new empty WAV file
        FileStream fileStream = new FileStream(filePath, FileMode.Create);

        // Create a binary writer
        BinaryWriter writer = new BinaryWriter(fileStream);

        // Get the audio data from the AudioClip
        float[] samples = new float[audioClip.samples * audioClip.channels];
        audioClip.GetData(samples, 0);

        // Convert the audio data to bytes
        Int16[] intData = new Int16[samples.Length];
        Byte[] bytesData = new Byte[samples.Length * 2];
        int rescaleFactor = 32767;

        for (int i = 0; i < samples.Length; i++)
        {
            intData[i] = (short)(samples[i] * rescaleFactor);
            byte[] byteArr = new byte[2];
            byteArr = BitConverter.GetBytes(intData[i]);
            byteArr.CopyTo(bytesData, i * 2);
        }

        // Write the WAV file header
        writer.Write(new char[4] { 'R', 'I', 'F', 'F' });
        writer.Write((Int32)(36 + bytesData.Length));
        writer.Write(new char[4] { 'W', 'A', 'V', 'E' });
        writer.Write(new char[4] { 'f', 'm', 't', ' ' });
        writer.Write((Int32)16);
        writer.Write((Int16)1);
        writer.Write((Int16)audioClip.channels);
        writer.Write((Int32)audioClip.frequency);
        writer.Write((Int32)(audioClip.frequency * audioClip.channels * 2));
        writer.Write((Int16)(audioClip.channels * 2));
        writer.Write((Int16)16);
        writer.Write(new char[4] { 'd', 'a', 't', 'a' });
        writer.Write((Int32)bytesData.Length);

        // Write the audio data
        writer.Write(bytesData);

        // Close the writer and file stream
        writer.Close();
        fileStream.Close();
    }
}
