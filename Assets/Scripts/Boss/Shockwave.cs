using UnityEngine;

public class Shockwave : MonoBehaviour
{
    [Header("Settings")]
    public float speed = 10f;
    public float damage = 15f;
    public float lifetime = 3f;

    public float immunityDuration = 0.5f;
 
     private Vector3 direction;
     private float spawnTime;

    public void Init(Vector3 travelDirection)
    {
        direction = travelDirection.normalized;
        spawnTime = Time.time;
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController ph = other.GetComponent<PlayerController>();
            if (ph != null) ph.TakeDamage((int)damage);
            Destroy(gameObject);
        }
        
        if (Time.time - spawnTime > immunityDuration)
        {
            if (other.CompareTag("Ground") || other.CompareTag("Wall"))
            {
                Destroy(gameObject);
            }
        }
    }
}
