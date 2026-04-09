using UnityEngine;
using TMPro;
public class StatsUI : MonoBehaviour
{
    public GameObject[] statsSlots;

    public void Start()
    {
        Debug.Log(statsSlots.Length);
        Debug.Log(statsSlots[0].GetComponentInChildren<TMP_Text>().text);
        if (StatsManager.Instance == null)
            Debug.Log("Ja jsem se nenacetl..");

        UpdateDamage();
    }
    public void UpdateDamage()
    {
        statsSlots[0].GetComponentInChildren<TMP_Text>().text = "Damage: " + StatsManager.Instance.ENEMY_DAMAGE_PER_SECOND;
    }
}
