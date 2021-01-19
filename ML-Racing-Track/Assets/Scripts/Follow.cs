using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    public Transform Target;
    public Vector3 Offset;
    public float followSpeed = 10;
    public float lookSpeed = 10;

    void FixedUpdate()
    {
        var targetPosition = Target.position +
                       Target.forward * Offset.z +
                       Target.right * Offset.x +
                       Target.up * Offset.y;
        var direction = Target.position - targetPosition;
        var targetRotation = Quaternion.LookRotation(direction, Vector3.up);

        // Move To Target
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);

        // Look At Target
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, lookSpeed * Time.deltaTime);
    }
}
