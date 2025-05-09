using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CockpitView : MonoBehaviour
{
    public Transform cameraTransform;           // OVRCameraRig/CenterEyeAnchor
    public Transform aircraftModelTransform;    // Your aircraft model
    public Vector3 offset;
    public float xRotateLimit = 15f;           

    void Update()
    {
        if (cameraTransform != null && aircraftModelTransform != null)
        {
            Vector3 cameraEuler = cameraTransform.eulerAngles;

            float xAngle = cameraEuler.x;
            if (xAngle > 180f) xAngle -= 360f;

            xAngle = Mathf.Clamp(xAngle, -xRotateLimit, xRotateLimit);

            Vector3 newEuler = new Vector3(xAngle, cameraEuler.y, cameraEuler.z);
            aircraftModelTransform.rotation = Quaternion.Euler(newEuler);

            aircraftModelTransform.position = cameraTransform.position + offset;
        }
    }
}
