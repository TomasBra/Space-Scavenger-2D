using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpgradeMenu : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] GameObject upgradeMenu;
    [SerializeField] GameObject player;

    [Header("Inventory UI")]
    [SerializeField] TMP_Text copperText;
    [SerializeField] TMP_Text ironText;
    [SerializeField] TMP_Text goldText;

    [Header("Upgrade Items")]
    [SerializeField] UpgradeItemUI[] upgradeItems;

    private Playah playerScript;

    private Dictionary<UPGRADE, Upgrades> upgrades;

    void Awake()
    {
        playerScript = player.GetComponent<Playah>();

        upgrades = new Dictionary<UPGRADE, Upgrades>
        {
            ///< zbrane 
            {
                UPGRADE.PROJECTILE_COUNT,
                new Upgrades(
                    new Price[]
                    {
                        new Price(6, 5, 5),
                        new Price(9, 9, 10),
                        new Price(13, 14, 16)
                    },
                    UPGRADE.PROJECTILE_COUNT
                )
            },

            {
                UPGRADE.PROJECTILE_SPAWN_COOL_DOWN,
                new Upgrades(
                    new Price[]
                    {
                        new Price(4, 0, 0),
                        new Price(6, 5, 0),
                        new Price(9, 10, 6),
                        new Price(13, 16, 13)
                    },
                    UPGRADE.PROJECTILE_SPAWN_COOL_DOWN
                )
            },

            {
                UPGRADE.PROJECTILE_DAMAGE,
                new Upgrades(
                    new Price[]
                    {
                        new Price(4, 0, 0),
                        new Price(6, 5, 0),
                        new Price(9, 10, 6),
                        new Price(13, 16, 13)
                    },
                    UPGRADE.PROJECTILE_DAMAGE
                )
            },

            {
                UPGRADE.PROJECTILE_BOUNCES,
                new Upgrades(
                    new Price[]
                    {
                        new Price(6, 5, 5),
                        new Price(9, 9, 10),
                        new Price(13, 14, 16)
                    },
                    UPGRADE.PROJECTILE_BOUNCES
                )
            },

            {
                UPGRADE.PROJECTILE_EXPLOSION,
                new Upgrades(
                    new Price[]
                    {
                        new Price(6, 5, 5),
                        new Price(9, 9, 10),
                        new Price(13, 14, 16)
                    },
                    UPGRADE.PROJECTILE_EXPLOSION
                )
            },
            ///< Laser
            {
                UPGRADE.LASER_DAMAGE,
                new Upgrades(
                    new Price[]
                    {
                        new Price(2, 2, 0),
                        new Price(4, 3, 0),
                        new Price(7, 5, 2),
                        new Price(11, 8, 7),
                        new Price(16, 12, 13)
                    },
                    UPGRADE.LASER_DAMAGE
                )
            },

            {
                UPGRADE.LASER_DISTANCE,
                new Upgrades(
                    new Price[]
                    {
                        new Price(30, 0, 0),
                        new Price(35, 25, 10)
                    },
                    UPGRADE.LASER_DISTANCE
                )
            },


            ///< Hrac 
            {
                UPGRADE.HP,
                new Upgrades(
                    new Price[]
                    {
                        new Price(3, 0, 0),
                        new Price(5, 2, 0),
                        new Price(8, 5, 2),
                        new Price(12, 8, 7),
                        new Price(17, 12, 13)
                    },
                    UPGRADE.HP
                )
            },
            {
                UPGRADE.MOVEMENT_SPEED,
                new Upgrades(
                    new Price[]
                    {
                        new Price(3, 0, 0),
                        new Price(5, 3, 0),
                        new Price(8, 7, 6),
                        new Price(12, 12, 13)
                    },
                    UPGRADE.MOVEMENT_SPEED
                )
            },
        };
        foreach (var upgrade in upgrades.Values)
        {
            upgrade.SetPlayer(player);
        }

        foreach (var item in upgradeItems)
        {
            item.Init(this);
        }
    }

    void OnEnable()
    {
        RefreshInventory();
        RefreshAllUpgradeItems();
    }

    public Upgrades GetUpgrade(UPGRADE type)
    {
        return upgrades[type];
    }

    public void TryBuyUpgrade(UPGRADE type)
    {
        bool bought = upgrades[type].BuyUpgrade();

        if (bought)
        {
            RefreshInventory();
            RefreshAllUpgradeItems();
        }
    }

    public void RefreshInventory()
    {
        if (playerScript == null) return;

        copperText.text = playerScript.copperOre.ToString();
        ironText.text = playerScript.ironOre.ToString();
        goldText.text = playerScript.goldOre.ToString();
    }

    public void RefreshAllUpgradeItems()
    {
        foreach (var item in upgradeItems)
        {
            item.Refresh();
        }
    }

    public void PauseGame()
    {
        upgradeMenu.SetActive(true);
        player.GetComponent<Playah>().enabled = false;
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        upgradeMenu.SetActive(false);
        player.GetComponent<Playah>().enabled = true;
        Time.timeScale = 1;
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
        copperPrice = copper;
        ironPrice = iron;
        goldPrice = gold;
    }
}

public enum UPGRADE
{
    HP,
    MOVEMENT_SPEED,
    PROJECTILE_COUNT,
    PROJECTILE_SPAWN_COOL_DOWN,
    PROJECTILE_DAMAGE,
    PROJECTILE_MINING_DAMAGE,
    PROJECTILE_BOUNCES,
    PROJECTILE_EXPLOSION,
    LASER_DAMAGE,
    LASER_DISTANCE
}

public class Upgrades
{
    Playah player;
    UPGRADE upgrade_type;

    Price[] prices;

    int owned_tier = 0;
    int max_tier = 0;

    public int OwnedTier => owned_tier;
    public int MaxTier => max_tier;
    public bool IsMaxed => owned_tier >= max_tier;

    public Upgrades(Price[] prices, UPGRADE upgrade_type)
    {
        this.prices = prices;
        this.max_tier = prices.Length;
        this.upgrade_type = upgrade_type;
    }

    public Price GetCurrentPrice()
    {
        if (IsMaxed) return null;
        return prices[owned_tier];
    }

    public void SetPlayer(GameObject playerObj)
    {
        player = playerObj.GetComponent<Playah>();

        if (player == null)
        {
            Debug.LogError("Hrac se nedokazal nacist");
        }
    }

    public bool BuyUpgrade()
    {
        if (IsMaxed)
        {
            Debug.Log("max tier mame");
            return false;
        }

        if (player == null) return false;

        Price price = prices[owned_tier];

        if (player.copperOre < price.copperPrice ||
            player.ironOre < price.ironPrice ||
            player.goldOre < price.goldPrice)
        {
            Debug.Log("Nemas dost surovin");
            return false;
        }

        player.copperOre -= price.copperPrice;
        player.ironOre -= price.ironPrice;
        player.goldOre -= price.goldPrice;

        owned_tier++;
        player.itemCounter.SetCopper(player.copperOre);
        player.itemCounter.SetIron(player.ironOre);
        player.itemCounter.SetGold(player.goldOre);
        ApplyUpgrade();

        return true;
    }

    private void ApplyUpgrade()
    {
        switch (upgrade_type)
        {
            case UPGRADE.HP:
                player.maxHP += 20;
                player.HP += 20;
                player.healthBar.SetMaxHealth(player.maxHP);
                player.healthBar.SetHealth(player.HP);
                break;
            case UPGRADE.MOVEMENT_SPEED:
                player.SPEED += 1.0f;
                break;

            case UPGRADE.PROJECTILE_COUNT:
                player.PROJECTILE_COUNT += 2;
                break;

            case UPGRADE.PROJECTILE_SPAWN_COOL_DOWN:
                player.PROJECTILE_SPAWN_COOL_DOWN *= 0.75f;
                break;

            case UPGRADE.PROJECTILE_DAMAGE:
                player.PROJECTILE_DAMAGE *= 1.37f;
                break;

            case UPGRADE.PROJECTILE_BOUNCES:
                player.PROJECTILE_BOUNCES += 1;
                break;

            case UPGRADE.PROJECTILE_EXPLOSION:
                player.EXPLOSION_SIZE += 1;
                break;

            case UPGRADE.LASER_DAMAGE:
                player.LASER_MINING_DAMAGE_PER_SECOND *= 2;
                break;

            case UPGRADE.LASER_DISTANCE:
                player.LASER_DISTANCE += 1.5f;
                break;

            default:
                Debug.LogWarning("Unknown upgrade");
                break;
        }
    }
}