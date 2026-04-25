using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeItemUI : MonoBehaviour
{
    [Header("Upgrade")]
    [SerializeField] UPGRADE upgradeType;

    [Header("Texts")]
    [SerializeField] TMP_Text copperCostText;
    [SerializeField] TMP_Text ironCostText;
    [SerializeField] TMP_Text goldCostText;
    [SerializeField] TMP_Text titleText;

    [Header("UI")]
    [SerializeField] Button buyButton;
    [SerializeField] Slider fillSlider;

    private UpgradeMenu menu;

    public void Init(UpgradeMenu menu)
    {
        this.menu = menu;

        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(() =>
        {
            menu.TryBuyUpgrade(upgradeType);
        });

        Refresh();
    }

    public void Refresh()
    {
        
        if (menu == null) return;

        Upgrades upgrade = menu.GetUpgrade(upgradeType);
        if (upgrade == null) return;

        if (upgrade.IsMaxed)
        {
            copperCostText.text = "-";
            ironCostText.text = "-";
            goldCostText.text = "-";

            buyButton.interactable = false;

            if (fillSlider != null)
                fillSlider.value = 1f;

            return;
        }

        Price price = upgrade.GetCurrentPrice();

        copperCostText.text = price.copperPrice.ToString();
        ironCostText.text = price.ironPrice.ToString();
        goldCostText.text = price.goldPrice.ToString();

        buyButton.interactable = true;

        if (fillSlider != null)
        {
            fillSlider.value = (float)upgrade.OwnedTier / upgrade.MaxTier;
        }
        Debug.Log($"{upgradeType}: {upgrade.OwnedTier}/{upgrade.MaxTier}");
    }
}