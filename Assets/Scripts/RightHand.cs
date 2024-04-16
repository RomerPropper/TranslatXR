using System.Collections;
using System.Collections.Generic;
using Normal.Realtime;
using UnityEngine;

public class RightHand : MonoBehaviour
{
    private plyerManagement playerManager;

    void Start()
    {
        // Find the player manager in the scene
        playerManager = FindObjectOfType<plyerManagement>();
        if (playerManager == null)
        {
            Debug.LogError("DisplayRightHandPositions: No player manager found in scene!");
            return;
        }

        // Optionally, start periodic updates
        InvokeRepeating("DisplayPositions", 1.0f, 1.0f); // Update every second
    }

    void DisplayPositions()
    {
        if (playerManager == null) return;

        Dictionary<int, Vector3> rightHandPositions = playerManager.GetAllheadPositions();
        foreach (KeyValuePair<int, Vector3> entry in rightHandPositions)
        {
            Debug.Log($"Player {entry.Key} Head Position: {entry.Value}");
        }
    }
}
