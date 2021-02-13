using System.Collections.Generic;
using System.Linq;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class AgentDrive : Agent
{
    private const float MAX_RAY_DISTANCE = 5f;
    Rigidbody rb;
    Driver driver;

    List<Checkpoint> checkpoints;
    Checkpoint lastCheckpoint;
    Checkpoint nextCheckpoint;
    int checkpointsCount = 0;

    Vector3 initialPosition = new Vector3(6f, 0.35f, 26f);
    Quaternion initialRotation = Quaternion.Euler(0, 90, 0);

    public bool AllowManualControl = false;

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
            checkpointsCount++;
        }
    }

    public override void OnEpisodeBegin()
    {
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        checkpointsCount = 0;

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
        sensor.AddObservation(GetDistanceFrom(-45));
        sensor.AddObservation(GetDistanceFrom(-22.5f));
        sensor.AddObservation(GetDistanceFrom(0));
        sensor.AddObservation(GetDistanceFrom(22.5f));
        sensor.AddObservation(GetDistanceFrom(45));
    }

    private float GetDistanceFrom(float angleFromAgentForward)
    {
        RaycastHit hitInfo;
        var ray = new Ray
        {
            origin = rb.transform.position,
            direction = Quaternion.Euler(0, angleFromAgentForward, 0) * rb.transform.forward.normalized
        };
        Physics.Raycast(ray, out hitInfo, MAX_RAY_DISTANCE, 1 << 10);
        //Debug.Log($"angle: {angleFromAgentForward} \tdist: {hitInfo.distance}\t {hitInfo.collider}");
        return hitInfo.distance >= 0 ? hitInfo.distance / MAX_RAY_DISTANCE : 1f;
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        UpdatePlayerMovement(vectorAction);
        SetPlayerRewards();
        CheckForInvalidStates();
    }

    public override void Heuristic(float[] actionsOut)
    {
        if (AllowManualControl)
        {
            actionsOut[0] = Input.GetAxis("Horizontal");
            actionsOut[1] = Input.GetAxis("Vertical");
            actionsOut[2] = Input.GetKey(KeyCode.Space) ? 1f : 0f;
        }
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

        // Set reward depending on completed checkpoints
        SetReward(0.03f * checkpointsCount);

        // Set reward for moving forward
        SetReward(rb.velocity.normalized.x > 0 ? rb.velocity.normalized.x * .01f : 0);

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
