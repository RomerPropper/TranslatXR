using Normal.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class GetRoomCode : MonoBehaviour
{
    [SerializeField] private Realtime realtime;
    [SerializeField] private string roomName;
    private bool isLoading;

    public TMP_InputField inputField;
    public void GetInputText()
    {
        roomName = inputField.text;
    }

    public void LoadRoom()
    {
        if (isLoading) return;
        isLoading = true;
        StartCoroutine(InTheRoom());
    }

    IEnumerator InTheRoom()
    {
        Debug.Log("Loading room: " + roomName);

        // Disconnect from current room if connected
        if (realtime != null && realtime.connected)
        {
            realtime.Disconnect();
            yield return new WaitForSeconds(1f); // Wait for disconnect to complete
        }

        // Find a new Realtime instance
        realtime = FindObjectOfType<Realtime>();

        // Connect to the new room
        if (realtime != null)
        {
            realtime.Connect(roomName);
            Debug.Log("Connected to room: " + roomName);
        }
        else
        {
            Debug.LogError("Failed to find Realtime instance.");
        }

        isLoading = false;
    }
}

