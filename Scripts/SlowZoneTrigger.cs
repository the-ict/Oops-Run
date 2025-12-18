using UnityEngine;
using System.Collections;


public class SlowZoneTrigger : MonoBehaviour
{     
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
	

        
    }

    void OnTriggerEnter (Collider other) {
	Debug.Log("Collider name" + other.name);
    }
};
