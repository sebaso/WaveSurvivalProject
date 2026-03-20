using UnityEngine;

public class Boulder : MonoBehaviour
{
    [Header("Settings")]
    public float damage = 30f;
    public float lifetime = 5f;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    public void Launch(Vector3 origin, Vector3 target, float launchSpeed)
    {
        transform.position = origin;

        Vector3 dir = (target - origin).normalized;
        dir.y += 0.3f;
        dir.Normalize();

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = dir * launchSpeed;
        }
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            PlayerController ph = col.gameObject.GetComponent<PlayerController>();
            if (ph != null) ph.TakeDamage((int)damage);
        }
        Destroy(gameObject);
    }
}
