using UnityEngine;

public interface IDamageable<T>
{
    void TakeDamage(T damage);
    void Die();
    bool IsDead { get; }
}
