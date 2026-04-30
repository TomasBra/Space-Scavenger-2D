using System;
using System.Collections.Generic;
using System.Text;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Enemy : Health
{
    public static float GetSpeedDepthCoef(int absoluteDepth)
    {
        float relativeDepth = absoluteDepth / (float)MapManager.MAP_HEIGHT;
        float bonus = relativeDepth * 0.8f;

        return 1.0f + bonus;
    }

    public static float GetHPDepthCoef(int absoluteDepth)
    {
        float relativeDepth = absoluteDepth / (float)MapManager.MAP_HEIGHT;
        float bonus = relativeDepth * 4.0f;

        return 1.0f + bonus;
    }

    public static float GetDamageDepthCoef(int absoluteDepth)
    {
        float relativeDepth = absoluteDepth / (float)MapManager.MAP_HEIGHT;
        float bonus = relativeDepth * 2.0f;

        return 1.0f + bonus;
    }

    public static float GetAttackCooldownDepthCoef(int absoluteDepth)
    {
        float relativeDepth = absoluteDepth / (float)MapManager.MAP_HEIGHT;
        float bonus = relativeDepth * 2.5f;

        return 1 / (1.0f + bonus);
    }

    public static float GetMiningDamageDepthCoef(int absoluteDepth)
    {
        float relativeDepth = absoluteDepth / (float)MapManager.MAP_HEIGHT;
        float bonus = relativeDepth * 5.0f;

        return 1.0f + bonus;
    }

    public const float WANDER_BOUNCE_COOLDOWN = 0.1f;
    protected DateTime lastWanderBounceTime;

    [SerializeField]
    protected float STOP_MOVE_DISTANCE;

    [SerializeField]
    protected float DEFAULT_SPEED;
    protected float speed;

    [SerializeField]
    protected float DEFAULT_DAMAGE;
    protected float damage;

    [SerializeField]
    protected float DEFAULT_MINING_DAMAGE;
    protected float miningDamage;

    [SerializeField]
    private float PROJECTILE_SPEED;
    [SerializeField]
    private float PROJECTILE_LIFETIME;

    [SerializeField]
    protected float DEFAULT_ATTACK_COOLDOWN;
    protected float attackCooldown; //v sekundach

    [SerializeField]
    protected float DEFAULT_HP;

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

    protected const float OPTIMIZATION_WANDER_DISTANCE = 22;
    protected const float MAX_TRIGGER_DISTANCE = 11;
    public bool isTriggered = false;
    protected Vector2 wanderDirection;
    protected const float WANDER_SPEED_COEF = 0.33f;

    protected string[] layersToIgnore = new string[] { "Enemies", "EnemyProjectiles", "PlayerProjectiles" };
    protected LayerMask raycastIgnoreMask;

    [SerializeField]
    protected GameObject MeatPrefab;

    protected CircleCollider2D enemyCollider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start()
    {
        base.Start();

        enemyCollider = GetComponent<CircleCollider2D>();
        raycastIgnoreMask = ~(LayerMask.GetMask(layersToIgnore) | collisionManager.RaycastIgnoreLayers);
        tilemap = GameObject.Find("DirtTilemap").GetComponent<Tilemap>();

        lastAttack = DateTime.Now;
        lastWanderBounceTime = DateTime.Now;

        prevPlayerGridPos = tilemap.WorldToCell(player.transform.position);

        SetRandomWanderDirection();
    }

    protected void SetRandomWanderDirection()
    {
        float angle = UnityEngine.Random.Range(0.0f, Mathf.PI * 2.0f);

        wanderDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
    }

    public void ScaleByDepth(int absoluteDepth)
    {
        speed = DEFAULT_SPEED * GetSpeedDepthCoef(absoluteDepth);
        damage = DEFAULT_DAMAGE * GetDamageDepthCoef(absoluteDepth);
        miningDamage = DEFAULT_MINING_DAMAGE * GetMiningDamageDepthCoef(absoluteDepth);
        attackCooldown = DEFAULT_ATTACK_COOLDOWN * GetAttackCooldownDepthCoef(absoluteDepth);
        maxHP = DEFAULT_HP * GetHPDepthCoef(absoluteDepth);
        HP = maxHP;
    }

    const float MAGIC = 0.08f;
    // Update is called once per frame
    public void Update()
    {
        base.Update();
        if (dead)
            return;

        Vector2 toPlayer = player.transform.position - transform.position;
        float playerDistance = toPlayer.magnitude;

        
        if (playerDistance >= OPTIMIZATION_WANDER_DISTANCE)
        {
            // wander around
            Move(wanderDirection, WANDER_SPEED_COEF * speed);
            return;
        }

        Vector2 moveDirection = Vector2.zero;
        float currentSpeed = speed;
        Vector2 pos2D = new Vector2(transform.position.x, transform.position.y);
        Vector2 playerPos2D = new Vector2(player.transform.position.x, player.transform.position.y);

        // hitting
        Vector2 origin1 = pos2D + toPlayer.RotateZ(90).normalized * enemyCollider.radius;
        RaycastHit2D hit1 = Physics2D.Raycast(origin1, playerPos2D - origin1, Mathf.Infinity, raycastIgnoreMask);

        Vector2 origin2 = pos2D + toPlayer.RotateZ(-90).normalized * enemyCollider.radius;
        RaycastHit2D hit2 = Physics2D.Raycast(origin2, playerPos2D - origin2, Mathf.Infinity, raycastIgnoreMask);

        bool hit1IsPlayer = hit1.collider != null && hit1.collider.CompareTag(PLAYER_TAG);
        bool hit2IsPlayer = hit2.collider != null && hit2.collider.CompareTag(PLAYER_TAG);

        bool isFullyInSight = hit1IsPlayer && hit2IsPlayer;
        bool isPartiallyInSight = hit1IsPlayer || hit2IsPlayer;


        if (!isTriggered)
        {
            // trigger if player is in sight
            if (isPartiallyInSight)
            {
                isTriggered = true;
            }
            else
            {
                // wander around
                currentSpeed = WANDER_SPEED_COEF * speed;
                moveDirection = wanderDirection;
            }
        }
        else
        {
            // untrigger
            if (playerDistance > MAX_TRIGGER_DISTANCE && !isPartiallyInSight)
            {
                isTriggered = false;
            }
            else
            {
                // go after player
                // go straight
                if (isFullyInSight)
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

        if (toPlayer.magnitude < MIN_ATTACK_DISTANCE && (DateTime.Now - lastAttack).TotalSeconds > attackCooldown)
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
            projectile.GetComponent<Projectile>().mining_damage = miningDamage;
            projectile.GetComponent<Projectile>().damage = damage;
            projectile.GetComponent<Projectile>().speed = PROJECTILE_SPEED;
            projectile.GetComponent<Projectile>().lifeTime = PROJECTILE_LIFETIME;

        }
    }

    public override bool TakeDamage(float damage, Vector2? knockbackDirection = null, bool destroyable = true)
    {
        if (dead)
            return true;

        System.Random random = new System.Random();
        bool died = base.TakeDamage(damage, knockbackDirection, true);
        if (died)
        {
            if (random.Next(0, 10) < 2)
            {
                Instantiate(MeatPrefab, this.transform.position, quaternion.identity);
            }
        }

        return died;
    }

    public void OnCollisionStay2D(Collision2D collision)
    {
        if ((float)(DateTime.Now - lastWanderBounceTime).TotalSeconds >= WANDER_BOUNCE_COOLDOWN)
        { 
            lastWanderBounceTime = DateTime.Now;
            // bounce
            wanderDirection = Vector2.Reflect(wanderDirection, collision.GetContact(0).normal);
        }
    }
}
