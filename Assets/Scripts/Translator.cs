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
        // string apiUrl = "https://translatxr.presidentialcorn.com";
        string apiUrl = "http://localhost:8000";
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
            var response = await client.PostAsync(apiUrl + "/transcribe", formContent);
            Debug.Log(response);
            // Read and return the response content
            string responseString = await response.Content.ReadAsStringAsync();
            JObject jsonObject = JObject.Parse(responseString);
            return jsonObject["transcription"].ToString();
        }
    }

    public static async Task<string> Translate(string message, string srcLang, string targetLang) {
        // string apiUrl = "https://translatxr.presidentialcorn.com";
        string apiUrl = "http://localhost:8000";

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
            var response = await client.PostAsync(apiUrl + "/translate/text", stringContent);

            // Check if the request was successful
            response.EnsureSuccessStatusCode();

            // Read and return the response content
            string responseString = await response.Content.ReadAsStringAsync();

            // Once incoming has been translated to english
                // try {
                //     var sentimentResponseString = await Sentiment(new StringContent(responseString, Encoding.UTF8, "application/json"));
                //     Debug.Log($"Sentiment Response: {sentimentResponseString}");
                // } 
                //     catch (Exception ex) {
                //     Debug.LogError($"Sentiment method failed: {ex.Message}");
                // }
            // TODO: Add emotion here
            var jsonObject = JObject.Parse(responseString);
            return jsonObject["translation"].ToString();
        }
    }

    public static async Task<string> Sentiment(StringContent jsonPayload) {
        // string apiUrl = "https://translatxr.presidentialcorn.com";
        string apiUrl = "http://localhost:8000";
        
        using (var httpClient = new HttpClient())
        {
            var response = await httpClient.PostAsync(apiUrl + "/sentiment", jsonPayload);
            Debug.Log($"Response Status Code: {response.StatusCode}");

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                Debug.Log($"Sentiment Analysis Response: {responseString}");
                return responseString;
            }
            else
            {
                throw new HttpRequestException($"Error: {response.ReasonPhrase}");
            }
        }
    }

    public static async Task<string> Sentiment(string textPayload) {
        // string apiUrl = "https://translatxr.presidentialcorn.com";
        string apiUrl = "http://localhost:8000";
        
        using (var httpClient = new HttpClient()) {
            var jsonPayload = JsonConvert.SerializeObject(new { text = textPayload }); // Assuming you need to wrap the text in a JSON object
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            Debug.Log($"Sending Sentiment Analysis Request: {jsonPayload}");

            var response = await httpClient.PostAsync(apiUrl + "/sentiment", content);

            if (response.IsSuccessStatusCode) {
                var responseString = await response.Content.ReadAsStringAsync();
                Debug.Log($"Sentiment Analysis Response: {responseString}");
                return responseString;
            } else {
                var errorContent = await response.Content.ReadAsStringAsync();
                Debug.LogError($"Sentiment method failed with {response.StatusCode}: {errorContent}");
                throw new HttpRequestException($"Error: {response.ReasonPhrase}");
            }
        }
    }

    public static void ConvertToWav(AudioClip audioClip)
    {
        string filePath = Path.Combine(Application.persistentDataPath, "tempRecordedAudio.wav");
        
        using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
        {
            using (BinaryWriter writer = new BinaryWriter(fileStream))
            {
                float[] samples = new float[audioClip.samples * audioClip.channels];
                audioClip.GetData(samples, 0);

                Int16[] intData = new Int16[samples.Length];
                Byte[] bytesData = new Byte[samples.Length * 2];
                int rescaleFactor = 32767;

                for (int i = 0; i < samples.Length; i++)
                {
                    intData[i] = (short)(samples[i] * rescaleFactor);
                    byte[] byteArr = BitConverter.GetBytes(intData[i]);
                    byteArr.CopyTo(bytesData, i * 2);
                }

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
                writer.Write(bytesData);
                writer.Close();
            }
            fileStream.Close();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}
