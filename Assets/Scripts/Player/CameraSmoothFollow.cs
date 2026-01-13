using UnityEngine;

public class CameraSmoothFollow : MonoBehaviour
{
    public Camera cam;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {

        cam.fieldOfView = PlayerShootyManager.instance.handlingStamina / 100f * 90f;
    }

}
