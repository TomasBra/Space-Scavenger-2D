using UnityEngine;

public class CameraManager : GameObject2D
{
    enum GameState
    { 
        LANDING,
        PLAY,
        TAKEOFF
    }

    GameState currentGameState = GameState.PLAY;

    [SerializeField]
    public float SPEED;

    [SerializeField]
    public Camera mainCamera;

    [SerializeField]
    public GameObject rocket;

    private MapManager mapManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        base.Start();

        transform.position = new Vector3(MapManager.MAP_WIDTH / 2.0f, 1.0f, -10.0f);

        mapManager = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();

        float destX;
        float destY;
        Vector3 trackedPosition;

        if (currentGameState == GameState.LANDING || currentGameState == GameState.TAKEOFF)
        {
            trackedPosition = rocket.transform.position;
        }
        else
        {
            trackedPosition = player.transform.position;
        }

        destY = trackedPosition.y;

        if (trackedPosition.y < 0.0f)
        {
            destX = trackedPosition.x;
        }
        else
        {
            destX = MapManager.MAP_WIDTH / 2.0f;
        }

        float cameraHalfWidth = mainCamera.orthographicSize * mainCamera.aspect;
        float cameraHalfHeight = mainCamera.orthographicSize / mainCamera.aspect;

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
