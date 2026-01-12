using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public Transform player;
    private NavMeshAgent nav;
    public float timeBetweenFetches;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        if (!player)
        {
            player = GameObject.FindWithTag("Player").transform;
        }
        StartCoroutine(ChasePlayer());
    }
    public void Die()
    {
        Destroy(gameObject);
        WaveManager.instance.enemiesLeft--;
    }

    private IEnumerator ChasePlayer()
    {
        nav.SetDestination(player.position);
        yield return new WaitForSeconds(timeBetweenFetches);
        StartCoroutine(ChasePlayer());
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            Die();
            Destroy(collision.gameObject);
        }
    }
}
