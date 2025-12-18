using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class SmartBotNavV2 : MonoBehaviour
{
    [Header("Target & Spawn")]
    public Transform finishPoint;
    public Transform spawnPoint;

    [Header("Movement")]
    public float speed = 5f;
    public float jumpForce = 7f;
    public float gravity = -20f;
    public float groundCheckDistance = 1.5f;
    public float cliffCheckDistance = 2.5f;
    public float sideOffset = 0.7f;
    public float botAvoidRadius = 1.2f; // radius to avoid other bots

    [Header("Slow / Crowded")]
    public float slowMultiplier = 0.5f;
    public LayerMask groundMask;
    public LayerMask botMask;

    private CharacterController controller;
    private Animator animator;
    private Vector3 velocity;
    private float currentSpeed;
    private float horizontalMove;
    private float randomTimer;
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
        if (state != BotState.Running || finishPoint == null) return;

        CheckFinish();
        MoveSmart();
        ApplyGravity();
    }

    // ===== FINISH CHECK =====
    void CheckFinish()
    {
        if (Vector3.Distance(transform.position, finishPoint.position) < 0.5f)
        {
            state = BotState.Finished;
            controller.enabled = false;
            animator.Play("Dancing");
        }
    }

    // ===== SMART MOVEMENT =====
    void MoveSmart()
    {
        Vector3 dir = (finishPoint.position - transform.position).normalized;
        dir.y = 0;

        // Avoid other bots
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

        HandleGroundAndCliff(moveDir);

        currentSpeed = isInSlowZone ? speed * slowMultiplier : speed;

        controller.Move(moveDir * currentSpeed * Time.deltaTime);
        animator.SetFloat("speed", currentSpeed);

        if (moveDir.magnitude > 0.1f)
            transform.forward = Vector3.Lerp(transform.forward, moveDir, Time.deltaTime * 5f);
    }

    // ===== GROUND / CLIFF / JUMP =====
    void HandleGroundAndCliff(Vector3 move)
    {
        Vector3 forwardPos = transform.position + Vector3.up * 0.5f + transform.forward * cliffCheckDistance;

        // Check for cliff ahead
        if (!HasGround(forwardPos))
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position + Vector3.up * 0.5f, transform.forward, out hit, cliffCheckDistance))
            {
                if (!hit.collider.CompareTag("Ground"))
                    Jump();
            }
            else Jump();
            return;
        }

        // Side checks for obstacles
        Vector3 leftPos = transform.position + Vector3.up * 0.5f - transform.right * sideOffset;
        Vector3 rightPos = transform.position + Vector3.up * 0.5f + transform.right * sideOffset;

        bool leftGround = HasGround(leftPos);
        bool rightGround = HasGround(rightPos);

        if (!leftGround && rightGround) RotateSmooth(-90);
        else if (!rightGround && leftGround) RotateSmooth(90);

        animator.SetBool("isGrounded", controller.isGrounded);
        if (controller.isGrounded && velocity.y < 0) velocity.y = -2f;
    }

    void ApplyGravity()
    {
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        if (transform.position.y < 0f) Die();
    }

    bool HasGround(Vector3 pos)
    {
        return Physics.Raycast(pos, Vector3.down, groundCheckDistance, groundMask);
    }

    void Jump()
    {
        velocity.y = jumpForce;
        animator.SetTrigger("Jump");
    }

    void RotateSmooth(float angle)
    {
        Quaternion target = Quaternion.Euler(0, transform.eulerAngles.y + angle, 0);
        transform.rotation = Quaternion.Lerp(transform.rotation, target, Time.deltaTime * 6f);
    }

    // ===== COLLISIONS / CHECKPOINTS =====
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
            case "Tunel1":
                currentCheckpoint = Checkpoint.TUNEL1;
                break;
            case "Tunel2":
                currentCheckpoint = Checkpoint.TUNEL2;
                break;
            case "Barrier":
                currentCheckpoint = Checkpoint.BARRIER;
                break;
            case "DeathZone":
                Die();
                break;
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
