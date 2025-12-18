using UnityEngine;

public class RotatingObstacle : MonoBehaviour
{
    public float rotationSpeed = 50f; 
    public bool reverse = false;

    private float currentX = -81.589f;

    void Update()
    {
        float speed = reverse ? -rotationSpeed : rotationSpeed;

        currentX += speed * Time.deltaTime;
        transform.rotation = Quaternion.Euler(0,0, currentX);
    }

    private void OnTriggerEnter(Collider other) {
        Debug.Log("this is collider.");
    }
}
