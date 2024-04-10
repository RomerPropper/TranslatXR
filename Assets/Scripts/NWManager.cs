using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Normal.Realtime;

public class NetworkManager : MonoBehaviour
{
    public GameObject userPrefab; 

    private Realtime _realtime;

    private void Awake()
    {
        _realtime = GetComponent<Realtime>(); 
    }

    private void Start()
    {
        // Register to the connected event to know when we join the room
        _realtime.didConnectToRoom += DidConnectToRoom;
    }

    private void DidConnectToRoom(Realtime realtime)
    {
        if (realtime.connected)
        {
            Realtime.Instantiate(userPrefab.name, // Prefab name
                position: Vector3.zero, // Start position
                rotation: Quaternion.identity, // Start rotation
                ownedByClient: true, // Make this client the owner
                preventOwnershipTakeover: true, // Prevent others from taking ownership
                useInstance: realtime); // Use this instance of Realtime
        }
    }
}
