using System.Collections;
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

    public System.Action OnWeaponListChanged;
    public System.Action OnWeaponChanged;
    public GameObject dropPrefab;
    public bool isReloading = false;
    void Awake()
    {
        instance = this;
        if (availableWeapons == null) availableWeapons = new List<WeaponData>();
    }

    void Start()
    {
        if (CurrentWeapon != null)
        {
            CurrentWeapon.currentAmmoInClip = CurrentWeapon.clipSize;
            CurrentWeapon.ammo = CurrentWeapon.ammoCapacity;
            UpdateWeaponHUD();
        }
    }

    public WeaponData CurrentWeapon
    {
        get
        {
            if (availableWeapons.Count == 0) return null;
            if (currentWeaponIndex >= availableWeapons.Count) return availableWeapons[0];
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
        if (Input.GetKeyDown(KeyCode.G))
        {
            DropWeapon();
        }
    }
    public void DropWeapon()
    {
        StopAllCoroutines();
        // Prevent dropping if there's only 1 weapon or no weapons
        if (availableWeapons.Count <= 1) return;
        
        GameObject weapon = Instantiate(dropPrefab, transform.position, transform.rotation);
        weapon.GetComponent<GroundItem>().weapon = availableWeapons[currentWeaponIndex];
        weapon.GetComponent<GroundItem>().ammo = availableWeapons[currentWeaponIndex].ammo + availableWeapons[currentWeaponIndex].currentAmmoInClip;
        availableWeapons.Remove(availableWeapons[currentWeaponIndex]);
        NextWeapon();
        OnWeaponListChanged?.Invoke();
        UpdateWeaponHUD();

    }
    public IEnumerator DisplayGoofyMessage()
    {

        yield return new WaitForSeconds(1f);
        InteractionUI.instance.Show("Buen intento... pero no.");

        yield return new WaitForSeconds(1f);
        InteractionUI.instance.Hide();
    }

    public void AddWeapon(WeaponData weapon)
    {
        foreach (var item in availableWeapons)
        {
            if (item.weaponName == weapon.weaponName && !item.consumable)
            {
                StartCoroutine(DisplayGoofyMessage());
                item.ammo += weapon.ammo + weapon.currentAmmoInClip;

                return;
            }
        }
        availableWeapons.Add(weapon);
        currentWeaponIndex = availableWeapons.Count - 1;
        UpdateWeaponHUD();
        OnWeaponListChanged?.Invoke();
        OnWeaponChanged?.Invoke();

    }
    public void RemoveWeapon(WeaponData weapon)
    {
        availableWeapons.Remove(weapon);
        OnWeaponListChanged?.Invoke();
        NextWeapon();
        UpdateWeaponHUD();
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
        float reloadTime = CurrentWeapon.reloadTime * PlayerShootyManager.instance.reloadTimeModifier;
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
        if (CurrentWeapon.consumable)
        {
            ammoText.text = "Press E to use";
        }
        else
        {
            ammoText.text = CurrentWeapon.currentAmmoInClip.ToString() + "/" + CurrentWeapon.ammo.ToString();
        }
    }

    public void EquipWeapon(int index)
    {
        if (index >= 0 && index < availableWeapons.Count)
        {
            CancelReload();
            currentWeaponIndex = index;
            Debug.Log("Equipado:" + availableWeapons[currentWeaponIndex].weaponName);
            CheckItemType();
            OnWeaponChanged?.Invoke();
        }
    }

    public void NextWeapon()
    {
        if (availableWeapons.Count == 0) return;
        CancelReload();
        currentWeaponIndex = (currentWeaponIndex + 1) % availableWeapons.Count;
        if (availableWeapons[currentWeaponIndex] == null)
        {
            currentWeaponIndex = 0;
        }
        Debug.Log("Equipado:" + availableWeapons[currentWeaponIndex].weaponName);
        CheckItemType();
        OnWeaponChanged?.Invoke();
    }
    public void CheckItemType()
    {
        if (availableWeapons[currentWeaponIndex].consumable == false)
        {
            PlayerShootyManager.instance.itemType = PlayerShootyManager.ItemType.Weapon;
        }
        else
        {
            PlayerShootyManager.instance.itemType = PlayerShootyManager.ItemType.Consumable;
        }
    }
}
