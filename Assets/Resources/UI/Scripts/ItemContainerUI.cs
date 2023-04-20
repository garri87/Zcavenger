using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ItemContainerUI : MonoBehaviour
{

    public UIDocument itemContainerUI;
    public VisualElement root;

    public Label containerLabel;

    public VisualElement containerSlotArea;
    public VisualElement containerSlot;
    public List<VisualElement> containerSlotList;

    public Button closeButton;

    public ItemContainer itemContainer;
    public Inventory playerInventory;

    public int selectedSlot;

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



    private void OnEnable()
    {
        itemContainerUI = GetComponent<UIDocument>();
        root = itemContainerUI.rootVisualElement;

        containerLabel = root.Q<Label>("ContainerName");
        containerSlotArea = root.Q<VisualElement>("SlotsPanel");
        containerSlot = containerSlotArea.Q<VisualElement>("Slot");

        #region Inspect Window

        inspectItemPanel = root.Q<VisualElement>("InspectWindow");
        inspectItemImage = inspectItemPanel.Q<VisualElement>("Image");
        inspectItemTitle = inspectItemPanel.Q<Label>("ItemTitle");
        inspectItemInfo = inspectItemPanel.Q<Label>("ItemInfo");
        statsPanel = inspectItemPanel.Q<VisualElement>("StatsPanel");
        itemStats = inspectItemPanel.Query<VisualElement>("Stat").ToList();

        inspectCloseButton = root.Q<Button>("InspectCloseButton");
        inspectCloseButton.RegisterCallback<ClickEvent, VisualElement>(ToggleVisibility, inspectItemPanel);

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
    }



    public void FillInventoryWithSlots(int capacity)
    {
        for (int i = 0; i < capacity; i++)
        {
            containerSlotArea.Add(containerSlot);
        }

        containerSlotList = containerSlotArea.Query<VisualElement>("Slot").ToList();

        for (int i = 0; i < containerSlotList.Count; i++)
        {
            containerSlotList[i].name = "Slot_" + i;
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

        if (item.itemClass == Item.ItemClass.Weapon || item.itemClass == Item.ItemClass.Outfit)
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

}
