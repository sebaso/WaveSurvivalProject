using UnityEngine;

[CreateAssetMenu(fileName = "New Wave", menuName = "Wave System/Wave")]
public class Wave : ScriptableObject
{
    public string waveName;
    public float spawnRate = 1.0f;
    public EnemyGroup[] enemies;

    [System.Serializable]
    public class EnemyGroup
    {
        public GameObject enemy;
        public int count;
        public bool pureSpawn;
    }

    public int GetTotalEnemyCount()
    {
        int total = 0;
        if (enemies == null) return 0;
        foreach (var group in enemies)
        {
            total += group.count;
        }
        return total;
    }
}
