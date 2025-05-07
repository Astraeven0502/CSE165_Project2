//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.XR.Hands;
//using UnityEngine.XR.Management;

//public class test : MonoBehaviour
//{
//    // Start is called before the first frame update
//    void Start()
//    {
//        XRHandSubsystem m_Subsystem =
//            XRGeneralSettings.Instance?
//                .Manager?
//                .activeLoader?
//                .GetLoadedSubsystem<XRHandSubsystem>();

//        if (m_Subsystem != null)
//            m_Subsystem.updatedHands += OnHandUpdate;
//    }

//    //Update is called once per frame
//    void OnHandUpdate(XRHandSubsystem subsystem,
//                    XRHandSubsystem.UpdateSuccessFlags updateSuccessFlags,
//                    XRHandSubsystem.UpdateType updateType)
//    {
//        switch (updateType)
//        {
//            case XRHandSubsystem.UpdateType.Dynamic:
//                // Update game logic that uses hand data
//                break;
//            case XRHandSubsystem.UpdateType.BeforeRender:
//                // Update visual objects that use hand data
//                break;
//        }
//    }

//}

using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.SubsystemsImplementation;
using System.Collections.Generic;

public class HandDirectionMover : MonoBehaviour
{
    XRHandSubsystem handSubsystem;
    public float moveSpeed = 1.5f;

    void Start()
    {
        var subsystems = new List<XRHandSubsystem>();
        SubsystemManager.GetInstances(subsystems);
        if (subsystems.Count > 0)
        {
            handSubsystem = subsystems[0];
        }
    }

    void Update()
    {
        if (handSubsystem == null || !handSubsystem.running)
            return;

        XRHand rightHand = handSubsystem.rightHand;

        if (!rightHand.isTracked)
            return;

        var tipJoint = rightHand.GetJoint(XRHandJointID.IndexTip);
        var knuckleJoint = rightHand.GetJoint(XRHandJointID.IndexProximal);

        Vector3 tip = new Vector3();
        Vector3 knuckle = new Vector3();

        if (tipJoint.TryGetPose(out Pose pose1) && knuckleJoint.TryGetPose(out Pose pose2))
        {
            // displayTransform is some GameObject's Transform component
            tip = pose1.position;
            knuckle = pose2.position;
        }

        if ((tipJoint.trackingState & XRHandJointTrackingState.Pose) == 0
           || (knuckleJoint.trackingState & XRHandJointTrackingState.Pose) == 0)
           return;

        //Vector3 tip = tipJoint.;
        //Vector3 knuckle = knuckleJoint.pose.position;

        Vector3 direction = (tip - knuckle).normalized;
        // direction.y = 0; // Optional: restrict to horizontal movement

        // Move the object (e.g., XR Origin or camera rig)
        transform.position += direction * moveSpeed * Time.deltaTime;
    }
}
