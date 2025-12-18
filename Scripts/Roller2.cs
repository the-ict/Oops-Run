using UnityEngine;

public class Roller2 : MonoBehaviour
{
    public float rotationSpeed = 50f;

    private float currentX = 81.589f;

    void Update()
    {
        // currentX ni frame by frame oshirib borish
        currentX += rotationSpeed * Time.deltaTime;

        // transform rotation
        transform.rotation = Quaternion.Euler(currentX, 90, 90); // x o‘qi bo‘yicha
    }

    private void onTriggerEnter(Collider other) {
        Debug.Log("this is collider.");
    }
}

