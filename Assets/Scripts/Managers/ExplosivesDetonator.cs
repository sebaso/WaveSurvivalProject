using UnityEngine;
using UnityEngine.Events;

public class ExplosivesDetonator : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactionDistance = 4f;
    private float sqrInteractionDistance;

    [Header("Events")]
    public UnityEvent OnExplosivesPlaced;

    private Transform playerTransform;

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
        if (playerTransform == null) return;

        float sqrDistance = (transform.position - playerTransform.position).sqrMagnitude;
        bool inRange = sqrDistance < sqrInteractionDistance;

        if (inRange)
        {
            if (ExplosiveInventory.instance != null)
            {
                if (ExplosiveInventory.instance.hasExplosives && PowerManager.instance.isPowerOn)
                {
                    InteractionUI.instance.Show("Press E to place explosives");

                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        PlaceExplosives();
                    }
                }
                else if (ExplosiveInventory.instance.hasExplosives && !PowerManager.instance.isPowerOn)
                {
                    InteractionUI.instance.Show("It's too dark to go in...");
                }
                else
                {
                    InteractionUI.instance.Show("Looks like this wall could be blown up...");
                }
            }
        }
    }

    private void PlaceExplosives()
    {
        ExplosiveInventory.instance.UseExplosives();
        OnExplosivesPlaced?.Invoke();

        // Disable this script to prevent further interaction
        this.enabled = false;
    }
}
