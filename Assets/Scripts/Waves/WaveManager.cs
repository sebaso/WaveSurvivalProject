using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Pool;

public class WaveManager : MonoBehaviour
{
    public static WaveManager instance;


    [Header("References")]
    public Spawner spawner;

    [Header("Wave Settings")]
    public List<Wave> waves = new();
    public List<Wave> objectiveWaves = new();
    public int currentWaveIndex = 0;

    [Header("Statistics")]
    public int totalEnemies;
    public int enemiesLeft;
    public int timeBetweenWaves;
    public float timer;
    public bool wavesArePaused;
    public GameObject[] enemyTypes;
    public int amountToPool;
    public static Dictionary<GameObject, ObjectPool<GameObject>> SharedInstance = new();

    private void Awake()
    {
        instance = this;
        SharedInstance.Clear();
    }

    void Start()
    {
        currentWaveIndex = 1;
        if (RoundNumeralUI.instance != null)
            RoundNumeralUI.instance.UpdateRound(currentWaveIndex);

        if (enemyTypes == null) return;
        foreach (GameObject prefab in enemyTypes)
        {
            InitializePool(prefab);
        }
    }

    private void InitializePool(GameObject prefab)
    {
        if (prefab == null || SharedInstance.ContainsKey(prefab)) return;

        SharedInstance.Add(prefab, new ObjectPool<GameObject>(
            () => CreateEnemyInstance(prefab, Vector3.zero),
            OnGet,
            OnRelease,
            OnDestroy,
            true,
            amountToPool,
            1000
        ));

        List<GameObject> temp = new();
        if (amountToPool > 0)
        {
            for (int i = 0; i < amountToPool; i++)
            {
                var obj = SharedInstance[prefab].Get();
                if (obj != null) temp.Add(obj);
            }
        }

        foreach (GameObject obj in temp)
        {
            SharedInstance[prefab].Release(obj);
        }
    }

    private GameObject CreateEnemyInstance(GameObject prefab, Vector3 position)
    {
        if (prefab == null) return null;
        GameObject obj = Instantiate(prefab, position, Quaternion.identity);
        obj.SetActive(false);

        if (obj.TryGetComponent<Enemy>(out var enemy))
        {
            enemy.originPrefab = prefab;
        }
        return obj;
    }

    public GameObject GetEnemyFromPool(GameObject prefab)
    {
        if (prefab == null) return null;

        if (!SharedInstance.ContainsKey(prefab))
        {
            InitializePool(prefab);
        }

        if (SharedInstance.ContainsKey(prefab))
        {
            return SharedInstance[prefab].Get();
        }
        return null;
    }

    public void ReturnEnemyToPool(GameObject enemyObj)
    {
        if (enemyObj.TryGetComponent<Enemy>(out var enemy) && enemy.originPrefab != null && SharedInstance.ContainsKey(enemy.originPrefab))
        {
            SharedInstance[enemy.originPrefab].Release(enemyObj);
        }
        else
        {
            Destroy(enemyObj);
        }
    }

    public void OnGet(GameObject obj)
    {

    }

    public void OnRelease(GameObject obj)
    {
        obj.SetActive(false);
    }

    public static void OnDestroy(GameObject obj)
    {
        Destroy(obj);
    }

    void Update()
    {
        if (enemiesLeft <= 0 && spawner != null)
        {
            if (ObjectiveManager.instance != null && ObjectiveManager.instance.isObjectiveActive)
            {
                if (!wavesArePaused && objectiveWaves.Count > 0)
                {
                    spawner.SpawnObjectiveWave();
                }
                return;
            }

            if (currentWaveIndex < waves.Count && !wavesArePaused)
            {
                wavesArePaused = true;
                
                // Flash the CURRENT round number on end
                if (RoundNumeralUI.instance != null)
                {
                    RoundNumeralUI.instance.FlashWaveEnd();
                }

                StartCoroutine(StartNextWave());
            }
        }
    }

    IEnumerator StartNextWave()
    {
        timer = timeBetweenWaves;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        currentWaveIndex++;

        // Update to NEW round number
        if (WaveUI.instance != null)
            WaveUI.instance.UpdateWaveText(currentWaveIndex);

        if (RoundNumeralUI.instance != null)
        {
            RoundNumeralUI.instance.UpdateRound(currentWaveIndex);
            // Flash the NEW round number on start
            RoundNumeralUI.instance.FlashWaveEnd();
        }

        wavesArePaused = false;
        spawner.NextWave();
        if (RoundFeedback.instance != null)
            RoundFeedback.instance.PlayRoundStartFeedback();
    }

}

