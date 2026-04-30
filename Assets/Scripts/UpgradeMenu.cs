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
            {
                UPGRADE.PROJECTILE_COUNT,
                new Upgrades(
                    new Price[]
                    {
                        new Price(20, 0, 0),
                        new Price(45, 10, 0),
                        new Price(90, 25, 1)
                    },
                    UPGRADE.PROJECTILE_COUNT
                )
            },

            {
                UPGRADE.PROJECTILE_SPAWN_COOL_DOWN,
                new Upgrades(
                    new Price[]
                    {
                        new Price(15, 5, 0),
                        new Price(40, 15, 0),
                        new Price(85, 35, 1)
                    },
                    UPGRADE.PROJECTILE_SPAWN_COOL_DOWN
                )
            },

            {
                UPGRADE.PROJECTILE_DAMAGE,
                new Upgrades(
                    new Price[]
                    {
                        new Price(10, 0, 0),
                        new Price(25, 5, 0),
                        new Price(50, 20, 1)
                    },
                    UPGRADE.PROJECTILE_DAMAGE
                )
            },

            {
                UPGRADE.PROJECTILE_SPEED,
                new Upgrades(
                    new Price[]
                    {
                        new Price(12, 0, 0),
                        new Price(30, 8, 0),
                        new Price(65, 20, 1)
                    },
                    UPGRADE.PROJECTILE_SPEED
                )
            },

            {
                UPGRADE.PROJECTILE_LIFETIME,
                new Upgrades(
                    new Price[]
                    {
                        new Price(15, 0, 0),
                        new Price(35, 10, 0),
                        new Price(75, 25, 1)
                    },
                    UPGRADE.PROJECTILE_LIFETIME
                )
            }
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
        player.SetActive(false);
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        upgradeMenu.SetActive(false);
        player.SetActive(true);
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
                break;

            case UPGRADE.PROJECTILE_COUNT:
                player.PROJECTILE_COUNT += 2;
                break;

            case UPGRADE.PROJECTILE_SPAWN_COOL_DOWN:
                player.PROJECTILE_SPAWN_COOL_DOWN *= 0.9f;
                break;

            case UPGRADE.PROJECTILE_DAMAGE:
                player.PROJECTILE_DAMAGE *= 1.2f;
                break;

            case UPGRADE.PROJECTILE_SPEED:
                player.PROJECTILE_SPEED *= 1.314f;
                break;

            case UPGRADE.PROJECTILE_LIFETIME:
                player.PROJECTILE_LIFETIME *= 1.314f;
                break;

            case UPGRADE.LASER_MINING_DAMAGE_PER_SECOND:
                player.LASER_MINING_DAMAGE_PER_SECOND *= 2;
                break;

            case UPGRADE.LASER_DISTANCE:
                player.LASER_DISTANCE *= 2;
                break;

            default:
                Debug.LogWarning("Unknown upgrade");
                break;
        }
    }
}