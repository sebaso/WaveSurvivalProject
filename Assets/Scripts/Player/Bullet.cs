using UnityEngine;
using UnityEngine.Pool;

public class Bullet : MonoBehaviour
{
    public int punchThrough;
    public int damage;
    private IObjectPool<Bullet> pool;
    private float deactivateTimer;
    private float maxLifeTime = 1f;

    public void SetPool(IObjectPool<Bullet> bulletPool)
    {
        pool = bulletPool;
    }

    private void OnEnable()
    {
        deactivateTimer = maxLifeTime;
    }

    private void Update()
    {
        deactivateTimer -= Time.deltaTime;
        if (deactivateTimer <= 0)
        {
            Deactivate();
        }
    }

    private void Deactivate()
    {
        if (pool != null)
        {
            gameObject.SetActive(false);
            pool.Release(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Add logic here if bullet should deactivate on hit
        // For now, handling punchthrough or other logic might be elsewhere
        // But common practice is to deactivate here if punchthrough is 0
        if (punchThrough <= 0 && !collision.gameObject.CompareTag("Player"))
        {
            Deactivate();
        }
        else
        {
            punchThrough--;
        }
    }
}
