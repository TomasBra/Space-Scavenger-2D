using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Tilemaps;
using static TileData;

public class Playah : Health
{
    private const string TILEMAP_TAG = "TileMap";
    private const string ENEMY_TAG = "Enemy";
    private const string PROJECTILE_TAG = "Projectile";

    private const float LASER_DISTANCE = 5;
    private const float MINING_DAMAGE_PER_SECOND = 5;
    private const float ENEMY_DAMAGE_PER_SECOND = 5;

    public int ironOre = 0;
    public int copperOre = 0;
    public int goldOre = 0;


    [SerializeField]
    private float SPEED;

    [SerializeField]
    private GameObject MapManager;

    [SerializeField]
    private GameObject FirePoint;

    [SerializeField]
    private GameObject StartVFX;

    [SerializeField]
    private GameObject EndVFX;

    [SerializeField]
    private HealthBar healthBar; //ukazatel zivota hrace

    [SerializeField]
    private ItemCounter itemCounter;

    private List<ParticleSystem> laserStartParticleSystems = new List<ParticleSystem>();
    private List<ParticleSystem> laserEndParticleSystems = new List<ParticleSystem>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    public void Start()
    {
        base.Start();
        healthBar.SetMaxHealth(maxHP);


        InitParticleSystems();
    }

    // Update is called once per frame
    public void Update()
    {
        base.Update();
        //DebugUI(); // testovaci funkce, potom smazat


        Vector2 direction = new Vector2(0.0f, 0.0f);
        if (Input.GetKey(KeyCode.D)) direction.x += 1.0f;
        if (Input.GetKey(KeyCode.A)) direction.x += -1.0f;
        if (Input.GetKey(KeyCode.W)) direction.y += 1.0f;
        if (Input.GetKey(KeyCode.S)) direction.y += -1.0f;

        direction.Normalize();

        if (rigidbody != null)
        {
            rigidbody.linearVelocity = direction * SPEED;
        }

        if (Input.GetMouseButton(0))
        {
            Laser2D();
        }
        else
        {
            GetComponent<LineRenderer>().positionCount = 0;
        }

        if (Input.GetMouseButtonDown(0))
        {
            StartLaserParticles();
        }
        if (Input.GetMouseButtonUp(0))
        {
            StopLaserParticles();
        }
    }


    public override bool TakeDamage(float damage, Vector2? knockbackDirection = null, bool destroyable = true)
    {
        bool shouldDie = base.TakeDamage(damage, knockbackDirection, false);
        healthBar.SetHealth(HP);
        return shouldDie;
    }

    void Laser2D()
    {
        Vector3 laserEndPosition;
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePosition - (Vector2)FirePoint.transform.position;
        direction.Normalize();

        StartVFX.transform.rotation = Quaternion.LookRotation(direction);

        laserEndPosition = shorterVector(FirePoint.transform.position + new Vector3(direction.x, direction.y, 0) * LASER_DISTANCE, mousePosition, FirePoint.transform.position);
        float distance = (laserEndPosition - FirePoint.transform.position).magnitude;

        RaycastHit2D hit = Physics2D.Raycast(FirePoint.transform.position, direction, distance);

        if (hit.collider != null)
        {
            laserEndPosition = hit.point;
        }

        EndVFX.transform.position = laserEndPosition;
        DrawLaser(FirePoint.transform.position, laserEndPosition);

        if (hit.collider == null)
            return;

        Health health;

        switch (hit.transform.gameObject.tag)
        {
            case TILEMAP_TAG:
                const float MAGIC = 0.02f;

                Vector3 tempHitPoint = new Vector3(hit.point.x - Math.Sign(hit.point.x) * MAGIC / 2.0f, hit.point.y - Math.Sign(hit.point.y) * MAGIC / 2.0f, 0.0f);
                Vector3 primaryHitPoint;
                Vector3 secondaryHitPoint;

                if (direction.x > 0)
                {
                    if (direction.y > 0)
                    {
                        if (Math.Abs(direction.x) > Math.Abs(direction.y))
                        {
                            primaryHitPoint = new Vector3(tempHitPoint.x + MAGIC, tempHitPoint.y, 0);
                            secondaryHitPoint = new Vector3(tempHitPoint.x, tempHitPoint.y + MAGIC, 0);
                        }
                        else
                        {
                            primaryHitPoint = new Vector3(tempHitPoint.x, tempHitPoint.y + MAGIC, 0);
                            secondaryHitPoint = new Vector3(tempHitPoint.x + MAGIC, tempHitPoint.y, 0);
                        }
                    }
                    else
                    {
                        if (Math.Abs(direction.x) > Math.Abs(direction.y))
                        {
                            primaryHitPoint = new Vector3(tempHitPoint.x + MAGIC, tempHitPoint.y, 0);
                            secondaryHitPoint = new Vector3(tempHitPoint.x, tempHitPoint.y - MAGIC, 0);
                        }
                        else
                        {
                            primaryHitPoint = new Vector3(tempHitPoint.x, tempHitPoint.y - MAGIC, 0);
                            secondaryHitPoint = new Vector3(tempHitPoint.x + MAGIC, tempHitPoint.y, 0);
                        }
                    }
                }
                else
                {
                    if (direction.y > 0)
                    {
                        if (Math.Abs(direction.x) > Math.Abs(direction.y))
                        {
                            primaryHitPoint = new Vector3(tempHitPoint.x - MAGIC, tempHitPoint.y, 0);
                            secondaryHitPoint = new Vector3(tempHitPoint.x, tempHitPoint.y + MAGIC, 0);
                        }
                        else
                        {
                            primaryHitPoint = new Vector3(tempHitPoint.x, tempHitPoint.y + MAGIC, 0);
                            secondaryHitPoint = new Vector3(tempHitPoint.x - MAGIC, tempHitPoint.y, 0);
                        }
                    }
                    else
                    {
                        if (Math.Abs(direction.x) > Math.Abs(direction.y))
                        {
                            primaryHitPoint = new Vector3(tempHitPoint.x - MAGIC, tempHitPoint.y, 0);
                            secondaryHitPoint = new Vector3(tempHitPoint.x, tempHitPoint.y - MAGIC, 0);
                        }
                        else
                        {
                            primaryHitPoint = new Vector3(tempHitPoint.x, tempHitPoint.y - MAGIC, 0);
                            secondaryHitPoint = new Vector3(tempHitPoint.x - MAGIC, tempHitPoint.y, 0);
                        }
                    }
                }

                MapManager mm = MapManager.GetComponent<MapManager>();
                TileData? tile = mm.HitTile(primaryHitPoint, MINING_DAMAGE_PER_SECOND);
                if (tile == null)
                {
                    tile = mm.HitTile(secondaryHitPoint, MINING_DAMAGE_PER_SECOND);
                }

                ProcessTile(tile);
                break;

            case ENEMY_TAG:
                health = hit.transform.GetComponent<Health>();
                if (health != null)
                    health.TakeDamage(ENEMY_DAMAGE_PER_SECOND * Time.deltaTime, laserEndPosition - FirePoint.transform.position);
                break;

            case PROJECTILE_TAG:
                health = hit.transform.GetComponent<Health>();
                if (health != null)
                    health.TakeDamage(ENEMY_DAMAGE_PER_SECOND * Time.deltaTime, laserEndPosition - FirePoint.transform.position);
                break;


        }




    }

    private void ProcessTile(TileData? tile)
    {
        if (tile == null) return;
        switch (tile.type)
        {
            case TileType.DIRT:
                break;

            case TileType.IRON:
                ironOre += tile.materialAmount;
                itemCounter.SetIron(ironOre);
                break;

            case TileType.COPPER:
                copperOre += tile.materialAmount;
                itemCounter.SetCopper(copperOre);
                break;

            case TileType.GOLD:
                goldOre += tile.materialAmount;
                itemCounter.SetGold(goldOre);
                break;
        }
    }

    void DrawLaser(Vector3 startPostion, Vector3 endPosition) {
        GetComponent<LineRenderer>().positionCount = 2;
        GetComponent<LineRenderer>().SetPosition(0, startPostion);
        GetComponent<LineRenderer>().SetPosition(1, endPosition);
    }

    void InitParticleSystems()
    {
        for (int i = 0; i < StartVFX.transform.childCount; i++)
        {
            laserStartParticleSystems.Add(StartVFX.transform.GetChild(i).GetComponent<ParticleSystem>());
        }

        for (int i = 0; i < EndVFX.transform.childCount; i++)
        {
            laserEndParticleSystems.Add(EndVFX.transform.GetChild(i).GetComponent<ParticleSystem>());
        }
    }

    void StartLaserParticles()
    {

        foreach (ParticleSystem partSys in laserStartParticleSystems)
        {
            partSys.Play();
        }

        foreach (ParticleSystem partSys in laserEndParticleSystems)
        {
            partSys.Play();
        }
    }


    void StopLaserParticles()
    {
        foreach (ParticleSystem partSys in laserStartParticleSystems)
        {
            partSys.Stop();
        }

        foreach (ParticleSystem partSys in laserEndParticleSystems)
        {
            partSys.Stop();
        }
    }
}
