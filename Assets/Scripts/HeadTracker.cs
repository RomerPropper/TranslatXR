using System.Collections;
using System.Collections.Generic;
using Normal.Realtime;
using Unity.VisualScripting;
using UnityEngine;

public class HeadTracker : MonoBehaviour
{
    private plyerManagement playerManager;
    public GameObject objectPrefab; // Prefab for representing other players' right hands
    private Dictionary<int, GameObject> playerObjects = new Dictionary<int, GameObject>();
    
    void Start()
    {
        playerManager = FindObjectOfType<plyerManagement>();
        if (playerManager == null)
        {
            Debug.LogError("RightHandObjectController: No player manager found in scene!");
            return;
        }
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
                if (!playerObjects.ContainsKey(entry.Key))
                {
                    // Create a new GameObject if one doesn't exist for this player
                    GameObject newObj = Instantiate(objectPrefab, entry.Value, Quaternion.identity);
                    playerObjects.Add(entry.Key, newObj);
                }
                else
                {
                    // Update the position of the existing GameObject
                    playerObjects[entry.Key].transform.position = entry.Value;
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
}