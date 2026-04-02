using UnityEngine;

public class CameraManager : GameObject2D
{
    [SerializeField]
    public float SPEED;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
     base.Start();   
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();

        float destY = player.transform.position.y;
        float destX;
        if (player.transform.position.y < 0.0f)
        {
            destX = player.transform.position.x;
        }
        else
        {
            destX = MapManager.MAP_WIDTH / 2.0f;
        }

        Vector3 dest = new Vector3(destX, destY, -10.0f);
        Vector3 toDest = dest - transform.position;
        if (toDest.magnitude < SPEED * Time.deltaTime)
        { 
            transform.position = dest;
        }
        else
        {
            transform.position += toDest * SPEED * Time.deltaTime;
        }
    }
}
