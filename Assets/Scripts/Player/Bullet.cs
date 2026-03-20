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

    private bool isDeactivated;
    private IObjectPool<Bullet> pool;
    private float deactivateTimer;
    private readonly float maxLifeTime = 1.5f;
    private Vector3 direction;
    private int currentPunchThrough;
    private readonly HashSet<IDamageable<int>> hitEnemies = new();
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
        isDeactivated = false;
    }

    private void OnEnable()
    {
        deactivateTimer = maxLifeTime;
        hitEnemies.Clear();
        currentPunchThrough = punchThrough;
        if (rb != null) rb.isKinematic = true;
        isDeactivated = false;
    }

    private void Update()
    {
        if (isDeactivated) return;

        deactivateTimer -= Time.deltaTime;
        if (deactivateTimer <= 0)
        {
            Deactivate();
            return;
        }

        float moveDistance = speed * Time.deltaTime;

        Vector3 castOrigin = transform.position - direction * radius;
        float castDist = moveDistance + radius;

        RaycastHit[] hits = Physics.SphereCastAll(castOrigin, radius, direction, castDist, hitMask, QueryTriggerInteraction.Collide);
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (var hit in hits)
        {
            if (isDeactivated) break;
            if (hit.collider.CompareTag("Player")) continue;

            IDamageable<int> damageable = hit.collider.GetComponentInParent<IDamageable<int>>();

            if (damageable != null)
            {
                if (hitEnemies.Contains(damageable)) continue;
                hitEnemies.Add(damageable);

                damageable.TakeDamage(damage, hit.point);

                if (currentPunchThrough > 0)
                {
                    currentPunchThrough--;
                }
                else
                {
                if (hit.point != Vector3.zero)
                {
                    transform.position = hit.point;
                }
                Deactivate();
                break;
                }
            }
            else if (hit.collider.isTrigger)
            {
                continue;
            }
            else
            {
                if (hit.point != Vector3.zero)
                {
                    transform.position = hit.point;
                }
                Deactivate();
                break;
            }
        }

        if (!isDeactivated)
            transform.position += direction * moveDistance;
    }

    private void Deactivate()
    {
        if (isDeactivated) return;
        isDeactivated = true;

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

