using UnityEngine;

public class DefenseObjective : MonoBehaviour
{
    public Transform playerTransform;
    public float interactRange = 2f;
    public bool isInRange;
    public static DefenseObjective instance;
    public bool hasBeenTriggered = false;
    public GameObject generatorWarning;

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

            if (InteractionUI.instance != null)
            {
                InteractionUI.instance.Show("Press E to power up the generator");
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                ActivateGeneratorWarning();
            }
        }

    }
    public void ActivateGeneratorWarning()
    {
        Time.timeScale = 0f;
        generatorWarning.SetActive(true);
    }
    public void DeactivateGeneratorWarning()
    {
        Time.timeScale = 1f;
        generatorWarning.SetActive(false);
    }
    public void ActivateGenerator()
    {
        Time.timeScale = 1f;
        ObjectiveManager.instance.GenerateDefendLocationObjective();
        hasBeenTriggered = true;
        generatorWarning.SetActive(false);
        if (AchievementManager.Instance != null)
            AchievementManager.Instance.IncreaseStat("generator_powered_up", 1);
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
