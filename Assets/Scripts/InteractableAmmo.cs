using UnityEngine;
using TMPro;

public class InteractableAmmo : MonoBehaviour
{
    public float interactionDistance = 5f;
    public TextMeshProUGUI ammoText;

    private float sqrInteractionDistance;
    private Transform playerTransform;
    private bool isInRange = false;
    private int cachedCost = -1;

    void Start()
    {
        ammoText = GameObject.FindGameObjectWithTag("MainText").GetComponent<TextMeshProUGUI>();
        ammoText.enabled = false;
        sqrInteractionDistance = interactionDistance * interactionDistance;
        playerTransform = PlayerController.instance.transform;
    }

    void Update()
    {
        float sqrDistance = (transform.position - playerTransform.position).sqrMagnitude;
        bool nowInRange = sqrDistance < sqrInteractionDistance;

        if (nowInRange != isInRange)
        {
            isInRange = nowInRange;
            ammoText.enabled = isInRange;
        }

        if (isInRange)
        {
            var weapon = WeaponHolder.instance.CurrentWeapon;

            if (cachedCost != weapon.ammoRefillCost)
            {
                cachedCost = weapon.ammoRefillCost;
                ammoText.text = "Press E to pay " + cachedCost + " to refill ammo.";
            }

            if (Input.GetKeyDown(KeyCode.E) && weapon.ammo < weapon.ammoCapacity && ScoreManager.instance.Score >= weapon.ammoRefillCost)
            {
                ScoreManager.instance.AddScore(-weapon.ammoRefillCost);
                weapon.ammo = weapon.ammoCapacity;
                WeaponHolder.instance.UpdateAmmo();
                cachedCost = -1;
            }
        }
    }
}
