using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseGame : GameObject2D
{
    
    public GameObject canvas;
    public GameObject UpgradeMenuCanvas;
    public MapManager mapManager;

    void Update()
    {
        base.Update();
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Time.timeScale == 1)
            {
                if (UpgradeMenuCanvas.active) return;
                Time.timeScale = 0; 
                if(mapManager.currentGameState != MapManager.GameState.LANDING)
                {
                    player.GetComponent<SpriteRenderer>().enabled = false;
                    game.Pause();
                }
                    
                
                canvas.SetActive(true);
                //player.SetActive(false); 
            }
            else
            {
                if (UpgradeMenuCanvas.active) return;
                Time.timeScale = 1;
                if (mapManager.currentGameState != MapManager.GameState.LANDING)
                {
                    player.GetComponent<SpriteRenderer>().enabled = true;
                    game.Resume();
                }
                    

                canvas.SetActive(false);
                //player.SetActive(true); 
            }
        }
    }

    public void ContinueGame()
    {
         if (Time.timeScale == 1)
            {
                Time.timeScale = 0; 
                if(mapManager.currentGameState != MapManager.GameState.LANDING)
                {
                    player.GetComponent<SpriteRenderer>().enabled = false;
                    game.Pause();
                }
                    
                
                canvas.SetActive(true);
                //player.SetActive(false); 
            }
            else
            {
                Time.timeScale = 1;
                if (mapManager.currentGameState != MapManager.GameState.LANDING)
                {
                    player.GetComponent<SpriteRenderer>().enabled = true;
                    game.Resume();
                }
                    

                canvas.SetActive(false);
                //player.SetActive(true); 
            }
    }
    public void LeaveGame() 
    {
        Time.timeScale = 1;
        SceneManager.LoadSceneAsync(0); //< nacteni sceny !
    }
}
