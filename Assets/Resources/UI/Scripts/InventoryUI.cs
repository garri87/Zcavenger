using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class InventoryUI : MonoBehaviour
{
    public UIDocument inventory;
    
    [Header("Items area")]
    public VisualElement inventorySlotArea;
    public Label capacityLabel;

    public VisualElement inventorySlot;
    public List<VisualElement> inventorySlotList;

    public VisualElement equipmentSlotsArea;
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

    public VisualElement inspectItemPanel;
    public VisualElement inspectItemImage;
    private Sprite itemIcon;
    public Label inspectItemTitle;
    private string itemName;
    public Label inspectItemInfo;
    private string itemDescription;
    public VisualElement statsPanel;
    public VisualTreeAsset statTemplate;
    public List<VisualElement> itemStats;
    public Button inspectCloseButton;

    public Button inventoryCloseButton;

    private int selectedSlot;

    private Inventory playerInventory;
    
    private void OnEnable()
    {
        playerInventory = GameObject.Find("Player").GetComponent<Inventory>();
        
        inventory = GetComponent<UIDocument>();
        
        
        VisualElement root = inventory.rootVisualElement;

        #region Inventory Slots Area

        inventorySlotArea = root.Q<VisualElement>("InventorySlots");
        capacityLabel = root.Q<Label>("Cap");

        inventorySlot = root.Q<VisualElement>("InventorySlot");

        #endregion

        #region Weapons and Equipment Area

        equipmentSlotsArea = root.Q<VisualElement>("EquipmentSlots");
        primaryWeaponSlot = equipmentSlotsArea.Q<VisualElement>("PrimaryWpnSlot");
        secondaryWeaponSlot = equipmentSlotsArea.Q<VisualElement>("SecondayWpnSlot");
        meleeWeaponSlot = equipmentSlotsArea.Q<VisualElement>("MeleeWpnSlot");
        throwableWeaponSlot = equipmentSlotsArea.Q<VisualElement>("ThrowableWpnSlot");

        headEquipSlot = equipmentSlotsArea.Q<VisualElement>("HeadEqSlot");
        vestEquipSlot = equipmentSlotsArea.Q<VisualElement>("VestEqSlot");
        torsoEquipSlot = equipmentSlotsArea.Q<VisualElement>("TorsoEqSlot");
        legsEquipSlot = equipmentSlotsArea.Q<VisualElement>("LegsEqSlot");
        feetEquipSlot = equipmentSlotsArea.Q<VisualElement>("FeetEqSlot");
        backpackEquipSlot = equipmentSlotsArea.Q<VisualElement>("BackPackEqSlot");

        List<VisualElement> equipSlotList = new List<VisualElement>()
        {
            primaryWeaponSlot,
            secondaryWeaponSlot,
            meleeWeaponSlot,
            throwableWeaponSlot,
            headEquipSlot,
            vestEquipSlot,
            torsoEquipSlot,
            legsEquipSlot,
            feetEquipSlot,
            backpackEquipSlot

        };

        foreach (VisualElement slot in equipSlotList)
        {
            slot.RegisterCallback<ClickEvent>(SlotClickEvent);
        }
        
        #endregion


        #region Context Menu

        contextMenu = root.Q<VisualElement>("ContextMenu");
        useButton = contextMenu.Q<Button>("UseButton");
        equipButton = contextMenu.Q<Button>("EquipButton");
        inspectButton = contextMenu.Q<Button>("InspectButton");
        throwButton = contextMenu.Q<Button>("ThrowButton");
       
        useButton.RegisterCallback<ClickEvent>(UseItem);
        equipButton.RegisterCallback<ClickEvent>(EquipItem);
        inspectButton.RegisterCallback<ClickEvent>(InspectItem);
        throwButton.RegisterCallback<ClickEvent>(DropItem); 
        
        #endregion

        #region General

        quickInfoLabel = root.Q<Label>("QuickInfoLabel");
        inventoryCloseButton = root.Q<Button>("InventoryCloseButton");

        #endregion

        #region Inspect Window

        inspectItemPanel = root.Q<VisualElement>("InspectWindow");
        inspectItemImage = inspectItemPanel.Q<VisualElement>("Image");
        inspectItemTitle = inspectItemPanel.Q<Label>("ItemTitle");
        inspectItemInfo = inspectItemPanel.Q<Label>("ItemInfo");
        statsPanel = inspectItemPanel.Q<VisualElement>("StatsPanel");
        itemStats = inspectItemPanel.Query<VisualElement>("Stat").ToList();
        
        inspectCloseButton = root.Q<Button>("InspectCloseButton");

        #endregion
    }

    public void CreateStatBar(int order, string name, int value)
    {
        VisualElement newStat = statTemplate.Instantiate();
        newStat.name = "Stat" + order;
        statsPanel.Add(newStat);
        VisualElement stat = statsPanel.Q<VisualElement>("Stat" + order);
        stat.AddToClassList("Stat");

        Label statName = stat.Q<Label>("StatName");
        statName.AddToClassList("StatName");
        statName.text = name;
        
        Label statValue = stat.Q<Label>("StatValue");
        statValue.AddToClassList("StatValue");
        statValue.text = value.ToString();
        
        VisualElement statBar = stat.Q<VisualElement>("StatBar");
        statBar.AddToClassList("StatBar");
        statBar.style.width = new StyleLength(value);
        
    }
    
    /// <summary>
    /// Get the item's stats and generate bars for the ui
    /// </summary>
    /// <param name="item"></param>
    public void GetItemStats(Item item)
    {
        statsPanel.Query<VisualElement>("Stat").ToList().Clear();//Clear the area before generating new stats
        
        if (item.itemClass == Item.ItemClass.Item)
        {
            CreateStatBar(0,"Health Restore", item.healthRestore);
            CreateStatBar(1,"Food Restore",item.foodRestore);
            CreateStatBar(2,"Water Restore",item.waterRestore);
            itemStats = inspectItemPanel.Query<VisualElement>("Stat").ToList();
        }
        
        if (item.itemClass == Item.ItemClass.Weapon)
        {
            CreateStatBar(0,"Damage", item.damage);
            CreateStatBar(1,"magazine Cap",item.magazineCap);
            CreateStatBar(2,"Fire Rate",Mathf.RoundToInt(item.fireRate));
            CreateStatBar(3,"Recoil", Mathf.RoundToInt(item.recoilMaxRotation));
            itemStats = inspectItemPanel.Query<VisualElement>("Stat").ToList();
        }

        if (item.itemClass == Item.ItemClass.Equipment)
        {
            CreateStatBar(0,"Defense", item.defense);
            itemStats = inspectItemPanel.Query<VisualElement>("Stat").ToList();
        }
    }

    public void FillInventoryWithSlots(int capacity)
    {
        for (int i = 0; i < capacity; i++)
        {
            inventorySlotArea.Add(inventorySlot);
        }

        inventorySlotList = inventorySlotArea.Query<VisualElement>("Slot").ToList();
        foreach (VisualElement slot in inventorySlotList)
        {
            slot.RegisterCallback<ClickEvent>(SlotClickEvent);
        }
    }

    public void SlotClickEvent(ClickEvent evt)
    {
        var target = evt.target as VisualElement;
        string slotNumber = target.name.Split("t").Last(); 
        selectedSlot = Int32.Parse(slotNumber);
        
        if (evt.button == (int)MouseButton.LeftMouse)
        {
            //TODO: USE ITEM EVENT
            contextMenu.visible = false;
        }
        if (evt.button == (int)MouseButton.RightMouse)
        {
            //TODO: TOGGLE CONTEXTUAL MENU
            contextMenu.transform.position = target.transform.position;
            contextMenu.visible = !contextMenu.visible;
           
        }
        
    }

    public void UseItem(ClickEvent evt)
    {
        contextMenu.visible = false;
        Item item = playerInventory.itemsList[selectedSlot];
        switch (item.itemClass)
        {
            case Item.ItemClass.Item:
                
                break;
            
            case Item.ItemClass.Weapon:
                
                break;
            
            case Item.ItemClass.Equipment:
                
                break;
        }
    }
    public void EquipItem(ClickEvent evt)
    {
        contextMenu.visible = false;
        Item item = playerInventory.itemsList[selectedSlot];
        switch (item.itemClass)
        {
            case Item.ItemClass.Item:
                
                break;
            
            case Item.ItemClass.Weapon:
                
                break;
            
            case Item.ItemClass.Equipment:
                
                break;
        }
    }
    public void DropItem(ClickEvent evt)
    {
        contextMenu.visible = false;
        Item item = playerInventory.itemsList[selectedSlot];
        switch (item.itemClass)
        {
            case Item.ItemClass.Item:
                
                break;
            
            case Item.ItemClass.Weapon:
                
                break;
            
            case Item.ItemClass.Equipment:
                
                break;
        }
    }
    public void InspectItem(ClickEvent evt)
    {
        contextMenu.visible = false;
        Item item = playerInventory.itemsList[selectedSlot];
        inspectItemPanel.visible = true;
        inspectItemImage.style.backgroundImage = new StyleBackground(item.itemIcon);
        inspectItemTitle.text = item.itemName;
        inspectItemInfo.text = item.description;
        GetItemStats(item);
    }
    
}