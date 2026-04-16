using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Enemy : Health
{
    [SerializeField]
    protected float STOP_MOVE_DISTANCE;

    [SerializeField]
    protected float SPEED;

    [SerializeField]
    protected float DAMAGE;

    [SerializeField]
    protected float MINING_DAMAGE;

    [SerializeField]
    private float PROJECTILE_SPEED;
    [SerializeField]
    private float PROJECTILE_LIFETIME;

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

    protected const float MAX_TRIGGER_DISTANCE = 11;
    public bool isTriggered = false;
    protected Vector2 wanderDirection;
    protected const float WANDER_SPEED_COEF = 0.25f;

    protected string[] layersToIgnore = new string[] { "Enemies", "EnemyProjectiles", "PlayerProjectiles" };
    protected LayerMask raycastIgnoreMask;

    [SerializeField]
    protected GameObject MeatPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start()
    {
        base.Start();
        lastAttack = DateTime.Now;

        tilemap = GameObject.Find("DirtTilemap").GetComponent<Tilemap>();

        prevPlayerGridPos = tilemap.WorldToCell(player.transform.position);

        raycastIgnoreMask = ~(LayerMask.GetMask(layersToIgnore));

        SetRandomWanderDirection();
    }

    protected void SetRandomWanderDirection()
    {
        float angle = UnityEngine.Random.Range(0.0f, Mathf.PI * 2.0f);

        wanderDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
    }

    const float MAGIC = 0.08f;

    // Update is called once per frame
    public void Update()
    {
        base.Update();
        if (dead)
            return;

        Vector2 toPlayer = player.transform.position - this.transform.position;
        float playerDistance = toPlayer.magnitude;
        Vector2 moveDirection = Vector2.zero;
        float currentSpeed = SPEED;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, toPlayer, Mathf.Infinity, raycastIgnoreMask);

        if (!isTriggered)
        {
            // trigger if player is in sight
            if (hit.collider.transform.tag == PLAYER_TAG)
            {
                isTriggered = true;
            }
            else
            {
                // wander around
                currentSpeed = WANDER_SPEED_COEF * SPEED;
                moveDirection = wanderDirection;
            }
        }
        else
        {
            // untrigger
            if (playerDistance > MAX_TRIGGER_DISTANCE && hit.collider.transform.tag != PLAYER_TAG)
            {
                isTriggered = false;
            }
            else
            {
                // go after player
                // go straight
                if (hit.collider.transform.tag == PLAYER_TAG)
                {
                    moveDirection = toPlayer;
                    if (playerDistance < STOP_MOVE_DISTANCE)
                    {
                        currentSpeed = 0.0f;
                    }
                }
                // use pathfinding
                else
                {
                    Vector3Int currPlayerGridPos = tilemap.WorldToCell(player.transform.position);

                    // player moved to different tile and the path must be recalculated
                    if (currPlayerGridPos != prevPlayerGridPos)
                    {
                        pathToPlayer = EnemyPathFinder.FindPath(tilemap, transform.position, player.transform.position);
                        prevPlayerGridPos = currPlayerGridPos;
                    }

                    // is player reachable
                    if (pathToPlayer.Count > 0)
                    {
                        Vector2 toNextPathPoint = pathToPlayer.Peek() - transform.position;
                        if (toNextPathPoint.magnitude < MAGIC)
                        {
                            pathToPlayer.Pop();
                        }

                        moveDirection = toNextPathPoint;
                    }
                }
            }
        
            if (toPlayer.magnitude < MIN_ATTACK_DISTANCE && (DateTime.Now - lastAttack).TotalSeconds > ATACK_COOL_DOWN)
            {
                lastAttack = DateTime.Now;
                Attack();
            }
        }

        Move(moveDirection, currentSpeed);
    }

    public virtual void Attack()
    {
        if (Projectile != null)
        {
            Vector3 direction = player.transform.position - this.transform.position;
            direction.z = 0;
            direction.Normalize();

            //vyst�elen� a nato�en� projektilu spr�vn�m sm�rem
            GameObject projectile = Instantiate(Projectile, position: transform.position, rotation: Quaternion.identity);
            projectile.GetComponent<Projectile>().direction = direction;
            projectile.GetComponent<Projectile>().mining_damage = MINING_DAMAGE;
            projectile.GetComponent<Projectile>().damage = DAMAGE;
            projectile.GetComponent<Projectile>().speed = PROJECTILE_SPEED;
            projectile.GetComponent<Projectile>().lifeTime = PROJECTILE_LIFETIME;

        }
    }

    public override bool TakeDamage(float damage, Vector2? knockbackDirection = null, bool destroyable = true)
    {
        System.Random random = new System.Random();
        bool died = base.TakeDamage(damage, knockbackDirection, false);


        if (died)
        {
            if (random.Next(0,10) < 2)
            {
                Instantiate(MeatPrefab, this.transform.position, quaternion.identity);
            }
            Destroy(this.gameObject);
        }

        return died;
    }
}
