using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
   public TMP_Dropdown audioModeDropDown;
   
   public void SelectAudioMode(int value)
   {
      switch (audioModeDropDown.value)
      {
         case 0:
            AudioSettings.speakerMode = AudioSpeakerMode.Stereo;
            break;
         
         case 1:
            AudioSettings.speakerMode = AudioSpeakerMode.Mono;

            break;
         
         case 2:
            AudioSettings.speakerMode = AudioSpeakerMode.Surround;
            break;
         
         case 3:
            AudioSettings.speakerMode = AudioSpeakerMode.Mode5point1;
            break;
         
         case 4:
            AudioSettings.speakerMode = AudioSpeakerMode.Mode7point1;
            break;
      }
      Debug.Log("AudioMode: " + AudioSettings.speakerMode);

   }
}
