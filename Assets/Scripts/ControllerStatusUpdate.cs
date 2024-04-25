using UnityEngine;
using System.Collections;
using TMPro;
using System.Collections.Generic;

public class StatusUpdate : MonoBehaviour
{
    [SerializeField] 
    private Transform cameraTransform;

    [SerializeField]
    [Tooltip("The controller that should be used for controller-based recording. This must be assigned.")]
    private OVRControllerHelper _controller;

    [SerializeField]
    private GameObject _statusObject;

    [SerializeField]
    private Vector3 offset = new Vector3(0, 0.03f, 0.05f); // Default offset values, adjust in the Inspector as needed

    void Update()
    {
        // Make the status object face the camera
        _statusObject.transform.LookAt(cameraTransform);

        // Update the position of the status object to match the controller with an offset
        _statusObject.transform.position = _controller.transform.position + _controller.transform.TransformDirection(offset);
    }
}
