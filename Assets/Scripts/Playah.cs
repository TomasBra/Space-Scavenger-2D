using UnityEngine;
using UnityEngine.Tilemaps;

public class Playah : MonoBehaviour
{
    private const float SPEED = 1.69f;
    private const float LASER_DISTANCE = 7;
    private const float DAMAGE_PER_SECOND = 5;

    new Rigidbody2D rigidbody;

    [SerializeField]
    private GameObject MapManager;

    [SerializeField]
    private GameObject FirePoint;

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

        if (Input.GetMouseButton(0))
        {
            Laser2D();
        }
        else
        {
            GetComponent<LineRenderer>().positionCount = 0;
        }
    }


    void Laser2D()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePosition - (Vector2)transform.position;
        direction.Normalize();
        
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction);
        MapManager.GetComponent<MapManager>().HitTile(hit.point, DAMAGE_PER_SECOND);

        Vector3 laserEndPosition;
        if(hit.collider != null)
            laserEndPosition = new Vector3(hit.point.x, hit.point.y, 0);
        else
            laserEndPosition = mousePosition+direction*LASER_DISTANCE;

        DrawLaser(FirePoint.transform.position, laserEndPosition);
    }

    void DrawLaser(Vector3 startPostion, Vector3 endPosition) {
        GetComponent<LineRenderer>().positionCount = 2;
        GetComponent<LineRenderer>().SetPosition(0, startPostion);
        GetComponent<LineRenderer>().SetPosition(1, endPosition);
    }
}
