using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashLight : MonoBehaviour
{
    private Light _light;
    private KeyAssignments _keyAssignments;

    public bool power;
    void Start()
    {
        _light = GetComponent<Light>();
        _keyAssignments = GameManager.Instance._keyAssignments;
    }

    void Update()
    {
        _light.enabled = power;
        if (_keyAssignments != null)
        {
            if (Input.GetKeyDown(_keyAssignments.flashLightKey.keyCode))
            {
                power = !power;
            }
        }
    }
}
