using UnityEngine;

public class BgMapParalax : MonoBehaviour
{
    [SerializeField]
    private GameObject camera;

    private const float PARALAX_SPEED = 0.33f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //transform.position = camera.transform.position * (1.0f - PARALAX_SPEED);
    }
}
