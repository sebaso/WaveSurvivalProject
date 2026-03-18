using UnityEngine;
using TMPro;

public class InteractableAmmo : MonoBehaviour
{
    public float interactionDistance = 5f;

    private float sqrInteractionDistance;
    private Transform playerTransform;
    private bool isInRange = false;
    private int cachedCost = -1;

    void Start()
    {
        sqrInteractionDistance = interactionDistance * interactionDistance;

        if (PlayerController.instance != null)
            playerTransform = PlayerController.instance.transform;
        else
            playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        float sqrDistance = (transform.position - playerTransform.position).sqrMagnitude;
        bool nowInRange = sqrDistance < sqrInteractionDistance;

        isInRange = nowInRange;

        if (isInRange)
        {
            var weapon = WeaponHolder.instance.CurrentWeapon;

            if (cachedCost != weapon.ammoRefillCost)
            {
                cachedCost = weapon.ammoRefillCost;
            }

            if (InteractionUI.instance != null)
            {
                InteractionUI.instance.Show("Press E to pay " + cachedCost + " to refill ammo.");
            }

            if (Input.GetKeyDown(KeyCode.E) && weapon.ammo < weapon.ammoCapacity && ScoreManager.instance.Score >= weapon.ammoRefillCost)
            {
                ScoreManager.instance.AddScore(-weapon.ammoRefillCost);
                weapon.ammo = weapon.ammoCapacity;
                WeaponHolder.instance.UpdateAmmo();
                cachedCost = -1;
                if (Tutoriel.instance != null && Tutoriel.instance.CurrentStep != null && Tutoriel.instance.CurrentStep.id == "refill") Tutoriel.CompleteStep("refill");
            }
        }
    }
}
