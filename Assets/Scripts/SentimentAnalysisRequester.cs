using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

public class SentimentAnalysisRequester : MonoBehaviour
{
    private string apiUrl = "http://localhost:8000/sentiment";

    void Start()
    {
        StartCoroutine(PostSentiment("I can't wait for this project to be finished and ready for production"));
    }

    IEnumerator PostSentiment(string text)
    {
        string jsonPayload = "{\"text\":\"" + text + "\"}";
        byte[] jsonToSend = new UTF8Encoding().GetBytes(jsonPayload);

        // UnityWebRequest for posting the sentiment analysis
        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
        {
            // Set the body of the request
            request.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

            // Set Headers
            request.SetRequestHeader("Content-Type", "application/json");

            // Send the request
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(request.error);
            }
            else
            {
                Debug.Log("Response: " + request.downloadHandler.text);
            }
        }
    }
}
