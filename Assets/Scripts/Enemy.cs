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
    private float DAMAGE_PER_SECOND;

    [SerializeField]
    private float MIN_ATTACK_DISTANCE;

    [SerializeField]
    private float HP;

    [SerializeField]
    private GameObject Player;



    private Rigidbody2D rigidbody;
    private Playah playah;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        playah = Player.GetComponent<Playah>();
    }

    // Update is called once per frame
    void FixedUpdate()
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

        if (direction.magnitude < MIN_ATTACK_DISTANCE)
        {
            Attack();
        }
       
    }

    void Move(Vector2 direction, float speed = 1)
    {
        direction.Normalize();
        Vector3 vec = new Vector3(direction.x, direction.y, 0);
        this.transform.position = this.transform.position + Time.deltaTime * speed * vec;
    }

    void Attack()
    {
        playah.TakeDamage(DAMAGE_PER_SECOND* Time.deltaTime);
    }

    public void TakeDamage(float damage, Vector2? knockbackDirection = null)
    {
        HP -= damage;
        if (knockbackDirection != null)
        {
            this.Move(knockbackDirection.Value, 1);
        }

        if (HP < 0)
            Destroy(this.gameObject);
    }
}
