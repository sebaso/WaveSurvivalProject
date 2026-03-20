using UnityEngine;
using System.Collections.Generic;

public class EasterEgg : MonoBehaviour, IDamageable<int>
{
    public int HP;
    public List<GameObject> deads;
    public GameObject turnon;

    public bool IsDead => HP <= 0;

    public void Die()
    {
        Debug.Log("Easter Egg Killed");
        foreach (var dead in deads)
        {
            if (dead != null) dead.SetActive(false);
        }
        if (turnon != null) turnon.SetActive(true);
    }

    void Start()
    {
    }

    public void TakeDamage(int damage)
    {
        TakeDamage(damage, transform.position);
    }

    public void TakeDamage(int damage, Vector3 hitPoint)
    {
        HP -= damage;
        if (HP <= 0)
        {
            Die();
        }
    }
}
