using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualDrive : MonoBehaviour
{
    public GameObject FrontLeftWheel, FrontRightWheel, RearLeftWheel, RearRightWheel;
    public float MotorForce = 100;
    public float SteerForce = 40;
    public float BrakeForce = 100;

    WheelCollider frontLeftCollider, frontRightCollider, rearLeftCollider, rearRightCollider;
    float vertical;
    float horizontal;
    bool brake;

    private void Start()
    {
        frontLeftCollider = FrontLeftWheel.GetComponent<WheelCollider>();
        frontRightCollider = FrontRightWheel.GetComponent<WheelCollider>();
        rearLeftCollider = RearLeftWheel.GetComponent<WheelCollider>();
        rearRightCollider = RearRightWheel.GetComponent<WheelCollider>();
}

    void Update()
    {
        vertical = Input.GetAxis("Vertical");
        horizontal = Input.GetAxis("Horizontal");
        brake = Input.GetKey(KeyCode.Space);
    }

    private void FixedUpdate()
    {
        ApplyForce();
        Rotate();
    }

    private void Rotate()
    {
        RotateWheel(horizontal, frontLeftCollider, FrontLeftWheel);
        RotateWheel(horizontal, frontRightCollider, FrontRightWheel);
    }

    private void ApplyForce()
    {
        ApplyWheelForce(vertical, rearLeftCollider);
        ApplyWheelForce(vertical, rearRightCollider);
    }

    private void ApplyWheelForce(float verticalChange, WheelCollider collider)
    {
        var torque = verticalChange * MotorForce;
        collider.motorTorque = torque;

        collider.brakeTorque = brake ? BrakeForce : 0;
    }

    private void RotateWheel(float change, WheelCollider collider, GameObject wheel)
    {
        var steerAngle = change * SteerForce;
        collider.steerAngle = steerAngle;

        // Visual rotation
        Quaternion rotation;
        collider.GetWorldPose(out _, out rotation);
        var fixedRotation = Quaternion.Euler(rotation.eulerAngles.x, rotation.eulerAngles.y, rotation.eulerAngles.z + 90);
        wheel.transform.rotation = fixedRotation;
    }
}
