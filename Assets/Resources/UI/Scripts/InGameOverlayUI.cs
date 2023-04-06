using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InGameOverlayUI : MonoBehaviour
{
    public UIDocument inGameOverlay;
    
    public VisualElement playerstatusPanel;
    public Label healthLabel;
    public Label hungerLabel;
    public Label thirstLabel;
    public VisualElement staminaBar;
    
    public VisualElement weaponIcon;
    public Label bulletCountLabel;

    public VisualElement quickInventoryPanel;
    public List<VisualElement> quickInventoryslots;

    private HealthManager playerHealthManager;
    
    private void OnEnable()
    {
        try
        {
            playerHealthManager = GameObject.Find("Player").GetComponent<HealthManager>();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            
        }

        inGameOverlay = GetComponent<UIDocument>();
        VisualElement root = inGameOverlay.rootVisualElement;
        
        
    playerstatusPanel = root.Q<VisualElement>("PlayerStatus");
    healthLabel = root.Q<Label>("HealthLabel");
    hungerLabel = root.Q<Label>("FoodLabel");
    thirstLabel = root.Q<Label>("WaterLabel");
    staminaBar = root.Q<VisualElement>("StaminaBar").Q<VisualElement>("ForeGround");
    
    weaponIcon = root.Q<VisualElement>("WeaponIcon");
    bulletCountLabel= root.Q<Label>("BulletCount");

    quickInventoryPanel= root.Q<VisualElement>("QuickInventory");
    quickInventoryslots = quickInventoryPanel.Query<VisualElement>("Item").ToList();
    
    }
    
    private void Update()
    {
        
    }
}
