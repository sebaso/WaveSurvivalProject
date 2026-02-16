using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class WeaponBuy : MonoBehaviour, IInteractible
{
    public WeaponData weapon;
    public int price;
    public int interactRange;
    public bool owned = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        weapon.weaponCost = price;
    }

    // Update is called once per frame
    void Update()
    {
        if (CanInteract())
        {

            if (InteractionUI.instance != null)
            {
                InteractionUI.instance.Show(GetInteractText());
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                if (owned)
                {
                    RefillAmmo();
                }
                else
                {
                    BuyWeapon();
                }
            }
        }
    }
    public void BuyWeapon()
    {
        if (ScoreManager.instance.Score >= price)
        {
            owned = true;
            ScoreManager.instance.Score -= price;
            WeaponHolder.instance.AddWeapon(weapon);
            WeaponHolder.instance.UpdateWeaponHUD();
        }
    }

    public void Interact()
    {
        if (owned)
        {
            RefillAmmo();
        }
        else
        {
            BuyWeapon();
        }
    }
    public void RefillAmmo()
    {
        if (ScoreManager.instance.Score >= weapon.ammoRefillCost)
        {
            ScoreManager.instance.Score -= weapon.ammoRefillCost;
            WeaponHolder.instance.CurrentWeapon.ammo = WeaponHolder.instance.CurrentWeapon.ammoCapacity;
            WeaponHolder.instance.UpdateWeaponHUD();
        }
    }
    public bool CanInteract()
    {
        Physics.Raycast(transform.position, PlayerController.instance.transform.position - transform.position, out RaycastHit hit, interactRange);
        if (hit.collider != null && hit.collider.gameObject == PlayerController.instance.gameObject)
        {
            return true;
        }
        return false;
    }
    public void SearchForWeapon()
    {
        foreach (var item in WeaponHolder.instance.availableWeapons)
        {
            print("Item: " + item.weaponName + " Weapon: " + weapon.weaponName);
            if (item.weaponName == weapon.weaponName)
            {
                owned = true;
                break;
            }
            else
            {
                owned = false;
            }
        }
    }

    public string GetInteractText()
    {
        SearchForWeapon();
        if (owned)
        {
            return "Press E to refill " + weapon.weaponName + " ammo for " + weapon.ammoRefillCost.ToString() + " points";
        }
        else
        {
            return "Press E to buy " + weapon.weaponName + " for " + weapon.weaponCost.ToString() + " points";
        }
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public void ScanForWeapons()
    {

    }

    public void UpdateHUD()
    {
        throw new System.NotImplementedException();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRange);
        Gizmos.DrawRay(transform.position, PlayerController.instance.transform.position - transform.position);
    }
}
