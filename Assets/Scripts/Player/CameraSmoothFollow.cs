using Unity.Cinemachine;
using UnityEngine;

public class CameraSmoothFollow : MonoBehaviour
{
    public CinemachineCamera cam;
    public float minFov = 70f;
    public float maxFov = 90f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = GameObject.Find("PlayerCamera").GetComponent<CinemachineCamera>();
    }

    // Update is called once per frame
    void Update()
    {

        cam.Lens.FieldOfView = Mathf.Clamp(PlayerShootyManager.instance.handlingStamina / 100f * 90f, minFov, maxFov);
    }

}
