using UnityEngine;

public class CameraSmoothFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 0.125f;
    public float xThreshold = 5f;
    public float zThreshold = 5f;
    public float xOffset;
    public float yOffset;
    public float zOffset;
    private Camera cam;

    public float maxRadius = 5f;
    private LayerMask layerMask;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!target)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player)
            {
                target = player.transform;
            }
            else
            {
                Debug.LogError("Player not found!");
            }
        }
        cam = Camera.main;
        layerMask = LayerMask.GetMask("Floor");
    }

    // Update is called once per frame
    void Update()
    {
        Follow();

    }
    void Follow()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 targetPos = target.position + mousePos / 2;
        targetPos.x = Mathf.Clamp(targetPos.x, target.position.x + xOffset, target.position.x + xThreshold);
        targetPos.y = Mathf.Clamp(targetPos.y, target.position.y + yOffset, target.position.y + zThreshold);
        targetPos.z = Mathf.Clamp(targetPos.z, target.position.z + zOffset, target.position.z + zThreshold);

        this.transform.position = targetPos;
    }
}
