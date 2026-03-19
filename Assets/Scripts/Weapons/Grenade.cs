using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Grenade : MonoBehaviour
{
    public float cookTime = 5f;
    private float timer;
    public bool bounced = false;
    public float bouncedTimeRemove = 3f;
    public float damage = 3f;
    public float explosionRadius = 5f;
    public float explosionForce = 10f;
    public GameObject explosionEffect;
    public AudioClip explosionSound;
    private bool hasExploded = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Removed coroutine to prevent duplicate explosions
    }

    // Update is called once per frame
    void Update()
    {
        if (hasExploded) return;

        timer += Time.deltaTime;
        if (timer > cookTime)
        {
            Explode();
        }

    }

    void Explode()
    {
        hasExploded = true;

        HashSet<Enemy> hitEnemies = new();
        HashSet<PlayerController> hitPlayers = new();

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider col in colliders)
        {
            if (col.CompareTag("Player"))
            {
                PlayerController pc = col.GetComponent<PlayerController>();
                if (pc != null && !hitPlayers.Contains(pc))
                {
                    pc.TakeDamage(1);
                    hitPlayers.Add(pc);
                }
            }
            if (col.CompareTag("Enemy"))
            {
                Enemy enemy = col.GetComponent<Enemy>();
                if (enemy != null && !hitEnemies.Contains(enemy))
                {
                    enemy.TakeDamage((int)damage);
                    hitEnemies.Add(enemy);
                }
            }
        }

        if (explosionEffect != null)
        {
            // Instantiate the explosion effect prefab at the grenade's location
            GameObject effectInstance = Instantiate(explosionEffect, transform.position, transform.rotation);
            effectInstance.SetActive(true); // Just in case the prefab is disabled

            ParticleSystem ps = effectInstance.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                ps.Play();
            }
            Destroy(effectInstance, 1.8f);
        }

        if (explosionSound != null)
        {
            AudioSource.PlayClipAtPoint(explosionSound, transform.position, 1f);
        }

        StartCoroutine(ApplyPhysicalExplosion());

        foreach (MeshRenderer mr in GetComponentsInChildren<MeshRenderer>()) mr.enabled = false;
        foreach (Collider c in GetComponentsInChildren<Collider>()) c.enabled = false;
    }

    private IEnumerator ApplyPhysicalExplosion()
    {
        yield return new WaitForFixedUpdate();

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider hit in colliders)
        {
            if (hit.TryGetComponent<Rigidbody>(out var rb))
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, 1f, ForceMode.Impulse);
            }
        }

        Destroy(gameObject);
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            timer += bouncedTimeRemove;
        }
        bounced = true;
    }


}
