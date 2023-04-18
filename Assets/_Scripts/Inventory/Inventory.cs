using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static Item;
using Debug = UnityEngine.Debug;

public class Inventory : MonoBehaviour
{
    public bool showInventory;

    public UIManager uIManager;
    public InventoryUI inventoryUI;
    public InGameOverlayUI inGameOverlayUI;
    public PlayerController _playerController;
    [HideInInspector] public Transform playerWeaponHolder;
    private Animator _playerAnimator;

    public List<Item> itemsList;
    public Dictionary<VisualElement, Item> equipmentItems = new Dictionary<VisualElement, Item>();


    [HideInInspector] public int currentCapacity;
    [HideInInspector] public int maxCapacity = 10;
    [HideInInspector] public int defaultMaxCapacity = 10;

    public bool inventoryFull;

    [HideInInspector] public bool onItem;
    [HideInInspector] public bool onWeaponItem;
    private Transform dropLocationItem;
    private Transform dropLocationWeapon;

    public bool drawWeapon;
    public bool holsterWeapon;

    public enum SelectedWeapon
    {
        Primary,
        Secondary,
        Melee,
        Throwable,
    }
    public SelectedWeapon selectedWeapon;
    public Item primaryWeapon;
    public Item secondaryWeapon;
    public Item meleeWeapon;
    public Item throwableWeapon;
    public Item headEquip;
    public Item vestEquip;
    public Item torsoEquip;
    public Item legsEquip;
    public Item feetEquip;
    public Item backpackEquip;
    private void Awake()
    {
        uIManager = GameManager.Instance.uiManager;
        inventoryUI = uIManager.inventoryUI.GetComponent<InventoryUI>();
        inGameOverlayUI = uIManager.inGameOverlayUI.GetComponent<InGameOverlayUI>();
        _playerAnimator = GetComponent<Animator>();
        playerWeaponHolder = _playerAnimator.GetBoneTransform(HumanBodyBones.RightHand).Find("WeaponHolder");
    }

    void Start()
    {
        inventoryUI.FillInventoryWithSlots(maxCapacity); //Setup the inventory slots in UI

        RefreshInventoryToUI();//Refresh the inventory UI information
    }

    private void FixedUpdate()
    {

    }

    void Update()
    {

        currentCapacity = itemsList.Count;

        if (currentCapacity >= maxCapacity)
        {
            inventoryFull = true;
        }
        else
        {
            inventoryFull = false;
        }

        try
        {
            maxCapacity = backpackEquip.backpackCapacity; //if a backpack is equipped, set its max capacity, else set to default
        }
        catch
        {
            maxCapacity = defaultMaxCapacity;
        }
        inventoryUI.capacityLabel.text = "Capacity: " + currentCapacity + "/" + maxCapacity;

        InventoryToggle();

        if (!inventoryFull)
        {
            if (onItem && Input.GetKeyDown(_playerController.keyAssignments.useKey.keyCode))
            {
                if (dropLocationItem != null)
                {
                    AddItemToInventory(dropLocationItem.GetComponent<Item>());
                    _playerController.grabItem = true;
                    onItem = false;
                }

            }
        }

        _playerAnimator.SetBool("DrawWeapon", drawWeapon);
        _playerAnimator.SetBool("HolsterWeapon", holsterWeapon);

    }


    /// <summary>
    /// Shows inventory current information to UI
    /// </summary>
    public void RefreshInventoryToUI()
    {
        if (itemsList.Count > 0)
        {
            for (int i = 0; i < itemsList.Count; i++)
            {
                Label quantity = inventoryUI.inventorySlotList[i].Q<Label>("SlotQuantity");//Get Quantity Label
                Item item = itemsList[i];
                IStyle style = inventoryUI.inventorySlotList[i].style; // Get the style of the slot
                style.backgroundImage = new StyleBackground(item.itemIcon);//Set the icon image
                quantity.text = item.quantity.ToString();//Set the quantity label
            }
        }

        if (primaryWeapon) inventoryUI.primaryWeaponSlot.style.backgroundImage = new StyleBackground(primaryWeapon.itemIcon);
        if (secondaryWeapon) inventoryUI.secondaryWeaponSlot.style.backgroundImage = new StyleBackground(secondaryWeapon.itemIcon);
        if (meleeWeapon) inventoryUI.meleeWeaponSlot.style.backgroundImage = new StyleBackground(meleeWeapon.itemIcon);
        if (throwableWeapon) inventoryUI.throwableWeaponSlot.style.backgroundImage = new StyleBackground(throwableWeapon.itemIcon);
        if (headEquip) inventoryUI.headEquipSlot.style.backgroundImage = new StyleBackground(headEquip.itemIcon);
        if (vestEquip) inventoryUI.vestEquipSlot.style.backgroundImage = new StyleBackground(vestEquip.itemIcon);
        if (torsoEquip) inventoryUI.torsoEquipSlot.style.backgroundImage = new StyleBackground(torsoEquip.itemIcon);
        if (legsEquip) inventoryUI.legsEquipSlot.style.backgroundImage = new StyleBackground(legsEquip.itemIcon);
        if (feetEquip) inventoryUI.feetEquipSlot.style.backgroundImage = new StyleBackground(feetEquip.itemIcon);
        if (backpackEquip) inventoryUI.backpackEquipSlot.style.backgroundImage = new StyleBackground(backpackEquip.itemIcon);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!inventoryFull)
        {
            if (other.CompareTag("Item"))
            {
                dropLocationItem = other.transform;
                onItem = true;

            }

            if (other.CompareTag("Weapon"))
            {
                dropLocationWeapon = other.transform;
                onWeaponItem = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            dropLocationItem = null;
            onItem = false;
        }

        if (other.CompareTag("Weapon"))
        {
            onWeaponItem = false;
            dropLocationWeapon = null;
        }
    }

    public void InventoryToggle()
    {
        bool enabled = false;

        if (Input.GetKeyDown(KeyAssignments.Instance.inventoryKey.keyCode))
        {
            enabled = !enabled;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            enabled = false;
        }

        if (enabled == true)
        {
            RefreshInventoryToUI();
        }

        if (enabled == false)
        {

        }
        inventoryUI.gameObject.SetActive(enabled);
    }

    /// <summary>
    /// Store a item into the inventory
    /// </summary>
    /// <param name="itemTransform"> Item Component</param>
    public void AddItemToInventory(Item newItem)
    {
        newItem.itemLocation = Item.ItemLocation.Inventory;

        if (newItem.isStackable)
        {
            for (int i = 0; i < itemsList.Count; i++)
            {
                if (itemsList[i].ID == newItem.ID && newItem.itemClass == Item.ItemClass.Item)
                // if item already exists in inventory, stack quantities
                {
                    if (itemsList[i].quantity < itemsList[i].maxStack)
                    {
                        itemsList[i].quantity += newItem.quantity;
                        CheckStackableItem(itemsList[i]);
                        Destroy(newItem.gameObject);
                        return;
                    }
                }
            }
        }
        itemsList.Add(newItem);
        Destroy(newItem.gameObject);
    }


    /// <summary>
    /// Controls that the item does not exceed the maximum stacking amount
    /// </summary>
    /// <param name="item"></param>
    public void CheckStackableItem(Item item)
    {
        if (item.isStackable)

            if (item.quantity > item.maxStack)
            // if the quantity surpasses the max stack capacity, transfer leftover units to the next slot
            {
                Debug.Log("The slot " + " has reached max stackable capacity of "
                    + item.maxStack + " for " + item.itemName);//example = current> 150/100 <max

                int leftOver = item.quantity - item.maxStack;//get the leftover units 150-100
                Debug.Log("remaining units: " + leftOver);//50

                item.quantity = item.maxStack;

                //Create a new item with the remainging quantities
                Item newItem = item;
                newItem.quantity = leftOver;
                if (itemsList.Count < maxCapacity)
                {
                    itemsList.Add(newItem);
                }
                else
                {//drop item to world
                    GenerateItemObject(newItem, transform);
                }
            }
    }

/// <summary>
/// Generates a new item and instantiates in world
/// </summary>
/// <param name="item"></param>
/// <param name="dropLocation"></param>
    public void GenerateItemObject(Item item, Transform dropLocation)
    {
        GameObject newItemGO = new GameObject(item.name);
        Item newItemComp = newItemGO.AddComponent<Item>();
        newItemComp = item;
        newItemComp.itemLocation = Item.ItemLocation.World;
        Instantiate(newItemGO, dropLocation.position, dropLocation.rotation);

    }

    public void ReplaceItem(Item itemToReplace, Item dropLocationEquipment)
    {
        if (!dropLocationEquipment)//if no item is present in the selected dropLocation, assign the item
        {
            dropLocationEquipment = itemToReplace;
        }
        else
        {//Otherwise store old weapon in inventory
            if (!inventoryFull)
            {
                AddItemToInventory(dropLocationEquipment);
                dropLocationEquipment = itemToReplace;
            }
            else //if inventory full, drop the item in world
            {
                GenerateItemObject(dropLocationEquipment, transform);
            }
        }
    }

    

    /// <summary>
    /// Replaces a clothing or weapon slot in the player equipment
    /// </summary>
    /// <param name="newItem"></param>
    public void ChangeEquipment(Item newItem)
    {
        switch (newItem.itemClass)
        {
            case Item.ItemClass.Weapon:
                switch (newItem.weaponClass)
                {
                    case WeaponScriptableObject.WeaponClass.None:
                        break;
                    case WeaponScriptableObject.WeaponClass.Primary:
                        ReplaceItem(newItem, primaryWeapon);

                        break;
                    case WeaponScriptableObject.WeaponClass.Secondary:
                        ReplaceItem(newItem, secondaryWeapon);
                        break;
                    case WeaponScriptableObject.WeaponClass.Melee:
                        ReplaceItem(newItem, meleeWeapon);
                        ;
                        break;
                    case WeaponScriptableObject.WeaponClass.Throwable:
                        ReplaceItem(newItem, throwableWeapon);
                        break;
                    default:
                        // Invalid Weapon Class
                        break;

                }
                break;

            case Item.ItemClass.Outfit:

                switch (newItem.outfitBodyPart)
                {
                    case OutfitScriptableObject.OutfitBodyPart.Head:
                        ReplaceItem(newItem, headEquip);
                        break;
                    case OutfitScriptableObject.OutfitBodyPart.Vest:
                        ReplaceItem(newItem, vestEquip);
                        break;
                    case OutfitScriptableObject.OutfitBodyPart.Torso:
                        ReplaceItem(newItem, torsoEquip);
                        break;
                    case OutfitScriptableObject.OutfitBodyPart.Legs:
                        ReplaceItem(newItem, legsEquip);
                        break;
                    case OutfitScriptableObject.OutfitBodyPart.Feet:
                        ReplaceItem(newItem, feetEquip);
                        break;
                    case OutfitScriptableObject.OutfitBodyPart.Backpack:
                        ReplaceItem(newItem, backpackEquip);
                        break;
                    default:

                        break;
                }

                break;
        }

    }




    public int CheckItemsLeft(int id, int counter)
    {
        //Debug.Log("seeking inventory for ID " + id);

        int total = 0;
        for (int i = 0; i < itemsList.Count; i++)
        {
            if (itemsList[i].ID == id)
            {
                total += itemsList[i].quantity;
            }
        }

        counter = total;
        //Debug.Log("return" + counter);
        return total;
    }


    public void UseItem(Item item){

        if(item.itemClass == ItemClass.Item)
        {
            switch(item.ID)
                {
                    case 1000: //TODO: CREAR BIBLIOTECA DE EFECTOS DE ITEMS
                    break;    
                }

        }
        
        RefreshInventoryToUI();
    }

    public void DropItem(Item item){
       
        GenerateItemObject(item,transform);
        itemsList.Remove(item);
        RefreshInventoryToUI();

    }



    /// <summary>
    /// Updates the bullet counter in the in-game overlay UI
    /// </summary>
    /// <param name="item"></param>
    public void UpdateBulletCounter(Item item)
    {
        Label bulletCounter = inGameOverlayUI.bulletCountLabel;
        if (selectedWeapon == SelectedWeapon.Melee
            || selectedWeapon == SelectedWeapon.Throwable)
        {
            bulletCounter.text = "-/-";
        }
        else
        {
            bulletCounter.text = item.bulletsInMag + "/" + item.totalBullets;
        }

        if (item.bulletsInMag < 5)
        {
            bulletCounter.style.color = Color.red;
        }
        else
        {
            bulletCounter.style.color = Color.white;
        }
    }



}


