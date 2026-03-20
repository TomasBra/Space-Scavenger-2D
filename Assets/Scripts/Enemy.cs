using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Enemy : Health
{

    [SerializeField]
    private float TRIGGER_DISTANCE;

    [SerializeField]
    private float STOP_MOVE_DISTANCE;

    [SerializeField]
    private float SPEED;

    [SerializeField]
    private float DAMAGE;

    [SerializeField]
    private float ATACK_COOL_DOWN; //v sekundach

    [SerializeField]
    private float MIN_ATTACK_DISTANCE;

    [SerializeField]
    private GameObject Projectile;

    private DateTime lastAttack;

    private Tilemap tilemap;
    private Stack<Vector3> pathToPlayer = new Stack<Vector3>();
    private Vector3Int prevPlayerGridPos;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start()
    {
        base.Start();
        lastAttack = DateTime.Now;

        tilemap = GameObject.Find("DirtTilemap").GetComponent<Tilemap>();

        prevPlayerGridPos = tilemap.WorldToCell(player.transform.position);
    }

    const float MAGIC = 0.08f;

    // Update is called once per frame
    public void Update()
    {
        base.Update();

        Vector2 toPlayer = player.transform.position - this.transform.position;

        if (toPlayer.magnitude > STOP_MOVE_DISTANCE
            && toPlayer.magnitude < TRIGGER_DISTANCE)
        {
            Vector3Int currPlayerGridPos = tilemap.WorldToCell(player.transform.position);
            
            if (currPlayerGridPos != prevPlayerGridPos)
            {
                pathToPlayer = EnemyPathFinder.FindPath(tilemap, transform.position, player.transform.position);
                prevPlayerGridPos = currPlayerGridPos;
            }

            if (pathToPlayer.Count > 0)
            {
                Vector2 toNextPathPoint = pathToPlayer.Peek() - transform.position;
                if (toNextPathPoint.magnitude < MAGIC)
                {
                    pathToPlayer.Pop();
                }

                Move(toNextPathPoint, SPEED);
            }
            else
            {
                Move(Vector2.zero, 0);
            }
        }
        else
        {
            Move(Vector2.zero, 0);
        }

        if (toPlayer.magnitude < MIN_ATTACK_DISTANCE && (DateTime.Now - lastAttack).TotalSeconds > ATACK_COOL_DOWN)
        {
            lastAttack = DateTime.Now;
            Attack();
        }

    }

    void Move(Vector2 direction, float speed = 1)
    {
        direction.Normalize();
        Vector3 vec = new Vector3(direction.x, direction.y, 0);
        //this.transform.position = this.transform.position + Time.deltaTime * speed * vec;
        rigidbody.linearVelocity = speed * vec;
    }

    void Attack()
    {
        if (Projectile != null)
        {
            Vector3 direction = player.transform.position - this.transform.position;
            direction.z = 0;
            direction.Normalize();

            //vystøelení a natoèení projektilu správným smìrem
            Instantiate(Projectile, position: transform.position, rotation: Quaternion.identity);

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
