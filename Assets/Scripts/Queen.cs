using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Queen : Health
{
    [SerializeField]
    private float SPAWN_COOL_DOWN; //v sekundach

    [SerializeField]
    private GameObject SpawnableObject;

    private DateTime lastSpawn;

    private string layersToIgnore = "Projectiles Ignore";

    private LayerMask raycastIgnoreMask;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start()
    {
        base.Start();
        lastSpawn = DateTime.Now;
    }

    // Update is called once per frame
    public void Update()
    {
        base.Update();
        Vector2 toPlayer = player.transform.position - this.transform.position;
        raycastIgnoreMask = ~(1 << LayerMask.NameToLayer(layersToIgnore));

        RaycastHit2D hit = Physics2D.Raycast(transform.position, toPlayer, Mathf.Infinity, raycastIgnoreMask);
        if (hit.transform.tag == PLAYER_TAG && (DateTime.Now - lastSpawn).TotalSeconds > SPAWN_COOL_DOWN)
        {
            Debug.Log("Spawn");
            lastSpawn = DateTime.Now;
            Spawn(SpawnableObject);
        }


    }

    void Spawn(GameObject objectToSpawn)
    {
        if (objectToSpawn != null)
        {
            //vystøelení a natoèení projektilu správným smìrem
            Instantiate(objectToSpawn, position: transform.position, rotation: Quaternion.identity);

        }
    }

    public void TakeDamage(float damage, Vector2? knockbackDirection = null)
    {
        HP -= damage;
        if (knockbackDirection != null)
        {
            knockbackDirection.Value.Normalize();
            rigidbody.AddForce(knockbackDirection.Value * 10f);
        }

        if (HP < 0)
            Destroy(this.gameObject);
    }
}
