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

        if (item.itemClass == Item.ItemClass.Outfit)
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

        for (int i = 0; i < inventorySlotList.Count; i++)
        {
          inventorySlotList[i].RegisterCallback<ClickEvent>(SlotClickEvent);
          inventorySlotList[i].name = "Slot_" + i;
        }
    }

    public void SlotClickEvent(ClickEvent evt)
    {
        var target = evt.target as VisualElement;
        var slotNumber = target.name.Split("_"); 
        selectedSlot = Int32.Parse(slotNumber[1]);
        
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