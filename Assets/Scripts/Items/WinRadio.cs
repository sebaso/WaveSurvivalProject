using UnityEngine;

public class WinRadio : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactionDistance = 3f;
    private float sqrInteractionDistance;
    private Transform playerTransform;
    public AudioClip winSound;
    public AudioSource audioSource;
    void Start()
    {
        sqrInteractionDistance = interactionDistance * interactionDistance;
        if (PlayerController.instance != null)
            playerTransform = PlayerController.instance.transform;
        else
            playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (audioSource == null) audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (playerTransform == null) return;

        float sqrDistance = (transform.position - playerTransform.position).sqrMagnitude;
        if (sqrDistance < sqrInteractionDistance)
        {
            if (BossAI.BossDefeated)
            {
                InteractionUI.instance.Show("Press E to use the radio and call for extraction.");
                if (Input.GetKeyDown(KeyCode.E))
                {
                    WinGame();
                    AchievementManager.Instance?.IncreaseStat("win", 1);
                }
            }
            else
            {
                InteractionUI.instance.Show("The radio is jammed. Defeat the Boss to clear the interference!");
            }
        }
    }

    private void WinGame()
    {
        if (audioSource != null && winSound != null)
        {
            audioSource.PlayOneShot(winSound);
        }

        if (GameWinUI.instance != null)
        {
            GameWinUI.instance.Show();
        }
        else
        {
            Debug.LogWarning("GameWinUI instance not found! Falling back to generic win log.");
            Debug.Log("You Win!");
        }

        // Disable script after winning
        this.enabled = false;
    }
}
