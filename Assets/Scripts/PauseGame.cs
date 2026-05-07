using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseGame : MonoBehaviour
{
    public GameObject player;
    public GameObject canvas;
    public MapManager mapManager;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Time.timeScale == 1)
            {
                Time.timeScale = 0; 
                if(mapManager.currentGameState != MapManager.GameState.LANDING)
                    player.GetComponent<Playah>().enabled = false;
                canvas.SetActive(true);
                //player.SetActive(false); 
            }
            else
            {
                Time.timeScale = 1;
                if (mapManager.currentGameState != MapManager.GameState.LANDING)
                    player.GetComponent<Playah>().enabled = true;
                canvas.SetActive(false);
                //player.SetActive(true); 
            }
        }
    }

    public void ContinueGame()
    {
        Time.timeScale = 1;
        player.GetComponent<Playah>().enabled = true;
        canvas.SetActive(false);
        //player.SetActive(true);
    }
    public void LeaveGame() 
    {
        Time.timeScale = 1;
        SceneManager.LoadSceneAsync(0); //< nacteni sceny !
    }
}
