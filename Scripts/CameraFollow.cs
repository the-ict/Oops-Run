using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;

    void Start() {
	    transform.position = new Vector3(target.position.x, target.position.y + 1.5f, target.position.z + 1f);
        transform.rotation = Quaternion.Euler(29.897f,180.52f,0f);
    }
    void Update() {	
	    transform.position = new Vector3(target.position.x, target.position.y + 1.5f, target.position.z + 1f);
        transform.rotation = Quaternion.Euler(29.897f,180.52f,0f);
    }
}
