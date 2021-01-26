using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Driver : MonoBehaviour
{
    public GameObject Vehicle;
    public Transform FrontLeftWheel, FrontRightWheel, RearLeftWheel, RearRightWheel, CenterOfMass;
    public WheelCollider FrontLeftWheelCollider, FrontRightWheelCollider, RearLeftWheelCollider, RearRightWheelCollider;
    public float MotorForce = 50;
    public float MaxSteerAngle = 40;
    public float BrakeForce = 50;

    private void Start()
    {
        GetComponentInChildren<Rigidbody>().centerOfMass = CenterOfMass.transform.localPosition;
    }

    public void Drive(float horizontal, float vertical, bool brake)
    {
        Accelerate(vertical, brake);
        Steer(horizontal);
        UpdateWheelPose();
    }

    private void Steer(float horizontal)
    {
        RotateWheel(horizontal, FrontLeftWheelCollider);
        RotateWheel(horizontal, FrontRightWheelCollider);
    }

    private void Accelerate(float vertical, bool brake)
    {
        ApplyWheelForce(vertical, brake, FrontLeftWheelCollider);
        ApplyWheelForce(vertical, brake, FrontRightWheelCollider);
    }

    private void UpdateWheelPose()
    {
        SetWheelPoseFromCollider(FrontRightWheelCollider, FrontRightWheel);
        SetWheelPoseFromCollider(FrontLeftWheelCollider, FrontLeftWheel);
        SetWheelPoseFromCollider(RearRightWheelCollider, RearRightWheel);
        SetWheelPoseFromCollider(RearLeftWheelCollider, RearLeftWheel);
    }

    private void ApplyWheelForce(float verticalChange, bool brake, WheelCollider collider)
    {
        if (!brake)
            collider.motorTorque = verticalChange * MotorForce;
        collider.brakeTorque = brake ? BrakeForce : 0;
    }

    private void RotateWheel(float horizontalChange, WheelCollider collider)
    {
        var steerAngle = horizontalChange * MaxSteerAngle;
        collider.steerAngle = steerAngle;
    }

    private void SetWheelPoseFromCollider(WheelCollider collider, Transform wheelTransform)
    {
        Vector3 position;
        Quaternion rotation;

        collider.GetWorldPose(out position, out rotation);
        wheelTransform.position = position;
        wheelTransform.rotation = rotation;
    }
}
