using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    private Rigidbody rb;
    public float speed = 10f;
    public float damage = 1f;
    public float lifeTime = 5f;
    public Transform target;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearVelocity = (target.position - transform.position).normalized * speed;
        Destroy(gameObject, lifeTime);
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerController>().TakeDamage((int)damage);
            Destroy(gameObject);
        }
    }
}
