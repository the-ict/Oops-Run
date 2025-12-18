using UnityEngine;

public class MovingObstacle : MonoBehaviour
{
    public float moveDistance = 0.8f;      // Tepaga/pastga qancha harakat qiladi
    public float moveSpeed = 0.5f;         // Harakat tezligi
    public float startDelay = 0f;          // Boshlanish vaqti (s)
    
    private Vector3 startPos;
    private bool goingUp = true;
    private float timer = 0f;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        // Delay timer
        if (timer < startDelay)
        {
            timer += Time.deltaTime;
            return;
        }

        Vector3 targetPos = goingUp ? startPos + Vector3.up * moveDistance : startPos;

        // Smooth movement
        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

        // Yo‘nalishni o‘zgartirish
        if (Vector3.Distance(transform.position, targetPos) < 0.01f)
            goingUp = !goingUp;
    }
}
