using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour
{
    public Transform player;
    private NavMeshAgent nav;
    public float timeBetweenFetches;
    public int hp;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        if (!player)
        {
            player = GameObject.FindWithTag("Player").transform;
        }
        else
        {
            print("No player.");
        }
        StartCoroutine(ChasePlayer());
    }
    public void Die()
    {
        Destroy(gameObject);
        WaveManager.instance.enemiesLeft--;
        ScoreManager.instance.AddScore(30);
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
            if (collision.gameObject.TryGetComponent<Bullet>(out var bullet))
            {
                hp -= bullet.damage;
                ScoreManager.instance.AddScore(10);
            }
            if (hp <= 0)
            {
                Die();
            }
            Destroy(collision.gameObject);
        }
        else
        {
            Destroy(collision.gameObject);
        }
    }
}
