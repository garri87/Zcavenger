using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSelectUI : MonoBehaviour
{
   public Image[] images;
   public Slot[] equipmentSlots;
   private int i = 0;

   private void Update()
   {
      UpdateImages();
   }

   public void UpdateImages()
   {
      i = 0;
      foreach (Slot slot in equipmentSlots)
      {
         if (slot.slotImage.sprite != null)
         {
            images[i].sprite = slot.slotImage.sprite;
         }
         i++;
      }
   }
   
}
