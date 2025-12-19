using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class BotNav : MonoBehaviour
{
    [Header("Target")]
    public Transform finishGameObject;

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float slowSpeed = 2f;
    public float rotationSpeed = 10f;

    [Header("Jump & Gravity")]
    public float jumpHeight = 1.2f;
    public float gravity = -9.81f;

    [Header("AI Settings")]
    public float groundCheckDistance = 1.5f;
    public float sideCheckOffset = 0.8f;
    public float obstacleCheckDistance = 1.2f;
    public float waitBarrierTime = 0.8f;

    private CharacterController controller;
    private Animator anim;
    private Vector3 velocity;

    private bool isGrounded;
    private bool isInSlowZone;
    private bool isFinish;

    private float currentSpeed;
    private float barrierWaitTimer;

    // ===== CHECKPOINT SYSTEM =====
    private enum Checkpoint
    {
        START,
        BOTQOQ1,
        TUNEL1,
        TUNEL2,
        BOTQOQ2,
        BARRIER
    }

    private Checkpoint currentCheckpoint = Checkpoint.START;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        currentSpeed = moveSpeed;
        velocity.y = -2f;
    }

    void Update()
    {
        if (isFinish) return;

        // ===== FINISH =====
        if (finishGameObject &&
            Vector3.Distance(transform.position, finishGameObject.position) < 1.5f)
        {
            isFinish = true;
            anim.Play("Dancing");
            return;
        }

        // ===== GROUND =====
        isGrounded = controller.isGrounded;
        anim.SetBool("isGrounded", isGrounded);

        if (isGrounded && velocity.y < 0)
            velocity.y = -5f;

        Vector3 aiDir = GetAIDirection();

        // ===== MOVE =====
        anim.SetFloat("speed", aiDir.magnitude);

        if (aiDir.magnitude > 0.1f)
        {
            Quaternion rot = Quaternion.LookRotation(aiDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, rotationSpeed * Time.deltaTime);
        }

        Vector3 move = transform.forward * currentSpeed;

        // ===== GRAVITY =====
        velocity.y += gravity * Time.deltaTime;
        controller.Move((move + velocity) * Time.deltaTime);

        // ===== SPEED =====
        currentSpeed = isInSlowZone ? slowSpeed : moveSpeed;
        isInSlowZone = false;
    }

    // ================= AI BRAIN =================
    Vector3 GetAIDirection()
    {
        Vector3 dir = (finishGameObject.position - transform.position);
        dir.y = 0;
        dir.Normalize();

        // ===== 1. JARLIK CHECK =====
        if (isGrounded && !HasGround(transform.position + transform.forward * sideCheckOffset))
        {
            bool left = HasGround(transform.position - transform.right * sideCheckOffset);
            bool right = HasGround(transform.position + transform.right * sideCheckOffset);

            if (left) return -transform.right;
            if (right) return transform.right;

            Jump(); // risk
        }

        // ===== 2. BARRIER CHECK =====
        if (Physics.Raycast(transform.position + Vector3.up * 0.5f,
                            transform.forward,
                            out RaycastHit hit,
                            obstacleCheckDistance))
        {
            if (hit.collider.CompareTag("Barrier"))
            {
                barrierWaitTimer += Time.deltaTime;
                if (barrierWaitTimer < waitBarrierTime)
                    return Vector3.zero; // kutadi
                else
                    barrierWaitTimer = 0f;
            }

            if (isGrounded)
                Jump();
        }

        return dir;
    }

    bool HasGround(Vector3 pos)
    {
        return Physics.Raycast(pos + Vector3.up * 0.5f,
                               Vector3.down,
                               groundCheckDistance);
    }

    void Jump()
    {
        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        anim.SetTrigger("Jump");
    }

    // ================= COLLISIONS =================
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("SlowZone"))
        {
            isInSlowZone = true;
            currentCheckpoint = Checkpoint.BOTQOQ1;
        }

        if (hit.gameObject.CompareTag("Tunel1"))
            currentCheckpoint = Checkpoint.TUNEL1;

        if (hit.gameObject.CompareTag("Tunel2"))
            currentCheckpoint = Checkpoint.TUNEL2;

        if (hit.gameObject.CompareTag("Botqoq2"))
        {
            isInSlowZone = true;
            currentCheckpoint = Checkpoint.BOTQOQ2;
        }

        if (hit.gameObject.CompareTag("Barrier"))
            currentCheckpoint = Checkpoint.BARRIER;

        if (hit.gameObject.CompareTag("DeathZone"))
            Respawn();
    }

    // ================= RESPAWN =================
    void Respawn()
    {
        controller.enabled = false;

        switch (currentCheckpoint)
        {
            case Checkpoint.START:
                transform.position = new Vector3(251.5f, 10.4f, -117.3f);
                break;
            case Checkpoint.BOTQOQ1:
                transform.position = new Vector3(251.24f, 10.5f, 0f);
                break;
            case Checkpoint.TUNEL1:
                transform.position = new Vector3(250.57f, 10.5f, -135.6f);
                break;
            case Checkpoint.TUNEL2:
                transform.position = new Vector3(250.42f, 10.5f, -160.83f);
                break;
            case Checkpoint.BOTQOQ2:
                transform.position = new Vector3(248.89f, 10.49f, -188.6f);
                break;
            case Checkpoint.BARRIER:
                transform.position = new Vector3(245.8f, 15.58f, -235.27f);
                break;
        }

        controller.enabled = true;
        velocity = Vector3.zero;
    }
}
