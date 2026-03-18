using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Renders the tutorial checklist as an in-game HUD overlay.
///
/// Requires:
///   - A Canvas > Panel (the HUD root).
///   - A StepRow prefab with:
///       * CheckIcon (Image)     — shown when complete
///       * PendingIcon (Image)   — shown when incomplete
///       * TitleText (TMP_Text)  — step title
///       * KeyText (TMP_Text)    — key hint (e.g. "WASD")
///   - A CompleteBanner object that is hidden by default.
///
/// Attach this component to the same GameObject as TutorialManager.
/// </summary>
public class TutorialHUD : MonoBehaviour
{
    [Header("References")]
    public GameObject hudRoot;               // Parent panel to show/hide
    public Transform stepListContainer;      // Vertical layout group
    public GameObject stepRowPrefab;         // Instantiated per step
    public GameObject completeBanner;        // "Tutorial Complete!" object

    [Header("Colors")]
    public Color activeColor = Color.cyan;
    public Color completedColor = Color.green;
    public Color pendingColor = Color.gray;

    [Header("Animation")]
    public float fadeInDuration = 0.25f;
    public float fadeOutDelay = 3f;
    public bool hideWhenComplete = true;

    [Tooltip("If true, the HUD behaves as a single popup by hiding steps that aren't currently active.")]
    public bool hideInactiveSteps = true;

    // ── Runtime ────────────────────────────────────────────────────────────
    private readonly List<StepRowUI> rows = new();
    private CanvasGroup canvasGroup;
    private Coroutine fadeRoutine;

    // ── Lifecycle ──────────────────────────────────────────────────────────
    private void Awake()
    {
        canvasGroup = hudRoot.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = hudRoot.AddComponent<CanvasGroup>();

        if (completeBanner != null)
            completeBanner.SetActive(false);
    }

    // ── Public API ─────────────────────────────────────────────────────────

    /// <summary>Rebuild the entire checklist UI.</summary>
    public void RefreshHUD(List<TutorialStep> steps, int activeIndex)
    {
        Debug.Log($"[TutorialHUD] RefreshHUD called. activeIndex={activeIndex}, stepCount={steps.Count}, hideInactive={hideInactiveSteps}");

        // Destroy ALL children of the container — not just tracked rows
        for (int i = stepListContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(stepListContainer.GetChild(i).gameObject);
        }
        rows.Clear();

        // Spawn rows only for steps we actually want to show
        for (int i = 0; i < steps.Count; i++)
        {
            TutorialStep step = steps[i];
            if (!step.isEnabled) continue;

            bool isActive = i == activeIndex;
            if (hideInactiveSteps && !isActive) continue;

            if (stepRowPrefab == null)
            {
                Debug.LogError("[TutorialHUD] stepRowPrefab is null!");
                return;
            }

            GameObject rowGO = Instantiate(stepRowPrefab, stepListContainer);
            StepRowUI row = rowGO.GetComponent<StepRowUI>();

            if (row == null)
            {
                Debug.LogWarning("[TutorialHUD] stepRowPrefab is missing a StepRowUI component.");
                Destroy(rowGO);
                continue;
            }

            row.Populate(step, isActive, activeColor, completedColor, pendingColor);
            rows.Add(row);
            Debug.Log($"[TutorialHUD] Created row for step '{step.id}' (isActive={isActive}, isComplete={step.isComplete})");
        }

        Debug.Log($"[TutorialHUD] Total rows created: {rows.Count}");
        ShowHUD();
    }

    /// <summary>Show the "Tutorial Complete!" banner and optionally hide the checklist.</summary>
    public void ShowComplete()
    {
        if (completeBanner != null)
            completeBanner.SetActive(true);

        if (hideWhenComplete)
        {
            if (fadeRoutine != null) StopCoroutine(fadeRoutine);
            fadeRoutine = StartCoroutine(FadeOutAfterDelay());
        }
    }

    // ── Internal ────────────────────────────────────────────────────────────

    private void ShowHUD()
    {
        hudRoot.SetActive(true);
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        float t = 0f;
        canvasGroup.alpha = 0f;
        while (t < fadeInDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(t / fadeInDuration);
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }

    private IEnumerator FadeOutAfterDelay()
    {
        yield return new WaitForSeconds(fadeOutDelay);

        float t = fadeInDuration;
        while (t > 0f)
        {
            t -= Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(t / fadeInDuration);
            yield return null;
        }
        hudRoot.SetActive(false);
    }
}

// ── Row UI Component ────────────────────────────────────────────────────────

/// <summary>
/// Attach this to the StepRow prefab root.
/// Wire up the child references in the Inspector.
/// </summary>

