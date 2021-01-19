using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualDrive : MonoBehaviour
{
    public Transform FrontLeftWheel, FrontRightWheel, RearLeftWheel, RearRightWheel, CenterOfMass;
    public WheelCollider FrontLeftWheelCollider, FrontRightWheelCollider, RearLeftWheelCollider, RearRightWheelCollider;
    public float MotorForce = 50;
    public float MaxSteerAngle = 40;
    public float BrakeForce = 50;

    float vertical;
    float horizontal;
    bool brake;

    private void Start()
    {
        GetComponentInChildren<Rigidbody>().centerOfMass = CenterOfMass.transform.localPosition;
    }

    void Update()
    {
        GetInput();
    }

    private void GetInput()
    {

        vertical = Input.GetAxis("Vertical");
        horizontal = Input.GetAxis("Horizontal");
        brake = Input.GetKey(KeyCode.Space);
    }

    private void FixedUpdate()
    {
        Accelerate();
        Steer();
        UpdateWheelPose();
    }

    private void Steer()
    {
        RotateWheel(horizontal, FrontLeftWheelCollider);
        RotateWheel(horizontal, FrontRightWheelCollider);
    }

    private void Accelerate()
    {
        ApplyWheelForce(vertical, FrontLeftWheelCollider);
        ApplyWheelForce(vertical, FrontRightWheelCollider);
    }

    private void UpdateWheelPose()
    {
        SetWheelPoseFromCollider(FrontRightWheelCollider, FrontRightWheel);
        SetWheelPoseFromCollider(FrontLeftWheelCollider, FrontLeftWheel);
        SetWheelPoseFromCollider(RearRightWheelCollider, RearRightWheel);
        SetWheelPoseFromCollider(RearLeftWheelCollider, RearLeftWheel);
    }

    private void ApplyWheelForce(float verticalChange, WheelCollider collider)
    {
        var torque = verticalChange * MotorForce;
        collider.motorTorque = !brake ? torque : 0;
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
