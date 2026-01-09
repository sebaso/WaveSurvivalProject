using UnityEngine;

public class CameraSmoothFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 0.125f;
    public Vector3 offset;
    private Camera cam;
    private float maxRadius = 5f;
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
        if (!target)
        {
            Debug.LogError("Target not found!");
            return;
        }

        //Vector3 desiredPosition = target.position + offset;
        //Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        // transform.position = smoothedPosition;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, layerMask))
        {
            Vector3 mousePos = raycastHit.point;

            Vector3 difference = mousePos - target.position;
            float magnitude = difference.magnitude;
            if (magnitude > maxRadius)
            {
                difference = difference * (maxRadius / magnitude);
            }
            transform.position = target.position + difference;
        }
    }
}

