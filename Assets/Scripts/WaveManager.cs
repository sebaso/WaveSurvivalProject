using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager instance;

    [Header("References")]
    public Spawner spawner;

    [Header("Wave Settings")]
    public List<Wave> waves = new();
    public int currentWaveIndex = 0;

    [Header("Statistics")]
    public int totalEnemies;
    public int enemiesLeft;
    public int timeBetweenWaves;
    public float timer;
    public bool wavesArePaused;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
    }

    void Update()
    {
        if (enemiesLeft <= 0 && spawner != null)
        {

            if (currentWaveIndex < waves.Count && !wavesArePaused)
            {
                StartCoroutine(StartNextWave());
            }
            else
            {
                Debug.Log("All waves complete!");
            }
        }
    }

    IEnumerator StartNextWave()
    {
        currentWaveIndex++;
        wavesArePaused = true;
        timer = timeBetweenWaves;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }
        wavesArePaused = false;
        spawner.NextWave();
    }

}
