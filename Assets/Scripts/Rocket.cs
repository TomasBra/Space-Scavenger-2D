using System;
using Unity.VisualScripting;
using UnityEngine;

public class Rocket : MonoBehaviour
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

    private float startTime;

    [SerializeField]
    private ParticleSystem smoke;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        MapManager mm = mapManager.GetComponent<MapManager>();
        float time = Time.time - startTime;
        float y = MIN_HEIGHT;
        if (mm.currentGameState == MapManager.GameState.LANDING)
        {
            if (time >= LANDING_TIME)
            {
                mm.currentGameState = MapManager.GameState.PLAY;
                smoke.Stop();
            }
            else
            {
                float relativeHeight = landingFunc(time);
                y = MIN_HEIGHT + (MAX_HEIGHT - MIN_HEIGHT) * relativeHeight;
                transform.position = new Vector3(MapManager.MAP_WIDTH / 2.0f - 2.5f, y, 0.0f);

                // relative height minimum 0, maximum 1
                // gravity minimum -0.1, maximum 0.4

                float gravity = -0.6f *Mathf.Pow(((1-relativeHeight)/1.1f), 4);
                smoke.gravityModifier = gravity;
                smoke.startLifetime = -2.9282f * Mathf.Pow((1f - relativeHeight) / 1.1f, 4) + 2.5f;
            }
        }

    }
}
