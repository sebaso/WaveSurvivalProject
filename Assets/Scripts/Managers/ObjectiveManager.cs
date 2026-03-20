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
    public bool hasPower;
    public AudioSource audioSource;
    public AudioClip generatorPoweredUpSound;

    public enum ObjectiveType
    {
        DefendLocation,
        TransportItem,
        CollectItems,
        None
    }
    public ObjectiveType objectiveType;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //Aqui hay muchisimas lagrimas sobre las cosas que no fueron ser.
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (isObjectiveActive)
        {
            float angle = GetCompassAngle(currentObjective.position);
            compassTransform.eulerAngles = new Vector3(0, angle, 0);
            compassTransform.position = new Vector3(player.transform.position.x, player.transform.position.y + compassOffsetY, player.transform.position.z);
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
            objectiveType = ObjectiveType.None;
            DeactivateObjective();
            hasPower = true;
            PowerManager.instance.TurnPowerOn();
            AchievementManager.Instance?.IncreaseStat("powered-up", 1);
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


    //Eran otros tiempos mas felices.
    public void GenerateDefendLocationObjective()
    {
        WaveManager.instance.spawner.SpawnObjectiveWave();
        currentObjective = defencePositions[Random.Range(0, defencePositions.Count)];
        objectiveType = ObjectiveType.DefendLocation;
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

        WaveManager wm = (waveManager != null) ? waveManager : WaveManager.instance;
        if (wm != null && wm.spawner != null)
        {
            wm.spawner.KillAllEnemies();
            wm.spawner.spawnQueue.Clear();
            wm.enemiesLeft = 0;

        }

        AchievementManager.Instance?.IncreaseStat("generator_powered_up", 1);
        currentObjective = null;
    }
}
