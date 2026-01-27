using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using System;
using TMPro;

[System.Serializable]
public class UnlockableRoom : MonoBehaviour
{
    public List<BoardedDoor> barriers = new List<BoardedDoor>();
    public List<Transform> enemySpawnPoints = new List<Transform>();
    public GameObject roomDoor;
    public RoomFlags flags;
    public TextMeshProUGUI buyText;
    public int roomCost;
    public int buyTextDistance;
    public bool isInRange;
    public bool isUnlocked;
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
        foreach (Transform spawner in enemySpawnPoints)
        {
            print(spawner.name);
            spawner.gameObject.SetActive(false);
        }
        RoomManager.instance.unlockableRooms.Add(this);

        if (barriers.Count == 0 || enemySpawnPoints.Count == 0)
        {
            Debug.LogError(gameObject.name + " has no barriers or spawners.");
        }
        foreach (Transform spawner in enemySpawnPoints)
        {
            spawner.gameObject.SetActive(false);
        }

    }
    void Update()
    {
        isInRange = IsInRange();
        DisplayBuyMessage();
    }

    public void UnlockRoom()
    {
        roomDoor.SetActive(false);
        isUnlocked = true;
        foreach (BoardedDoor barrier in barriers)
        {
            barrier.gameObject.SetActive(true);
        }

        foreach (Transform spawner in enemySpawnPoints)
        {
            spawner.gameObject.SetActive(true);
            WaveManager.instance.spawner.AddSpawnPoint(spawner);
        }

    }
    public bool IsInRange()
    {
        return Vector3.Distance(PlayerController.instance.transform.position, roomDoor.transform.position) < buyTextDistance;
    }
    public void DisplayBuyMessage()
    {
        if (isUnlocked && buyText != null)
        {
            buyText.enabled = false;
            buyText = null;
            return;
        }
        if (!isInRange && buyText != null)
        {
            buyText.enabled = false;
            buyText = null;
        }

        if (isInRange && !isUnlocked)
        {
            if (buyText == null)
            {
                buyText = GameObject.FindWithTag("MainText").GetComponent<TextMeshProUGUI>();
            }
            buyText.text = "Press E to pay " + roomCost + " to unlock the room.";
            buyText.enabled = true;

            if (Input.GetKeyDown(KeyCode.E) && ScoreManager.instance.Score >= roomCost)
            {
                ScoreManager.instance.AddScore(-roomCost);
                UnlockRoom();
            }
        }
    }
}