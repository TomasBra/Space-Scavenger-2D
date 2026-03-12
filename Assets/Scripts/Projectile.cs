using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField]
    private float lifeTime;

    private DateTime spawnTime;
    
    void Start()
    {
        spawnTime = DateTime.Now;
    }

    // Update is called once per frame
    void Update()
    {
        if ((DateTime.Now - spawnTime).TotalSeconds > lifeTime)
            Destroy(this.gameObject);

    }
}
