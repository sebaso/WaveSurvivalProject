using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialHUD : MonoBehaviour
{
    [Header("References")]
    public GameObject hudRoot;
    public Transform stepListContainer;
    public GameObject stepRowPrefab;
    public GameObject completeBanner;

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

    private readonly List<StepRowUI> rows = new();
    private CanvasGroup canvasGroup;
    private Coroutine fadeRoutine;
    private void Awake()
    {
        canvasGroup = hudRoot.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = hudRoot.AddComponent<CanvasGroup>();

        if (completeBanner != null)
            completeBanner.SetActive(false);
    }

    public void RefreshHUD(List<TutorialStep> steps, int activeIndex)
    {
        Debug.Log($"[TutorialHUD] RefreshHUD called. activeIndex={activeIndex}, stepCount={steps.Count}, hideInactive={hideInactiveSteps}");
        for (int i = stepListContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(stepListContainer.GetChild(i).gameObject);
        }
        rows.Clear();
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

            if (!rowGO.TryGetComponent<StepRowUI>(out var row))
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

