using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MeleeEnemy : Enemy
{


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
        if (dead)
            return;
    }

    public override void Attack()
    {
        Vector2 knockback_direction = player.transform.position - this.transform.position;
        knockback_direction.Normalize();
        playah.TakeDamage(damage, knockback_direction);
    }
}
