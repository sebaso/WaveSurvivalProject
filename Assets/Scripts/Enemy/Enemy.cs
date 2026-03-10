using System.Collections;
using Unity.Cinemachine;
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
    public float secondsToFreeze = 1;
    public int maxHp = 3;
    public int hp;
    public DissolveBehaviour dissolveBehaviour;
    public UnityEvent onDeathEvent;
    public UnityEvent onRespawnEvent;
    public bool useRagdoll;
    public GameObject ragdollPrefab;
    [HideInInspector] public GameObject originPrefab;

    private Coroutine chaseCoroutine;
    private int initialHp;

    public bool IsDead => hp <= 0;

    public Animator anim;
    void Awake()
    {
        dissolveBehaviour = GetComponent<DissolveBehaviour>();
        nav = GetComponent<NavMeshAgent>();
        initialHp = hp;
        if (PlayerController.instance != null && PlayerController.instance.transform != null)
            player = PlayerController.instance.transform;
        anim = GetComponentInChildren<Animator>();
    }

    private void OnEnable()
    {
        onRespawnEvent?.Invoke();
        if (dissolveBehaviour._renderer == null)
            dissolveBehaviour.Awake();
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
    private void Update()
    {
        if (IsDead || anim == null) return;
        anim.SetFloat("Speed", nav.velocity.magnitude);
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
        if (!useRagdoll)
        {
            onDeathEvent.Invoke();
            if (AchievementManager.Instance != null)
                AchievementManager.Instance.IncreaseStat("enemies_killed", 1);
            StartCoroutine(WaitForDissolve());
            if (ObjectiveManager.instance.objectiveType == ObjectiveManager.ObjectiveType.DefendLocation)
            {
                ObjectiveManager.instance.DefenseObjectiveLogic();
            }
        }
        else
        {
            //removemos el enemigo del wave manager
            WaveManager.instance.enemiesLeft--;
            //agregamos la puntuacion
            ScoreManager.instance.AddScore((int)(30f * scoreMultiplier));
            //invocamos el evento de muerte
            onDeathEvent.Invoke();
            //aumentamos la estadistica de enemigos eliminados
            if (AchievementManager.Instance != null)
                AchievementManager.Instance.IncreaseStat("enemies_killed", 1);

            //creamos el ragdoll
            GameObject ragdoll = Instantiate(ragdollPrefab, transform.position, transform.rotation);
            MatchTransforms(transform, ragdoll.transform, nav.velocity);
            //el ragdoll se agrega a si mismo al ragdoll manager
            //retornamos el enemigo al pool
            WaveManager.instance.ReturnEnemyToPool(gameObject);
            if (ObjectiveManager.instance.objectiveType == ObjectiveManager.ObjectiveType.DefendLocation)
            {
                ObjectiveManager.instance.DefenseObjectiveLogic();
            }
        }


    }

    private void MatchTransforms(Transform source, Transform destination, Vector3 velocity)
    {
        Animator destAnim = destination.GetComponent<Animator>();
        if (destAnim == null) destAnim = destination.GetComponentInChildren<Animator>();
        if (destAnim != null) destAnim.enabled = false;

        Transform[] sourceTransforms = source.GetComponentsInChildren<Transform>();
        Transform[] destTransforms = destination.GetComponentsInChildren<Transform>();

        System.Collections.Generic.Dictionary<string, Transform> destDict = new System.Collections.Generic.Dictionary<string, Transform>();
        foreach (Transform t in destTransforms)
        {
            if (!destDict.ContainsKey(t.name))
                destDict.Add(t.name, t);
        }

        foreach (Transform s in sourceTransforms)
        {
            if (destDict.TryGetValue(s.name, out Transform d))
            {
                d.SetPositionAndRotation(s.position, s.rotation);
            }
        }

        foreach (Rigidbody rb in destination.GetComponentsInChildren<Rigidbody>())
        {
            rb.linearVelocity = velocity;
        }
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
