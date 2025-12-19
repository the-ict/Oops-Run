using UnityEngine;
using UnityEngine.SceneManagement;


public class Finish : MonoBehaviour
{
    void Start()
    { 
       
    }

    void Update()
    {
        
    }

    public void RestartGame() {
 	Time.timeScale = 1f;
	SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);	  
    }
}
