using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class Projectile : Health
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    [SerializeField]
    private float lifeTime = 3;

    [SerializeField]
    private float speed = 3;

    private DateTime spawnTime;

    [SerializeField]
    private string targetTag = "Player";

    [SerializeField]
    private List<string> tagsToIgnore = new List<string>();

    private GameObject target;

    [SerializeField]
    private float damage = 5;

    void Start()
    {
        spawnTime = DateTime.Now;
        rigidbody = GetComponent<Rigidbody2D>();

        target = GameObject.FindGameObjectWithTag(targetTag);

        transform.Rotate(0, 0, LookAt2D(this.transform.position, target.transform.position));
    }

    // Update is called once per frame
    void Update()
    {
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


        // Debug.Log(col.gameObject.tag);
        Health health = col.gameObject.GetComponent<Health>();
        if (health != null)
        {
            health.TakeDamage(damage);
        }

        Destroy(this.gameObject);
    }
}

