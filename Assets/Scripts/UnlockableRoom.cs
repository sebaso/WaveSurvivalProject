using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using System;

[System.Serializable]
public class UnlockableRoom : MonoBehaviour
{
    public List<BoardedDoor> barriers;
    public List<Spawner> spawners;
    public GameObject roomDoor;
    public RoomFlags flags;

    [Flags]
    public enum RoomFlags
    {
        None = 0,
        Unlocked = 1 << 0,
        Destroyed = 1 << 1,
        HasRadioObjective = 1 << 2,

    }

    void Start()
    {
        RoomManager.instance.unlockableRooms.Add(this);
        if (barriers.Count == 0 && spawners.Count == 0)
        {
            Debug.LogError(gameObject.name + " has no barriers or spawners.");
        }
    }

    public void UnlockRoom()
    {
        roomDoor.SetActive(false);
        foreach (BoardedDoor barrier in barriers)
        {
            barrier.gameObject.SetActive(true);
        }
        foreach (Spawner spawner in spawners)
        {
            spawner.gameObject.SetActive(true);
        }
        flags |= RoomFlags.Unlocked;
    }
}