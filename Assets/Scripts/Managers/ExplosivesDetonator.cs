using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class ExplosivesDetonator : MonoBehaviour
{
    public static ExplosivesDetonator instance;
    [Header("Interaction Settings")]
    public float interactionDistance = 4f;
    private float sqrInteractionDistance;

    [Header("Events")]
    public UnityEvent OnExplosivesPlaced;
    public GameObject explosionPrefab;
    public AudioClip explosionSound;
    public List<GameObject> toDisable;
    public GameObject toEnable;

    private Transform playerTransform;

    void Start()
    {
        instance = this;
        sqrInteractionDistance = interactionDistance * interactionDistance;

        if (PlayerController.instance != null)
            playerTransform = PlayerController.instance.transform;

        if (playerTransform == null)
        {
            Debug.LogError("Player not found");
        }
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
                    InteractionUI.instance.Show("Press E to place explosives.");

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

    private IEnumerator Explosion()
    {
        yield return new WaitForSeconds(3);

        if (FadeToBlack.instance != null)
        {
            FadeToBlack.instance.FadeOut(1.5f);
            yield return new WaitForSeconds(1.5f);
        }

        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        AudioSource.PlayClipAtPoint(explosionSound, transform.position);

        foreach (GameObject obj in toDisable)
        {
            if (obj != null) obj.SetActive(false);
        }
        if (toEnable != null)
        {
            toEnable.SetActive(true);
        }

        yield return new WaitForSeconds(1.0f);

        if (FadeToBlack.instance != null)
        {
            FadeToBlack.instance.FadeIn(1.5f);
        }

        this.enabled = false;
    }

    public void PlaceExplosives()
    {
        ExplosiveInventory.instance.UseExplosives();
        OnExplosivesPlaced?.Invoke();
        StartCoroutine(Explosion());
        ExplosiveInventory.instance.hasExplosives = false;


    }
}
