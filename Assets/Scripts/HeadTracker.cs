using System.Collections;
using System.Collections.Generic;
using Normal.Realtime;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class HeadTracker : MonoBehaviour
{
    private plyerManagement playerManager;
    public GameObject objectPrefab; // Prefab for representing other players' right hands
    private Dictionary<int, GameObject> playerObjects = new Dictionary<int, GameObject>();
    
    public float yOffset = 0.2f;

    private NormcoreGM normcoreGM;

    void Start()
    {
        playerManager = FindObjectOfType<plyerManagement>();
        if (playerManager == null)
        {
            Debug.LogError("RightHandObjectController: No player manager found in scene!");
            return;
        }

        this.normcoreGM = FindObjectOfType<NormcoreGM>();
        if (playerManager == null)
        {
            Debug.LogError("HeadTracker: No NormcoreGM found in scene!");
            return;
        }
        this.normcoreGM._chatSync.MessageChanged += this.onNewMessage; //Subscribe onNewMessage to the MessageChanged event, so it is called each time a new message is recieved.
    }

    void Update()
    {
        if (playerManager == null) return;

        Dictionary<int, Vector3> rightHandPositions = playerManager.GetAllheadPositions();
        int localPlayerID = playerManager.localAvatar != null ? playerManager.localAvatar.realtimeView.ownerIDSelf : -1;

        // Update or create GameObjects for each player's right hand
        foreach (KeyValuePair<int, Vector3> entry in rightHandPositions)
        {
            if (entry.Key != localPlayerID)
            {
                Vector3 adjustedPosition = entry.Value;
                adjustedPosition.y += yOffset;

                if (!playerObjects.ContainsKey(entry.Key))
                {
                    // Create a new GameObject if one doesn't exist for this player
                    GameObject newObj = Instantiate(objectPrefab, adjustedPosition, Quaternion.identity);
                    playerObjects.Add(entry.Key, newObj);
                }
                else
                {
                    // Update the position of the existing GameObject
                    playerObjects[entry.Key].transform.position = adjustedPosition;
                }

                if (Camera.main != null)
                {
                    playerObjects[entry.Key].transform.LookAt(Camera.main.transform);
                    playerObjects[entry.Key].transform.Rotate(0, 180f, 0); // Rotate 180 degrees around the y-axis
                }
            }
        }

        // Remove GameObjects for players who are no longer tracked
        List<int> keysToRemove = new List<int>();
        foreach (var key in playerObjects.Keys)
        {
            if (!rightHandPositions.ContainsKey(key))
            {
                Destroy(playerObjects[key]);
                keysToRemove.Add(key);
            }
        }
        foreach (int key in keysToRemove)
        {
            playerObjects.Remove(key);
        }
    }

    public void onNewMessage(Message newMessage) {
        if (newMessage.ClientID == -1 || newMessage.ClientID == null) {
            Debug.Log("HeadTracker (onNewMessage): Client ID does not exists!");
            return;
        }
        if (!playerObjects.ContainsKey(newMessage.ClientID)) {
            Debug.LogError("HeadTracker (onNewMessage): Client ID does not exists in playerObjects!");
            return;
        }
        if (playerObjects[newMessage.ClientID].GetComponentInChildren<TextMeshProUGUI>() == null) {
            Debug.LogError("HeadTracker (onNewMessage): Could not find TextMeshProGUI inside the UI Chat bubble!");
            return;
        }
        string newMessageContent = newMessage.MessageContent + "\n";
        playerObjects[newMessage.ClientID].GetComponentInChildren<TextMeshProUGUI>().text += newMessageContent;
    }
}