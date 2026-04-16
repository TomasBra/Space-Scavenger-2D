using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class Queen : Health
{
    [SerializeField]
    private float SPAWN_COOL_DOWN; //v sekundach

    [SerializeField]
    private GameObject SpawnableObject;

    [SerializeField]
    public float lastSpawnTime;

    protected string[] layersToIgnore = new string[] { "Enemies", "PlayerProjectiles", "EnemyProjectiles" };

    private LayerMask raycastIgnoreMask;

    private MapManager mapManager;

    protected const float MAX_TRIGGER_DISTANCE = 11;
    public bool isTriggered = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start()
    {
        base.Start();
        lastSpawnTime = Time.time;
        mapManager = GameObject.FindGameObjectWithTag(MAP_MANAGER_TAG).GetComponent<MapManager>();

        raycastIgnoreMask = ~(LayerMask.GetMask(layersToIgnore));
    }

    // Update is called once per frame
    public void Update()
    {
        base.Update();

        if (dead)
            return;

        Vector2 toPlayer = player.transform.position - this.transform.position;
        float playerDistance = toPlayer.magnitude;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, toPlayer, Mathf.Infinity, raycastIgnoreMask);

        if (!isTriggered)
        {
            // trigger if player is in sight
            if (hit.collider.transform.tag == PLAYER_TAG)
            {
                isTriggered = true;
            }
        }
        else
        {
            // untrigger
            if (playerDistance > MAX_TRIGGER_DISTANCE && hit.collider.transform.tag != PLAYER_TAG)
            {
                isTriggered = false;
            }
            else if ((Time.time - lastSpawnTime) >= SPAWN_COOL_DOWN)
            {
                lastSpawnTime = Time.time;
                Spawn(SpawnableObject);
            }
        }
    }

    void Spawn(GameObject objectToSpawn)
    {
        if (objectToSpawn != null)
        {
            //vystøelení a natoèení projektilu správným smìrem
            GameObject enemy = Instantiate(objectToSpawn, position: transform.position, rotation: Quaternion.identity);
            enemy.GetComponent<Enemy>().isTriggered = true;
        }
    }

    public override bool TakeDamage(float damage, Vector2? knockbackDirection = null, bool destroyable = true)
    {
        bool dead = base.TakeDamage(damage, knockbackDirection, destroyable);

        //pokud neni mrtva a dostal jsem damage, tak snizim zbyvajici cas do spawnuti
        if (!dead)
        {
            lastSpawnTime -= damage * Mathf.Min(maxHP / HP, 10); //koeficient od 1 do 10
        }
        else
        {
            mapManager.QueenKilled();
        }

        return dead;
    }
}
