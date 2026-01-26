using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using System;
using TMPro;

[System.Serializable]
public class UnlockableRoom : MonoBehaviour
{
    public List<BoardedDoor> barriers = new List<BoardedDoor>();
    public List<Transform> spawners = new List<Transform>();
    public GameObject roomDoor;
    public RoomFlags flags;
    public TextMeshProUGUI buyText;
    public int roomCost;
    public int buyTextDistance;
    public bool isInRange;
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
        foreach (Transform spawner in spawners)
        {
            spawner.gameObject.SetActive(false);
        }
        UnlockRoom();

    }
    void Update()
    {
        isInRange = IsInRange();
        DisplayBuyMessage();
    }

    public void UnlockRoom()
    {
        roomDoor.SetActive(false);
        foreach (BoardedDoor barrier in barriers)
        {
            barrier.gameObject.SetActive(true);
        }
        foreach (Transform spawner in spawners)
        {

            spawner.gameObject.SetActive(true);
            spawner.parent = null;
        }
        flags |= RoomFlags.Unlocked;

    }
    public bool IsInRange()
    {
        return Vector3.Distance(PlayerController.instance.transform.position, roomDoor.transform.position) < buyTextDistance;
    }
    public void DisplayBuyMessage()
    {
        if (!isInRange && buyText != null)
        {
            buyText.enabled = false;
            buyText = null;
        }

        if (isInRange)
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