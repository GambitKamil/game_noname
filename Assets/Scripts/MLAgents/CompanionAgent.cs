
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class CompanionAgent : Agent
{
    [Header("Training Settings")]
    [SerializeField] private Transform target;
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float maxEpisodeTime = 30f;

    [Header("Game Mode")]
    [SerializeField] private bool trainingMode = true;
    [SerializeField] private bool startAutomatically = true;

    private bool targetReached = false;
    private bool navigationActive = false;

    [Header("Reward Settings")]
    [SerializeField] private float successReward = 2f;
    [SerializeField] private float stepPenalty = -0.0002f;
    [SerializeField] private float progressRewardScale = 0.05f;

    private Rigidbody2D rb;
    private Vector2 startPosition;
    private float episodeTimer;
    private float previousDistance;

    public bool TargetReached => targetReached;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position;

        navigationActive = trainingMode || startAutomatically;
    }

    public override void OnEpisodeBegin()
    {
        targetReached = false;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        transform.position = startPosition;
        episodeTimer = 0f;

        navigationActive = trainingMode || startAutomatically;

        if (target != null)
        {
            previousDistance = Vector2.Distance(
                transform.position,
                target.position
            );
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        if (target == null)
        {
            sensor.AddObservation(Vector2.zero);
        }
        else
        {
            Vector2 direction =
                target.position - transform.position;

            sensor.AddObservation(direction / 12f);
        }

        sensor.AddObservation(rb.linearVelocity / moveSpeed);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        if (!trainingMode &&
            (!navigationActive || targetReached || target == null))
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        int horizontal = actions.DiscreteActions[0] - 1;
        int vertical = actions.DiscreteActions[1] - 1;

        Vector2 movement = new Vector2(
            horizontal,
            vertical
        ).normalized;

        rb.linearVelocity = movement * moveSpeed;

        if (trainingMode)
        {
            AddReward(stepPenalty);
        }

        if (trainingMode && target != null)
        {
            float currentDistance = Vector2.Distance(
                transform.position,
                target.position
            );

            float distanceImprovement =
                previousDistance - currentDistance;

            AddReward(
                distanceImprovement * progressRewardScale
            );

            previousDistance = currentDistance;
        }

        episodeTimer += Time.fixedDeltaTime;

        if (trainingMode && episodeTimer >= maxEpisodeTime)
        {
            EndEpisode();
        }
    }

    public void StartNavigation()
    {
        if (trainingMode)
        {
            Debug.LogWarning(
                "StartNavigation is only for game mode."
            );
            return;
        }

        if (targetReached || navigationActive)
            return;

        if (target == null)
        {
            Debug.LogWarning(
                "Companion target is not assigned!"
            );
            return;
        }

        navigationActive = true;
        Debug.Log("Companion navigation started!");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Finish") || targetReached)
            return;

        if (!trainingMode && !navigationActive)
            return;

        targetReached = true;

        if (trainingMode)
        {
            AddReward(successReward);
            EndEpisode();
        }
        else
        {
            navigationActive = false;
            rb.linearVelocity = Vector2.zero;

            Debug.Log(
                "Companion reached switch in game mode!"
            );
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActions = actionsOut.DiscreteActions;

        discreteActions[0] = 1;
        discreteActions[1] = 1;

        if (Input.GetKey(KeyCode.A))
            discreteActions[0] = 0;

        if (Input.GetKey(KeyCode.D))
            discreteActions[0] = 2;

        if (Input.GetKey(KeyCode.S))
            discreteActions[1] = 0;

        if (Input.GetKey(KeyCode.W))
            discreteActions[1] = 2;
    }
}
