using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ToggleLight : MonoBehaviour
{
    public bool lightsEnabled;
    public Light[] lightSources;

    private void OnValidate()
    {
        Toggle();        
    }

    private void Start()
    {
        Toggle();
    }

    private void Toggle()
    {
        if (lightSources != null)
        {
            foreach (Light light in lightSources)
            {
                light.enabled = lightsEnabled;

            }
        }
    }
}
