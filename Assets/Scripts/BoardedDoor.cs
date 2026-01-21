using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using Unity.VisualScripting;
using System.Collections;
using UnityEngine.AI;
using TMPro;

[RequireComponent(typeof(NavMeshObstacle))]
public class BoardedDoor : MonoBehaviour
{
    public List<GameObject> boards;
    public int interactionRange = 2;
    public int boardCount;
    public int maxBoardCount;
    public float invulnerabilityTime = 1f;
    private float invulnerabilityTimer;
    public float repairDelay;
    private NavMeshObstacle navMeshObstacle;
    public TextMeshProUGUI repairText;

    private bool isDestroyed;
    public bool isFullHealth;
    private int maxBoards;
    private int currIndex;
    private int numActiveBoards;
    private float nextRepair;

    const int repairValue = 10;

    void Start()
    {
        navMeshObstacle = GetComponent<NavMeshObstacle>();
        boards = new List<GameObject>();
        foreach (Transform child in transform)
        {
            if (child.CompareTag("DoorBoard"))
            {
                boards.Add(child.gameObject);
            }
        }
        maxBoards = boards.Count;
        isDestroyed = false;
        currIndex = maxBoards;
    }
    public bool IsInRange()
    {
        return Vector3.Distance(transform.position, PlayerController.instance.transform.position) < interactionRange;
    }


    void Update()
    {
        if (currIndex < maxBoards)
        {
            isFullHealth = false;
        }

        if (currIndex == 0)
        {
            isDestroyed = true;
            navMeshObstacle.enabled = false;
        }

        if (currIndex == maxBoards)
        {
            isFullHealth = true;
            if (repairText != null)
            {
                repairText.enabled = false;
            }
        }

        if (IsInRange() && repairText == null)
        {
            repairText = GameObject.FindGameObjectWithTag("MainText").GetComponent<TextMeshProUGUI>();
            repairText.enabled = true;
        }

        if (IsInRange() & !isFullHealth)
        {
            repairText.enabled = true;
            repairText.text = "Hold E to pay to repair the door.";
            if (Input.GetKey(KeyCode.E))
                Repair();
        }
        if (!IsInRange() && repairText != null)
        {

            repairText.enabled = false;
            repairText = null;
        }
    }

    public void TakeDamage()
    {
        if (boards.Count > 0 && Time.time > invulnerabilityTimer)
        {
            boards[currIndex - 1].SetActive(false);
            currIndex -= 1;
            invulnerabilityTimer = Time.time + invulnerabilityTime;
        }
    }

    public void Repair()
    {
        if (Time.time > nextRepair)
        {
            isDestroyed = false;
            boards[currIndex].SetActive(true);
            currIndex += 1;
            navMeshObstacle.enabled = true;
            ScoreManager.instance.AddScore(repairValue);
            nextRepair = Time.time + repairDelay;
        }
    }

    public bool GetIsDestroyed()
    {
        return this.isDestroyed;
    }

    public bool GetIsFullHealth()
    {
        return this.isFullHealth;
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}
