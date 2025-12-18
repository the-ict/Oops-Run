using UnityEngine;

public class Barrier : MonoBehaviour
{
    [Header("Movement Settings")]
    public Vector3 moveDirection = Vector3.down;  
    public float moveDistance = 3f;             
    public float moveSpeed = 2f;              
    public float startDelay = 0f; 

    private Vector3 startPos;
    private Vector3 targetPos;
    private bool movingForward = false;
    private float timer = 0f;

    void Start()
    {
        startPos = transform.position;
        targetPos = startPos + moveDirection.normalized * moveDistance;
    }

    void Update()
    {
        // Delay timer
        if(timer < startDelay)
        {
            timer += Time.deltaTime;
            return;
        }

        Vector3 nextPos = movingForward ? targetPos : startPos;

        transform.position = Vector3.MoveTowards(transform.position, nextPos, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, nextPos) < 0.01f)
            movingForward = !movingForward;
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if(hit.gameObject.CompareTag("Player"))
        {
            hit.transform.parent = transform;
        }
    }

    void OnControllerColliderExit(ControllerColliderHit hit)
    {
        if(hit.gameObject.CompareTag("Player"))
        {
            hit.transform.parent = null;
        }
    }
}
