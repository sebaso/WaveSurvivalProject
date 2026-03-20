using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.VFX;
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
    public List<GameObject> ragdollPrefab = new();
    [HideInInspector] public GameObject originPrefab;
    public int variantIndex;
    public bool doVariantLogic;
    public List<GameObject> variantPrefab = new();
    public float speed;
    public int walkingAnimationsCount;
    public Transform hitPoint;

    private Coroutine chaseCoroutine;
    private int initialHp;
    private float baseNavSpeed;
    public List<AudioClip> hitSounds;
    public List<AudioClip> deathSounds;
    public AudioSource audioSource;
    public AudioSource audioSource3D;
    public VisualEffectAsset hitVFX;
    public VisualEffectAsset deathVFX;


    public bool IsDead => hp <= 0;

    public Animator anim;
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        dissolveBehaviour = GetComponent<DissolveBehaviour>();
        nav = GetComponent<NavMeshAgent>();
        baseNavSpeed = nav.speed;
        initialHp = hp;
        if (PlayerController.instance != null && PlayerController.instance.transform != null)
            player = PlayerController.instance.transform;
        anim = GetComponentInChildren<Animator>();
        if (doVariantLogic)
        {
            foreach (var variant in variantPrefab)
            {
                variant.SetActive(false);
            }
            variantIndex = Random.Range(0, variantPrefab.Count);
            variantPrefab[variantIndex].SetActive(true);
            anim = variantPrefab[variantIndex].GetComponent<Animator>();
        }
    }

    private void OnEnable()
    {
        speed = Random.Range(baseNavSpeed * 0.8f, baseNavSpeed * 1.2f);
        nav.speed = speed;
        if (doVariantLogic)
        {
            foreach (var variant in variantPrefab)
            {
                variant.SetActive(false);
            }
            variantIndex = Random.Range(0, variantPrefab.Count);
            variantPrefab[variantIndex].SetActive(true);
            anim = variantPrefab[variantIndex].GetComponent<Animator>();
        }

        if (anim != null && walkingAnimationsCount > 0)
        {
            //en un mundo ideal, hay mas de 2 animaciones zombis en mixamo :,)
            anim.SetFloat("Walking", Random.Range(0, walkingAnimationsCount));
        }

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
        if (deathSounds.Count > 0)
        {
            AudioClip clip = deathSounds[Random.Range(0, deathSounds.Count)];
            GameObject tempAudio = new("TempHitAudio");
            tempAudio.transform.position = transform.position;
            AudioSource tempSource = tempAudio.AddComponent<AudioSource>();
            if (audioSource3D != null)
            {
                tempSource.spatialBlend = audioSource3D.spatialBlend;
                tempSource.volume = audioSource3D.volume;
                tempSource.minDistance = audioSource3D.minDistance;
                tempSource.maxDistance = audioSource3D.maxDistance;
                tempSource.rolloffMode = audioSource.rolloffMode;
            }
            else
            {
                tempSource.spatialBlend = 1f;
            }
            tempSource.pitch = Random.Range(0.8f, 1.2f);
            tempSource.clip = clip;
            tempSource.Play();
            Destroy(tempAudio, clip.length / tempSource.pitch + 0.1f);
        }
        if (!useRagdoll)
        {
            onDeathEvent.Invoke();
            if (AchievementManager.Instance != null)
                AchievementManager.Instance.IncreaseStat("enemies_killed", 1);
            if (ScoreManager.instance != null)
                ScoreManager.instance.IncrementKills();
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
            AchievementManager.Instance?.IncreaseStat("enemies_killed", 1);
            ScoreManager.instance?.IncrementKills();

            //creamos el ragdoll
            GameObject ragdoll = Instantiate(ragdollPrefab[variantIndex], transform.position, transform.rotation);
            MatchTransforms(transform, ragdoll.transform, nav.velocity.sqrMagnitude);
            //el ragdoll se agrega a si mismo al ragdoll manager
            //retornamos el enemigo al pool
            WaveManager.instance.ReturnEnemyToPool(gameObject);
            if (ObjectiveManager.instance.objectiveType == ObjectiveManager.ObjectiveType.DefendLocation)
            {
                ObjectiveManager.instance.DefenseObjectiveLogic();
            }
            if (deathVFX != null)
            {
                SpawnVFX(deathVFX, 30f);
            }
            if (Tutoriel.instance != null && Tutoriel.instance.CurrentStep != null && Tutoriel.instance.CurrentStep.id == "shoot")
            {
                //Y COMO OLVIDARSE DEL TUTORIAL. EL BENDITO TUTORIAL.
                Tutoriel.CompleteStep("shoot");
            }
        }


    }

    private void MatchTransforms(Transform source, Transform destination, float velocity)
    {
        if (!destination.TryGetComponent<Animator>(out var destAnim)) destAnim = destination.GetComponentInChildren<Animator>();
        if (destAnim != null) destAnim.enabled = false;

        Transform[] sourceTransforms = source.GetComponentsInChildren<Transform>();
        Transform[] destTransforms = destination.GetComponentsInChildren<Transform>();

        System.Collections.Generic.Dictionary<string, Transform> destDict = new();
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
            rb.linearVelocity = new Vector3(velocity, 0, velocity);
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
        TakeDamage(damage, transform.position + Vector3.up * (nav != null ? nav.height * 0.6f : 1f));
    }

    public void TakeDamage(int damage, Vector3 hitPoint)
    {
        if (hitPoint == Vector3.zero)
        {
            hitPoint = transform.position + Vector3.up * (nav != null ? nav.height * 0.6f : 1f);
        }

        hp -= damage;
        ScoreManager.instance.AddScore((int)(10f * scoreMultiplier));
        if (hitSounds.Count > 0)
        {
            AudioClip clip = hitSounds[Random.Range(0, hitSounds.Count)];
            GameObject tempAudio = new("TempHitAudio");
            tempAudio.transform.position = transform.position;
            AudioSource tempSource = tempAudio.AddComponent<AudioSource>();
            if (audioSource != null)
            {
                tempSource.spatialBlend = audioSource.spatialBlend;
                tempSource.volume = audioSource.volume;
                tempSource.minDistance = audioSource.minDistance;
                tempSource.maxDistance = audioSource.maxDistance;
                tempSource.rolloffMode = audioSource.rolloffMode;
            }
            else
            {
                tempSource.spatialBlend = 1f;
            }
            tempSource.pitch = Random.Range(0.8f, 1.2f);
            tempSource.clip = clip;
            tempSource.Play();
            Destroy(tempAudio, clip.length / tempSource.pitch + 0.1f);
        }
        if (hitVFX != null)
        {
            SpawnVFXAtPoint(hitVFX, hitPoint, 2f); //en otro mundo ideal, el VFX simplemente apareceria
                                                   //y simplemente apareceria en el medio, dando la sensacion de
                                                   //que el disparo impacta en el enemigo. PERO NO. NO NO CARAJO NO.
                                                   //DEDIQUEMOSLE TIEMPO A ESTO CUANDO FALTAN MENOS DE 10 HORAS PARA PRESENTAR
                                                   //PORQUE EL VFX SIMPLEMENTE NO SABE OBEDECER. NO LO HACE. NO LO HACE.
        }

        if (hp <= 0)
        {
            Die();
        }
    }

    private IEnumerator ChasePlayer()
    {
        if (nav.stoppingDistance < 1.4f) nav.stoppingDistance = 1.4f;

        while (gameObject.activeSelf)
        {
            if (player != null && nav.isOnNavMesh)
            {
                nav.SetDestination(player.position);

                float dist = Vector3.Distance(transform.position, player.position);
                int calculatedPriority = Mathf.Clamp(Mathf.FloorToInt(dist * 10f), 10, 99);
                if (Mathf.Abs(nav.avoidancePriority - calculatedPriority) > 5)
                {
                    nav.avoidancePriority = calculatedPriority;
                }
            }
            yield return new WaitForSeconds(timeBetweenFetches);
        }
    }

    private void SpawnVFXAtPoint(VisualEffectAsset asset, Vector3 position, float destroyDelay)
    {
        GameObject vfxObj = new(asset.name + "_Temp");
        vfxObj.transform.position = position;
        vfxObj.transform.rotation = transform.rotation;

        VisualEffect vfx = vfxObj.AddComponent<VisualEffect>();
        vfx.visualEffectAsset = asset;

        TimedDestruction destroyScript = vfxObj.AddComponent<TimedDestruction>();
        destroyScript.timeToDestroy = destroyDelay;

        vfx.Play();
    }

    private void SpawnVFX(VisualEffectAsset asset, float destroyDelay)
    {
        Vector3 spawnPos;
        if (hitPoint != null)
        {
            spawnPos = hitPoint.position;
        }
        else
        {
            float h = (nav != null) ? nav.height : 2f;
            spawnPos = transform.TransformPoint(new Vector3(0, h * 0.6f, 0));
        }
        SpawnVFXAtPoint(asset, spawnPos, destroyDelay);
    }
}
