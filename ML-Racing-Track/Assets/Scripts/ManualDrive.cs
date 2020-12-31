using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualDrive : MonoBehaviour
{
    Rigidbody rb;

    float vertical;
    float horizontal;

    float speed = 8f;
    Vector3 rotationRight = new Vector3(0, 60, 0);
    Vector3 rotationLeft = new Vector3(0, -60, 0);

    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
    }

    void Update()
    {
        vertical = Input.GetAxis("Vertical");
        horizontal = Input.GetAxis("Horizontal");
    }

    private void FixedUpdate()
    {
        if (vertical != 0)
        {
            rb.MovePosition(transform.position + transform.forward * vertical * speed * Time.deltaTime);

            if (horizontal != 0)
            {
                var rotation = horizontal > 0 ? rotationRight : rotationLeft;
                var deltaRotation = Quaternion.Euler(rotation * Time.deltaTime);
                rb.MoveRotation(rb.rotation * deltaRotation);
            }
        }
    }
}
