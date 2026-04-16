using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Projectile : GameObject2D
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField]
    public float lifeTime = 3;

    [SerializeField]
    public float speed = 3;

    private DateTime spawnTime;

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

    void Start()
    {
        base.Start();
        spawnTime = DateTime.Now;
        SetUpIngoreLayer();
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
            Destroy(this.gameObject);

        rigidbody.linearVelocity = direction.normalized * speed;

    }

    void OnCollisionStay2D(Collision2D col)
    {
        if (tagsToIgnore.Any(entry => entry == col.gameObject.tag))
            return;

        if (col.gameObject.CompareTag(TILEMAP_TAG))
        {
            ContactPoint2D contact = col.GetContact(0);

            // posun lehce dovnit� zasa�en�ho tile
            Vector2 hitPoint = contact.point - contact.normal * 0.05f;

            MapManager mm = GameObject.FindGameObjectWithTag(MAP_MANAGER_TAG).GetComponent<MapManager>();
            TileData? tile = mm.HitTile(hitPoint, mining_damage);
        }

        Health health = col.gameObject.GetComponent<Health>();
        if (health != null)
        {
            health.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}

