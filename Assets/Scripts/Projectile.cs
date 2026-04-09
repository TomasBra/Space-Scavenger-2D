using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Projectile : GameObject2D
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField]
    private float lifeTime = 3;

    [SerializeField]
    private float speed = 3;

    private DateTime spawnTime;

    [SerializeField]
    private List<string> tagsToIgnore = new List<string>();
    [SerializeField]
    private List<string> layersToIgnore= new List<string>();

    //private GameObject target;

    [SerializeField]
    private float damage = 5;

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

        rigidbody.linearVelocity = - transform.right.normalized * speed;

    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (tagsToIgnore.Any(entry => entry == col.gameObject.tag))
        {
            return;
        }

        Health health = col.gameObject.GetComponent<Health>();
        if (health != null)
        {
            health.TakeDamage(damage);
        }

        //if (col.gameObject.tag == TILEMAP_TAG)
        //{
        //    Vector2 hitPoint = col.contacts[0].point;
        //    MapManager mm = GameObject.FindGameObjectWithTag(MAP_MANAGER_TAG).GetComponent<MapManager>();
        //    TileData? tile = mm.HitTile(hitPoint, damage);
        //}

        Destroy(this.gameObject);
    }
}

