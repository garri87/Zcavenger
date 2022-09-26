using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyStr
{
   public KeyCode keyCode;
   public string keyName;
   
   public KeyStr(KeyCode keyCode, string keyName)
   {
      this.keyCode = keyCode;
      this.keyName = keyName;
   }
   
}

public class KeyAssignments : MonoBehaviour
{
   public static KeyAssignments SharedInstance;
   public Transform keyAssignTransform;

   
   public KeyStr leftKey = new KeyStr(KeyCode.A,"left");
   public KeyStr rightKey = new KeyStr(KeyCode.D,"right");
   public KeyStr upKey = new KeyStr(KeyCode.W,"up");
   public KeyStr downKey = new KeyStr(KeyCode.S,"down");
   public KeyStr jumpKey = new KeyStr(KeyCode.Space,"jump");
   public KeyStr walkKey = new KeyStr(KeyCode.LeftShift,"walk");
   public KeyStr crouchKey = new KeyStr(KeyCode.C,"crouch");
   public KeyStr proneKey = new KeyStr(KeyCode.Z,"prone");
   public KeyStr attackKey = new KeyStr(KeyCode.Mouse0,"attack");
   public KeyStr aimBlockKey = new KeyStr(KeyCode.Mouse1,"aim/Block");
   public KeyStr useKey = new KeyStr(KeyCode.E,"use");
   public KeyStr reloadKey = new KeyStr(KeyCode.R,"reload");
   public KeyStr flashLightKey = new KeyStr(KeyCode.F,"flashlight");
   public KeyStr inventoryKey = new KeyStr(KeyCode.Tab,"inventory");
   public KeyStr pauseKey = new KeyStr(KeyCode.Escape,"pause");
   public KeyStr primaryKey = new KeyStr(KeyCode.Alpha1,"Draw primary");
   public KeyStr secondaryKey = new KeyStr(KeyCode.Alpha2,"Draw secondary");
   public KeyStr meleeKey = new KeyStr(KeyCode.Alpha3,"Draw melee");
   public KeyStr throwableKey = new KeyStr(KeyCode.Alpha4,"Draw Throwable");
   
   
   public KeyStr[] keyCodes;
   public int keyCodesLength;
   
   private int keybindChildCount;
   private void OnValidate()
   {
      keyCodes = new KeyStr[]
      {
         jumpKey,
         walkKey,
         crouchKey,
         proneKey,
         attackKey,
         aimBlockKey,
         useKey,
         reloadKey,
         flashLightKey,
         inventoryKey,
         primaryKey,
         secondaryKey,
         meleeKey,
         throwableKey,
      };
      keyCodesLength = keyCodes.Length;
      keybindChildCount = keyAssignTransform.childCount;
      InitInputOptionsKeys();

   }

   private void Awake()
   {    
      SharedInstance = this;
      InitInputOptionsKeys();
   }

   public void InitInputOptionsKeys()
   {
      if (keybindChildCount == keyCodes.Length)
      {
         for (int i = 0; i < keybindChildCount; i++)
         {
            Key keyIndex = keyAssignTransform.GetChild(i).GetComponent<Key>();
            keyIndex.currentKeyCode = keyCodes[i].keyCode;
            keyIndex.currentKeyName = keyCodes[i].keyName;
         }
      }
      else if(keybindChildCount > keyCodes.Length)
      {
         Debug.Log("The Keybindings group is greater than bindable keys");
      }
      else if (keybindChildCount < keyCodes.Length)
      {
         Debug.Log("The Keybindings group is smaller than bindable keys");
      }
   }

   /// <summary>
   /// Update the key assignment for a given key name
   /// </summary>
   /// <param name="keyCodeName"></param>
   public void UpdateKeyBinding(string keyCodeName)
   {
      for (int i = 0; i < keybindChildCount; i++)
      {
         Key keyIndex = keyAssignTransform.GetChild(i).GetComponent<Key>();
         if (keyCodes[i].keyName == keyCodeName)
         {
            keyCodes[i].keyCode = keyIndex.currentKeyCode;
            keyCodes[i].keyName = keyIndex.currentKeyName;
            Debug.Log("The " + keyCodes[i].keyName + " key was successfully rebinded to" + keyCodes[i].keyCode.ToString());
         }
         
      }
   }
}

