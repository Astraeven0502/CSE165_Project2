//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class CockpitView : MonoBehaviour
//{

//    public Transform cameraTransform;      // Drag in OVRCameraRig/CenterEyeAnchor
//    public Transform aircraftModelTransform; // Drag in your aircraft model
//    public Vector3 offset;
//    public float x_rotate_limit;

//    // Start is called before the first frame update
//    void Start()
//    {

//    }

//    // Update is called once per frame
//    void Update()
//    {
//        if (cameraTransform != null && aircraftModelTransform != null)
//        {
//            float x_rotate;
//            x_rotate = cameraTransform.rotation.x;
//            if (x_rotate > x_rotate_limit || -x_rotate > x_rotate_limit)
//            {
//                x_rotate = x_rotate_limit;
//            }

//            aircraftModelTransform.rotation = (x_rotate, cameraTransform.rotation.y, cameraTransform.rotation.z, 0);
//            aircraftModelTransform.position = cameraTransform.position + offset;
//        }
//    }
//}

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
