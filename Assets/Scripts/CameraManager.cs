using UnityEngine;

public class CameraManager : GameObject2D
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
     base.Start();   
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);
    }
}
