using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReverbZone : MonoBehaviour
{
    public AudioReverbZone[] _reverbZones;    
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (AudioReverbZone reverbZone in _reverbZones)
            {
                reverbZone.enabled = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (AudioReverbZone reverbZone in _reverbZones)
            {
                reverbZone.enabled = false;
            }
        }
    }
}
