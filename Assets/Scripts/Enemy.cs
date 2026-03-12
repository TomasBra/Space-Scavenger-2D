using System;
using UnityEngine;

public class Enemy : MonoBehaviour
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
    private float HP;

    [SerializeField]
    private GameObject Player;

    [SerializeField]
    private GameObject Projectile;



    private Rigidbody2D rigidbody;
    private Playah playah;

    private DateTime lastAttack;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        playah = Player.GetComponent<Playah>();
        lastAttack = DateTime.Now;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 direction = Player.transform.position - this.transform.position;
        if (direction.magnitude > STOP_MOVE_DISTANCE && direction.magnitude < TRIGGER_DISTANCE)
        {
            Move(direction, SPEED);
        }
        else
        {
            Move(Vector2.zero, 0);
        }

        if (direction.magnitude < MIN_ATTACK_DISTANCE && (DateTime.Now - lastAttack).TotalSeconds > ATACK_COOL_DOWN)
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
            Instantiate(Projectile, position: transform.position, rotation: Quaternion.identity);
        }

        playah.TakeDamage(DAMAGE);
    }

    public void TakeDamage(float damage, Vector2? knockbackDirection = null)
    {
        HP -= damage;
        if (knockbackDirection != null)
        {
            knockbackDirection.Value.Normalize();
            rigidbody.AddForce(knockbackDirection.Value*10f);
        }

        if (HP < 0)
            Destroy(this.gameObject);
    }
}
