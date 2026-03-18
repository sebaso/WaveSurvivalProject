using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AchievementUI : MonoBehaviour
{
    public GameObject achievementCard;
    public GameObject achievementPrefab;
    public Transform achievementParent;
    public float displayDuration = 3f;
    public float fadeDuration = 0.4f;

    private readonly Queue<(string name, string iconName)> pending = new();
    private bool isExecuting = false;

    void Start()
    {
        if (achievementCard != null)
            achievementCard.SetActive(false);
    }

    void OnEnable() { AchievementManager.OnAchievementUnlocked += Enqueue; }

    void OnDisable()
    {
        AchievementManager.OnAchievementUnlocked -= Enqueue;
        StopAllCoroutines();
        isExecuting = false;
        achievementCard?.SetActive(false);
    }

    private void Enqueue(string name, string iconName)
    {
        pending.Enqueue((name, iconName));
        if (!isExecuting) StartCoroutine(DisplayRoutine());
    }

    private IEnumerator DisplayRoutine()
    {
        isExecuting = true;

        achievementCard?.SetActive(true);

        while (pending.Count > 0)
        {
            var data = pending.Dequeue();

            if (achievementParent != null)
            {
                for (int i = achievementParent.childCount - 1; i >= 0; i--)
                {
                    Transform child = achievementParent.GetChild(i);
                    if (child != null) Destroy(child.gameObject);
                }
            }
            GameObject spawnedPrefab = null;
            if (achievementPrefab != null && achievementParent != null &&
                DataManager.Instance != null && DataManager.Instance._data != null)
            {
                spawnedPrefab = Instantiate(achievementPrefab, achievementParent);
                Achievement achievement = DataManager.Instance._data._achievement
                    .FirstOrDefault(a => a != null && a._name == data.name);

                if (achievement != null && spawnedPrefab != null)
                {
                    Stat stat = DataManager.Instance.GetStateWithCode(achievement._statCode);
                    spawnedPrefab.GetComponent<AchievementPrefab>()?.SetAchievement(
                        achievement._name, achievement._description, achievement._imageName,
                        achievement._statCode, achievement._targetAmmount,
                        stat != null ? stat._value : 0);
                }
            }

            if (spawnedPrefab != null)
            {
                spawnedPrefab.transform.localScale = Vector3.zero;
                float t = 0;
                while (t < fadeDuration)
                {
                    t += Time.deltaTime;
                    spawnedPrefab.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t / fadeDuration);
                    yield return null;
                }
                spawnedPrefab.transform.localScale = Vector3.one;
            }

            yield return new WaitForSeconds(displayDuration);

            if (spawnedPrefab != null)
            {
                float t = 0;
                while (t < fadeDuration)
                {
                    t += Time.deltaTime;
                    spawnedPrefab.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, t / fadeDuration);
                    yield return null;
                }

                Destroy(spawnedPrefab);
            }
        }

        achievementCard?.SetActive(false);

        isExecuting = false;
    }
}
