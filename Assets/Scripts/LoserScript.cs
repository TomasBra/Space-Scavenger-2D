using UnityEngine;
using UnityEngine.SceneManagement;

public class LoserScript : MonoBehaviour
{
    public void RestartGame()
    {
        SceneManager.LoadSceneAsync(1); //< nacteni sceny !
    }

    public void LoserGaveUp()
    {
        SceneManager.LoadSceneAsync(0); //< nacteni sceny !
    }
}