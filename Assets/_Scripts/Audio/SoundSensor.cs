using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class SoundSensor : MonoBehaviour
{
    public AudioSource _audioSource;
    public SphereCollider sphereCollider;
    public float sensorScale;
    private float soundAmplitude;
    private void Update()
    {
        if (_audioSource.isPlaying)
        {
            sphereCollider.radius = sensorScale;
        }
        else
        {
            sphereCollider.radius = 0;
        }
        
    }

    public void SetSoundSensorScale(float value)
    {
        sensorScale = value;
    }
    
}
