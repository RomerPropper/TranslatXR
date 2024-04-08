using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// public class ChatBubblePositioner : MonoBehaviour
// {
//     public float smoothFactor = 4f; 
//     // Offset can be made to however we want it to look (test with other headset)
//     public Vector3 offset = new Vector3(0, 1f, -5f); 

//     private Transform anchorTransform;

//     private void Start()
//     {
//         // Find the headset center within the camera
//         anchorTransform = this.transform.parent; 
//     }

//     private void Update()
//     {
//         if (anchorTransform == null) return; // return if no anchor

//         // Get the position of the chat bubble
//         Vector3 desiredPosition = anchorTransform.position + offset;
        
//         // Smoothly move
//         transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothFactor * Time.deltaTime);
        
//         // Optionally, ensure the chat bubble always faces the camera/main viewpoint
//         transform.LookAt(Camera.main.transform);
//         transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f); // Keep the bubble upright
//     }
// }

public class ChatBubble : MonoBehaviour
{
    private Transform target;

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    void LateUpdate()
    {
        if(target != null)
        {
            Vector3 targetPosition = target.position + Vector3.up * 0.5f;
            transform.position = targetPosition;
        }
    }
}