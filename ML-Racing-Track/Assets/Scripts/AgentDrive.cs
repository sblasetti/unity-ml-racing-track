using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class AgentDrive : Agent
{
    Rigidbody rb;

    float speed = 4f;
    Vector3 rotationRight = new Vector3(0, 60, 0);
    Vector3 rotationLeft = new Vector3(0, -60, 0);

    public GameObject StartLine;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        if (PlayerIsOffTheTrack())
        {
            CancelPlayerVelocity();
            ResetPlayerStartPosition();
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Player's position
        sensor.AddObservation(transform.localPosition);

        // Player's velocity
        sensor.AddObservation(rb.velocity.x);
        sensor.AddObservation(rb.velocity.z);
        
        // Start/finish line position
        sensor.AddObservation(StartLine.transform.localPosition);
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        SetPlayerMovement(vectorAction);
        SetPlayerRewards();
        CheckForInvalidStates();
    }

    private void SetPlayerMovement(float[] vectorAction)
    {
        var vertical = vectorAction[0];
        var horizontal = vectorAction[1];

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

    private void SetPlayerRewards()
    {
        // Set reward depending on track checkpoints
        // Set reward when finish line is reached
    }

    private void CheckForInvalidStates()
    {
        // End episode when player falls off the track
        if (PlayerIsOffTheTrack())
        {
            EndEpisode();
        }
    }

    private void ResetPlayerStartPosition()
    {
        
        transform.localPosition = StartLine.transform.position;
    }

    private void CancelPlayerVelocity()
    {
        this.rb.angularVelocity = Vector3.zero;
        this.rb.velocity = Vector3.zero;
    }

    private bool PlayerIsOffTheTrack()
    {
        return transform.position.y <= 0;
    }
}
