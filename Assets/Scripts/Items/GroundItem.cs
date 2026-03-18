using System.Diagnostics.Contracts;
using UnityEngine;

public class GroundItem : MonoBehaviour
{
    public WeaponData weapon;
    public Transform playerTransform;
    public float interactRange = 5f;
    public float sqrInteractionDistance;
    private bool isInRange;
    public bool isConsumable;
    public int ammo;
    public bool canBePickedUp = true;
    public bool refillOnPickUp = false;

    public bool tutorialGun = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sqrInteractionDistance = interactRange * interactRange;
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (!canBePickedUp) return;
        isInRange = CanInteract();


        if (isInRange)
        {

            if (InteractionUI.instance != null)
            {
                InteractionUI.instance.Show("Press E to to pick up " + weapon.weaponName);
            }
            if (Input.GetKeyDown(KeyCode.E) && weapon.weaponName == "Grenade")
            {
                PlayerShootyManager.instance.AddGrenades(1);
                Destroy(gameObject);
                return;
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                WeaponHolder.instance.AddWeapon(weapon);
                Destroy(gameObject);
                if (weapon.weaponName == WeaponHolder.instance.CurrentWeapon.weaponName)
                {
                    WeaponHolder.instance.UpdateWeaponHUD();
                }
                if (Tutoriel.instance != null && tutorialGun) Tutoriel.CompleteStep("pick");
                if (refillOnPickUp)
                {
                    WeaponHolder.instance.CurrentWeapon.ammo = WeaponHolder.instance.CurrentWeapon.ammoCapacity;
                    WeaponHolder.instance.UpdateWeaponHUD();
                }
            }
        }

    }

    public bool CanInteract()
    {
        Physics.Raycast(transform.position, playerTransform.position - transform.position, out RaycastHit hit, interactRange);
        if (hit.collider != null && hit.collider.gameObject == playerTransform.gameObject)
        {
            return true;
        }
        return false;
    }
}
