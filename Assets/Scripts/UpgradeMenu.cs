using System.Linq;
using UnityEngine;

public class UpgradeMenu : MonoBehaviour
{
    [SerializeField] GameObject upgradeMenu;
    [SerializeField] GameObject player;

    /// <summary>
    ///  tady se budou inicializovat ceny
    /// </summary>
    public void Start()
    {
    }

    public void PauseGame()
    {
        upgradeMenu.SetActive(true);
        player.SetActive(false);
        Time.timeScale = 0;
    }
    public void ResumeGame()
    {
        upgradeMenu.SetActive(false);
        player.SetActive(true);
        Time.timeScale = 1;
    }

    public void UpgradeLaserDamage()
    {
    }
}

/// <summary>
/// trida pro urcovani cen
/// </summary>
public class Price
{
    public int copperPrice;
    public int ironPrice;
    public int goldPrice;

    public Price(int copper, int iron = 0, int gold = 0)
    {
        this.copperPrice = copper; 
        this.ironPrice = iron;
        this.goldPrice = gold;
    }
}


public enum UPGRADE
{
    HP,
    PROJECTILE_COUNT,
    PROJECTILE_SPAWN_COOL_DOWN,
    PROJECTILE_DAMAGE,
    PROJECTILE_MINING_DAMAGE,
    PROJECTILE_SPEED,
    PROJECTILE_LIFETIME,
    LASER_MINING_DAMAGE_PER_SECOND,
    LASER_DISTANCE
}

public class Upgrades
{

    Playah player;
    UPGRADE upgrade_type;
    Price[] prices { get; set; }
    int owned_tier = 0; ///< jaky tier mam koupeny, ze zacatku nula :]
    int max_tier = 0;
    public Upgrades(Price[] prices, UPGRADE upgrade_type)
    {
        this.prices = prices; 
        this.max_tier = prices.Length;
        this.upgrade_type = upgrade_type;
    }

    public Upgrades(Price price, UPGRADE upgrade_type)
    {
        this.prices = new Price[1];
        this.prices[0] = price;
        max_tier = 1;
    }

    public void SetPlayer(GameObject playerObj)
    {
        player = playerObj.GetComponent<Playah>();
        if (player == null) {
            Debug.Log("Hrac se nedokazal nacist");
        }
    }

    public bool buyUpgrade()
    {
        if (owned_tier >= max_tier)
        {
            Debug.Log("Max tier reached");
            return false;
        }

        if (player == null)
        {
            return false;
        }

        Price price = prices[owned_tier];

        if (player.copperOre < price.copperPrice ||
            player.ironOre < price.ironPrice ||
            player.goldOre < price.goldPrice)
        {
            return false;
        }

        // odečtení
        player.copperOre -= price.copperPrice;
        player.ironOre -= price.ironPrice;
        player.goldOre -= price.goldPrice;

        owned_tier++;

        ApplyUpgrade();///< aplikace upgradu

        return true;
    }


    private void ApplyUpgrade()
    {
        switch (upgrade_type)
        {
            /// < HP upgrade
            case UPGRADE.HP:
                player.maxHP += 20; ///< pridam o 2o hp
                player.HP += 20; ///< pak ho i o ty hp dohealuju protoze je to super
                break;

            ///< Projektyly upgrade
            case UPGRADE.PROJECTILE_COUNT:
                player.PROJECTILE_COUNT += 2; //< vzdy pridavam dve navic
                break;

            case UPGRADE.PROJECTILE_SPAWN_COOL_DOWN:
                player.PROJECTILE_SPAWN_COOL_DOWN *= 0.9f; //<= strilim o deset procent rychleji
                break;

            case UPGRADE.PROJECTILE_DAMAGE:
                player.PROJECTILE_DAMAGE *= 1.2f; //<dmg o dvacet procent vetsi
                break;


            case UPGRADE.PROJECTILE_SPEED:
                player.PROJECTILE_SPEED *= 1.314f; ///<magicka konstanta
                break;

            case UPGRADE.PROJECTILE_LIFETIME:
                player.PROJECTILE_LIFETIME *= 1.314f; ///<magicka konstanta
                break;


            ///< Laser Upgrade
            case UPGRADE.LASER_MINING_DAMAGE_PER_SECOND:
                player.LASER_MINING_DAMAGE_PER_SECOND *= 2; ///< zatim se zvetsuje dvakrat
                break;

            case UPGRADE.LASER_DISTANCE:
                player.LASER_DISTANCE *= 2; ///< zatim se zvetsuje dvakrat
                break;

            default:
                Debug.LogWarning("wtf");
                break;
        }
    }


}