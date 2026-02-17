using System.Collections.Generic;
using UnityEngine;

public class WeaponHUD : MonoBehaviour
{
    public Transform container;
    public GameObject weaponItemPrefab;
    public float displayDuration = 3f;
    private float hideTimer;
    private CanvasGroup canvasGroup;
    public static WeaponHUD instance;

    private void Awake()
    {
        if (instance == null) instance = this;
        if (container == null) container = transform;
        canvasGroup = container.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = container.gameObject.AddComponent<CanvasGroup>();
        }
    }

    private void Start()
    {
        // Wait for WeaponHolder to be ready
        if (WeaponHolder.instance != null)
        {
            SubscribeToEvents();
            RedrawHUD();
        }
    }

    private void Update()
    {
        if (hideTimer > 0)
        {
            hideTimer -= Time.deltaTime;
            if (hideTimer <= 0)
            {
                HideHUD();
            }
        }
    }

    private void SubscribeToEvents()
    {
        if (WeaponHolder.instance != null)
        {
            WeaponHolder.instance.OnWeaponListChanged += RedrawHUD;
            WeaponHolder.instance.OnWeaponChanged += UpdateSelection;
        }
    }

    private void OnDestroy()
    {
        if (WeaponHolder.instance != null)
        {
            WeaponHolder.instance.OnWeaponListChanged -= RedrawHUD;
            WeaponHolder.instance.OnWeaponChanged -= UpdateSelection;
        }
    }

    public void ShowHUD()
    {
        hideTimer = displayDuration;
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
        }
        else if (container != null)
        {
            container.gameObject.SetActive(true);
        }
    }

    public void HideHUD()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
        }
        else if (container != null)
        {
            container.gameObject.SetActive(false);
        }
    }

    public void RedrawHUD()
    {
        // Clear existing items
        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }
        container.DetachChildren(); // Ensures childCount is 0 immediately for subsequent logic

        if (WeaponHolder.instance == null) return;

        List<WeaponData> weapons = WeaponHolder.instance.availableWeapons;

        if (weapons.Count == 0)
        {
            HideHUD();
            return;
        }

        ShowHUD();

        for (int i = 0; i < weapons.Count; i++)
        {
            GameObject itemObj = Instantiate(weaponItemPrefab, container);
            WeaponHUDItem itemScript = itemObj.GetComponent<WeaponHUDItem>();
            if (itemScript != null)
            {
                bool isSelected = (weapons[i] == WeaponHolder.instance.CurrentWeapon);
                itemScript.Setup(weapons[i], i, isSelected);
            }
        }
    }

    public void UpdateSelection()
    {
        if (WeaponHolder.instance == null) return;

        List<WeaponData> weapons = WeaponHolder.instance.availableWeapons;

        if (weapons.Count == 0)
        {
            HideHUD();
            return;
        }

        ShowHUD();

        int childCount = container.childCount;

        if (childCount != weapons.Count)
        {
            RedrawHUD();
            return;
        }

        for (int i = 0; i < childCount; i++)
        {
            Transform child = container.GetChild(i);
            WeaponHUDItem itemScript = child.GetComponent<WeaponHUDItem>();
            if (itemScript != null)
            {
                bool isSelected = (weapons[i] == WeaponHolder.instance.CurrentWeapon);
                itemScript.Setup(weapons[i], i, isSelected);
            }
        }
    }
}
