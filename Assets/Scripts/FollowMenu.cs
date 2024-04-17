using UnityEngine;

public class FollowPlayerView : MonoBehaviour
{
    private Transform playerCamera; // Reference to the player's camera

    void Start()
    {
        // Find the player's camera
        playerCamera = Camera.main.transform;
    }

    void LateUpdate()
    {
        // Make the input field face the player's camera
        transform.LookAt(transform.position + playerCamera.forward, playerCamera.up);
    }
}
