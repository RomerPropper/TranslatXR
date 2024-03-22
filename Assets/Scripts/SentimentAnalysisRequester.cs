// using System.Collections;
// using UnityEngine;
// using UnityEngine.Networking;
// using System.Text;

public class SentimentAnalysisRequester : MonoBehaviour
{
    // private string apiUrl = "http://localhost:8000/sentiment";
    private ConfigData configData;
    private string apiUrl;

    void Start()
    {
        configData = ConfigData.LoadConfig("config");
        if (configData != null)
        {
            apiUrl = configData.apiUrl;
            StartCoroutine(PostSentiment("I just won the lottery, but I had a terrible day today"));
        }
        else
        {
            Debug.LogError("Failed to load config!");
        }
    }

    IEnumerator PostSentiment(string text)
    {
        string endpoint = apiUrl + "/sentiment";
        Debug.Log("ENDPOINT: " + endpoint);
        string jsonPayload = "{\"text\":\"" + text + "\"}";
        byte[] jsonToSend = new UTF8Encoding().GetBytes(jsonPayload);

        // UnityWebRequest for posting the sentiment analysis
        using (UnityWebRequest request = new UnityWebRequest(endpoint, "POST"))
        {
            // Set the body of the request
            request.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

//             // Set Headers
//             request.SetRequestHeader("Content-Type", "application/json");

//             // Send the request
//             yield return request.SendWebRequest();

//             if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
//             {
//                 Debug.LogError(request.error);
//             }
//             else
//             {
//                 Debug.Log("Response: " + request.downloadHandler.text);
//                 callback?.Invoke(request.downloadHandler.text);
//             }
//         }
//     }
// }
