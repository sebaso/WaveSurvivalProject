using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Enemy))]
public class EnemyAttack : MonoBehaviour
{
    private Enemy enemy;
    public float attackRate = 1f;
    private float nextAttack = 0f;
    public float sphereCastRadius = 3f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemy = GetComponent<Enemy>();
    }

    // Update is called once per frame
    void Update()
    {
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
        if (enemy.nav.remainingDistance <= enemy.nav.stoppingDistance)
        {
            //Attack(enemy.player.gameObject);
        }
    }
    public void Attack(GameObject target)
    {
        if (Time.time > nextAttack)
        {
            nextAttack = Time.time + attackRate;
            target.GetComponent<PlayerController>().TakeDamage(1);
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
                boardedDoor.TakeDamage();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Attack(other.gameObject);
        }
    }
}
