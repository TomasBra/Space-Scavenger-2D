using UnityEngine;

public class PauseGame : MonoBehaviour
{
    public GameObject player;

    void Pause()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Time.timeScale == 1)
            {
                Time.timeScale = 0; 
                player.SetActive(false); 
            }
            else
            {
                Time.timeScale = 1; 
                player.SetActive(true); 
            }
        }
    }
}
