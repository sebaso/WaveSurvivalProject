using UnityEngine;

public class Crosshair : MonoBehaviour
{
    public RectTransform defaultCrosshair;
    public RectTransform reloadCrosshair;
    void Update()
    {
        if (!WeaponHolder.instance.isReloading)
        {
            defaultCrosshair.position = Input.mousePosition;
            defaultCrosshair.gameObject.SetActive(true);
            reloadCrosshair.gameObject.SetActive(false);
        }
        else
        {
            defaultCrosshair.gameObject.SetActive(false);
            reloadCrosshair.position = Input.mousePosition;
            reloadCrosshair.gameObject.SetActive(true);
        }

    }
}
