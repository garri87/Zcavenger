using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
   public AudioMixer audioMixer;
   
   public void SelectAudioMode(string value)
   {
      switch (value)
      {
         case "Stereo":
            AudioSettings.speakerMode = AudioSpeakerMode.Stereo;
            break;
         
         case "Mono":
            AudioSettings.speakerMode = AudioSpeakerMode.Mono;

            break;
         
         case "Surround":
            AudioSettings.speakerMode = AudioSpeakerMode.Surround;
            break;
         
         case "5.1":
            AudioSettings.speakerMode = AudioSpeakerMode.Mode5point1;
            break;
         
         case "7.1":
            AudioSettings.speakerMode = AudioSpeakerMode.Mode7point1;
            break;

            default:
            Debug.Log("Invalid Audio Mode: " + value);
            break;
      }
      Debug.Log("AudioMode: " + AudioSettings.speakerMode);

   }
}
