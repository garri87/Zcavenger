using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashLight : MonoBehaviour
{
    private Light _light;

    public bool power;
    void Start()
    {
        _light = GetComponent<Light>();
    }

    void Update()
    {
        _light.enabled = power;
        if (Input.GetKeyDown(KeyAssignments.SharedInstance.flashLightKey.keyCode))
        {
            power = !power;
        }
    }
}
