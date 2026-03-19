using UnityEngine;

public class GameObject2D : MonoBehaviour
{
    /*
    Slouží jako tøída k dìdìní všemi herními objekty a obsahuje všeobecnì potøebné vìci. 
     */


    [HideInInspector]
    public Rigidbody2D rigidbody; //public nutný kvùli dìdìní

    [HideInInspector]
    public GameObject player; //public nutný kvùli dìdìní

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    public void Update()
    {
        
    }

    //vrací úhel v stupních v ose z
    public float LookAt2D(Vector2 sourcePosition, Vector2 targetPosition)
    {
        Vector2 look = transform.InverseTransformPoint(targetPosition);
        float angle = Mathf.Atan2(look.y, look.x) * Mathf.Rad2Deg - 180;

        return angle;
    }

    Vector3 shorterVector(Vector3 a, Vector3 b, Vector3 start)
    {
        Vector3 StartA = a - start;
        Vector3 StartB = b - start;

        float distA = StartA.sqrMagnitude;
        float distB = StartB.sqrMagnitude;

        if (distA < distB)
        {
            return a;
        }
        else
        {
            return b;
        }
    }
}
