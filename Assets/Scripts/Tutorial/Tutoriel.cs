using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Tutoriel : MonoBehaviour
{
    public static Tutoriel instance;

    [Header("Steps (drag to reorder, toggle active to enable/disable)")]
    public List<TutorialStep> steps = new();

    [Header("Settings")]
    [Tooltip("Automatically advance to the next step when the current one completes.")]
    public bool autoAdvance = true;

    [Tooltip("Play this clip when a step is completed.")]
    public AudioClip stepCompleteSound;

    [Tooltip("Play this clip when the entire tutorial finishes.")]
    public AudioClip tutorialCompleteSound;

    [Header("Events")]
    public UnityEvent<TutorialStep> OnStepStarted;
    public UnityEvent<TutorialStep> OnStepCompleted;
    public UnityEvent OnTutorialCompleted;
    public GameObject tutorialGun;

    [Header("Enemy Spawning")]
    public GameObject enemyPrefab;
    public GameObject enemyPrefab2;
    public Transform enemySpawnLocation;

    private int currentStepIndex = -1;
    private AudioSource audioSource;
    private TutorialHUD hud;
    private void Awake()
    {
        if (instance != null && instance != this) { Destroy(gameObject); return; }
        instance = this;
        DontDestroyOnLoad(gameObject);
        audioSource = GetComponent<AudioSource>();
        hud = GetComponent<TutorialHUD>();
    }

    private void Start()
    {
        var ids = new HashSet<string>();
        foreach (var step in steps)
        {
            if (!ids.Add(step.id))
                Debug.LogWarning($"[TutorialManager] Duplicate step id: '{step.id}'. Steps must have unique IDs.");
        }

        AdvanceToNextStep();
    }

    public static void CompleteStep(string stepId)
    {
        if (instance == null) return;
        instance.CompleteStepInternal(stepId);
    }

    public static void TriggerStep(string stepId)
    {
        if (instance == null) return;
        instance.TriggerStepInternal(stepId);
    }

    public TutorialStep CurrentStep =>
        currentStepIndex >= 0 && currentStepIndex < steps.Count
            ? steps[currentStepIndex]
            : null;
    public bool IsComplete
    {
        get
        {
            foreach (var step in steps)
                if (step.isEnabled && !step.isComplete) return false;
            return true;
        }
    }
    public void GivePlayerTutorialGun()
    {
        if (tutorialGun != null)
        {
            if (tutorialGun.TryGetComponent<GroundItem>(out var groundItem)) groundItem.canBePickedUp = true;
        }
    }


    private void CompleteStepInternal(string stepId)
    {
        int idx = steps.FindIndex(s => s.id == stepId);
        if (idx < 0)
        {
            Debug.LogWarning($"[TutorialManager] CompleteStep: no step with id '{stepId}'.");
            return;
        }

        TutorialStep step = steps[idx];

        if (!step.isEnabled || step.isComplete) return;

        if (idx != currentStepIndex)
        {
            if (stepId != "aim" && stepId != "move") Debug.Log($"[TutorialManager] Ignored out-of-order completion for '{stepId}'");
            return;
        }

        step.isComplete = true;
        PlaySound(stepCompleteSound);
        if (step.id == "reload" && ScoreManager.instance != null)
        {
            ScoreManager.instance.AddScore(1050);
        }
        if (step.id == "weapon" && ScoreManager.instance != null)
        {
            ScoreManager.instance.AddScore(500);
            ScoreManager.instance.AddScore(500);
        }

        OnStepCompleted?.Invoke(step);

        if (IsComplete)
        {
            hud?.RefreshHUD(steps, currentStepIndex);
            PlaySound(tutorialCompleteSound);
            OnTutorialCompleted?.Invoke();
            hud?.ShowComplete();
            return;
        }

        if (autoAdvance)
        {
            int nextIdx = steps.FindIndex(s => s.isEnabled && !s.isComplete);
            if (nextIdx >= 0)
            {
                currentStepIndex = nextIdx;
                var nextStep = steps[nextIdx];
                if (nextStep.id == "pick" || nextStep.id == "shoot") GivePlayerTutorialGun();
                if (nextStep.id == "shoot") SpawnTutorialEnemy();
                OnStepStarted?.Invoke(nextStep);
                Debug.Log($"[TutorialManager] Step started: '{nextStep.id}' — {nextStep.title}");
            }
        }
        hud?.RefreshHUD(steps, currentStepIndex);
    }

    private void TriggerStepInternal(string stepId)
    {
        int idx = steps.FindIndex(s => s.id == stepId);
        if (idx < 0) return;

        TutorialStep step = steps[idx];
        if (step.isComplete) return;

        currentStepIndex = idx;
        if (step.id == "pick" || step.id == "shoot") GivePlayerTutorialGun();
        if (step.id == "shoot") SpawnTutorialEnemy();

        OnStepStarted?.Invoke(step);
        hud?.RefreshHUD(steps, currentStepIndex);
        Debug.Log($"[TutorialManager] Tutorial jumped to step '{stepId}'.");
        if (steps.Count == currentStepIndex)
        {
            Debug.Log("[TutorialManager] Tutorial completed.");
            SpawnLotsofEnemies();
            CompleteStepInternal("weapon");
        }
    }
    public void SpawnLotsofEnemies()
    {
        for (int i = 0; i < 100; i++)
        {
            Instantiate(enemyPrefab2, enemySpawnLocation.position, enemySpawnLocation.rotation);
            ScoreManager.instance.Score = 0;
        }
        for (int i = 0; i < WeaponHolder.instance.availableWeapons.Count; i++)
        {
            WeaponHolder.instance.availableWeapons[i].currentAmmoInClip = 0;
            WeaponHolder.instance.availableWeapons[i].ammo = 0;
            WeaponHolder.instance.UpdateWeaponHUD();
        }
    }

    private void AdvanceToNextStep()
    {
        for (int i = currentStepIndex + 1; i < steps.Count; i++)
        {
            if (steps[i].isEnabled && !steps[i].isComplete)
            {
                currentStepIndex = i;
                TutorialStep step = steps[i];
                if (step.id == "pick" || step.id == "shoot") GivePlayerTutorialGun();
                if (step.id == "shoot") SpawnTutorialEnemy();


                OnStepStarted?.Invoke(step);
                hud?.RefreshHUD(steps, currentStepIndex);
                Debug.Log($"[TutorialManager] Step started: '{step.id}' — {step.title}");
                return;
            }
        }
    }

    private void SpawnTutorialEnemy()
    {
        if (enemyPrefab != null && enemySpawnLocation != null)
        {
            Instantiate(enemyPrefab, enemySpawnLocation.position, enemySpawnLocation.rotation);
            Debug.Log("[TutorialManager] Spawned target enemy for shoot step.");
        }
        else
        {
            Debug.LogWarning("[TutorialManager] Cannot spawn enemy. Prefab or spawn location is missing!");
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
            audioSource.PlayOneShot(clip);
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.F1) && CurrentStep != null)
            CompleteStepInternal(CurrentStep.id);

        if (Input.GetKeyDown(KeyCode.F2))
            ResetTutorial();
    }

    [ContextMenu("Reset Tutorial")]
    public void ResetTutorial()
    {
        foreach (var step in steps) step.isComplete = false;
        currentStepIndex = -1;
        AdvanceToNextStep();
        Debug.Log("[TutorialManager] Tutorial reset.");
    }

}


[System.Serializable]
public class TutorialStep
{
    [Tooltip("Unique identifier used to complete or trigger this step from code.")]
    public string id;

    [Tooltip("Short title shown in the HUD checklist.")]
    public string title;

    [Tooltip("Key hint shown below the title (e.g. 'WASD', 'LMB', 'R').")]
    public string keyHint;

    [Tooltip("Optional longer description shown when step is active.")]
    [TextArea(1, 3)]
    public string description;

    [Tooltip("Uncheck to exclude this step from the tutorial without deleting it.")]
    public bool isEnabled = true;

    [HideInInspector] public bool isComplete;
}