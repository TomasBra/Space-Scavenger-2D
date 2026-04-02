using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Enemy : Health
{

    [SerializeField]
    protected float TRIGGER_DISTANCE;

    [SerializeField]
    protected float STOP_MOVE_DISTANCE;

    [SerializeField]
    protected float SPEED;

    [SerializeField]
    protected float DAMAGE;

    [SerializeField]
    protected float ATACK_COOL_DOWN; //v sekundach

    [SerializeField]
    protected float MIN_ATTACK_DISTANCE;

    [SerializeField]
    protected GameObject Projectile;

    [HideInInspector]
    protected DateTime lastAttack;

    [HideInInspector]
    protected Tilemap tilemap;
    [HideInInspector]
    protected Stack<Vector3> pathToPlayer = new Stack<Vector3>();
    [HideInInspector]
    protected Vector3Int prevPlayerGridPos;

    protected string layersToIgnore = "Projectiles Ignore";
    protected LayerMask raycastIgnoreMask;

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
            raycastIgnoreMask = ~(1 << LayerMask.NameToLayer(layersToIgnore));
            RaycastHit2D hit = Physics2D.Raycast(transform.position, toPlayer, Mathf.Infinity, raycastIgnoreMask);
            if (hit.collider.transform.tag == PLAYER_TAG)
            {
                Move(toPlayer, SPEED);
            }
            else
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

    public virtual void Attack()
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
}
