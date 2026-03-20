using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VFX;

[RequireComponent(typeof(NavMeshAgent))]
public class BossAI : MonoBehaviour, IDamageable<int>
{
    [Header("References")]
    public Transform player;
    public GameObject shockwavePrefab;
    public GameObject boulderPrefab;
    public Transform shockwaveSpawnPoint;
    public Transform boulderSpawnPoint;
    public Animator animator;
    public AudioClip[] hitsound;
    public AudioClip dunkSound;
    public AudioClip deathSound;

    [Header("Movement")]
    public float moveSpeed = 4f;
    public float meleeRange = 1.8f;
    public float chaseTimeout = 4f;

    [Header("Melee Attack")]
    public float meleeDamage = 20f;
    public float meleeAttackDuration = 0.6f;
    public float meleeAttackCooldown = 0.3f;

    [Header("Slam Attack")]
    public float slamDamage = 25f;
    public float slamRadius = 2f;
    public float slamWindupDuration = 0.8f;
    public float slamActiveDuration = 0.3f;
    public int shockwaveCount = 3;
    public float shockwaveSpread = 20f;

    [Header("Boulder Throw")]
    public float boulderLaunchSpeed = 14f;
    public float boulderWindupDuration = 1.0f;

    [Header("Give-Up Pause")]
    public float giveUpDuration = 1.2f;

    [Header("Health & Feedback")]
    public int maxHealth = 300;
    public float scoreMultiplier = 10f;
    public AudioClip dunkedSound;
    public VisualEffectAsset hitVFX;
    public AudioSource audioSource;

    private BossStateID currentState = BossStateID.Idle;
    private NavMeshAgent agent;
    private int health;

    public bool IsDead => health <= 0;

    private int cycleStep = 0;
    private int attacksThisCycle = 0;

    private bool isActing = false;
    private float chaseTimer = 0f;

    public static System.Action<int, int> OnHealthChanged;
    public static System.Action OnBossDefeated;
    public static bool BossDefeated = false;

    private static readonly int AnimWalk = Animator.StringToHash("Walk");
    private static readonly int AnimMelee = Animator.StringToHash("MeleeAttack");
    private static readonly int AnimSlam = Animator.StringToHash("SlamAttack");
    private static readonly int AnimThrow = Animator.StringToHash("ThrowBoulder");
    private static readonly int AnimIdle = Animator.StringToHash("Idle");
    private static readonly int AnimDeath = Animator.StringToHash("Die");

    private void Awake()
    {
        BossDefeated = false;
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
        agent.stoppingDistance = meleeRange;

        player = GameObject.FindGameObjectWithTag("Player").transform;
        health = maxHealth;
        animator = GetComponentInChildren<Animator>();
        if (audioSource == null) audioSource = GetComponent<AudioSource>();

        OnHealthChanged?.Invoke(health, maxHealth);
    }

    private void Update()
    {
        UpdateAnimationState();

        if (currentState == BossStateID.Dead || isActing) return;

        switch (currentState)
        {
            case BossStateID.Idle:
                TransitionTo(BossStateID.Chase);
                break;

            case BossStateID.Chase:
                UpdateChase();
                break;
        }
    }

    private void UpdateAnimationState()
    {
        if (animator == null) return;

        float speed = (agent != null) ? agent.velocity.magnitude : 0f;
        bool isWalking = (currentState == BossStateID.Chase) && (speed > 0.1f) && !isActing;

        bool isIdle = !isWalking && !isActing;

        SetBoolIfChanged(AnimWalk, isWalking);
        SetBoolIfChanged(AnimIdle, isIdle);
    }

    private void SetBoolIfChanged(int hash, bool value)
    {
        if (animator.GetBool(hash) != value)
            animator.SetBool(hash, value);
    }

    private void UpdateChase()
    {
        if (player == null) return;

        chaseTimer += Time.deltaTime;

        float dist = Vector3.Distance(transform.position, player.position);

        if (dist <= meleeRange)
        {
            chaseTimer = 0f;
            TransitionTo(BossStateID.MeleeAttack);
            return;
        }

        if (chaseTimer >= chaseTimeout)
        {
            chaseTimer = 0f;
            TransitionTo(BossStateID.GiveUp);
            return;
        }

        agent.isStopped = false;
        agent.SetDestination(player.position);

        Vector3 dir = agent.velocity.normalized;
        if (dir.sqrMagnitude > 0.1f)
            FaceDirection(dir);
    }

    private void TransitionTo(BossStateID next)
    {
        currentState = next;

        switch (next)
        {
            case BossStateID.Chase:
                chaseTimer = 0f;
                if (agent != null) agent.isStopped = false;
                break;

            case BossStateID.MeleeAttack:
                if (agent != null) agent.isStopped = true;
                StartCoroutine(DoMeleeAttack());
                break;

            case BossStateID.SlamAttack:
                if (agent != null) agent.isStopped = true;
                StartCoroutine(DoSlamAttack());
                break;

            case BossStateID.ThrowBoulder:
                if (agent != null) agent.isStopped = true;
                StartCoroutine(DoThrowBoulder());
                break;

            case BossStateID.GiveUp:
                if (agent != null) agent.isStopped = true;
                StartCoroutine(DoGiveUp());
                break;

            case BossStateID.Dead:
                if (agent != null) agent.isStopped = true;
                StartCoroutine(DoDeath());
                break;
        }
    }

    private IEnumerator DoMeleeAttack()
    {
        isActing = true;
        TriggerAnim(AnimMelee);

        yield return new WaitForSeconds(meleeAttackDuration * 0.5f);

        if (player != null &&
            Vector3.Distance(transform.position, player.position) <= meleeRange * 1.2f)
        {
            PlayerController ph = player.GetComponent<PlayerController>();
            ph?.TakeDamage((int)meleeDamage);
            PlayDunkSound();
        }

        yield return new WaitForSeconds(meleeAttackDuration * 0.5f + meleeAttackCooldown);

        isActing = false;
        OnAttackFinished(wasSuccessful: true);
    }
    private void PlayDunkSound()
    {
        if (dunkSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(dunkSound);
        }
    }

    private IEnumerator DoSlamAttack()
    {
        isActing = true;
        TriggerAnim(AnimSlam);

        yield return new WaitForSeconds(slamWindupDuration);

        yield return new WaitForSeconds(slamActiveDuration);

        if (player != null &&
            Vector3.Distance(transform.position, player.position) <= slamRadius)
        {
            PlayerController ph = player.GetComponent<PlayerController>();
            ph?.TakeDamage((int)slamDamage);
        }

        SpawnShockwaves();

        yield return new WaitForSeconds(0.5f);

        isActing = false;
        AdvanceCycleAfterSlam();
    }

    private IEnumerator DoThrowBoulder()
    {
        isActing = true;
        if (agent != null)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }

        TriggerAnim(AnimThrow);

        float timer = 0f;
        while (timer < boulderWindupDuration)
        {
            if (player != null)
            {
                Vector3 dir = (player.position - transform.position).normalized;
                dir.y = 0f;
                if (dir.sqrMagnitude > 0.001f)
                {
                    transform.rotation = Quaternion.Slerp(
                        transform.rotation,
                        Quaternion.LookRotation(dir),
                        Time.deltaTime * 5f);
                }
            }
            timer += Time.deltaTime;
            yield return null;
        }

        if (boulderPrefab != null && player != null)
        {
            Vector3 spawnPos = boulderSpawnPoint != null
                ? boulderSpawnPoint.position
                : transform.position + Vector3.up * 1.5f;

            GameObject boulderGO = Instantiate(boulderPrefab, spawnPos, Quaternion.identity);
            Boulder boulder = boulderGO.GetComponent<Boulder>();
            if (boulder != null)
                boulder.Launch(spawnPos, player.position, boulderLaunchSpeed);
        }

        yield return new WaitForSeconds(0.6f);

        isActing = false;
        RestartPattern();
    }

    private IEnumerator DoGiveUp()
    {
        isActing = true;

        yield return new WaitForSeconds(giveUpDuration);

        isActing = false;

        OnAttackFinished(wasSuccessful: false);
    }

    private IEnumerator DoDeath()
    {
        isActing = true;

        TriggerAnim(AnimDeath);



        yield return new WaitForSeconds(2f);
        if (deathSound != null)
        {
            AudioSource.PlayClipAtPoint(deathSound, transform.position);
        }
        Destroy(gameObject);
    }

    private void OnAttackFinished(bool wasSuccessful)
    {
        attacksThisCycle++;

        if (attacksThisCycle >= 2)
        {
            attacksThisCycle = 0;

            if (cycleStep < 2)
            {
                cycleStep = 2;
                TransitionTo(BossStateID.SlamAttack);
            }
            else
            {
                cycleStep = 4;
                TransitionTo(BossStateID.ThrowBoulder);
            }
        }
        else
        {
            TransitionTo(BossStateID.Chase);
        }
    }

    private void AdvanceCycleAfterSlam()
    {
        cycleStep = 3;
        attacksThisCycle = 0;
        TransitionTo(BossStateID.Chase);
    }

    private void RestartPattern()
    {
        cycleStep = 0;
        attacksThisCycle = 0;
        TransitionTo(BossStateID.Chase);
    }

    private void SpawnShockwaves()
    {
        if (shockwavePrefab == null) return;

        Vector3 spawnPos = shockwaveSpawnPoint != null
            ? shockwaveSpawnPoint.position
            : transform.position;

        Vector3 baseDir = player != null
            ? (player.position - transform.position).normalized
            : transform.forward;
        baseDir.y = 0f;
        baseDir.Normalize();

        float startAngle = -(shockwaveSpread * (shockwaveCount - 1)) * 0.5f;

        for (int i = 0; i < shockwaveCount; i++)
        {
            float angle = startAngle + shockwaveSpread * i;
            Vector3 dir = Quaternion.Euler(0f, angle, 0f) * baseDir;

            GameObject sw = Instantiate(shockwavePrefab, spawnPos, Quaternion.LookRotation(dir));
            Shockwave shockwave = sw.GetComponent<Shockwave>();
            if (shockwave != null) shockwave.Init(dir);
        }
    }

    public void TakeDamage(int amount)
    {
        TakeDamage(amount, transform.position + Vector3.up * 1.5f);
    }

    public void TakeDamage(int amount, Vector3 hitPoint)
    {
        if (currentState == BossStateID.Dead) return;

        if (hitPoint == Vector3.zero)
        {
            hitPoint = transform.position + Vector3.up * 1.5f;
        }

        health -= amount;
        OnHealthChanged?.Invoke(health, maxHealth);

        PlayHitSound();
        SpawnHitVFXAtPoint(hitPoint);

        if (ScoreManager.instance != null)
            ScoreManager.instance.AddScore((int)(10f * scoreMultiplier));

        if (health <= 0)
        {
            health = 0;
            Die();
        }
    }

    public void Die()
    {
        BossDefeated = true;
        OnBossDefeated?.Invoke();

        StopAllCoroutines();

        if (agent != null)
        {
            agent.isStopped = true;
            agent.enabled = false;
        }

        TransitionTo(BossStateID.Dead);
    }

    private void PlayHitSound()
    {
        if (hitsound != null && hitsound.Length > 0 && audioSource != null)
        {
            audioSource.PlayOneShot(hitsound[Random.Range(0, hitsound.Length)]);
        }
    }

    private void SpawnHitVFXAtPoint(Vector3 position)
    {
        if (hitVFX == null) return;

        GameObject vfxObj = new(hitVFX.name + "_Temp");
        vfxObj.transform.position = position;
        vfxObj.transform.rotation = transform.rotation;

        VisualEffect vfx = vfxObj.AddComponent<VisualEffect>();
        vfx.visualEffectAsset = hitVFX;

        TimedDestruction td = vfxObj.AddComponent<TimedDestruction>();
        td.timeToDestroy = 2f;

        vfx.Play();
    }

    private void SpawnHitVFX()
    {
        SpawnHitVFXAtPoint(transform.position + Vector3.up * 1.5f);
    }

    private void FaceDirection(Vector3 dir)
    {
        if (dir.sqrMagnitude < 0.001f) return;
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.LookRotation(dir),
            Time.deltaTime * 10f);
    }

    private void SetAnim(int hash, bool value)
    {
        animator?.SetBool(hash, value);
    }

    private void TriggerAnim(int hash)
    {
        animator?.SetTrigger(hash);
    }
}
