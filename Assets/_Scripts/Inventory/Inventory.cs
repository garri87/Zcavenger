using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static Item;
using Debug = UnityEngine.Debug;

public class Inventory : MonoBehaviour
{
    public bool showInventory = false;

    public UIManager uIManager;
    public InventoryUI inventoryUI;
    public InGameOverlayUI inGameOverlayUI;
    public PlayerController _playerController;
    [HideInInspector] public Transform playerWeaponHolder;
    private Animator _playerAnimator;

    public List<Item> itemsList;
    public Dictionary<VisualElement, Item> equipmentItems = new Dictionary<VisualElement, Item>();


    [HideInInspector] public int currentCapacity;
    public int maxCapacity = 10;
    public int defaultMaxCapacity = 10;

    public bool inventoryFull;

    [HideInInspector] public bool onItem;
    [HideInInspector] public bool onWeaponItem;
    private Transform collectItemTransform;

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

    #region Player Equipment Slots

    public Item equippedPrimaryWeapon;
    public Item equippedSecondaryWeapon;
    public Item equippedMeleeWeapon;
    public Item equippedThrowableWeapon;

    public Item equippedHeadOutfit;
    public Item equippedVestOutfit;
    public Item equippedTorsoOutfit;
    public Item equippedLegsOutfit;
    public Item equippedFeetOutfit;
    public Item equippedBackpackOutfit;
    #endregion

    public GameObject itemCollectiblePrefab;

    private void Awake()
    {
        _playerController = GetComponent<PlayerController>();
        uIManager = GameManager.Instance.uiManager;
        inventoryUI = uIManager.inventoryUI.GetComponent<InventoryUI>();
        inGameOverlayUI = uIManager.inGameOverlayUI.GetComponent<InGameOverlayUI>();
        _playerAnimator = GetComponent<Animator>();
        playerWeaponHolder = _playerAnimator.GetBoneTransform(HumanBodyBones.RightHand).Find("WeaponHolder");
    }

    void Start()
    {
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
            maxCapacity = equippedBackpackOutfit.backpackCapacity; //if a backpack is equipped, set its max capacity, else set to default
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
                if (collectItemTransform != null)
                {
                    AddItemToInventory(collectItemTransform.GetComponent<Item>());
                    _playerController.grabItem = true;
                    onItem = false;
                }

            }
        }

        _playerAnimator.SetBool("DrawWeapon", drawWeapon);
        _playerAnimator.SetBool("HolsterWeapon", holsterWeapon);

    }


    /// <summary>
    /// Updates the inventory capacity and stored items information to UI
    /// </summary>
    public void RefreshInventoryToUI()
    {
        inventoryUI.FillInventoryWithSlots(maxCapacity);
                
        if (itemsList.Count > 0)
        {
            for (int i = 0; i < itemsList.Count; i++)
            {
                Label quantity = inventoryUI.inventorySlotList[i].Q<Label>("SlotQuantity");//Get Quantity Label
                Item item = itemsList[i];
                IStyle style = inventoryUI.inventorySlotList[i].style; // Get the style of the slot
                style.backgroundImage = new StyleBackground(item.itemIcon);//Set the icon image
                quantity.text = item.quantity.ToString();//Set the quantity label
                inventoryUI.inventorySlotList[i].UnregisterCallback<MouseDownEvent, int>(inventoryUI.SlotClickEvent,TrickleDown.NoTrickleDown);
                inventoryUI.inventorySlotList[i].RegisterCallback<MouseDownEvent, int>(inventoryUI.SlotClickEvent, i);
            }
        }

        

        if (equippedPrimaryWeapon) inventoryUI.primaryWeaponSlot.style.backgroundImage = new StyleBackground(equippedPrimaryWeapon.itemIcon);
        if (equippedSecondaryWeapon) inventoryUI.secondaryWeaponSlot.style.backgroundImage = new StyleBackground(equippedSecondaryWeapon.itemIcon);
        if (equippedMeleeWeapon) inventoryUI.meleeWeaponSlot.style.backgroundImage = new StyleBackground(equippedMeleeWeapon.itemIcon);
        if (equippedThrowableWeapon) inventoryUI.throwableWeaponSlot.style.backgroundImage = new StyleBackground(equippedThrowableWeapon.itemIcon);
        if (equippedHeadOutfit) inventoryUI.headEquipSlot.style.backgroundImage = new StyleBackground(equippedHeadOutfit.itemIcon);
        if (equippedVestOutfit) inventoryUI.vestEquipSlot.style.backgroundImage = new StyleBackground(equippedVestOutfit.itemIcon);
        if (equippedTorsoOutfit) inventoryUI.torsoEquipSlot.style.backgroundImage = new StyleBackground(equippedTorsoOutfit.itemIcon);
        if (equippedLegsOutfit) inventoryUI.legsEquipSlot.style.backgroundImage = new StyleBackground(equippedLegsOutfit.itemIcon);
        if (equippedFeetOutfit) inventoryUI.feetEquipSlot.style.backgroundImage = new StyleBackground(equippedFeetOutfit.itemIcon);
        if (equippedBackpackOutfit) inventoryUI.backpackEquipSlot.style.backgroundImage = new StyleBackground(equippedBackpackOutfit.itemIcon);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!inventoryFull)
        { 
            switch (other.tag)
            {
                case "Item":
                case "Weapon":

                    collectItemTransform = other.transform;
                    onItem = true;

                    break;


                default:
                    break;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        switch (other.tag)
        {
            case "Item":
            case "Weapon":

                collectItemTransform = null;
                onItem = false;

                break;

          
            default:
                break;
        }

       

    
    }

    public void InventoryToggle()
    {
        
        if (Input.GetKeyDown(GameManager.Instance._keyAssignments.inventoryKey.keyCode))
        {
            showInventory = !showInventory;
            if (showInventory)
            {
                RefreshInventoryToUI();
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            showInventory = false;
        }

        uIManager.ToggleUI(uIManager.inventoryUI, showInventory);

    }

    /// <summary>
    /// Store a item into the inventory
    /// </summary>
    /// <param name="itemTransform"> Item Component</param>
    public void AddItemToInventory(Item newItem)
    {
        if (newItem.isStackable)
        {
            for (int i = 0; i < itemsList.Count; i++)
            {
                Item item = itemsList[i];
                if (item.ID == newItem.ID )
                // if item already exists in inventory, stack quantities
                {
                    
                        item.quantity += newItem.quantity;
                        CheckStackableItem(item);
                        newItem.itemLocation = Item.ItemLocation.Inventory;

                    //Destroy(item.gameObject);
                    return;
                    
                }
            }
        }
        if (!inventoryFull)
        {
            newItem.itemLocation = Item.ItemLocation.Inventory;
            newItem.InitItem();
            itemsList.Add(newItem);

         //   Destroy(item.gameObject);
        }
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
                if (!inventoryFull)
                {
                    AddItemToInventory(newItem);
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
    public GameObject GenerateItemObject(Item item, Transform dropLocation)
    {
        GameObject newItemGO = Instantiate(itemCollectiblePrefab, dropLocation.position + Vector3.up, dropLocation.rotation);
        Item newItemComp = newItemGO.GetComponent<Item>();
        newItemComp = item; 
        newItemComp.itemLocation = Item.ItemLocation.World;
        newItemComp.InitItem();
        return newItemGO;
    }
       

    /// <summary>
    /// Replaces a clothing or weapon slot in the player equipment
    /// </summary>
    /// <param name="newItem"> Item to equip </param>
    public void ChangeEquipment(Item newItem)//TODO: MEJORAR
    {
        switch (newItem.itemClass)
        {
            case Item.ItemClass.Weapon:
                switch (newItem.weaponClass)
                {
                    case WeaponScriptableObject.WeaponClass.Primary:
                        ReplaceItem(newItem, equippedPrimaryWeapon);
                        break;
                    case WeaponScriptableObject.WeaponClass.Secondary:
                        ReplaceItem(newItem, equippedSecondaryWeapon);

                        break;
                    case WeaponScriptableObject.WeaponClass.Melee:
                        ReplaceItem(newItem, equippedMeleeWeapon);
                        ;
                        break;
                    case WeaponScriptableObject.WeaponClass.Throwable:
                        ReplaceItem(newItem, equippedThrowableWeapon);
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
                        ReplaceItem(newItem, equippedHeadOutfit);
                        break;
                    case OutfitScriptableObject.OutfitBodyPart.Vest:
                        ReplaceItem(newItem, equippedVestOutfit);
                        break;
                    case OutfitScriptableObject.OutfitBodyPart.Torso:
                        ReplaceItem(newItem, equippedTorsoOutfit);
                        break;
                    case OutfitScriptableObject.OutfitBodyPart.Legs:
                        ReplaceItem(newItem, equippedLegsOutfit);
                        break;
                    case OutfitScriptableObject.OutfitBodyPart.Feet:
                        ReplaceItem(newItem, equippedFeetOutfit);
                        break;
                    case OutfitScriptableObject.OutfitBodyPart.Backpack:
                        ReplaceItem(newItem, equippedBackpackOutfit);
                        break;
                    default:

                        break;
                }

                break;
        }

    }

    public void ReplaceItem(Item newItem, Item playerEquipmentSlotItem)
    {
        if (playerEquipmentSlotItem)
        {//if equipment slot was not empty, try to store old weapon in inventory
         
            if (!inventoryFull)
            {
                AddItemToInventory(playerEquipmentSlotItem);
            }
            else //if inventory full, drop the item in world
            {
                    GenerateItemObject(playerEquipmentSlotItem, transform);
            }
        }

        playerEquipmentSlotItem = newItem;

        playerEquipmentSlotItem.GetScriptableObject();//Update item information
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


    public void UseItem(int itemListIndex){

        Item item = itemsList[itemListIndex];
        if(item.itemClass == ItemClass.Item)
        {
            switch(item.ID)
                {
                    case 000: //TODO: CREAR BIBLIOTECA DE EFECTOS DE ITEMS
                    break;    
                }

        }
        
        RefreshInventoryToUI();
    }

    public void DropItem(int itemListIndex){
       
        GenerateItemObject(itemsList[itemListIndex],transform);
        itemsList.RemoveAt(itemListIndex);
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


