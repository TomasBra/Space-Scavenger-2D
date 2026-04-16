using UnityEngine;

public class UpgradeMenu : MonoBehaviour
{
    [SerializeField] GameObject upgradeMenu;
    [SerializeField] GameObject player;
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
