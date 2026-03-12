using UnityEngine;
using UnityEngine.Pool;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    public int punchThrough;
    public int damage;
    public float speed = 50f;

    public float radius = 0.2f;
    public LayerMask hitMask = Physics.DefaultRaycastLayers;

    private IObjectPool<Bullet> pool;
    private float deactivateTimer;
    private readonly float maxLifeTime = 1.5f;
    private Vector3 direction;
    private int currentPunchThrough;
    private readonly HashSet<GameObject> hitEnemies = new();
    private Rigidbody rb;

    public void SetPool(IObjectPool<Bullet> bulletPool)
    {
        pool = bulletPool;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (TryGetComponent<CapsuleCollider>(out var capsuleCollider))
        {
            radius = capsuleCollider.radius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.z);
        }
        else if (TryGetComponent<Collider>(out var col))
        {
            // Fallback
            radius = Mathf.Min(col.bounds.extents.x, Mathf.Min(col.bounds.extents.y, col.bounds.extents.z));
        }
    }

    public void Initialize(Vector3 shootDirection, float bulletSpeed, int bulletDamage, int bulletPunchThrough)
    {
        direction = shootDirection;
        direction.y = 0;
        direction.Normalize();

        speed = bulletSpeed;
        damage = bulletDamage;
        punchThrough = bulletPunchThrough;
        currentPunchThrough = bulletPunchThrough;

        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }

        if (rb != null) rb.isKinematic = true;
    }

    private void OnEnable()
    {
        deactivateTimer = maxLifeTime;
        hitEnemies.Clear();
        currentPunchThrough = punchThrough;
        if (rb != null) rb.isKinematic = true;
    }

    private void Update()
    {
        deactivateTimer -= Time.deltaTime;
        if (deactivateTimer <= 0)
        {
            Deactivate();
            return;
        }

        float moveDistance = speed * Time.deltaTime;
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, radius, direction, moveDistance, hitMask, QueryTriggerInteraction.Ignore);
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (var hit in hits)
        {
            if (hit.collider.CompareTag("Player")) continue;

            if (hit.collider.CompareTag("Enemy"))
            {
                GameObject enemyRoot = hit.collider.transform.root.gameObject;
                IDamageable<int> enemyScript = hit.collider.GetComponentInParent<IDamageable<int>>();

                if (enemyScript != null)
                {
                    if (hitEnemies.Contains(enemyRoot)) continue;
                    hitEnemies.Add(enemyRoot);

                    enemyScript.TakeDamage(damage);

                    if (currentPunchThrough > 0)
                    {
                        currentPunchThrough--;
                    }
                    else
                    {
                        transform.position = hit.point;
                        Deactivate();
                        return;
                    }
                }
                else
                {
                    transform.position = hit.point;
                    Deactivate();
                    return;
                }
            }
            else
            {
                transform.position = hit.point;
                Deactivate();
                return;
            }
        }
        transform.position += direction * moveDistance;
    }

    private void Deactivate()
    {
        if (pool != null)
        {
            pool.Release(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}

