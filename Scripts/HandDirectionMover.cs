using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.SubsystemsImplementation;
using System.Collections.Generic;

public class HandDirectionMover : MonoBehaviour
{
    XRHandSubsystem handSubsystem;
    public float maxSpeed = 50.0f;
    public float moveSpeed = 0f;
    public float smoothing = 5.0f;
    public float maxGestureDistance = 0.06f;

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
        XRHand leftHand = handSubsystem.leftHand;

        if (!rightHand.isTracked || !leftHand.isTracked)
            return;

        var tipJoint = rightHand.GetJoint(XRHandJointID.IndexTip);
        var knuckleJoint = rightHand.GetJoint(XRHandJointID.IndexProximal);

        if (!tipJoint.TryGetPose(out Pose tipPose) || !knuckleJoint.TryGetPose(out Pose knucklePose))
            return;

        Vector3 direction = (tipPose.position - knucklePose.position).normalized;

        var thumbTip = leftHand.GetJoint(XRHandJointID.ThumbTip);
        var indexDistal = leftHand.GetJoint(XRHandJointID.IndexDistal);

        if (!thumbTip.TryGetPose(out Pose thumbPose) || !indexDistal.TryGetPose(out Pose indexPose))
            return;

        float distance = Vector3.Distance(thumbPose.position, indexPose.position);


        Debug.Log(distance);
        if (distance <= 0.035) {
            distance = 0;
        }

        float normalized = Mathf.Clamp01(distance / maxGestureDistance);
        float targetSpeed = normalized * maxSpeed;
        moveSpeed = Mathf.Lerp(moveSpeed, targetSpeed, Time.deltaTime * smoothing);

        //float targetSpeed = Mathf.Clamp(distance * 1000f, 0f, maxSpeed);
        //moveSpeed = Mathf.Lerp(moveSpeed, targetSpeed, Time.deltaTime * smoothing);

        transform.position += direction * moveSpeed * Time.deltaTime;
    }
}
