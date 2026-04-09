using UnityEngine;
using TMPro;
public class StatsUI : MonoBehaviour
{
    public GameObject[] statsSlots;
    public CanvasGroup statsCanvas;

    private bool statsOpen = false;
    public void Start()
    {
        UpdateAllStats();
    }

    private void Update()
    {
        if (Input.GetButtonDown("ToggleStats"))
        {
            if (statsOpen)
            {
                Time.timeScale = 1;
                statsCanvas.alpha = 0;
                statsOpen = false;
            }

            else
            {
                Time.timeScale = 0;
                statsCanvas.alpha = 1;
                statsOpen = true;
            }
                
        }
    }
    public void UpdateDamage()
    {
        statsSlots[0].GetComponentInChildren<TMP_Text>().text = "Damage: " + StatsManager.Instance.ENEMY_DAMAGE_PER_SECOND;
    }

    public void UpdateMiningSpeed()
    {
        statsSlots[1].GetComponentInChildren<TMP_Text>().text = "Mining Speed: " + StatsManager.Instance.MINING_DAMAGE_PER_SECOND;
    }

    public void UpdateAllStats()
    {
        UpdateDamage();
        UpdateMiningSpeed();
    }
}
