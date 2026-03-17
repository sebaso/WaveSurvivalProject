using UnityEngine;

public class ExplosiveInventory : MonoBehaviour
{
    public static ExplosiveInventory instance;

    public bool hasExplosives = false;

    public System.Action OnExplosivesChanged;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void GiveExplosives()
    {
        hasExplosives = true;
        OnExplosivesChanged?.Invoke();
    }

    public void UseExplosives()
    {
        hasExplosives = false;
        OnExplosivesChanged?.Invoke();
    }
}
