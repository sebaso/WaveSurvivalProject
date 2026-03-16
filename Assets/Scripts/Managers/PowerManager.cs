using UnityEngine;
using System.Collections.Generic;

public class PowerManager : MonoBehaviour
{
    public static PowerManager instance;
    public bool isPowerOn;
    public List<Light> lights;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;
    }
    public void TurnPowerOn()
    {
        VendingMachine.instance.powered = true;
        isPowerOn = true;
        foreach (Light light in lights)
        {
            light.enabled = true;
        }
    }
    public void TurnPowerOff()
    {
        isPowerOn = false;
        foreach (Light light in lights)
        {
            light.enabled = false;
        }
    }


}
