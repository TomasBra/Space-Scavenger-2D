using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Projectile : GameObject2D
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created



    [SerializeField]
    public AnimationClip ExplosionAnimationClip;

    [SerializeField]
    public float lifeTime = 3;

    [SerializeField]
    public float speed = 3;

    private DateTime spawnTime;


    [SerializeField]
    private List<string> tagsExplosionDealDamage = new List<string>(); //zde jsou vsechny tagy, kterym dealujeme damage pri explozi
    [SerializeField]
    private List<string> tagsToIgnore = new List<string>();
    [SerializeField]
    private List<string> layersToIgnore= new List<string>();

    //private GameObject target;

    [SerializeField]
    public float damage = 5;

    [SerializeField]
    public float mining_damage = 10;

    public Vector2 direction;

    public int bounces = 0;

    public float explosion_radius = 10;

    private float explosion_offset;

    private bool dead = false;

    void Start()
    {
        base.Start();
        spawnTime = DateTime.Now;
        SetUpIngoreLayer();

        if (ExplosionAnimationClip != null)
            explosion_offset = ExplosionAnimationClip.length;
        else
            explosion_offset = 0;
    }

    void SetUpIngoreLayer()
    {
        for (int i = 0; i < layersToIgnore.Count; i++)
        {
            Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer(layersToIgnore[i]));
            Physics.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer(layersToIgnore[i]));
        }
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
        if ((DateTime.Now - spawnTime).TotalSeconds > lifeTime)
        {
            Explode(this.transform.position);
            this.Invoke(() => Destroy(this.gameObject), explosion_offset);
            dead = true;
        }


        if (dead)
            return;

        rigidbody.linearVelocity = direction.normalized * speed;

    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (dead)
            return;

        if (tagsToIgnore.Any(entry => entry == col.gameObject.tag))
            return;

        switch (col.gameObject.tag)
        {
            case TILEMAP_TAG:
                ContactPoint2D contact = col.GetContact(0);

                // posun lehce dovnit� zasa�en�ho tile
                Vector2 hitPoint = contact.point - contact.normal * 0.05f;

                MapManager mm = GameObject.FindGameObjectWithTag(MAP_MANAGER_TAG).GetComponent<MapManager>();
                TileData? tile = mm.HitTile(hitPoint, mining_damage);
                break;

            case ENEMY_TAG:
                //zajisti, ze se znici pri prvnim narazu s nepritelem
                bounces = 0;
                break;
        }

        Health health = col.gameObject.GetComponent<Health>();
        if (health != null)
        {
            health.TakeDamage(damage);
        }

        if (bounces == 0)
        {
            direction = new Vector2(0, 0);
            ContactPoint2D contact = col.GetContact(0);
            // posun lehce dovnit� zasa�en�ho tile
            Vector2 hitPoint = contact.point - contact.normal * 0.05f;
            Explode(hitPoint);
            rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
            this.Invoke(() => Destroy(this.gameObject), explosion_offset);
            dead = true;
        }

        direction = Vector2.Reflect(direction, col.GetContact(0).normal);
        spawnTime = DateTime.Now;
        bounces--;
    }

    private void Explode(Vector2 position)
    {
        for (int i = 0; i < tagsExplosionDealDamage.Count; i++)
        {
            switch (tagsExplosionDealDamage[i])
            {
                case TILEMAP_TAG:
                    MapManager mapManager = GameObject.FindGameObjectWithTag(MAP_MANAGER_TAG).GetComponent<MapManager>();
                    List<TileData> tiles = mapManager.GetTilesNear(position, explosion_radius);

                    foreach (TileData tile in tiles)
                        mapManager.HitTile(tile, mining_damage);
                    break;

                default:
                    GameObject[] objects = GameObject.FindGameObjectsWithTag(tagsExplosionDealDamage[i]);
                    foreach (GameObject obj in objects)
                    {
                        if (Vector2.Distance(obj.transform.position, this.transform.position) > explosion_radius)
                            continue;
                        Health health = obj.gameObject.GetComponent<Health>();
                        if (health != null)
                        {
                            health.TakeDamage(damage);
                        }
                    }
                    break;
            }
        }
        SetAnimatorTrigger("Explode");



    }
}

