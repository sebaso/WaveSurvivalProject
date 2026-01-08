using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager instance;

    [Header("References")]
    public Spawner spawner;

    [Header("Wave Settings")]
    public Wave[] waves;
    public int currentWaveIndex = 0;

    [Header("Statistics")]
    public int totalEnemies;
    public int enemiesLeft;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if (waves.Length > 0 && spawner != null)
        {
            StartNextWave();
        }
    }

    void Update()
    {
        // Check if all enemies in the current wave are dead
        if (enemiesLeft <= 0 && spawner != null)
        {
            currentWaveIndex++;
            if (currentWaveIndex < waves.Length)
            {
                StartNextWave();
            }
            else
            {
                Debug.Log("All waves complete!");
            }
        }
    }

    void StartNextWave()
    {
        if (currentWaveIndex < waves.Length)
        {
            spawner.InitializeWave(waves[currentWaveIndex]);
        }
    }
}
