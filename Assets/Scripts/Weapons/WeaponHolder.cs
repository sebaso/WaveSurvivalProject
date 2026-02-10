using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class WeaponHolder : MonoBehaviour
{
    public List<WeaponData> availableWeapons = new();
    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI weaponNameText;
    private int currentWeaponIndex = 0;
    public static WeaponHolder instance;
    public Image reloadImage;
    public bool isReloading = false;
    void Start()
    {
        instance = this;
        CurrentWeapon.currentAmmoInClip = CurrentWeapon.clipSize;
        CurrentWeapon.ammo = CurrentWeapon.ammoCapacity;
        UpdateWeaponHUD();
    }

    public WeaponData CurrentWeapon
    {
        get
        {
            if (availableWeapons.Count == 0) return null;
            return availableWeapons[currentWeaponIndex];
        }
    }

    private Coroutine currentReloadCoroutine;

    void Update()
    {
        for (int i = 0; i < 9; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                EquipWeapon(i);
                UpdateWeaponHUD();
            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            NextWeapon();
            UpdateWeaponHUD();
        }
    }

    public void Reload()
    {
        if (availableWeapons.Count == 0 || CurrentWeapon == null) return;

        if (CurrentWeapon.currentAmmoInClip >= CurrentWeapon.clipSize) return;
        if (CurrentWeapon.ammo <= 0) return;

        if (isReloading) return;

        currentReloadCoroutine = StartCoroutine(ReloadRoutine());
    }

    public void CancelReload()
    {
        if (isReloading)
        {
            if (currentReloadCoroutine != null)
                StopCoroutine(currentReloadCoroutine);

            isReloading = false;
            reloadImage.enabled = false;
            reloadImage.fillAmount = 0;
            currentReloadCoroutine = null;
        }
    }

    private System.Collections.IEnumerator ReloadRoutine()
    {
        isReloading = true;
        reloadImage.enabled = true;
        reloadImage.fillAmount = 0;
        float reloadTime = CurrentWeapon.reloadTime;
        float reloadTimer = 0;

        while (reloadTimer < reloadTime)
        {
            reloadTimer += Time.deltaTime;
            reloadImage.fillAmount = reloadTimer / reloadTime;
            yield return null;
        }

        isReloading = false;
        reloadImage.enabled = false;

        int ammoNeeded = CurrentWeapon.clipSize - CurrentWeapon.currentAmmoInClip;
        int ammoToReload = Mathf.Min(ammoNeeded, CurrentWeapon.ammo);

        CurrentWeapon.currentAmmoInClip += ammoToReload;
        CurrentWeapon.ammo -= ammoToReload;

        UpdateAmmo();
        currentReloadCoroutine = null;
    }

    public void UpdateWeaponHUD()
    {
        if (CurrentWeapon == null) return;
        weaponNameText.text = CurrentWeapon.weaponName;
        UpdateAmmo();
    }
    public void UpdateAmmo()
    {
        if (CurrentWeapon == null) return;
        ammoText.text = CurrentWeapon.currentAmmoInClip.ToString() + "/" + CurrentWeapon.ammo.ToString();
    }

    public void EquipWeapon(int index)
    {
        if (index >= 0 && index < availableWeapons.Count)
        {
            CancelReload();
            currentWeaponIndex = index;
            Debug.Log("Equipado:" + availableWeapons[currentWeaponIndex].weaponName);
        }
    }

    public void NextWeapon()
    {
        if (availableWeapons.Count == 0) return;
        CancelReload();
        currentWeaponIndex = (currentWeaponIndex + 1) % availableWeapons.Count;
        Debug.Log("Equipado:" + availableWeapons[currentWeaponIndex].weaponName);
    }
}
