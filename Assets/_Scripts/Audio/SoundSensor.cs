using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class SoundSensor : MonoBehaviour
{
    public AudioSource _audioSource;
    public SphereCollider sphereCollider;
    public float sensorScale = 100000;
    private float soundAmplitude;
    [SerializeField]private float[] audioSamples = new float[256];
    [SerializeField]private float amplitude = 0; 
    private void Update()
    {
       
        if (_audioSource.isPlaying)
        {
            GetSoundAmplitude();
        }
        else
        {
            amplitude = 0;
        }

        if (amplitude > 0)
        {
            sphereCollider.radius = amplitude * sensorScale;  
        }
        else
        {
            sphereCollider.radius = 0;  
        }
        
    }

    public void GetSoundAmplitude()
    {
        _audioSource.GetOutputData(audioSamples, 0);
      // _audioSource.GetSpectrumData(audioSamples, 0, FFTWindow.Blackman);
        
        for (int i = 0; i < audioSamples.Length; i++)
        {
            if (audioSamples[i] > 0)
            {
                amplitude = Mathf.Abs(audioSamples[i]);
            }
        }
    }
}
