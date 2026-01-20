using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour
{
    public Transform player;
    private NavMeshAgent nav;
    public float timeBetweenFetches = 0.5f;
    public float scoreMultiplier = 1f;
    public int maxHp = 3;
    public int hp;

    // The prefab this enemy was instantiated from, used for pooling
    [HideInInspector] public GameObject originPrefab;

    private Coroutine chaseCoroutine;
    private int initialHp;

    void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
        initialHp = hp;
    }

    private void OnEnable()
    {
        // Reset state when reused from pool
        hp = initialHp > 0 ? initialHp : maxHp;

        if (player == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }

        if (chaseCoroutine != null) StopCoroutine(chaseCoroutine);
        chaseCoroutine = StartCoroutine(ChasePlayer());
    }

    public void Die()
    {
        WaveManager.instance.enemiesLeft--;
        ScoreManager.instance.AddScore((int)(30f * scoreMultiplier));
        transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        WaveManager.instance.ReturnEnemyToPool(gameObject);
    }

    public void TakeDamage(int damage)
    {
        hp -= damage;
        ScoreManager.instance.AddScore((int)(10f * scoreMultiplier));

        if (hp <= 0)
        {
            Die();
        }
    }

    private IEnumerator ChasePlayer()
    {
        while (gameObject.activeSelf)
        {
            if (player != null && nav.isOnNavMesh)
            {
                nav.SetDestination(player.position);
            }
            yield return new WaitForSeconds(timeBetweenFetches);
        }
    }
}
