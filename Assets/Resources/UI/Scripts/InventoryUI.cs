using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class InventoryUI : MonoBehaviour
{
    public UIDocument inventory;

    public VisualElement inventorySlotArea;
    public Label capacityLabel;

    public VisualElement inventorySlot;
    public List<VisualElement> inventorySlotList;
    public VisualElement primaryWeaponSlot;
    public VisualElement secondaryWeaponSlot;
    public VisualElement meleeWeaponSlot;
    public VisualElement throwableWeaponSlot;
    public VisualElement headEquipSlot;
    public VisualElement vestEquipSlot;
    public VisualElement torsoEquipSlot;

    public VisualElement legsEquipSlot;
    public VisualElement feetEquipSlot;
    public VisualElement backpackEquipSlot;

    [Header("Context Menu")] public VisualElement contextMenu;
    public Button useButton;
    public Button equipButton;
    public Button inspectButton;
    public Button throwButton;

    [Header("Inspect & Quick Info Panels")]
    public Label quickInfoLabel;

    public VisualElement inspectPanel;
    public VisualElement inspectImage;
    public Label inspectItemTitle;
    public Label inspectItemInfo;
    public List<VisualElement> itemStats;
    public Button inspectCloseButton;

    public Button inventoryCloseButton;


    private void OnEnable()
    {
        inventory = GetComponent<UIDocument>();
        VisualElement root = inventory.rootVisualElement;

        #region Inventory Slots Area

        inventorySlotArea = root.Q<VisualElement>();
        capacityLabel = root.Q<Label>();

        inventorySlot = root.Q<VisualElement>("InventorySlot");

        #endregion

        #region Weapons and Equipment Area

        primaryWeaponSlot = root.Q<VisualElement>("PrimaryWpnSlot");
        secondaryWeaponSlot = root.Q<VisualElement>("SecondayWpnSlot");
        meleeWeaponSlot = root.Q<VisualElement>("MeleeWpnSlot");
        throwableWeaponSlot = root.Q<VisualElement>("ThrowableWpnSlot");

        headEquipSlot = root.Q<VisualElement>("HeadEqSlot");
        vestEquipSlot = root.Q<VisualElement>("VestEqSlot");
        torsoEquipSlot = root.Q<VisualElement>("TorsoEqSlot");
        legsEquipSlot = root.Q<VisualElement>("LegsEqSlot");
        feetEquipSlot = root.Q<VisualElement>("FeetEqSlot");
        backpackEquipSlot = root.Q<VisualElement>("BackPackEqSlot");

        #endregion


        #region Context Menu

        contextMenu = root.Q<VisualElement>("ContextMenu");
        useButton = root.Q<Button>("UseButton");
        equipButton = root.Q<Button>("EquipButton");
        inspectButton = root.Q<Button>("InspectButton");
        throwButton = root.Q<Button>("ThrowButton");

        #endregion

        #region General

        quickInfoLabel = root.Q<Label>("QuickInfoLabel");
        inventoryCloseButton = root.Q<Button>("InventoryCloseButton");

        #endregion

        #region Inspect Window

        inspectPanel = root.Q<VisualElement>("InspectWindow");
        inspectImage = inspectPanel.Q<VisualElement>("Image");
        inspectItemTitle = inspectPanel.Q<Label>("ItemTitle");
        inspectItemInfo = inspectPanel.Q<Label>("ItemInfo");
        itemStats = inspectPanel.Query<VisualElement>("Stat").ToList();
        
        inspectCloseButton = root.Q<Button>("InspectCloseButton");

        #endregion
    }

    private void Update()
    {
        
    }

    public void ToggleUI(VisualElement element, bool enabled)
    {
        element.visible = enabled;
    }

    public void GetItemStats()
    {
        //TODO: OBTENER CANTIDAD DE ATRIBUTOS DEL ITEM E INSTANCIAR BARRAS
        
        itemStats = inspectPanel.Query<VisualElement>("Stat").ToList();
    }

    public void FillInventoryWithSlots(VisualElement parentElement, int capacity)
    {
        for (int i = 0; i < capacity; i++)
        {
            parentElement.Add(inventorySlot);   
            
        }

        inventorySlotList = parentElement.Query<VisualElement>("Slot").ToList();
    }
}