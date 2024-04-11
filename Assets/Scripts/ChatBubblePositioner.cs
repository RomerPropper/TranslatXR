using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatBubblePositioner : MonoBehaviour
{
    public float smoothFactor = 4f; 
    public Vector3 offset = new Vector3(0, 2.5f, 0);
    private Transform targetTransform; 

    // SetTarget is public to be called by avatar creation
    public void SetTarget(Transform newTarget)
    {
        targetTransform = newTarget;
    }

    private void LateUpdate()
    {
        if(targetTransform != null)
        {
            Vector3 targetPosition = targetTransform.position + offset;
            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothFactor * Time.deltaTime);

            // Make the chat bubble always face the camera
            transform.LookAt(Camera.main.transform.position);
            transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
        }
    }
}
