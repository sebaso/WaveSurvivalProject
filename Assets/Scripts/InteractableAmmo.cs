using UnityEngine;

public class InteractableAmmo : MonoBehaviour
{
    public int interactionDistance = 5;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && Vector3.Distance(transform.position, PlayerController.instance.transform.position) < interactionDistance && WeaponHolder.instance.CurrentWeapon.ammo < WeaponHolder.instance.CurrentWeapon.ammoCapacity && ScoreManager.instance.Score >= WeaponHolder.instance.CurrentWeapon.ammoRefillCost)
        {
            ScoreManager.instance.AddScore(-WeaponHolder.instance.CurrentWeapon.ammoRefillCost);
            WeaponHolder.instance.CurrentWeapon.ammo = WeaponHolder.instance.CurrentWeapon.ammoCapacity;
            WeaponHolder.instance.UpdateAmmo();
        }
    }
}
