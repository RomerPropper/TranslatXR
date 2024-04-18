using System.Collections;
using UnityEngine;

public class MenuPositioner : MonoBehaviour
{
    public Vector3 offset = new Vector3(0, .15f, .4f);
    public Vector3 euler = new Vector3(15, 0, 0);

    IEnumerator Start()
    {
        // Wait for one frame to ensure all Start methods have completed.
        // This uses the fact that Start is only called once all objects are initialized.
        yield return null;

        Transform cameraTransform = Camera.main.transform;

        // Set position relative to the camera with a fixed offset
        Vector3 targetPos = cameraTransform.position + cameraTransform.TransformDirection(offset);
        transform.position = targetPos;

        // Set rotation to a fixed orientation based on the camera's rotation
        Vector3 targetEuler = new Vector3(euler.x, cameraTransform.eulerAngles.y, euler.z);
        transform.rotation = Quaternion.Euler(targetEuler);
    }
}
