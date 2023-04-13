using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LoadingScreenUI : MonoBehaviour
{
   public UIDocument loadingScreen;
   
   public VisualElement progressBar;
   public Label hintsLabel;
   public HintsText hintsText; // Obtener los hints desde este script

   private void OnEnable()
   {
      
   }
}
