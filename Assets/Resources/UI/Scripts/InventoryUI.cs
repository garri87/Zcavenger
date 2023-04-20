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

    public VisualElement outfitSlotsArea;
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

    [Header("Context Menu")] 
    public VisualElement contextMenu;
    public Button useButton;
    public Button equipButton;
    public Button inspectButton;
    public Button throwButton;

    [Header("Inspect & Quick Info Panels")]
    public Label quickInfoLabel;

    public VisualElement inspectItemPanel;
    public VisualElement inspectItemImage;
    public Label inspectItemTitle;
    public Label inspectItemInfo;
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

        outfitSlotsArea = root.Q<VisualElement>("EquipmentSlots");
        primaryWeaponSlot = outfitSlotsArea.Q<VisualElement>("PrimaryWpnSlot");
        secondaryWeaponSlot = outfitSlotsArea.Q<VisualElement>("SecondayWpnSlot");
        meleeWeaponSlot = outfitSlotsArea.Q<VisualElement>("MeleeWpnSlot");
        throwableWeaponSlot = outfitSlotsArea.Q<VisualElement>("ThrowableWpnSlot");

        headEquipSlot = outfitSlotsArea.Q<VisualElement>("HeadEqSlot");
        vestEquipSlot = outfitSlotsArea.Q<VisualElement>("VestEqSlot");
        torsoEquipSlot = outfitSlotsArea.Q<VisualElement>("TorsoEqSlot");
        legsEquipSlot = outfitSlotsArea.Q<VisualElement>("LegsEqSlot");
        feetEquipSlot = outfitSlotsArea.Q<VisualElement>("FeetEqSlot");
        backpackEquipSlot = outfitSlotsArea.Q<VisualElement>("BackPackEqSlot");

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
        
        #endregion

        #region Inspect Window

        inspectItemPanel = root.Q<VisualElement>("InspectWindow");
        inspectItemImage = inspectItemPanel.Q<VisualElement>("Image");
        inspectItemTitle = inspectItemPanel.Q<Label>("ItemTitle");
        inspectItemInfo = inspectItemPanel.Q<Label>("ItemInfo");
        statsPanel = inspectItemPanel.Q<VisualElement>("StatsPanel");
        itemStats = inspectItemPanel.Query<VisualElement>("Stat").ToList();
        
        inspectCloseButton = root.Q<Button>("InspectCloseButton");
        inspectCloseButton.RegisterCallback<ClickEvent, VisualElement>(ToggleVisibility,inspectItemPanel);

        #endregion
        #region Context Menu

        contextMenu = root.Q<VisualElement>("ContextMenu");
        useButton = contextMenu.Q<Button>("UseButton");
        equipButton = contextMenu.Q<Button>("EquipButton");
        inspectButton = contextMenu.Q<Button>("InspectButton");
        throwButton = contextMenu.Q<Button>("ThrowButton");

        useButton.RegisterCallback<ClickEvent>(UseItem);
        equipButton.RegisterCallback<ClickEvent>(EquipItem);
        inspectButton.RegisterCallback<ClickEvent, VisualElement>(ToggleVisibility, inspectItemPanel);
        throwButton.RegisterCallback<ClickEvent>(DropItem);

        #endregion


        #region General

        quickInfoLabel = root.Q<Label>("QuickInfoLabel");
        inventoryCloseButton = root.Q<Button>("InventoryCloseButton");
        inventoryCloseButton.RegisterCallback<ClickEvent, VisualElement>(ToggleVisibility, root);

        #endregion
    }

    public void ToggleVisibility(ClickEvent evt, VisualElement element)
    {
        if (element.visible)
        {
            element.visible = false;
        }
        else
        {
            element.visible = true;
        }
    }

   
  

    public void FillInventoryWithSlots(int capacity)
    {
        for (int i = 0; i < capacity; i++)
        {
            inventorySlotArea.Add(inventorySlot);
        }

        inventorySlotList = inventorySlotArea.Query<VisualElement>("Slot").ToList();

        for (int i = 0; i < inventorySlotList.Count; i++)
        {
            //inventorySlotList[i].RegisterCallback<ClickEvent>(SlotClickEvent);
            inventorySlotList[i].name = "Slot_" + i;
        }
    }

    public void SlotClickEvent(ClickEvent evt, int itemIndex)
    {
        var target = evt.target as VisualElement;
        selectedSlot = itemIndex;
        
        if (evt.button == (int)MouseButton.LeftMouse)
        {
            //TODO: USE ITEM EVENT
            if (contextMenu.visible)
            {
               contextMenu.visible = false; 
            }
        }
        if (evt.button == (int)MouseButton.RightMouse)
        {
            contextMenu.transform.position = target.transform.position; //Set the menu transform to the slot
            contextMenu.visible = !contextMenu.visible;
        }
        inspectButton.UnregisterCallback<ClickEvent, Item>(InspectItem);
        inspectButton.RegisterCallback<ClickEvent, Item>(InspectItem, playerInventory.itemsList[selectedSlot]);
    }

    public void UseItem(ClickEvent evt)
    {
        contextMenu.visible = false;
        Item item = playerInventory.itemsList[selectedSlot];
        switch (item.itemClass)
        {
            case Item.ItemClass.Item:
                playerInventory.UseItem(playerInventory.itemsList[selectedSlot]);
                break;
            
            case Item.ItemClass.Weapon:
                
                break;
            
            case Item.ItemClass.Outfit:
                
                break;
        }
    }
    public void EquipItem(ClickEvent evt)
    {
        contextMenu.visible = false;
        Item item = playerInventory.itemsList[selectedSlot];
        
        if(item.itemClass == Item.ItemClass.Weapon || item.itemClass == Item.ItemClass.Outfit)
        {
            playerInventory.ChangeEquipment(item);
        }
    }
    public void DropItem(ClickEvent evt)
    {
        contextMenu.visible = false;
        Item item = playerInventory.itemsList[selectedSlot];
        playerInventory.DropItem(item);

    }
    public void InspectItem(ClickEvent evt, Item item)
    {
        contextMenu.visible = false;
        GetItemStats(item);
    }



    /// <summary>
    /// Get the item's stats and generate bars for the ui
    /// </summary>
    /// <param name="item"></param>
    public void GetItemStats(Item item)
    {
        inspectItemImage.style.backgroundImage = new StyleBackground(item.itemIcon);
        inspectItemTitle.text = item.itemName;
        inspectItemInfo.text = item.description;

        VisualElement statTemplate = statsPanel.Q<VisualElement>("Stat");

        statsPanel.Clear();//Clear the area before generating new stats

        if (item.itemClass == Item.ItemClass.Item)
        {
            CreateStatBar(statTemplate, 0, "Health Restore", item.healthRestore);
            CreateStatBar(statTemplate, 1, "Food Restore", item.foodRestore);
            CreateStatBar(statTemplate, 2, "Water Restore", item.waterRestore);
            itemStats = inspectItemPanel.Query<VisualElement>("Stat").ToList();
        }

        if (item.itemClass == Item.ItemClass.Weapon)
        {
            CreateStatBar(statTemplate, 0, "Damage", item.damage);
            CreateStatBar(statTemplate, 1, "magazine Cap", item.magazineCap);
            CreateStatBar(statTemplate, 2, "Fire Rate", Mathf.RoundToInt(item.fireRate));
            CreateStatBar(statTemplate, 3, "Recoil", Mathf.RoundToInt(item.recoilMaxRotation));
            itemStats = inspectItemPanel.Query<VisualElement>("Stat").ToList();
        }

        if (item.itemClass == Item.ItemClass.Outfit)
        {
            CreateStatBar(statTemplate, 0, "Defense", item.defense);
            itemStats = inspectItemPanel.Query<VisualElement>("Stat").ToList();
        }
    }

    public void CreateStatBar(VisualElement template, int order, string name, int value)
    {
        VisualElement newStat = template;
        newStat.name = "Stat" + order;
        statsPanel.Add(newStat);
        VisualElement stat = statsPanel.Q<VisualElement>(newStat.name);
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
}