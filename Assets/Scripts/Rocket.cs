using System;
using Unity.VisualScripting;
using UnityEngine;

public class Rocket : GameObject2D
{
    private float landingFunc(float time)
    {
        float relativeTime = time / LANDING_TIME;

        return Mathf.Pow(relativeTime - 1.0f, 2);
    }

    [SerializeField]
    private GameObject mapManager;

    private const float MAX_HEIGHT = 20;
    private const float MIN_HEIGHT = 3.0f;
    public const float LANDING_TIME = 8.0f;
    public const float TAKEOFF_TIME = 12.0f;

    private float startTime;

    [SerializeField]
    private ParticleSystem smoke;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        base.Start();
        startTime = Time.time;
        player.GetComponent<SpriteRenderer>().enabled = false;
        player.GetComponent<Playah>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
        MapManager mm = mapManager.GetComponent<MapManager>();
        float time = Time.time - startTime;
        float y = MIN_HEIGHT;
        if (mm.currentGameState == MapManager.GameState.LANDING)
        {
            if (time >= LANDING_TIME)
            {
                mm.currentGameState = MapManager.GameState.PLAY;
                smoke.Stop();
                player.GetComponent<SpriteRenderer>().enabled = true;
                player.GetComponent<Playah>().enabled = true;
            }
            else
            {
                float relativeHeight = landingFunc(time);
                y = MIN_HEIGHT + (MAX_HEIGHT - MIN_HEIGHT) * relativeHeight;
                transform.position = new Vector3(MapManager.MAP_WIDTH / 2.0f - 2.5f, y, 0.0f);

                // relative height minimum 0, maximum 1
                // gravity minimum -0.1, maximum 0.4

                smoke.gravityModifier = -0.6f * Mathf.Pow(((1 - relativeHeight) / 1.1f), 4);
                smoke.startLifetime = -2.9282f * Mathf.Pow((1f - relativeHeight) / 1.1f, 4) + 2.5f;
            }
        }
        else if (mm.currentGameState == MapManager.GameState.TAKEOFF)
        {
            if (time > TAKEOFF_TIME)
            {
                mm.GoToWinScreen();
            }
            else
            {
                float relativeHeight = 1 - landingFunc(time);
                y = MIN_HEIGHT + (MAX_HEIGHT - MIN_HEIGHT) * relativeHeight;
                transform.position = new Vector3(MapManager.MAP_WIDTH / 2.0f - 2.5f, y, 0.0f);

                // relative height minimum 0, maximum 1
                // gravity minimum -0.1, maximum 0.4

                smoke.gravityModifier = -0.6f * Mathf.Pow(((1 - relativeHeight) / 1.1f), 4);
                smoke.startLifetime = -2.9282f * Mathf.Pow((1f - relativeHeight) / 1.1f, 4) + 2.5f;
            }
        }

    }

    public void TakeOff()
    {
        smoke.Play();
        startTime = Time.time;
    }
}
