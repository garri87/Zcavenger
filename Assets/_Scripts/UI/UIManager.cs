using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Slider = UnityEngine.UI.Slider;

public class UIManager : MonoBehaviour
{
    public UIDocument _UIDocument;
    public VisualTreeAsset currentVisualTreeAsset;

    [Header("Main Menu")] 
    public VisualTreeAsset mainMenuDocument;

   
    
    [Header("Loading Screen")]
    public VisualTreeAsset loadingScreenDocument;
    
    
    [Header("In Game Overlay")]
    public VisualTreeAsset inGameOverlayUIDocument;
    
    
    
    [Header("Pause Menu, Options Menu & Game Over Panel")]
    public VisualTreeAsset pauseMenuUIDocument;

    
 
    public VisualTreeAsset optionsMenuUIDocument;
    public Button displayButton;
    public Dropdown resolutionDropdown;
    
    
    
    public VisualTreeAsset gameOverUIDocument;
    public Label gameOverLabel;
    
    [Header("Inventory Area")]
    public VisualTreeAsset inventoryUIDocument;
    public VisualElement inventorySlotArea;
    public VisualElement capacityPanel;
    public List<Slot> inventorySlotList = new List<Slot>();
    [Space]
    public VisualElement primaryEquipSlot;
    public VisualElement secondaryEquipSlot;
    public VisualElement meleeEquipSlot;
    public VisualElement throwableEquipSlot;
    [Space]
    public VisualElement headEquipSlot;
    public VisualElement vestEquipSlot;
    public VisualElement legsEquipSlot;
    public VisualElement backpackEquipSlot;

    [Header("Inspect & Quick Info Panels")]
    public VisualElement inspectPanel;

    private void OnEnable()
    {
        _UIDocument = GetComponent<UIDocument>();
        
        
        
    }

    public void ToggleUI(VisualTreeAsset visualTreeAsset = null, bool enabled = false)
    {
        if (visualTreeAsset != null)
        {
            _UIDocument.visualTreeAsset = visualTreeAsset;
            _UIDocument.rootVisualElement.visible = enabled;
        }
        else
        {
            _UIDocument.visualTreeAsset = null;
        }
    }

    public void ToggleInventory(bool enabled)
    {
        
    }
}
