using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ChatBubblePositioner : MonoBehaviour
{
    public float smoothFactor = 4f;
    public Vector3 offset = new Vector3(0, 2.5f, 0); // Adjust this offset as needed
    private Transform targetTransform;

    private void Start()
    {
        // Initial placement, in case you want to set the bubble active without waiting for Update/LateUpdate
        if (targetTransform != null)
        {
            transform.position = GetTargetPosition();
            transform.rotation = GetTargetRotation();
        }
    }

    private void Update()
    {
        if (targetTransform != null)
        {
            // Interpolate towards the target position
            Vector3 targetPosition = GetTargetPosition();
            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothFactor * Time.deltaTime);

            // Interpolate towards the target rotation
            Quaternion targetRotation = GetTargetRotation();
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, smoothFactor * Time.deltaTime);
        }
    }

    public void SetTarget(Transform newTarget)
    {
        targetTransform = newTarget;
    }

    private Vector3 GetTargetPosition()
    {
        // Compute the position directly above the target's head with the given offset
        return targetTransform.position + targetTransform.up * offset.y + targetTransform.forward * offset.z + targetTransform.right * offset.x;
    }

    private Quaternion GetTargetRotation()
    {
        // Rotate the bubble to face the camera
        Vector3 toCamera = Camera.main.transform.position - transform.position;
        // This keeps the bubble upright and only rotates around the y-axis
        toCamera.y = 0;
        return Quaternion.LookRotation(toCamera);
    }
}

