using UnityEngine;
using System.Collections;
public class Grenade : MonoBehaviour
{
    public float cookTime = 5f;
    public bool bounced = false;
    public float bouncedTimeRemove = 4f;
    public float damage = 3f;
    public float explosionRadius = 5f;
    public float explosionForce = 10f;
    public GameObject explosionEffect;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(CookGrenade());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator CookGrenade()
    {
        yield return new WaitForSeconds(cookTime);
        Explode();
    }
    void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider col in colliders)
        {
            if (col.CompareTag("Player"))
            {
                col.GetComponent<PlayerShootyManager>().TakeDamage(2);
            }
            if (col.CompareTag("Enemy"))
            {
                col.GetComponent<Enemy>().TakeDamage(damage);
            }
        }
        Destroy(gameObject);
    }

}
