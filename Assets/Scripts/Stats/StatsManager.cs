using UnityEngine;

public class StatsManager : MonoBehaviour
{

    //public static StatsManager Instance;
    [Header("Tool Stats")]
    public int LASER_DISTANCE = 10;
    public int MINING_DAMAGE_PER_SECOND = 10;

    [Header("Weapon Stats")]
    public int ENEMY_DAMAGE_PER_SECOND = 10;


    [Header("Movement Stats")]
    public int SPEED = 10;

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

    public static StatsManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
