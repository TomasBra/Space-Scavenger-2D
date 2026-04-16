using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameObject2D : MonoBehaviour
{
    /*
    Slouží jako tøída k dìdìní všemi herními objekty a obsahuje všeobecnì potøebné vìci. 
     */
    [HideInInspector]
    public const string MAP_MANAGER_TAG = "MapManager";
    [HideInInspector]
    public const string TILEMAP_TAG = "TileMap";
    [HideInInspector]
    public const string ENEMY_TAG = "Enemy";
    [HideInInspector]
    public const string PROJECTILE_TAG = "Projectile";
    [HideInInspector]
    public const string PLAYER_TAG = "Player"; //public nutný kvùli dìdìní

    [HideInInspector]
    public const string COPPER_TAG = "Copper"; //public nutný kvùli dìdìní

    [HideInInspector]
    public const string IRON_TAG = "Iron"; //public nutný kvùli dìdìní

    [HideInInspector]
    public const string GOLD_TAG = "Gold"; //public nutný kvùli dìdìní

    [HideInInspector]
    public const string MEAT_TAG = "Meat"; //public nutný kvùli dìdìní

    [HideInInspector]
    public Rigidbody2D rigidbody; //public nutný kvùli dìdìní

    [HideInInspector]
    public GameObject player; //public nutný kvùli dìdìní

    [HideInInspector]
    public Playah playah;

    [HideInInspector]
    public Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag(PLAYER_TAG);
        playah = player.GetComponent<Playah>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    public void Update()
    {

    }

    public void Move(Vector2 direction, float speed = 1)
    {
        if (direction.magnitude == 0.0f || speed == 0.0f)
        {
            rigidbody.linearVelocity = Vector2.zero;

            return;
        }

        direction.Normalize();
        Vector3 vec = new Vector3(direction.x, direction.y, 0);
        rigidbody.linearVelocity = speed * vec;
    }

    public Vector2 randomOffsettedPosition(Vector2 position, float maxOffset)
    {
        int x = (int)(2*Random.value*maxOffset - maxOffset);
        int y = (int)(2*Random.value*maxOffset - maxOffset);


        return new Vector2(position.x + x, position.y + y);
    }


    //vrací úhel v stupních v ose z
    public float LookAt2D(Vector2 sourcePosition, Vector2 targetPosition)
    {
        Vector2 look = transform.InverseTransformPoint(targetPosition);
        float angle = Mathf.Atan2(look.y, look.x) * Mathf.Rad2Deg - 180;

        return angle;
    }

    public Vector3 shorterVector(Vector3 a, Vector3 b, Vector3 start)
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

    public void SetAnimatorInt(string name, int value)
    {
        if (animator == null)
            return;

        animator.SetInteger(name, value);
    }

    public void SetAnimatorFloat(string name, float value)
    {
        if (animator == null)
            return;

        animator.SetFloat(name, value);
    }

    public void SetAnimatorBool(string name, bool value)
    {
        if (animator == null)
            return;

        animator.SetBool(name, value);
    }

    public void SetAnimatorTrigger(string name)
    {
        if (animator == null)
            return;

        animator.SetTrigger(name);
    }

}


public static class Extensions {
    //radiány no more
    public static Vector2 RotateZ(this Vector2 v, float angle)
    {
        angle *= Mathf.Deg2Rad;

        Vector2 vec = new Vector2(v.x, v.y);

        float sin = Mathf.Sin(angle);
        float cos = Mathf.Cos(angle);

        float tx = v.x;
        float ty = v.y;
        vec.x = (cos * tx) - (sin * ty);
        vec.y = (cos * ty) + (sin * tx);

        return vec;
    }

}