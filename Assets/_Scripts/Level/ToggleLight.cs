using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ToggleLight : MonoBehaviour
{
    public bool toogleLight;
    public GameObject[] lightShapes;
    public Light[] lightSources;

    private void OnValidate()
    {
        if (lightShapes != null && lightSources != null) 
        {
            foreach (Light light in lightSources)
            {
                light.enabled = toogleLight;
            }
        }

        
    }

    private void Start()
    {
        if (lightShapes != null && lightSources != null) 
        {
            foreach (Light light in lightSources)
            {
                light.enabled = toogleLight;
                foreach (GameObject lightShape in lightShapes) 
                {
                    if (light.enabled == false)
                    {
                        lightShape.SetActive(false);
                    }
                    else
                    {
                        lightShape.SetActive(true);
                    }
                }
            }
        }
    }
}
