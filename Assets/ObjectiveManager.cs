using System.Collections.Generic;
using UnityEngine;

public class ObjectiveManager : MonoBehaviour
{
    public static ObjectiveManager instance;
    public GameObject objectiveMarker;
    public Transform currentObjective;
    public bool isObjectiveActive = false;
    public WaveManager waveManager;
    public GameObject player;
    public Transform compassTransform;
    public float compassOffsetY = 1f;
    public List<Transform> defencePositions = new();
    public float objectiveDistance = 10f;
    public float compassDistanceToHide = 10f;
    public int enemiesToDefend = 10;
    public List<Transform> transportObjectives = new();
    public List<Transform> collectObjectives = new();

    public enum ObjectiveType
    {
        DefendLocation,
        TransportItem,
        CollectItems,
        None
    }
    public ObjectiveType objectiveType;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            RollObjective();
        }
        if (isObjectiveActive)
        {
            float angle = GetCompassAngle(currentObjective.position);
            compassTransform.eulerAngles = new Vector3(0, angle, 0);
            compassTransform.position = new Vector3(player.transform.position.x, player.transform.position.y + compassOffsetY, player.transform.position.z);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            DeactivateObjective();
        }
    }
    public void DefenseObjectiveLogic()
    {
        if (objectiveDistance < compassDistanceToHide + 5f)
        {
            enemiesToDefend--;
            Debug.Log("Enemies left to defend: " + enemiesToDefend);
        }
        if (enemiesToDefend <= 0)
        {
            DeactivateObjective();
        }
    }
    public void RollObjective()
    {
        objectiveType = (ObjectiveType)Random.Range(0, System.Enum.GetNames(typeof(ObjectiveType)).Length);
        Debug.Log("Objective: " + objectiveType);
        switch (objectiveType)
        {
            case ObjectiveType.DefendLocation:
                if (defencePositions.Count > 0)
                {
                    GenerateDefendLocationObjective();
                }
                break;
            case ObjectiveType.TransportItem:
                if (transportObjectives.Count > 0)
                {
                    GenerateTransportItemObjective();
                }
                break;
            case ObjectiveType.CollectItems:
                if (collectObjectives.Count > 0)
                {
                    GenerateCollectItemsObjective();
                }
                break;
        }
    }
    public float GetCompassAngle(Vector3 targetPosition)
    {
        objectiveDistance = Vector3.Distance(player.transform.position, currentObjective.position);
        if (objectiveDistance > compassDistanceToHide)
        {
            objectiveMarker.SetActive(true);
        }
        else
        {
            objectiveMarker.SetActive(false);
        }
        Vector3 offset = targetPosition - player.transform.position;
        offset.y = 0; // Ignore vertical
        return Mathf.Atan2(offset.x, offset.z) * Mathf.Rad2Deg;
    }



    public void GenerateDefendLocationObjective()
    {
        WaveManager.instance.spawner.SpawnObjectiveWave();
        currentObjective = defencePositions[Random.Range(0, defencePositions.Count)];
        ActivateObjectiveCompass();
    }
    public void GenerateTransportItemObjective()
    {
        ActivateObjectiveCompass();
        currentObjective = transportObjectives[Random.Range(0, transportObjectives.Count)];
    }
    public void GenerateCollectItemsObjective()
    {
        ActivateObjectiveCompass();
        currentObjective = collectObjectives[Random.Range(0, collectObjectives.Count)];
    }

    public void ActivateObjectiveCompass()
    {
        objectiveDistance = Vector3.Distance(player.transform.position, currentObjective.position);
        isObjectiveActive = true;
        objectiveMarker.SetActive(true);

    }

    public void DeactivateObjective()
    {
        objectiveType = ObjectiveType.None;
        Debug.Log("Objective Deactivated");
        isObjectiveActive = false;
        objectiveMarker.SetActive(false);
        waveManager.wavesArePaused = false;
        waveManager.timer = 10;
        waveManager.spawner.KillAllEnemies();
        currentObjective = null;
        waveManager.spawner.NextWave();
    }
}
