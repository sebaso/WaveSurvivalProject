using UnityEngine;

public class VendingMachine : MonoBehaviour
{
    public static VendingMachine instance;
    public int interactionDistance = 2;
    public GameObject vendingMachineUI;
    public int playerSpeedUpgrade;
    public int playerSpeedUpgradeMax = 3;
    public int playerHealthUpgrade;
    public int playerHealthUpgradeMax = 4;
    public int playerReloadSpeedUpgrade;
    public int playerReloadSpeedUpgradeMax = 3;
    public int playerSpeedUpgradeCost = 2000;
    public int playerHealthUpgradeCost = 1000;
    public int playerReloadSpeedUpgradeCost = 1500;
    public bool powered;
    private void Awake()
    {
        instance = this;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void UpgradeSpeed()
    {
        if (ScoreManager.instance.Score >= playerSpeedUpgradeCost && playerSpeedUpgrade < playerSpeedUpgradeMax)
        {
            ScoreManager.instance.Score -= playerSpeedUpgradeCost;
            playerSpeedUpgrade++;
            playerSpeedUpgradeCost += 2000;
            PlayerController.instance.speed += 0.5f;
        }
    }
    public void UpgradeHealth()
    {
        if (ScoreManager.instance.Score >= playerHealthUpgradeCost && playerHealthUpgrade < playerHealthUpgradeMax)
        {
            ScoreManager.instance.Score -= playerHealthUpgradeCost;
            playerHealthUpgrade++;
            playerHealthUpgradeCost += 1000;
            PlayerController.instance.maxHp += 1;
            PlayerController.instance.hp += 1;
        }
    }
    public void UpgradeReloadSpeed()
    {
        if (ScoreManager.instance.Score >= playerReloadSpeedUpgradeCost && playerReloadSpeedUpgrade < playerReloadSpeedUpgradeMax)
        {
            ScoreManager.instance.Score -= playerReloadSpeedUpgradeCost;
            playerReloadSpeedUpgrade++;
            playerReloadSpeedUpgradeCost += 1500;
            PlayerShootyManager.instance.reloadTimeModifier -= 0.1f;
        }
    }
}
