using System.Collections.Generic;
using System.Linq;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class AgentDrive : Agent
{
    private const float DIRECTION_ANGLE_LIMIT_PERCENTAGE = 0.75f;
    Rigidbody rb;
    Driver driver;

    List<Checkpoint> checkpoints;
    Checkpoint nextCheckpoint;

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
            nextCheckpoint = checkpoints.FirstOrDefault(x => x.Order == nextCheckpoint.Order + 1);
            if (nextCheckpoint == null)
            {
                nextCheckpoint = checkpoints.First();
            }
        }
    }

    public override void OnEpisodeBegin()
    {
        transform.position = GetInitialPosition();
        transform.rotation = initialRotation;

        if (checkpoints == null)
        {
            checkpoints = FindObjectOfType<CheckpointsContainer>().listOfCheckpoints;
        }

        nextCheckpoint = checkpoints.First();

        if (PlayerIsOffTheTrack())
        {
            CancelPlayerVelocity();
            ResetPlayerStartPosition();
        }
    }

    private Vector3 GetInitialPosition()
    {
        var xOffset = Random.Range(-1f, 1f);
        var zOffset = Random.Range(-1.5f, 1.5f);
        return new Vector3(initialPosition.x + xOffset, initialPosition.y, initialPosition.z + zOffset);
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

        // Player radar (raycast):
        // Automatically added via a Ray Perception Sensor 3D

        // Next checkpoint position (uncomment to watch while playing)
        //Debug.DrawLine(transform.position, nextCheckpoint.transform.position, Color.yellow, .2f);
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
        // The direction reward is via the angle between the agent's direction and the direction to the next checkpoint
        // The reward is then calculated using a logarithmic function
        var directionToNextCheckpoint = (nextCheckpoint.transform.position - transform.position);
        var angleToCheckpoint = Vector3.Angle(rb.transform.forward, directionToNextCheckpoint) / 180;
        var directionReward = -Mathf.Log10(angleToCheckpoint + DIRECTION_ANGLE_LIMIT_PERCENTAGE);
        SetReward(directionReward);

        // Set reward for moving forward/penalize moving in reverse
        // Magnitude depends on velocity (higher velocity higher magnitude)
        float localForwardVelocity = rb.transform.InverseTransformDirection(rb.velocity).z * 0.001f;
        SetReward(localForwardVelocity);
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
        this.rb.angularVelocity = Vector3.zero;
        this.rb.velocity = Vector3.zero;
    }

    private bool PlayerIsOffTheTrack()
    {
        const float offset = 0.3f;
        var points = new Vector3[]
        {
            new Vector3(rb.position.x + offset, rb.position.y, rb.position.z + offset),
            new Vector3(rb.position.x - offset, rb.position.y, rb.position.z + offset),
            new Vector3(rb.position.x + offset, rb.position.y, rb.position.z - offset),
            new Vector3(rb.position.x - offset, rb.position.y, rb.position.z - offset),
        };

        RaycastHit hit;
        const int trackLayerMask = 1 << 8; // layer 8 is for the track
        foreach (var point in points)
        {
            Physics.Raycast(point, rb.transform.TransformDirection(-rb.transform.up), out hit, 5f, trackLayerMask);
            if (hit.collider == null)
            {
                return true;
            }
        }
        return false;

    }
}
