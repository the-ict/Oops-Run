using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class BotNav : MonoBehaviour
{
    [Header("Target")]
    public Transform finishPoint;
    public Transform spawnPoint;

    [Header("Movement")]
    public float speed = 5f;
    public float jumpForce = 7f;
    public float gravity = -20f;
    public float groundCheckDistance = 1.5f;
    public float sideOffset = 0.7f;

    enum BotState
{
    Running,
    Finished,
    Dead
}


    [Header("Slow")]
    public float slowMultiplier = 0.5f;

    CharacterController controller;
    Animator animator;
    Vector3 velocity;

    float randomTimer;
    float horizontalMove;
    float currentSpeed;
    bool isInSlowZone;

    BotState state = BotState.Running;

    int finishLayer;
    int slowLayer;
    int deathLayer;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        finishLayer = LayerMask.NameToLayer("Finish");
        slowLayer   = LayerMask.NameToLayer("SlowZone");
        deathLayer  = LayerMask.NameToLayer("DeathZone");
    }

    void Start()
    {
        currentSpeed = speed;
        randomTimer = Random.Range(0.5f, 1.5f);
    }

    void Update()
    {
        if (state != BotState.Running) return;
        if (finishPoint == null) return;

        CheckFinish();
        MoveToFinish();
        ApplyGravity();
    }

    // =============================
    // FINISH CHECK
    // =============================
    void CheckFinish()
    {
        if (Vector3.Distance(transform.position, finishPoint.position) < 0.5f)
        {
            state = BotState.Finished;
            controller.enabled = false;
            animator.Play("Dancing");
        }
    }

    // =============================
    // MAIN MOVEMENT
    // =============================
    void MoveToFinish()
    {
        Vector3 dir = (finishPoint.position - transform.position).normalized;
        dir.y = 0;

        randomTimer -= Time.deltaTime;
        if (randomTimer <= 0)
        {
            horizontalMove = Random.Range(-0.8f, 0.8f);
            randomTimer = Random.Range(0.5f, 1.5f);
        }

        Vector3 move = (dir + transform.right * horizontalMove).normalized;

        HandleGroundCheck(move);

        currentSpeed = isInSlowZone ? speed * slowMultiplier : speed;

        controller.Move(move * currentSpeed * Time.deltaTime);
        animator.SetFloat("speed", currentSpeed);

        if (move.magnitude > 0.1f)
            transform.forward = Vector3.Lerp(transform.forward, move, Time.deltaTime * 5f);
    }

    // =============================
    // GROUND / EDGE CHECK
    // =============================
    void HandleGroundCheck(Vector3 move)
    {
        if (!controller.isGrounded) return;

        velocity.y = -2f;
        animator.SetBool("isGrounded", true);

        Vector3 forwardPos = transform.position + Vector3.up * 0.5f + transform.forward * 0.5f;
        if (!HasGround(forwardPos))
        {
            Vector3 leftPos = transform.position + Vector3.up * 0.5f - transform.right * sideOffset;
            Vector3 rightPos = transform.position + Vector3.up * 0.5f + transform.right * sideOffset;

            bool left = HasGround(leftPos);
            bool right = HasGround(rightPos);

            if (left && !right) RotateSmooth(-90);
            else if (right && !left) RotateSmooth(90);
            else Jump();
        }
    }

    void ApplyGravity()
    {
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    bool HasGround(Vector3 pos)
    {
        return Physics.Raycast(pos, Vector3.down, groundCheckDistance, LayerMask.GetMask("Ground"));
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

    // =============================
    // COLLISION
    // =============================
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        int layer = hit.gameObject.layer;

        if (layer == slowLayer)
            isInSlowZone = true;

        if (layer == deathLayer)
            Die();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == slowLayer)
            isInSlowZone = false;
    }

    void Die()
    {
        state = BotState.Dead;
        controller.enabled = false;

        Invoke(nameof(Respawn), 1.2f);
    }

    void Respawn()
    {
        transform.position = spawnPoint.position;
        velocity = Vector3.zero;
        controller.enabled = true;
        state = BotState.Running;
    }
}

