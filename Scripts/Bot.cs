using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(CharacterController))]
public class Bot : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 4f;
    public float jumpHeight = 1.2f;
    public float gravity = -9.81f;
    public float rotationSpeed = 5f;

    [Header("Path")]
    public Transform[] waypoints;
    private int currentWaypoint = 0;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        velocity.y = -2f;
    }

    void Update()
    {
        if (waypoints.Length == 0) return;

        // ===== MOVE TOWARD CURRENT WAYPOINT =====
        Vector3 targetPos = waypoints[currentWaypoint].position;
        Vector3 dir = (targetPos - transform.position);
        dir.y = 0;

        // ==== ROTATE SMOOTHLY =====
        if (dir.magnitude > 0.1f)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }

        // ==== MOVE FORWARD =====
        Vector3 move = transform.forward * moveSpeed;

        // ===== GROUND CHECK =====
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        // ===== GRAVITY =====
        velocity.y += gravity * Time.deltaTime;

        // ===== JUMP IF NEEDED =====
        if (dir.magnitude < 1.5f) // waypointga yetib borish
        {
            currentWaypoint++;
            if (currentWaypoint >= waypoints.Length) currentWaypoint = waypoints.Length - 1;
        }

        controller.Move((move + velocity) * Time.deltaTime);
    }

    // ===== COLLISION TRIGGERS FOR JUMP =====
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // Tunnel ustidan o'tish yoki sakrash
        if (hit.gameObject.CompareTag("Tunel1") || hit.gameObject.CompareTag("Tunel2"))
        {
            if (isGrounded)
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Ko‘prik variantlari
        if (hit.gameObject.CompareTag("Ko'prik"))
        {
            // random tanlash
            // misol uchun yo‘l chap yoki o‘ng
            Vector3 randomOffset = (Random.value > 0.5f) ? Vector3.left : Vector3.right;
            transform.position += randomOffset;
        }
    }
}
