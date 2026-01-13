using UnityEngine;

public class MouseCamera : MonoBehaviour
{
    public Transform mouse;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, 1000f, LayerMask.GetMask("Floor")))
        {
            mouse.position = hitInfo.point;
        }
    }
}
