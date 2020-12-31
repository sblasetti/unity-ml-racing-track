using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    public GameObject Target;
    float frontOffset = -3f;
    float topOffset = 1f;

    void Update()
    {
        var offset = Target.transform.forward * frontOffset + Target.transform.up * topOffset;
        transform.position = Target.transform.position + offset;
        transform.rotation = Quaternion.LookRotation(Target.transform.forward);
    }
}
