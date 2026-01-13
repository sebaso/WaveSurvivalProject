using System.Collections.Generic;
using UnityEngine;

public class WeaponHolder : MonoBehaviour
{
    public List<WeaponData> availableWeapons = new List<WeaponData>();
    private int currentWeaponIndex = 0;

    public WeaponData CurrentWeapon
    {
        get
        {
            if (availableWeapons.Count == 0) return null;
            return availableWeapons[currentWeaponIndex];
        }
    }

    void Update()
    {
        for (int i = 0; i < 9; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                EquipWeapon(i);
            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            NextWeapon();
        }
    }

    public void EquipWeapon(int index)
    {
        if (index >= 0 && index < availableWeapons.Count)
        {
            currentWeaponIndex = index;
            Debug.Log("Equipado:" + availableWeapons[currentWeaponIndex].weaponName);
        }
    }

    public void NextWeapon()
    {
        if (availableWeapons.Count == 0) return;
        currentWeaponIndex = (currentWeaponIndex + 1) % availableWeapons.Count;
        Debug.Log("Equipado:" + availableWeapons[currentWeaponIndex].weaponName);
    }
}
