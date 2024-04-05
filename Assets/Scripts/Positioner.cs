using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatBubblePositioner : MonoBehaviour
{
    public float smoothFactor = 4f; 
    // Offset can be made to however we want it to look (test with other headset)
    public Vector3 offset = new Vector3(0, 2f, -1f); 

    private Transform anchorTransform;

    private void Start()
    {
        // Find the headset center within the camera rig
        anchorTransform = this.transform.parent; 
    }

    private void Update()
    {
        if (anchorTransform == null) return; // return if no anchor

        // Get the position of the chat bubble
        Vector3 desiredPosition = anchorTransform.position + offset;
        
        // Smoothly move
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothFactor * Time.deltaTime);
        
        // Optionally, ensure the chat bubble always faces the camera/main viewpoint
        transform.LookAt(Camera.main.transform);
        transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f); // Keep the bubble upright
    }
}
