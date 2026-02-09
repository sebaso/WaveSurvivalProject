using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(DissolveBehaviour))]
public class Enemy : MonoBehaviour, IDamageable<int>
{
    public Transform player;
    public NavMeshAgent nav;
    public float timeBetweenFetches = 0.5f;
    public float scoreMultiplier = 1f;
    public int maxHp = 3;
    public int hp;
    public DissolveBehaviour dissolveBehaviour;
    public UnityEvent onDeathEvent;
    public UnityEvent onRespawnEvent;

    [HideInInspector] public GameObject originPrefab;

    private Coroutine chaseCoroutine;
    private int initialHp;

    public bool IsDead => hp <= 0;

    void Awake()
    {
        dissolveBehaviour = GetComponent<DissolveBehaviour>();
        nav = GetComponent<NavMeshAgent>();
        initialHp = hp;
        if (PlayerController.instance != null && PlayerController.instance.transform != null)
            player = PlayerController.instance.transform;
    }

    private void OnEnable()
    {
        onRespawnEvent?.Invoke();
        dissolveBehaviour.ResetDissolve();
        hp = initialHp > 0 ? initialHp : maxHp;

        if (player == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }

        if (chaseCoroutine != null) StopCoroutine(chaseCoroutine);
        chaseCoroutine = StartCoroutine(ChasePlayer());
    }
    public IEnumerator WaitForDissolve()
    {
        WaveManager.instance.enemiesLeft--;
        ScoreManager.instance.AddScore((int)(30f * scoreMultiplier));
        dissolveBehaviour.StartDissolve();
        yield return new WaitForSeconds(dissolveBehaviour.dissolveTime);
        WaveManager.instance.ReturnEnemyToPool(gameObject);

        transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        if (ObjectiveManager.instance.objectiveType == ObjectiveManager.ObjectiveType.DefendLocation)
        {
            ObjectiveManager.instance.DefenseObjectiveLogic();
        }
    }
    public void Die()
    {
        onDeathEvent?.Invoke();
        StartCoroutine(WaitForDissolve());

    }
    public void ObjectiveDelete()
    {
        transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        WaveManager.instance.ReturnEnemyToPool(gameObject);
        WaveManager.instance.enemiesLeft--;
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
