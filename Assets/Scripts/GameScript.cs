using UnityEngine;

public class GameScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public bool running = true;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Pause()
    {
        running = false;
    }

    public void Resume()
    { 
        running = true;
    }
}
