using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class Key : MonoBehaviour
{
   public TextMeshProUGUI titleTextTMP;
   public Button keyButton;
   public TextMeshProUGUI keyButtonText;
   public KeyCode currentKeyCode;
   public string currentKeyName;
   public bool waitForKey;
   
   private float timer;
   public float cancelTime = 10;
   
   private void OnValidate()
   {

      titleTextTMP.text = currentKeyName;
      keyButtonText.text = currentKeyCode.ToString();
      
   }

   private void Awake()
   {
      timer = cancelTime;
   }
   
   private void OnEnable()
   {
      
      timer = cancelTime;
   }

   private void Update()
   {
      if (waitForKey)
      {
         
      }
   }

   private void OnGUI()
   {
      if (waitForKey)
      {
         if (Event.current.isKey) 
         {
                ListenForKeyCode();
            
         }
      }
     else
     {
        titleTextTMP.text = currentKeyName;
        keyButtonText.text = currentKeyCode.ToString();
     }
   }

    private void ListenForKeyCode()
    {
        KeyCode newKeyCode = Event.current.keyCode;
        if (newKeyCode != KeyCode.Escape)
        {
            currentKeyCode = newKeyCode;
            KeyAssignments.Instance.UpdateKeyBinding(currentKeyName);
            timer = cancelTime;
            waitForKey = false;
        }
        else
        {
            timer = cancelTime;
            waitForKey = false;
        }
    }

    public void SetKeyAssignment()
   {
      keyButtonText.text = "Press a Key";
      waitForKey = true;
   }
}
