using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ToggleLight : MonoBehaviour
{
    public bool toogleLight;
    public Light[] lightSources;

    private void OnValidate()
    {
        if (lightSources != null) 
        {
            foreach (Light light in lightSources)
            {
                light.enabled = toogleLight;
            }
        }

        
    }

    private void Start()
    {
        if (lightSources != null) 
        {
            foreach (Light light in lightSources)
            {
                light.enabled = toogleLight;
               
            }
        }
    }
}
