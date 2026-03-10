using System.Collections.Generic;
using UnityEngine;

public class RagdollManager : MonoBehaviour
{
    public static RagdollManager instance;
    public List<GameObject> ragdolls = new();
    public int maxRagdolls = 10;
    private void Awake()
    {
        instance = this;
    }
    public void AddRagdoll(GameObject ragdoll)
    {
        if (IsRagdollFull())
        {
            RemoveOldestRagdoll();
        }
        ragdolls.Add(ragdoll);

    }
    public void RemoveRagdoll(GameObject ragdoll)
    {
        ragdolls.Remove(ragdoll);
    }
    public void ClearRagdolls()
    {
        ragdolls.Clear();
    }
    public bool IsRagdollFull()
    {
        return ragdolls.Count >= maxRagdolls;
    }
    public void RemoveOldestRagdoll()
    {
        if (ragdolls.Count > 0)
        {
            Destroy(ragdolls[0]);
            ragdolls.RemoveAt(0);
        }
    }

}