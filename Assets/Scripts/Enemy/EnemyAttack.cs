using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Enemy))]
public class EnemyAttack : MonoBehaviour
{
    private static readonly WaitForSeconds _waitForSeconds1 = new(1f);
    private Enemy enemy;
    public float attackRate = 1f;
    private float nextAttack = 0f;
    public float sphereCastRadius = 3f;
    public AudioClip attackSound;
    public float contactDamageThreshold = 0.2f;
    private float currentContactTimer = 0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemy = GetComponent<Enemy>();
    }

    private bool isTouchingTrigger = false;

    // Update is called once per frame
    void Update()
    {
        if (enemy.IsDead)
        {
            return;
        }
        if (enemy.nav.pathStatus == NavMeshPathStatus.PathPartial || enemy.nav.pathStatus == NavMeshPathStatus.PathInvalid)
        {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, sphereCastRadius, Vector3.down, 10f, LayerMask.GetMask("BoardedDoor"));
            foreach (RaycastHit hit in hits)
            {
                if (hit.transform.GetComponent<BoardedDoor>().enabled)
                {
                    enemy.nav.SetDestination(hit.collider.gameObject.transform.position);
                    DestroyBoardedDoor(hit.collider.gameObject);
                    break;
                }
            }
        }

        bool isNearPlayer = false;
        if (enemy.nav.remainingDistance <= enemy.nav.stoppingDistance)
        {
            if (enemy.player != null && Vector3.Distance(transform.position, enemy.player.position) <= enemy.nav.stoppingDistance + 0.2f)
            {
                isNearPlayer = true;
            }
        }

        if ((isNearPlayer || isTouchingTrigger) && enemy.player != null)
        {
            currentContactTimer += Time.deltaTime;
            if (currentContactTimer >= contactDamageThreshold)
            {
                Attack(enemy.player.gameObject);
                currentContactTimer = 0f;
            }

            Vector3 lookDir = enemy.player.position - transform.position;
            lookDir.y = 0;
            if (lookDir.sqrMagnitude > 0.01f)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir), Time.deltaTime * 5f);
            }
        }
        else
        {
            currentContactTimer = 0f;
        }

        isTouchingTrigger = false;
    }

    public IEnumerator AttackInPlace()
    {
        if (enemy.anim != null)
        {
            enemy.nav.isStopped = true;
            yield return new WaitForSeconds(enemy.secondsToFreeze);
            if (enemy.anim == null)
            {
                yield break;
            }
            enemy.anim.SetTrigger("Attack");
            enemy.audioSource3D.PlayOneShot(enemy.deathSounds[0]);
            yield return _waitForSeconds1;
            enemy.nav.isStopped = false;
        }
    }
    public void Attack(GameObject target)
    {
        if (Time.time > nextAttack)
        {
            nextAttack = Time.time + attackRate;
            target.GetComponent<PlayerController>().TakeDamage(1);
            StartCoroutine(AttackInPlace());
        }
    }
    public void DestroyBoardedDoor(GameObject target)
    {
        if (Time.time > nextAttack)
        {
            nextAttack = Time.time + attackRate;
            BoardedDoor boardedDoor = target.GetComponent<BoardedDoor>();
            if (boardedDoor != null && !boardedDoor.isDestroyed)
            {
                StartCoroutine(AttackInPlace());
                boardedDoor.TakeDamage();
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && !enemy.IsDead)
        {
            isTouchingTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isTouchingTrigger = false;
        }
    }
}
