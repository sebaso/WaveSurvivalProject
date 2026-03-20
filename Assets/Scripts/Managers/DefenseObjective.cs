using UnityEngine;

public class DefenseObjective : MonoBehaviour
{
    public Transform playerTransform;
    public float interactRange = 2f;
    public bool isInRange;
    public static DefenseObjective instance;
    public bool hasBeenTriggered = false;
    public GameObject generatorWarning;
    public AudioClip generatorPoweredUpSound, generatorTriedToTurnOnSound;
    public AudioSource audioSource;


    void Start()
    {
        playerTransform = PlayerController.instance.transform;
        instance = this;
    }
    // Update is called once per frame
    void Update()
    {
        if (playerTransform == null)
        {
            playerTransform = PlayerController.instance.transform;
        }
        isInRange = CanInteract();


        if (isInRange)
        {
            if (hasBeenTriggered) return;

            InteractionUI.instance?.Show("Press E to power up the generator");

            if (Input.GetKeyDown(KeyCode.E))
            {
                ActivateGeneratorWarning();
            }
        }

    }
    public void ActivateGeneratorWarning()
    {
        TimeManager.instance.RequestPause(this);
        generatorWarning.SetActive(true);
    }
    public void DeactivateGeneratorWarning()
    {
        TimeManager.instance.RequestResume(this);
        generatorWarning.SetActive(false);
    }
    public void ActivateGenerator()
    {
        TimeManager.instance.RequestResume(this);
        ObjectiveManager.instance.GenerateDefendLocationObjective();
        hasBeenTriggered = true;
        generatorWarning.SetActive(false);
        audioSource.PlayOneShot(generatorTriedToTurnOnSound);

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
