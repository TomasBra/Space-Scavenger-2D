using System.Collections.Generic;
using UnityEngine;

public class Optimalization : MonoBehaviour
{
    public List<GameObject> enemies = new List<GameObject>();
    protected GameObject player;

    private float lastCheckTime;

    [SerializeField]
    private float checkInterval;

    [SerializeField]
    private float enableDistance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        lastCheckTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (Time.time - lastCheckTime > checkInterval)
        {
            Vector2 playerPosition = player.transform.position;
            enemies.RemoveAll(enemy => enemy == null);

            for (int i = 0; i < enemies.Count; i++) { 
                Vector2 enemyPosition = enemies[i].transform.position;
                if (Vector2.Distance(playerPosition, enemyPosition) < enableDistance)
                {
                    enemies[i].SetActive(true);
                }
                else
                {
                    enemies[i].SetActive(false);
                }
            }

            lastCheckTime = Time.time;
        }

    }
}
