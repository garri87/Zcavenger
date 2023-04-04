using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InGameOverlayUI : MonoBehaviour
{
    public UIDocument inGameOverlay;
    
    public VisualElement statsPanel;
    public Label healthLabel;
    public Label hungerLabel;
    public Label thirstLabel;
    public VisualElement staminaBar;
    
    public VisualElement weaponIcon;
    public Label bulletCountLabel;
    
    public VisualElement quickInventoryPanel1;
    public VisualElement quickInventoryPanel2;
    public VisualElement quickInventoryPanel3;
    public VisualElement quickInventoryPanel4;
    public VisualElement quickInventoryPanel5;

    private void OnEnable()
    {
        
    }
}
