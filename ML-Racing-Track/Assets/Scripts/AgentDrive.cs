using System.Collections.Generic;
using System.Linq;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class AgentDrive : Agent
{
    Rigidbody rb;
    Driver driver;

    List<Checkpoint> checkpoints;
    Checkpoint lastCheckpoint;
    Checkpoint nextCheckpoint;

    void Start()
    {
        rb = GetComponentInChildren<Vehicle>().GetComponent<Rigidbody>();
        driver = GetComponent<Driver>();

        checkpoints = FindObjectOfType<CheckpointsContainer>().listOfCheckpoints;
        nextCheckpoint = checkpoints.First();
        lastCheckpoint = null;
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

        // Player radar (raycast)

        // Checkpoints
        sensor.AddObservation(lastCheckpoint);
        sensor.AddObservation(nextCheckpoint);
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        UpdatePlayerMovement(vectorAction);
        SetPlayerRewards();
        CheckForInvalidStates();
    }

    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = Input.GetAxis("Horizontal");
        actionsOut[1] = Input.GetAxis("Vertical");
        actionsOut[2] = Input.GetKey(KeyCode.Space) ? 1f : 0f;
    }

    private void UpdatePlayerMovement(float[] vectorAction)
    {
        var horizontal = vectorAction[0];
        var vertical = vectorAction[1];
        var brake = vectorAction[2] != 0;

        driver.Drive(horizontal, vertical, brake);
    }

    private void SetPlayerRewards()
    {
        // Set reward if moving forward 
        if (rb.velocity.x > 0 || rb.velocity.z > 0)
        {
            SetReward(0.1f);
        }

        // Set reward depending on track checkpoints
        // TBD

        // Set reward when finish line is reached
        if (lastCheckpoint == checkpoints.Last())
        {
            SetReward(1f);
            EndEpisode();
        }
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
        //transform.localPosition = StartLine.transform.position;
    }

    private void CancelPlayerVelocity()
    {
        //this.rb.angularVelocity = Vector3.zero;
        //this.rb.velocity = Vector3.zero;
    }

    private bool PlayerIsOffTheTrack()
    {
        RaycastHit hit;
        const int trackLayerMask = 1 << 8; // layer 8 is for the track
        Physics.Raycast(rb.position, rb.transform.TransformDirection(-rb.transform.up), out hit, 5f, trackLayerMask);

        return hit.collider == null;
    }
}
