using UnityEngine;

public class ObjectiveManager : MonoBehaviour
{
    public static ObjectiveManager instance;
    public GameObject objectiveMarker;
    public bool isObjectiveActive = false;
    public WaveManager waveManager;
    public GameObject player;
    public float objectiveDistance = 10f;

    public enum ObjectiveType
    {
        DefendLocation,
        TransportItem,
        CollectItems
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
    }
    public void RollObjective()
    {
        objectiveType = (ObjectiveType)Random.Range(0, System.Enum.GetNames(typeof(ObjectiveType)).Length);
        Debug.Log("Objective: " + objectiveType);
    }
    public void GenerateDefendLocationObjective()
    {

    }
    public void GenerateTransportItemObjective()
    {

    }
    public void GenerateCollectItemsObjective()
    {

    }

    public void ActivateObjective()
    {
        isObjectiveActive = true;
        objectiveMarker.SetActive(true);
        waveManager.wavesArePaused = true;
    }

    public void DeactivateObjective()
    {
        isObjectiveActive = false;
        objectiveMarker.SetActive(false);
        waveManager.wavesArePaused = false;
    }
}
