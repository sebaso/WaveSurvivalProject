using UnityEngine;
using UnityEngine.UI;

public class GrenadeUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject[] grenadeIcons;
    
    private PlayerShootyManager player;

    void Start()
    {
        player = PlayerShootyManager.instance;
        if (player != null)
        {
            player.OnGrenadeCountChanged += UpdateGrenadeIcons;
            UpdateGrenadeIcons(player.grenadeCount, player.maxGrenadeCount);
        }
    }

    void OnDestroy()
    {
        if (player != null)
        {
            player.OnGrenadeCountChanged -= UpdateGrenadeIcons;
        }
    }

    public void UpdateGrenadeIcons(int current, int max)
    {
        if (grenadeIcons == null) return;

        for (int i = 0; i < grenadeIcons.Length; i++)
        {
            if (grenadeIcons[i] != null)
            {
                grenadeIcons[i].SetActive(i < current);
            }
        }
    }
}
