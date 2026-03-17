using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VendingMachine : MonoBehaviour
{
    public static VendingMachine instance;
    public int interactionDistance = 2;
    public GameObject vendingMachineUI;

    [Header("Power State")]
    public bool powered;
    public int unpoweredBuyLimit = 1;

    [Header("Speed Upgrade")]
    public int playerSpeedUpgrade;
    public int playerSpeedUpgradeMax = 3;
    public int playerSpeedUpgradeCost = 2000;
    public Button speedButton;
    public Image[] speedUpgradeImages;

    [Header("Health Upgrade")]
    public int playerHealthUpgrade;
    public int playerHealthUpgradeMax = 4;
    public int playerHealthUpgradeCost = 1000;
    public Button healthButton;
    public Image[] healthUpgradeImages;

    [Header("Reload Speed Upgrade")]
    public int playerReloadSpeedUpgrade;
    public int playerReloadSpeedUpgradeMax = 3;
    public int playerReloadSpeedUpgradeCost = 1500;
    public Button reloadSpeedButton;
    public Image[] reloadSpeedUpgradeImages;

    [Header("Upgrade Tier Colors")]
    public Color availableColor = Color.orange;
    public Color lockedColor = Color.red;
    public Color boughtColor = Color.green;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        UpdateUI();
    }

    void DisplayInteractText()
    {
        if (powered)
        {
            InteractionUI.instance.Show("Press E to interact");
        }
        else
        {
            InteractionUI.instance.Show("Press E to interact (unpowered)");
        }
    }

    public bool CanInteract()
    {
        Physics.Raycast(transform.position, PlayerController.instance.transform.position - transform.position, out RaycastHit hit, interactionDistance);
        if (hit.collider != null && hit.collider.gameObject == PlayerController.instance.gameObject)
        {
            return true;
        }
        return false;
    }

    void Update()
    {
        if (CanInteract())
        {
            DisplayInteractText();
            if (Input.GetKeyDown(KeyCode.E))
            {
                vendingMachineUI.SetActive(true);
                TimeManager.instance.RequestPause(this);
                PlayerController.instance.enabled = false;
                PlayerShootyManager.instance.enabled = false;
                //Cursor.lockState = CursorLockMode.None;
                //Cursor.visible = true;
                UpdateUI();
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseVendingMachine();
        }
    }

    public void CloseVendingMachine()
    {
        if (vendingMachineUI.activeSelf)
        {
            vendingMachineUI.SetActive(false);
            TimeManager.instance.RequestResume(this);
            PlayerController.instance.enabled = true;
            PlayerShootyManager.instance.enabled = true;
            //Cursor.lockState = CursorLockMode.Locked;
            //Cursor.visible = false;
        }
    }
    int GetEffectiveMax(int upgradeMax)
    {
        return powered ? upgradeMax : Mathf.Min(unpoweredBuyLimit, upgradeMax);
    }
    void UpdateUI()
    {
        int speedMax = GetEffectiveMax(playerSpeedUpgradeMax);
        int healthMax = GetEffectiveMax(playerHealthUpgradeMax);
        int reloadMax = GetEffectiveMax(playerReloadSpeedUpgradeMax);


        speedButton.GetComponentInChildren<TextMeshProUGUI>().text =
            "Speed Upgrade " + playerSpeedUpgrade + "/" + speedMax + " Cost: " + playerSpeedUpgradeCost;
        healthButton.GetComponentInChildren<TextMeshProUGUI>().text =
            "Health Upgrade " + playerHealthUpgrade + "/" + healthMax + " Cost: " + playerHealthUpgradeCost;
        reloadSpeedButton.GetComponentInChildren<TextMeshProUGUI>().text =
            "Reload Speed Upgrade " + playerReloadSpeedUpgrade + "/" + reloadMax + " Cost: " + playerReloadSpeedUpgradeCost;

        speedButton.interactable = playerSpeedUpgrade < speedMax;
        healthButton.interactable = playerHealthUpgrade < healthMax;
        reloadSpeedButton.interactable = playerReloadSpeedUpgrade < reloadMax;
        UpdateTierImages(speedUpgradeImages, playerSpeedUpgrade, playerSpeedUpgradeMax);
        UpdateTierImages(healthUpgradeImages, playerHealthUpgrade, playerHealthUpgradeMax);
        UpdateTierImages(reloadSpeedUpgradeImages, playerReloadSpeedUpgrade, playerReloadSpeedUpgradeMax);
    }

    void UpdateTierImages(Image[] tierImages, int currentLevel, int upgradeMax)
    {
        if (tierImages == null) return;

        int effectiveMax = GetEffectiveMax(upgradeMax);

        for (int i = 0; i < tierImages.Length; i++)
        {
            if (tierImages[i] == null) continue;

            if (i < currentLevel)
            {
                tierImages[i].color = boughtColor;
            }
            else if (i < effectiveMax)
            {
                tierImages[i].color = availableColor;
            }
            else
            {
                tierImages[i].color = lockedColor;
            }
        }
    }

    public void UpgradeSpeed()
    {
        int effectiveMax = GetEffectiveMax(playerSpeedUpgradeMax);
        if (ScoreManager.instance.Score >= playerSpeedUpgradeCost && playerSpeedUpgrade < effectiveMax)
        {
            ScoreManager.instance.Score -= playerSpeedUpgradeCost;
            playerSpeedUpgrade++;
            playerSpeedUpgradeCost += 2000;
            PlayerController.instance.maxSpeed += 1f;
        }
        UpdateUI();
    }

    public void UpgradeHealth()
    {
        int effectiveMax = GetEffectiveMax(playerHealthUpgradeMax);
        if (ScoreManager.instance.Score >= playerHealthUpgradeCost && playerHealthUpgrade < effectiveMax)
        {
            ScoreManager.instance.Score -= playerHealthUpgradeCost;
            playerHealthUpgrade++;
            playerHealthUpgradeCost += 1000;
            PlayerController.instance.maxHp += 1;
            PlayerController.instance.hp += 1;
        }
        UpdateUI();
    }

    public void UpgradeReloadSpeed()
    {
        int effectiveMax = GetEffectiveMax(playerReloadSpeedUpgradeMax);
        if (ScoreManager.instance.Score >= playerReloadSpeedUpgradeCost && playerReloadSpeedUpgrade < effectiveMax)
        {
            ScoreManager.instance.Score -= playerReloadSpeedUpgradeCost;
            playerReloadSpeedUpgrade++;
            playerReloadSpeedUpgradeCost += 1500;
            PlayerShootyManager.instance.reloadTimeModifier -= 0.1f;
        }
        UpdateUI();
    }
}
