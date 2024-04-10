using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatBubble : MonoBehaviour
{
    public float smoothFactor = 4f; 
    private Transform target;
    public Vector3 offset = new Vector3(0, 1f, -5f); 
    private Transform anchorTransform;
    public void SetTarget(Transform newTarget)
    {
        anchorTransform = newTarget;
    }

    private void LateUpdate()
    {
        if(target != null)
        {
            Vector3 targetPosition = anchorTransform.position + offset;
            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothFactor * Time.deltaTime);
        }
    }
}