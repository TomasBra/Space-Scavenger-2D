using UnityEngine;
using UnityEngine.Tilemaps;

public class Playah : MonoBehaviour
{
    private const float SPEED = 1.69f;

    new Rigidbody2D rigidbody;

    [SerializeField]
    private GameObject MapManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();

    }

    // Update is called once per frame
    void Update()
    {
        Vector2 direction = new Vector2(0.0f, 0.0f);
        if (Input.GetKey(KeyCode.D)) direction.x += 1.0f;
        if (Input.GetKey(KeyCode.A)) direction.x += -1.0f;
        if (Input.GetKey(KeyCode.W)) direction.y += 1.0f;
        if (Input.GetKey(KeyCode.S)) direction.y += -1.0f;
        direction.Normalize();

        rigidbody.linearVelocity = direction * SPEED;

        if (Input.GetMouseButtonDown(0))
        {
            RayCast2D();
        }
    }

    void RayCast2D()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePosition - (Vector2)transform.position;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction);

        MapManager.GetComponent<MapManager>().HitTile(hit.point);

    }
}
