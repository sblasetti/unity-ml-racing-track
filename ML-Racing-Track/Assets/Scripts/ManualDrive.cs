using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualDrive : MonoBehaviour
{
    Driver driver;

    float vertical;
    float horizontal;
    bool brake;

    private void Start()
    {
        driver = GetComponent<Driver>();
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
        driver.Drive(horizontal, vertical, brake);
    }
}
