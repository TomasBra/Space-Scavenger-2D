using UnityEngine;

public class StatsManager : MonoBehaviour
{

    public static StatsManager Instance;
    [Header("Tool Stats")]
    public int LASER_DISTANCE;
    public int MINING_DAMAGE_PER_SECOND;

    [Header("Weapon Stats")]
    public int ENEMY_DAMAGE_PER_SECOND;


    [Header("Movement Stats")]
    public int SPEED;

    /// <summary>
    /// dostupnost itemCounteru
    /// </summary>
    [Header("Inventory stats")]
    public ItemCounter itemCounter;

    /// <summary>
    /// dostupnost itemCounteru
    /// </summary>
    [Header("Health Stats")]
    public HealthBar healthBar; //ukazatel zivota hrace

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this; 
        }
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
