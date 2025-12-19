using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;

    void Start() {
	    transform.position = new Vector3(target.position.x, target.position.y + 1.7f, target.position.z + 0.5f);
	    transform.rotation = Quaternion.Euler(34,180,0);
    }
    void Update() {	
 	    transform.position = new Vector3(target.position.x, target.position.y + 3.5f, target.position.z +2f);
    }
}
