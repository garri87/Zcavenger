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
    public Transform playerWeaponHolder;
    private Animator _playerAnimator;

    public List<GameObject> itemsList;
    public GameObject inventoryGo;

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

    public SkinnedMeshRenderer playerModelRenderer;


    #endregion

    public GameObject itemCollectiblePrefab;



    private void Awake()
    {
        _playerController = GetComponent<PlayerController>();
        inventoryGo = transform.Find("Inventory").gameObject;
        uIManager = GameManager.Instance.uiManager;
        inventoryUI = uIManager.inventoryUI.GetComponent<InventoryUI>();
        inGameOverlayUI = uIManager.inGameOverlayUI.GetComponent<InGameOverlayUI>();
        _playerAnimator = GetComponent<Animator>();
        playerWeaponHolder = transform.Find("WeaponHolder");
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
        try
        {
            maxCapacity = equippedBackpackOutfit.backpackCapacity; //if a backpack is equipped, set its max capacity, else set to default

        }
        catch
        {
            maxCapacity = defaultMaxCapacity;
        }

        currentCapacity = itemsList.Count;

        if (currentCapacity >= maxCapacity)
        {
            inventoryFull = true;
            //Remove items that exceeds the max capacity
            if (currentCapacity > maxCapacity)
            {
                for (int i = maxCapacity + 1; i < currentCapacity; i++)
                {
                    DropItem(itemsList[i]);
                }
            }
        }
        else
        {
            inventoryFull = false;
        }

        inventoryUI.capacityLabel.text = "Capacity: " + currentCapacity + "/" + maxCapacity;

        InventoryToggle();

        if (!inventoryFull)
        {
            if (onItem && Input.GetKeyDown(GameManager.Instance._keyAssignments.useKey.keyCode))
            {
                if (collectItemTransform)
                {
                    AddItemToInventory(collectItemTransform.gameObject);
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
                Item item = itemsList[i].GetComponent<Item>();
                IStyle style = inventoryUI.inventorySlotList[i].style; // Get the style of the slot
                style.backgroundImage = new StyleBackground(item.itemIcon);//Set the icon image
                quantity.text = item.quantity.ToString();//Set the quantity label
                inventoryUI.inventorySlotList[i].UnregisterCallback<MouseDownEvent, int>(inventoryUI.SlotClickEvent, TrickleDown.NoTrickleDown);
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

    /// <summary>
    /// Manages the open/close inventory actions 
    /// </summary>
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
    public void AddItemToInventory(GameObject newItemGO)
    {
        Item item = newItemGO.GetComponent<Item>();

        if (item.isStackable)
        {
            for (int i = 0; i < itemsList.Count; i++)
            {
                Item inventoryItem = itemsList[i].GetComponent<Item>();

                if (inventoryItem.ID == item.ID && inventoryItem.quantity < inventoryItem.maxStack)
                // if item already exists in inventory, stack quantities
                {
                    inventoryItem.quantity += item.quantity;
                    CheckStackableItem(inventoryItem); //Check if quanity exceeds max stack value
                    Destroy(newItemGO);//Destroy the world item object
                    return;

                }
            }
        }
        if (!inventoryFull)
        {

            itemsList.Add(newItemGO);
            item.itemLocation = ItemLocation.Inventory;
        }
        RefreshInventoryToUI();
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

                int leftOver = item.quantity - item.maxStack;//extract the leftover units 150-100
                Debug.Log("remaining units: " + leftOver);//50

                item.quantity = item.maxStack;

                //Create a new item with the remainging quantities
                GameObject newItemGO = GenerateItemObject(item.scriptableObject, leftOver);

                if (!inventoryFull)
                {
                    AddItemToInventory(newItemGO);
                }
                else
                {
                    Debug.Log("Inventory is full");
                    //drop item to world
                }
            }
    }

    /// <summary>
    /// Generates a new item and instantiates in desired location
    /// </summary>
    /// <param name="scriptableObject"></param>
    /// <param name="quantity"></param>
    /// <param name="dropLocation"></param>
    /// <returns></returns>
    public GameObject GenerateItemObject(ScriptableObject scriptableObject, int quantity = 1, Item.ItemLocation dropLocation = ItemLocation.World)
    {

        GameObject newItemGO = Instantiate(itemCollectiblePrefab, transform.position, transform.rotation);
        Item newItemComp = newItemGO.GetComponent<Item>();
        newItemComp.scriptableObject = scriptableObject;
        newItemComp.itemLocation = dropLocation;
        newItemComp.quantity = quantity;
        newItemComp.InitItem();
        return newItemGO;
    }


    /// <summary>
    /// Equips a clothing or weapon to the player
    /// </summary>
    /// <param name="itemGO"> Item gameobject to equip </param>
    public void EquipItem(GameObject itemGO)//TODO: MEJORAR
    {
        Item newItem = itemGO.GetComponent<Item>();

        switch (newItem.itemClass)
        {
            case Item.ItemClass.Weapon:
                switch (newItem.weaponClass)
                {
                    case WeaponScriptableObject.WeaponClass.Primary:
                        equippedPrimaryWeapon = newItem;
                        break;
                    case WeaponScriptableObject.WeaponClass.Secondary:
                        equippedSecondaryWeapon = newItem;
                        break;
                    case WeaponScriptableObject.WeaponClass.Melee:
                        equippedMeleeWeapon = newItem;
                        break;
                    case WeaponScriptableObject.WeaponClass.Throwable:
                        equippedThrowableWeapon = newItem;
                        break;
                    default:
                        // Invalid Weapon Class
                        break;
                }
                break;

            case Item.ItemClass.Outfit:

                if (!newItem.itemEquipped)
                {
                    RenderOutfit(itemGO);
                }
                else
                {
                    UnrenderOutfit(itemGO);
                }
                switch (newItem.equipmentSlot)
                {

                    case EquipmentSlot.Head:
                        equippedHeadOutfit = newItem;
                        break;
                    case EquipmentSlot.Torso:
                        equippedTorsoOutfit = newItem;


                        break;
                    case EquipmentSlot.Vest:
                        equippedVestOutfit = newItem;


                        break;
                    case EquipmentSlot.Legs:
                        equippedLegsOutfit = newItem;


                        break;
                    case EquipmentSlot.Feet:
                        equippedFeetOutfit = newItem;

                        break;
                    case EquipmentSlot.Backpack:
                        equippedBackpackOutfit = newItem;

                        break;
                    default:
                        break;
                }
            break;
        }
        newItem.itemLocation = ItemLocation.Player;

        RefreshInventoryToUI();

    }



    public int CheckItemsLeft(int id, int counter)
    {
        //Debug.Log("seeking inventory for ID " + id);

        int total = 0;
        for (int i = 0; i < itemsList.Count; i++)
        {
            Item item = itemsList[i].GetComponent<Item>();
            if (item.ID == id)
            {
                total += item.quantity;
            }
        }

        counter = total;
        //Debug.Log("return" + counter);
        return total;
    }


    public void UseItem(GameObject itemGO)
    {


        Item item = itemGO.GetComponent<Item>();
        if (item.itemClass == ItemClass.Item)
        {
            switch (item.ID)
            {
                case 000: //TODO: CREAR BIBLIOTECA DE EFECTOS DE ITEMS
                    break;
            }

        }

        RefreshInventoryToUI();
    }

    public void DropItem(GameObject droppedGO)
    {

        Item droppedItem = droppedGO.GetComponent<Item>();

        Debug.Log("Dropping item " + droppedItem.name);
        droppedItem.itemLocation = ItemLocation.World;
        droppedGO.transform.position = transform.position + Vector3.up;
        droppedGO.transform.parent = null;
        itemsList.Remove(droppedGO);

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


/// <summary>
/// Renders a item mesh over the player model
/// </summary>
/// <param name="itemGO"> Item GameObject </param>
    public void RenderOutfit(GameObject itemGO)
    {
        Item item = itemGO.GetComponent<Item>();
        SkinnedMeshRenderer itemRenderer = item.itemModelGO.GetComponent<SkinnedMeshRenderer>();
        itemRenderer.bones = playerModelRenderer.bones;
        itemRenderer.rootBone = playerModelRenderer.rootBone;
    }


/// <summary>
/// Disables the item rendering
/// </summary>
/// <param name="itemGO">Item GameObject</param>
    public void UnrenderOutfit(GameObject itemGO)
    {
        Item item = itemGO.GetComponent<Item>();
        SkinnedMeshRenderer itemRenderer = item.itemModelGO.GetComponent<SkinnedMeshRenderer>();
        itemRenderer.bones = null;
        itemRenderer.rootBone = null;
    }

}


