using UnityEngine;

public class CameraManager : GameObject2D
{
    [SerializeField]
    public float SPEED;

    [SerializeField]
    public Camera camera;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        base.Start();

        transform.position = new Vector3(MapManager.MAP_WIDTH / 2.0f, 1.0f, -10.0f);
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

        float cameraHalfWidth = camera.orthographicSize * camera.aspect;
        float cameraHalfHeight = camera.orthographicSize / camera.aspect;

        // limit in X
        if (destX + cameraHalfWidth > MapManager.MAP_WIDTH)
        {
            destX = MapManager.MAP_WIDTH - cameraHalfWidth;
        }
        else if (destX - cameraHalfWidth < 0.0f)
        {
            destX = cameraHalfWidth;
        }

        // limit in Y
        if (destY + cameraHalfHeight > MapManager.SKY_HEIGHT)
        {
            destY = MapManager.SKY_HEIGHT - cameraHalfHeight;
        }
        else if (destY - cameraHalfHeight < -MapManager.MAP_HEIGHT)
        {
            destY = -MapManager.MAP_HEIGHT + cameraHalfHeight;
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
