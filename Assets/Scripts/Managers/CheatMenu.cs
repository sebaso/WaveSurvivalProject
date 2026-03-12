using Unity.VisualScripting;
using UnityEngine;

public class CheatMenu : MonoBehaviour
{
    public GameObject cheatMenu;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            cheatMenu.SetActive(!cheatMenu.activeSelf);
        }
        if (Input.GetKeyDown(KeyCode.L) && cheatMenu.activeSelf)
        {
            ScoreManager.instance.AddScore(1000);
        }
    }
    public void InfiniteAmmo()
    {
        WeaponHolder.instance.CurrentWeapon.currentAmmoInClip = 9999;
    }
    public void InfiniteHealth()
    {
        PlayerController.instance.hp = PlayerController.instance.maxHp;
    }
    public void UpgradeHealth()
    {
        VendingMachine.instance.UpgradeHealth();
    }
    public void UpgradeSpeed()
    {
        VendingMachine.instance.UpgradeSpeed();
    }
    public void UpgradeReloadSpeed()
    {
        VendingMachine.instance.UpgradeReloadSpeed();
    }
}
