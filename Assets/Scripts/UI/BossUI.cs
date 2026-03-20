using UnityEngine;
using UnityEngine.UI;

public class BossUI : MonoBehaviour
{
    public static BossUI instance;
    public GameObject uiPanel;
    public Image healthFill;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        if (uiPanel != null)
            uiPanel.SetActive(false);
    }

    private void OnEnable()
    {
        BossAI.OnHealthChanged += UpdateHealth;
        BossAI.OnBossDefeated += HideBossUI;
    }

    private void OnDisable()
    {
        BossAI.OnHealthChanged -= UpdateHealth;
        BossAI.OnBossDefeated -= HideBossUI;
    }

    public void ShowBossUI()
    {
        if (uiPanel != null)
            uiPanel.SetActive(true);
    }

    public void HideBossUI()
    {
        if (uiPanel != null)
            uiPanel.SetActive(false);
    }

    private void UpdateHealth(int currentHealth, int maxHealth)
    {
        ShowBossUI();

        if (healthFill != null)
        {
            healthFill.fillAmount = (float)currentHealth / maxHealth;
        }
    }
}
