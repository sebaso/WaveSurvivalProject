using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public PlayerController player;
    public Image fillImage;
    public static HealthBar instance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (player == null)
            player = FindFirstObjectByType<PlayerController>();
        instance = this;
        UpdateHealthBar();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void UpdateHealthBar()
    {
        fillImage.fillAmount = (float)player.hp / player.maxHp;
    }
}
