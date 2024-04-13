using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPositioner : MonoBehaviour
{
    public float smoothFactor = 2;
    public Transform target;
    public Vector3 offset = new Vector3(0,.15f, .4f);
    public Vector3 euler = new Vector3(15, 0, 0);
    private void Start()
    {
        target = Camera.main.transform;
        transform.position = GetTargetPos();
        transform.rotation = GetTargetRot();
    }
    void Update()
    {
        Vector3 targetPos = GetTargetPos();
        transform.position = Vector3.Lerp(transform.position, targetPos, smoothFactor * Time.deltaTime);

        Quaternion targetRot = GetTargetRot();
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, smoothFactor * Time.deltaTime);
    }
    Vector3 GetTargetPos()
    {
        Vector3 targetPos = target.TransformPoint(offset);
        Vector3 forward = Vector3.ProjectOnPlane(target.forward, Vector3.up);
        targetPos = target.position + (forward * offset.z);
        targetPos.y = target.position.y - offset.y;
        return targetPos;
    }
    Quaternion GetTargetRot()
    {
        return Quaternion.Euler(euler.x, target.eulerAngles.y, euler.z);
    }
}
