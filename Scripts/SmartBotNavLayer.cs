using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class SmartBotNavLayer : MonoBehaviour
{
    [Header("Waypoints & Spawn")]
    public Transform spawnPoint;
    public Transform[] waypoints; // Assign all important waypoints in order
    private int currentWaypointIndex = 0;

    [Header("Movement")]
    public float speed = 5f;
    public float rotationSpeed = 5f;
    public float jumpForce = 7f;
    public float gravity = -20f;
    public float groundCheckDistance = 1.5f;
    public float cliffCheckDistance = 2.5f;
    public float sideOffset = 0.7f;
    public float botAvoidRadius = 1.2f;

    [Header("Layers")]
    public LayerMask groundMask; // Ground layer
    public LayerMask botMask;    // Bot layer

    [Header("Slow / Crowd")]
    public float slowMultiplier = 0.5f;

    private CharacterController controller;
    private Animator animator;
    private Vector3 velocity;
    private float currentSpeed;
    private float randomTimer;
    private float horizontalMove;
    private bool isInSlowZone;

    private enum BotState { Running, Finished, Dead }
    private BotState state = BotState.Running;

    private enum Checkpoint { START, BOTQOQ1, TUNEL1, TUNEL2, BOTQOQ2, BARRIER }
    private Checkpoint currentCheckpoint = Checkpoint.START;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        currentSpeed = speed;
        randomTimer = Random.Range(0.5f, 1.5f);
    }

    void Update()
    {
        if (state != BotState.Running || waypoints.Length == 0) return;

        CheckFinish();
        MoveAlongWaypoints();
        ApplyGravity();
    }

    void CheckFinish()
    {
        if (currentWaypointIndex >= waypoints.Length)
        {
            state = BotState.Finished;
            controller.enabled = false;
            animator.Play("Dancing");
        }
    }

    void MoveAlongWaypoints()
    {
        if (currentWaypointIndex >= waypoints.Length) return;

        Transform target = waypoints[currentWaypointIndex];
        Vector3 dir = (target.position - transform.position).normalized;
        dir.y = 0;

        // Smooth rotation toward target
        if (dir.magnitude > 0.1f)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }

        // Crowd avoidance
        Collider[] botsNearby = Physics.OverlapSphere(transform.position, botAvoidRadius, botMask);
        Vector3 avoidDir = Vector3.zero;
        foreach (var bot in botsNearby)
        {
            if (bot.gameObject != this.gameObject)
            {
                Vector3 away = transform.position - bot.transform.position;
                avoidDir += away.normalized / away.magnitude;
            }
        }

        // Random lateral movement
        randomTimer -= Time.deltaTime;
        if (randomTimer <= 0)
        {
            horizontalMove = Random.Range(-0.3f, 0.3f);
            randomTimer = Random.Range(0.5f, 1.2f);
        }

        Vector3 moveDir = (dir + transform.right * horizontalMove + avoidDir).normalized;

        // Decide whether to jump or move forward
        if (IsPathSafe(moveDir))
        {
            currentSpeed = isInSlowZone ? speed * slowMultiplier : speed;
            controller.Move(transform.forward * currentSpeed * Time.deltaTime);
            animator.SetFloat("speed", currentSpeed);
        }
        else
        {
            Jump();
        }

        // Check if reached waypoint
        if (Vector3.Distance(transform.position, target.position) < 0.5f)
        {
            currentWaypointIndex++;
        }
    }

    bool IsPathSafe(Vector3 direction)
    {
        Vector3 checkPos = transform.position + direction * cliffCheckDistance + Vector3.up * 0.5f;
        // Return true if ground exists ahead
        return Physics.Raycast(checkPos, Vector3.down, groundCheckDistance, groundMask);
    }

    void Jump()
    {
        if (controller.isGrounded)
        {
            velocity.y = jumpForce;
            animator.SetTrigger("Jump");
        }
    }

    void ApplyGravity()
    {
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        if (controller.isGrounded && velocity.y < 0) velocity.y = -2f;

        if (transform.position.y < 0f) Die();
    }

    // ===== COLLISIONS & CHECKPOINTS =====
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        string tag = hit.gameObject.tag;

        switch (tag)
        {
            case "SlowZone":
                isInSlowZone = true;
                currentCheckpoint = Checkpoint.BOTQOQ1;
                break;
            case "Botqoq2":
                isInSlowZone = true;
                currentCheckpoint = Checkpoint.BOTQOQ2;
                break;
            case "Tunel1": currentCheckpoint = Checkpoint.TUNEL1; break;
            case "Tunel2": currentCheckpoint = Checkpoint.TUNEL2; break;
            case "Barrier": currentCheckpoint = Checkpoint.BARRIER; break;
            case "DeathZone": Die(); break;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("SlowZone") || other.CompareTag("Botqoq2")) isInSlowZone = false;
    }

    void Die()
    {
        if (state == BotState.Dead) return;
        state = BotState.Dead;
        controller.enabled = false;
        Invoke(nameof(Respawn), 1f);
    }

    void Respawn()
    {
        Vector3 respawnPos = spawnPoint.position;
        switch (currentCheckpoint)
        {
            case Checkpoint.START: respawnPos = spawnPoint.position; break;
            case Checkpoint.BOTQOQ1: respawnPos = new Vector3(251.24f, 10.5f, 0f); break;
            case Checkpoint.TUNEL1: respawnPos = new Vector3(250.57f, 10.5f, -135.6f); break;
            case Checkpoint.TUNEL2: respawnPos = new Vector3(250.42f, 10.5f, -160.83f); break;
            case Checkpoint.BOTQOQ2: respawnPos = new Vector3(248.89f, 10.49f, -188.6f); break;
            case Checkpoint.BARRIER: respawnPos = new Vector3(245.8f, 15.58f, -235.27f); break;
        }

        transform.position = respawnPos;
        velocity = Vector3.zero;
        controller.enabled = true;
        state = BotState.Running;
    }
}
