using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Wave Configuration")]
    public Wave currentWave;
    public List<Transform> spawnPoints = new List<Transform>();

    [Header("Internal State")]
    private List<GameObject> spawnQueue = new List<GameObject>();
    private float nextSpawnTime;
    private bool isSpawning = false;


    private void Start()
    {
        if (spawnPoints.Count == 0)
        {
            GameObject[] points = GameObject.FindGameObjectsWithTag("Spawner");
            foreach (GameObject point in points)
            {
                spawnPoints.Add(point.transform);
            }
        }

        if (currentWave != null)
        {
            InitializeWave(currentWave);
        }

    }
    public void NextWave()
    {
        WaveManager.instance.currentWaveIndex++;
        InitializeWave(WaveManager.instance.waves[WaveManager.instance.currentWaveIndex - 1]);
    }

    private void Update()
    {
        if (isSpawning && spawnQueue.Count > 0)
        {
            if (Time.time >= nextSpawnTime)
            {
                SpawnNextEnemy();
                nextSpawnTime = Time.time + currentWave.spawnRate;
            }
        }
        else if (isSpawning && spawnQueue.Count == 0)
        {
            isSpawning = false;
        }
    }

    public void InitializeWave(Wave wave)
    {
        currentWave = wave;
        spawnQueue.Clear();

        List<GameObject> randomPool = new List<GameObject>();

        foreach (var group in wave.enemies)
        {
            if (group.pureSpawn)
            {
                if (randomPool.Count > 0)
                {
                    Shuffle(randomPool);
                    spawnQueue.AddRange(randomPool);
                    randomPool.Clear();
                }

                for (int i = 0; i < group.count; i++)
                {
                    spawnQueue.Add(group.enemy);
                }
            }
            else
            {
                for (int i = 0; i < group.count; i++)
                {
                    randomPool.Add(group.enemy);
                }
            }
        }

        if (randomPool.Count > 0)
        {
            Shuffle(randomPool);
            spawnQueue.AddRange(randomPool);
        }

        if (WaveManager.instance != null)
        {
            WaveManager.instance.totalEnemies = spawnQueue.Count;
            WaveManager.instance.enemiesLeft = spawnQueue.Count;
        }

        isSpawning = true;
        nextSpawnTime = Time.time + currentWave.spawnRate;
    }

    private void SpawnNextEnemy()
    {
        if (spawnQueue.Count == 0 || spawnPoints.Count == 0) return;

        GameObject enemyPrefab = spawnQueue[0];
        spawnQueue.RemoveAt(0);

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
        Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
    }

    private void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            T temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}
