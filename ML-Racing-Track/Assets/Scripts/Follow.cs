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
        LookAtTarget();
        MoveToTarget();
    }

    void LookAtTarget()
    {
        var direction = Target.position - transform.position;
        var rotation = Quaternion.LookRotation(direction, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, lookSpeed * Time.deltaTime);
    }

    void MoveToTarget()
    {
        var position = Target.position +
                       Target.forward * Offset.z +
                       Target.right * Offset.x +
                       Target.up * Offset.y;
        transform.position = Vector3.Lerp(transform.position, position, followSpeed * Time.deltaTime);
    }
}
