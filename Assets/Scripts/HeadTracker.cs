using System.Collections;
using System.Collections.Generic;
using Normal.Realtime;
using TMPro;
using UnityEngine;

public class HeadTracker : MonoBehaviour
{
    private plyerManagement playerManager;
    public GameObject objectPrefab; // Prefab for representing other players' right hands
    private Dictionary<int, GameObject> playerObjects = new Dictionary<int, GameObject>();

    public float yOffset = 1.0f; // Offset to place the bubble above the head

    // Cache the main camera's transform for efficiency
    [SerializeField] 
    private Transform cameraTransform;

    public NormcoreGM normcoreGM;

    void Start()
    {
        if (normcoreGM is null) {
            Debug.LogError("HeadTracker [Start]: normcoreGM is not set to a reference! ChatSync will not work!");
        }
        this.normcoreGM._chatSync.MessageChanged += this.onNewMessage;
        playerManager = FindObjectOfType<plyerManagement>();
        if (playerManager == null)
        {
            Debug.LogError("RightHandObjectController: No player manager found in scene!");
            return;
        }

        // Assume the main camera is the player's camera
        cameraTransform = Camera.main.transform;
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
                adjustedPosition.y += yOffset; // Apply the vertical offset
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

                // Make the GameObject face the camera
                if (cameraTransform != null)
                {
                    playerObjects[entry.Key].transform.LookAt(cameraTransform);
                    playerObjects[entry.Key].transform.Rotate(0, 180, 0);
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

    //This function is called anytime a new message arrives
    public void onNewMessage(Message newMessage)
    {
        //Check if ClientID exists inside playerObjects
        if (!playerObjects.ContainsKey(newMessage.ClientID))
        {
            Debug.LogError("HeadTracker[onNewMessage]: Client ID does not exists inside playerObjects!");
            return;
        }

        //Check if UI Chatbox has TextMeshPro on it we can use
        if (playerObjects[newMessage.ClientID].GetComponentInChildren<TextMeshProUGUI>() == null)
        {
            Debug.LogError("HeadTracker[onNewMessage]: Could not locate textmesh pro on chatbox ui");
            return;
        }
        string newMessageContent = newMessage.MessageContent;
        playerObjects[newMessage.ClientID].GetComponentInChildren<TextMeshProUGUI>().text = newMessageContent;
    }
}
