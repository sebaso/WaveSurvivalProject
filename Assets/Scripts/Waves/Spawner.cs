using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Wave Configuration")]
    public Wave currentWave;
    public List<Transform> spawnPoints = new();

    [Header("Internal State")]
    public List<GameObject> spawnQueue = new();
    private float nextSpawnTime;
    private bool isSpawning = false;
    public Transform player;


    private void Start()
    {
        if (PlayerController.instance != null && PlayerController.instance.transform != null)
            player = PlayerController.instance.transform;
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
        InitializeWave(WaveManager.instance.waves[WaveManager.instance.currentWaveIndex - 1]);
    }

    private void Update()
    {
        if (WaveManager.instance.wavesArePaused)
        {
            return;
        }
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

    public void KillAllEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            if (enemy.TryGetComponent<Enemy>(out var e)) e.ObjectiveDelete();
        }
    }

    public void InitializeWave(Wave wave)
    {
        currentWave = wave;
        spawnQueue.Clear();

        List<GameObject> randomPool = new();

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

                // Pure spawn means this enemy appears exactly once at this point in the sequence
                spawnQueue.Add(group.enemy);
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

        List<Transform> validPoints = new();
        foreach (Transform pt in spawnPoints)
        {
            if (!IsPointVisible(pt.position) && pt.gameObject.activeInHierarchy)
            {
                validPoints.Add(pt);
            }
        }

        if (validPoints.Count == 0)
        {
            Debug.LogWarning("All spawn points are visible! Skipping spawn until a point is hidden.");
            return;
        }

        GameObject enemyPrefab = spawnQueue[0];
        spawnQueue.RemoveAt(0);

        if (enemyPrefab == null) return;

        Transform spawnPoint = validPoints[Random.Range(0, validPoints.Count)];


        GameObject enemyInstance = WaveManager.instance.GetEnemyFromPool(enemyPrefab);
        if (enemyInstance != null)
        {
            enemyInstance.transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);
            enemyInstance.transform.SetParent(this.transform);
            enemyInstance.SetActive(true);
        }
    }

    private bool IsPointVisible(Vector3 position)
    {
        if (Camera.main == null) return false;

        Vector3 screenPoint = Camera.main.WorldToViewportPoint(position);
        bool onScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
        return onScreen;
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
    public void AddSpawnPoints(List<Transform> newPoints)
    {
        if (newPoints == null) return;

        foreach (Transform point in newPoints)
        {
            AddSpawnPoint(point);
        }
    }
    public void AddSpawnPoint(Transform point)
    {
        if (point == null) return;
        if (spawnPoints.Contains(point)) return;

        spawnPoints.Add(point);
    }
    public void RemoveSpawnPoint(Transform point)
    {
        if (point == null) return;
        spawnPoints.Remove(point);
    }
    public void SpawnObjectiveWave()
    {
        currentWave = WaveManager.instance.objectiveWaves[0];
        InitializeWave(currentWave);
        WaveManager.instance.wavesArePaused = false;
    }
}
