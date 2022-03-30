using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour
{
    public string volumeParameter; 
    public AudioMixer audioMixer; 
    public Slider slider; 
    public float multiplier = 30;


  private void Awake()
  { 
   slider.onValueChanged.AddListener(HandleSliderValue);
  }

  private void Start()
  {
      slider.value = PlayerPrefs.GetFloat(volumeParameter, slider.value);
  }

  private void Update()
  {
    
  }

  
  private void OnDisable()
  {
      PlayerPrefs.SetFloat(volumeParameter,slider.value);
  }
  
  private void HandleSliderValue(float value)
  {
      audioMixer.SetFloat(volumeParameter, Mathf.Log10(value)*multiplier);
  }
}
