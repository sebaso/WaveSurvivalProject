using UnityEngine;

public interface IDamageable<T>
{
    void TakeDamage(T damage);
    void TakeDamage(T damage, Vector3 hitPoint);
    void Die();
    bool IsDead { get; }
}
