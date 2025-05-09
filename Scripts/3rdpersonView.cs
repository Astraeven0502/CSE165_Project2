using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonAircraftView : MonoBehaviour
{
    public Transform cameraTransform;         // OVRCameraRig/CenterEyeAnchor
    public Transform aircraftModelTransform;  // Your aircraft model
    public float distance = 3f;               // Distance in front of camera
    public float heightOffset = 1f;           // Offset downward from camera
    public float rotationSmoothSpeed = 5f;    // Smooth rotation

    void Update()
    {
        if (cameraTransform != null && aircraftModelTransform != null)
        {
            Vector3 forwardOffset = cameraTransform.forward * distance - cameraTransform.up * heightOffset;
            aircraftModelTransform.position = cameraTransform.position + forwardOffset;

            Quaternion targetRotation = Quaternion.Euler(0f, cameraTransform.eulerAngles.y, 0f);

            aircraftModelTransform.rotation = Quaternion.Slerp(aircraftModelTransform.rotation, targetRotation, Time.deltaTime * rotationSmoothSpeed);
        }
    }
}
