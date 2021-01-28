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

    Vector3 initialPosition = new Vector3(6f, 0.35f, 26f);
    Quaternion initialRotation = Quaternion.Euler(0, 90, 0);

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        driver = GetComponent<Driver>();

        driver.OnCheckpointEntered += OnCheckpointEnteredHandler;
    }

    private void OnCheckpointEnteredHandler(Checkpoint checkpoint)
    {
        if (checkpoint == nextCheckpoint)
        {
            lastCheckpoint = nextCheckpoint;
            nextCheckpoint = checkpoints.FirstOrDefault(x => x.Order == nextCheckpoint.Order + 1);
            if (nextCheckpoint == null)
            {
                nextCheckpoint = checkpoints.First();
            }
        }
    }

    public override void OnEpisodeBegin()
    {
        transform.position = initialPosition;
        transform.rotation = initialRotation;

        if (checkpoints == null)
        {
            checkpoints = FindObjectOfType<CheckpointsContainer>().listOfCheckpoints;
        }

        nextCheckpoint = checkpoints.First();
        lastCheckpoint = null;

        if (PlayerIsOffTheTrack())
        {
            CancelPlayerVelocity();
            ResetPlayerStartPosition();
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Player's position
        sensor.AddObservation(transform.position.normalized);

        // Player's velocity
        sensor.AddObservation(rb.velocity.normalized);

        // Next checkpoint position
        sensor.AddObservation(transform.InverseTransformPoint(nextCheckpoint.transform.position));

        // Player direction
        sensor.AddObservation(transform.forward);
        sensor.AddObservation(transform.right);

        // Player direction to next checkpoint
        var directionToNextCheckpoint = (nextCheckpoint.transform.position - transform.position).normalized;
        sensor.AddObservation(directionToNextCheckpoint);

        // Player radar (raycast)

        // Checkpoints
        //sensor.AddObservation(lastCheckpoint != null ? lastCheckpoint.Order : -1);
        //sensor.AddObservation(nextCheckpoint.Order);
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
        var brake = vectorAction[2] < 0;

        driver.Drive(horizontal, vertical, brake);
    }

    private void SetPlayerRewards()
    {
        // Set reward if moving in the right direction
        var directionToNextCheckpoint = (nextCheckpoint.transform.position - transform.position);
        var velocityAlignment = Vector3.Dot(directionToNextCheckpoint, rb.velocity);
        SetReward(0.001f * velocityAlignment);

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
