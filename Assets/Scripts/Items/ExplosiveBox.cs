using UnityEngine;

public class ExplosiveBox : MonoBehaviour
{
    [Header("Search Settings")]
    public float searchDuration = 3f;
    public float interactionDistance = 3f;

    [Header("Find Chances (per consecutive box searched)")]
    public float[] findChances = { 0.15f, 0.30f, 0.50f, 0.75f, 1f };

    private static int boxesSearched = 0;

    private float searchProgress = 0f;
    private float sqrInteractionDistance;
    private Transform playerTransform;
    private bool isSearched = false;
    private bool showingResult = false;
    private float resultTimer = 0f;
    private string resultText = "";

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
        if (isSearched)
        {
            HandleResultDisplay();
            return;
        }

        if (playerTransform == null) return;

        float sqrDistance = (transform.position - playerTransform.position).sqrMagnitude;
        bool inRange = sqrDistance < sqrInteractionDistance;

        if (!inRange)
        {
            return;
        }
        float percent = Mathf.Clamp01(searchProgress / searchDuration) * 100f;
        bool isShooting = Input.GetMouseButton(0);
        bool isReloading = WeaponHolder.instance != null && WeaponHolder.instance.isReloading;

        if (isShooting || isReloading)
        {
            InteractionUI.instance.Show("Searching paused... " + Mathf.FloorToInt(percent) + "%");
            return;
        }

        if (Input.GetKey(KeyCode.E))
        {
            searchProgress += Time.deltaTime;
            percent = Mathf.Clamp01(searchProgress / searchDuration) * 100f;

            if (searchProgress >= searchDuration)
            {
                CompleteSearch();
                return;
            }

            InteractionUI.instance.Show("Searching... " + Mathf.FloorToInt(percent) + "%");
        }
        else
        {
            InteractionUI.instance.Show("Hold E to search [" + Mathf.FloorToInt(percent) + "%]");
        }
    }

    void CompleteSearch()
    {
        isSearched = true;

        int index = Mathf.Min(boxesSearched, findChances.Length - 1);
        float chance = findChances[index];
        bool found = Random.value <= chance;

        boxesSearched++;

        if (found && ExplosiveInventory.instance != null)
        {
            ExplosiveInventory.instance.GiveExplosives();
            resultText = "Found explosives!";
        }
        else
        {
            resultText = "Empty...";
        }

        showingResult = true;
        resultTimer = 2f;
    }

    void HandleResultDisplay()
    {
        if (!showingResult) return;

        resultTimer -= Time.deltaTime;
        InteractionUI.instance.Show(resultText);

        if (resultTimer <= 0f)
        {
            showingResult = false;
            // Optionally disable the box visually
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Call this to reset the static search counter (e.g., on scene reload).
    /// </summary>
    public static void ResetBoxesSearched()
    {
        boxesSearched = 0;
    }
}
