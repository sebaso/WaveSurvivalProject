using UnityEngine;

public interface IDamageableObserver
{
    public void OnHealthUpdate(int damageAmount);
    public void OnDead();

}
