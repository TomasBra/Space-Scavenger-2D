using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Tilemaps;

public class Playah : MonoBehaviour
{
    private const float SPEED = 1.69f;
    private const float LASER_DISTANCE = 5;
    private const float DAMAGE_PER_SECOND = 5;

    new Rigidbody2D rigidbody;

    [SerializeField]
    private GameObject MapManager;

    [SerializeField]
    private GameObject FirePoint;

    [SerializeField]
    private GameObject StartVFX;

    [SerializeField]
    private GameObject EndVFX;

    private List<ParticleSystem> laserStartParticleSystems = new List<ParticleSystem>();
    private List<ParticleSystem> laserEndParticleSystems = new List<ParticleSystem>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        InitParticleSystems();
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

        if (Input.GetMouseButtonDown(0))
        {
            StartLaserParticles();
        }
        if (Input.GetMouseButtonUp(0))
        {
            StopLaserParticles();
        }
    }


    void Laser2D()
    {
        Vector3 laserEndPosition;
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePosition - (Vector2)FirePoint.transform.position;
        direction.Normalize();

        StartVFX.transform.rotation = Quaternion.LookRotation(direction);

        laserEndPosition = shorterVector(FirePoint.transform.position + new Vector3(direction.x, direction.y, 0) * LASER_DISTANCE, mousePosition, FirePoint.transform.position);
        float distance = (laserEndPosition - FirePoint.transform.position).magnitude;
        Debug.Log(distance);

        RaycastHit2D hit = Physics2D.Raycast(FirePoint.transform.position, direction, distance);

        
        if (hit.collider != null)
        {
            Vector3 hitPoint = new Vector3(hit.point.x+direction.x*.25f, hit.point.y+direction.y*.25f, 0);
            laserEndPosition = hit.point;
            MapManager.GetComponent<MapManager>().HitTile(hitPoint, DAMAGE_PER_SECOND);
        }

        EndVFX.transform.position = laserEndPosition;
        DrawLaser(FirePoint.transform.position, laserEndPosition);
    }

    void DrawLaser(Vector3 startPostion, Vector3 endPosition) {
        GetComponent<LineRenderer>().positionCount = 2;
        GetComponent<LineRenderer>().SetPosition(0, startPostion);
        GetComponent<LineRenderer>().SetPosition(1, endPosition);
    }

    void InitParticleSystems()
    {
        for (int i = 0; i < StartVFX.transform.childCount; i++)
        {
            laserStartParticleSystems.Add(StartVFX.transform.GetChild(i).GetComponent<ParticleSystem>());
        }

        for (int i = 0; i < EndVFX.transform.childCount; i++)
        {
            laserEndParticleSystems.Add(EndVFX.transform.GetChild(i).GetComponent<ParticleSystem>());
        }
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

    void StartLaserParticles()
    {

        foreach (ParticleSystem partSys in laserStartParticleSystems)
        {
            partSys.Play();
        }

        foreach (ParticleSystem partSys in laserEndParticleSystems)
        {
            partSys.Play();
        }
    }


    void StopLaserParticles()
    {
        foreach (ParticleSystem partSys in laserStartParticleSystems)
        {
            partSys.Stop();
        }

        foreach (ParticleSystem partSys in laserEndParticleSystems)
        {
            partSys.Stop();
        }
    }
}
