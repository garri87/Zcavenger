using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static Item;
using System.Linq;
using static UnityEditor.Progress;
using Autodesk.Fbx;

public class Inventory : MonoBehaviour
{
    #region UI

    public bool showInventory = false;

    public UIManager uIManager;
    public InventoryUI inventoryUI;
    public InGameOverlayUI inGameOverlayUI;

    #endregion

    #region PLAYER REFERENCES

    public PlayerController _playerController;
    public Transform playerWeaponHolder;
    private Animator _playerAnimator;
    public GameObject inventoryGo;
    public SkinnedMeshRenderer playerModelRenderer;

    #endregion

    #region INVENTORY LIST AND SETTINGS

    public List<GameObject> itemsList;

    public int maxCapacity = 10;
    public int defaultMaxCapacity = 10;
    [HideInInspector] public int currentCapacity;
    public bool inventoryFull;

    #endregion

    #region COLLECTIBLE REFERENCES

    [HideInInspector] public bool onItem; //Is player over a collectible?
    private Transform collectItemTransform; // Transform in world of current collectible
    public GameObject itemCollectiblePrefab; //Collectible prefab for instantiate new items

    #endregion

    #region EQUIPMENT VARIABLES

    public Item drawnWeaponItem; //Item Component of current drawn weapon
    public bool drawWeapon; //Animator trigger for draw weapons
    public bool holsterWeapon; //Animator trigger for holster weapons

    public enum SelectedWeapon //Current drawn weapon class
    {
        Primary,
        Secondary,
        Melee,
        Throwable,
        None,
    }

    public SelectedWeapon selectedWeapon;
    public Dictionary<SelectedWeapon, Item> selectedWeapons;
    public Dictionary<WeaponScriptableObject.WeaponClass, Item> weaponsClasses;
    public IKAimer iKAimer;

    #endregion

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
    public Dictionary<EquipmentSlot, Item> equipmentSlots;

    #endregion


    private void Awake()
    {
        _playerController = GetComponent<PlayerController>();
        inventoryGo = transform.Find("Inventory").gameObject;
        _playerAnimator = GetComponent<Animator>();
        //playerWeaponHolder = transform.Find("WeaponHolder");
        iKAimer = GetComponent<IKAimer>();
        uIManager = GameManager.Instance.uiManager;
        inventoryUI = uIManager.inventoryUI.GetComponent<InventoryUI>();
        inGameOverlayUI = uIManager.inGameOverlayUI.GetComponent<InGameOverlayUI>();


        selectedWeapons = new Dictionary<SelectedWeapon, Item>
        {
            { SelectedWeapon.Primary, equippedPrimaryWeapon },
            { SelectedWeapon.Secondary, equippedSecondaryWeapon },
            { SelectedWeapon.Melee, equippedMeleeWeapon },
            { SelectedWeapon.Throwable, equippedThrowableWeapon }
        };

        weaponsClasses = new Dictionary<WeaponScriptableObject.WeaponClass, Item>
        {
            { WeaponScriptableObject.WeaponClass.Primary, equippedPrimaryWeapon },
            { WeaponScriptableObject.WeaponClass.Secondary, equippedSecondaryWeapon },
            { WeaponScriptableObject.WeaponClass.Melee, equippedMeleeWeapon },
            { WeaponScriptableObject.WeaponClass.Throwable, equippedThrowableWeapon },
        };

        equipmentSlots = new Dictionary<EquipmentSlot, Item>
        {
            { EquipmentSlot.Head, equippedHeadOutfit },
            { EquipmentSlot.Torso, equippedTorsoOutfit },
            { EquipmentSlot.Vest, equippedVestOutfit },
            { EquipmentSlot.Legs, equippedLegsOutfit },
            { EquipmentSlot.Feet, equippedFeetOutfit }
        };
    }

    void Start()
    {
        RefreshInventoryToUI(); //Refresh the inventory UI information
    }

    private void FixedUpdate()
    {
    }

    void Update()
    {
        playerWeaponHolder.forward = playerWeaponHolder.parent.up;
        InventoryToggle();

        if (equippedBackpackOutfit)
        {
            maxCapacity =
                equippedBackpackOutfit
                    .backpackCapacity; //if a backpack is equipped, set its max capacity, else set to default
        }
        else
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
                    DropItem(itemsList[i],ItemLocation.World);
                }
            }
        }
        else
        {
            inventoryFull = false;
        }

        inventoryUI.capacityLabel.text = "Capacity: " + currentCapacity + "/" + maxCapacity;


        if (onItem && Input.GetKeyDown(GameManager.Instance._keyAssignments.useKey.keyCode))
        {
            if (!inventoryFull)
            {
                if (collectItemTransform)
                {
                    AddItemToInventory(collectItemTransform.gameObject);
                    _playerController.grabbingItem = true;
                    onItem = false;
                }
            }
            else
            {
                Debug.Log("Inventory Full!");
            }
        }

        CheckEquippedWeapon();

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
                Label quantity = inventoryUI.inventorySlotList[i].Q<Label>("SlotQuantity"); //Get Quantity Label
                Item item = itemsList[i].GetComponent<Item>();
                IStyle style = inventoryUI.inventorySlotList[i].style; // Get the style of the slot
                style.backgroundImage = new StyleBackground(item.itemIcon); //Set the icon image
                quantity.text = item.quantity.ToString(); //Set the quantity label
                inventoryUI.inventorySlotList[i]
                    .UnregisterCallback<MouseDownEvent, int>(inventoryUI.SlotClickEvent, TrickleDown.NoTrickleDown);
                inventoryUI.inventorySlotList[i].RegisterCallback<MouseDownEvent, int>(inventoryUI.SlotClickEvent, i);
                if (item.itemEquipped)
                {
                    style.backgroundColor = new StyleColor(new Color(0.17f, 0.74f, 0.17f, 0.6f));
                }
                else
                {
                    style.backgroundColor = new StyleColor(Color.black);
                }
            }
        }

        inventoryUI.primaryWeaponSlot.style.backgroundImage = (equippedPrimaryWeapon)
            ? new StyleBackground(equippedPrimaryWeapon.itemIcon)
            : new StyleBackground(inventoryUI.emptySlotImg);

        inventoryUI.secondaryWeaponSlot.style.backgroundImage = (equippedSecondaryWeapon)
            ? new StyleBackground(equippedSecondaryWeapon.itemIcon)
            : new StyleBackground(inventoryUI.emptySlotImg);

        inventoryUI.meleeWeaponSlot.style.backgroundImage = (equippedMeleeWeapon)
            ? new StyleBackground(equippedMeleeWeapon.itemIcon)
            : new StyleBackground(inventoryUI.emptySlotImg);

        inventoryUI.throwableWeaponSlot.style.backgroundImage = (equippedThrowableWeapon)
            ? new StyleBackground(equippedThrowableWeapon.itemIcon)
            : new StyleBackground(inventoryUI.emptySlotImg);

        inventoryUI.headEquipSlot.style.backgroundImage = (equippedHeadOutfit)
            ? new StyleBackground(equippedHeadOutfit.itemIcon)
            : new StyleBackground(inventoryUI.emptySlotImg);

        inventoryUI.vestEquipSlot.style.backgroundImage = (equippedVestOutfit)
            ? new StyleBackground(equippedVestOutfit.itemIcon)
            : new StyleBackground(inventoryUI.emptySlotImg);

        inventoryUI.torsoEquipSlot.style.backgroundImage = (equippedTorsoOutfit)
            ? new StyleBackground(equippedTorsoOutfit.itemIcon)
            : new StyleBackground(inventoryUI.emptySlotImg);

        inventoryUI.legsEquipSlot.style.backgroundImage = (equippedLegsOutfit)
            ? new StyleBackground(equippedLegsOutfit.itemIcon)
            : new StyleBackground(inventoryUI.emptySlotImg);

        inventoryUI.feetEquipSlot.style.backgroundImage = (equippedFeetOutfit)
            ? new StyleBackground(equippedFeetOutfit.itemIcon)
            : new StyleBackground(inventoryUI.emptySlotImg);

        inventoryUI.backpackEquipSlot.style.backgroundImage = (equippedBackpackOutfit)
            ? new StyleBackground(equippedBackpackOutfit.itemIcon)
            : new StyleBackground(inventoryUI.emptySlotImg);
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
    /// <param name="newItemGO">GameObject that contains item component+</param>
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
                    Destroy(newItemGO); //Destroy the world item object
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
                                      + item.maxStack + " for " + item.itemName); //example = current> 150/100 <max

                int leftOver = item.quantity - item.maxStack; //extract the leftover units 150-100
                Debug.Log("remaining units: " + leftOver); //50

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

    public void CheckEquippedWeapon()
    {
        bool[] equippedWeapons =
        {
            equippedPrimaryWeapon,
            equippedSecondaryWeapon,
            equippedMeleeWeapon,
            equippedThrowableWeapon
        };

        if (equippedWeapons.All(weapon => weapon == false))
        {
            selectedWeapon = SelectedWeapon.None;
        }
    }

    /// <summary>
    /// Generates a new item and instantiates in desired location
    /// </summary>
    /// <param name="scriptableObject"></param>
    /// <param name="quantity"></param>
    /// <param name="dropLocation"></param>
    /// <returns></returns>
    public GameObject GenerateItemObject(ScriptableObject scriptableObject, int quantity = 1,
        Item.ItemLocation dropLocation = ItemLocation.World)
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
    /// Equips a clothing or weapon to the player. Unequip if item is already equipped
    /// </summary>
    /// <param name="itemGO"> Item gameobject to equip </param>
    public void EquipItem(GameObject itemGO)
    {
        Item item = itemGO.GetComponent<Item>();
        if (item.itemEquipped)
        {
            UnequipItem(itemGO);
            item.itemLocation = ItemLocation.Inventory;
        }
        else
        {
            switch (item.itemClass)
            {
                case Item.ItemClass.Weapon:

                    switch (item.weaponClass)
                    {
                        case WeaponScriptableObject.WeaponClass.Primary:
                            equippedPrimaryWeapon = item;
                            break;
                        case WeaponScriptableObject.WeaponClass.Secondary:

                            equippedSecondaryWeapon = item;

                            break;
                        case WeaponScriptableObject.WeaponClass.Melee:

                            equippedMeleeWeapon = item;

                            break;
                        case WeaponScriptableObject.WeaponClass.Throwable:

                            equippedThrowableWeapon = item;

                            break;
                        
                        case WeaponScriptableObject.WeaponClass.None:
                            break;
                        
                        default:
                            break;
                    }
                    
                    

                    if (!drawnWeaponItem) //AutoDraw the weapon if no weapon in handas
                    {
                        DrawWeapon(item, true);
                    }
                    else
                    {
                        if (drawnWeaponItem.weaponClass == item.weaponClass)
                        {
                            DrawWeapon(drawnWeaponItem, false);
                            DrawWeapon(item, true);
                        }
                    }


                    break;

                case Item.ItemClass.Outfit:

                    switch (item.equipmentSlot)
                    {
                        case EquipmentSlot.Head:
                            equippedHeadOutfit = SwapItem(equippedHeadOutfit, item);
                            break;

                        case EquipmentSlot.Vest:
                            equippedVestOutfit = SwapItem(equippedVestOutfit, item);

                            break;

                        case EquipmentSlot.Torso:
                            equippedTorsoOutfit = SwapItem(equippedTorsoOutfit, item);

                            break;

                        case EquipmentSlot.Legs:
                            equippedLegsOutfit = SwapItem(equippedLegsOutfit, item);

                            break;

                        case EquipmentSlot.Feet:
                            equippedFeetOutfit = SwapItem(equippedFeetOutfit, item);

                            break;

                        case EquipmentSlot.Backpack:
                            equippedBackpackOutfit = SwapItem(equippedBackpackOutfit, item);

                            break;

                        default:

                            break;
                    }

                    break;
            }
        }
        RefreshInventoryToUI();
        
        Item SwapItem(Item oldItem, Item newItem)
        {
            
            if (oldItem != null || newItem == oldItem) 
                // if target item slot is occupied or is the same item unequip current item
            {
                oldItem.itemEquipped = false;
                oldItem.itemLocation = ItemLocation.Inventory;

                if (oldItem.itemClass == ItemClass.Outfit)
                {
                    UnrenderOutfit(oldItem.gameObject);
                }
                else
                {
                    DrawWeapon(oldItem, false);
                }

                if (newItem == oldItem)
                {
                    newItem = null;

                    Debug.Log("Item Unequipped");
                }
                else
                {
                    newItem.itemEquipped = true;
                    newItem.itemLocation = ItemLocation.Player;

                    if (newItem.itemClass == ItemClass.Outfit)
                    {
                        RenderOutfit(newItem.gameObject);
                    }
                    else
                    {
                        iKAimer.ConstraintLeftHand(newItem);
                    }

                    Debug.Log("Item Equipped");
                }
            }
            else
            {
                newItem.itemEquipped = true;
                newItem.itemLocation = ItemLocation.Player;

                if (newItem.itemClass == ItemClass.Outfit)
                {
                    RenderOutfit(newItem.gameObject);
                }
                else
                {
                    iKAimer.ConstraintLeftHand(newItem);
                }
            }

            return newItem;
        }
    }

    public void UnequipItem(GameObject itemGO)
    {
       Item item = itemGO.GetComponent<Item>();
            if (item.itemClass == ItemClass.Weapon)
            {
                if (item.weaponClass != WeaponScriptableObject.WeaponClass.Throwable)
                {
                    DrawWeapon(item, false);
                }
                switch (item.weaponClass)
                {
                    case WeaponScriptableObject.WeaponClass.Primary:
                        equippedPrimaryWeapon = null;
                        break;
                    
                    case WeaponScriptableObject.WeaponClass.Secondary:
                        equippedSecondaryWeapon = null;
                        break;
                    
                    case WeaponScriptableObject.WeaponClass.Melee:
                        equippedMeleeWeapon = null;
                        break;
                    
                    case WeaponScriptableObject.WeaponClass.Throwable:
                        equippedThrowableWeapon = null;

                        break;
                }
            }else if (item.itemClass == ItemClass.Outfit)
            {
                UnrenderOutfit(item.gameObject);
                switch (item.equipmentSlot)
                {
                    case EquipmentSlot.Head:
                        equippedHeadOutfit = null;
                        break;

                    case EquipmentSlot.Vest:
                        equippedVestOutfit = null;
                        break;

                    case EquipmentSlot.Torso:
                        equippedTorsoOutfit = null;
                        break;

                    case EquipmentSlot.Legs:
                        equippedLegsOutfit = null;
                        break;

                    case EquipmentSlot.Feet:
                        equippedFeetOutfit = null;
                        break;

                    case EquipmentSlot.Backpack:
                        equippedBackpackOutfit = null;
                        break;

                    default:

                        break;
                }
            }
            item.itemEquipped = false;
            RefreshInventoryToUI();
        }
    
    public int CheckItemsLeft(int id)
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

    public void DropItem(GameObject droppedGO, ItemLocation targetLocation = ItemLocation.World )
    {
        Item droppedItem = droppedGO.GetComponent<Item>();
        Debug.Log("Dropping item " + droppedItem.name);
        droppedItem.itemLocation = targetLocation;
        droppedGO.transform.position = transform.position + Vector3.up;
        droppedGO.transform.parent = null;
        UnequipItem(droppedGO);
        itemsList.Remove(droppedGO);

        RefreshInventoryToUI();
    }


    /// <summary>
    /// Updates the bullet counter in the in-game overlay UI
    /// </summary>
    /// <param name="item"></param>
    public void UpdateBulletCounterUI(Item item)
    {
        Label bulletCounter = inGameOverlayUI.bulletCountLabel;
        if (selectedWeapon == SelectedWeapon.Melee
            || selectedWeapon == SelectedWeapon.Throwable)
        {
            bulletCounter.text = "-/-";
        }
        else
        {
            item.totalBullets = CheckItemsLeft(item.bulletID);
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
        Debug.Log("Item: " + item.itemName + " rendered");
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
        Debug.Log("Item: " + item.itemName + " unrendered");
    }


    /// <summary>
    /// Triggers draw weapon animation
    /// </summary>
    /// <param name="weaponClass"></param>
    /// <param name="draw"></param>
    public void DrawWeapon(Item item, bool draw)
    {
        if (draw)
        {
            if (drawnWeaponItem)//If player already have a weapon drawn, holster current weapon first
            {
                if (drawnWeaponItem.weaponClass == item.weaponClass) // if weapons are the same class, unequip the current weapon
                {
                    drawnWeaponItem.itemEquipped = false;
                }
                DrawWeapon(drawnWeaponItem, false);
                
            }//then proceed to draw the new weapon

            switch (item.weaponClass)
            {
                case WeaponScriptableObject.WeaponClass.None:
                    break;

                case WeaponScriptableObject.WeaponClass.Primary:
                    _playerAnimator.SetBool("RifleEquip", draw);
                    _playerAnimator.SetBool("PistolEquip", false);
                    _playerAnimator.SetBool("MeleeEquip", false);
                    _playerAnimator.SetBool("ThrowableEquip", false);
                    selectedWeapon = SelectedWeapon.Primary;

                    break;

                case WeaponScriptableObject.WeaponClass.Secondary:
                    _playerAnimator.SetBool("RifleEquip", false);
                    _playerAnimator.SetBool("PistolEquip", draw);
                    _playerAnimator.SetBool("MeleeEquip", false);
                    _playerAnimator.SetBool("ThrowableEquip", false);
                    selectedWeapon = SelectedWeapon.Secondary;

                    break;

                case WeaponScriptableObject.WeaponClass.Melee:
                    _playerAnimator.SetBool("RifleEquip", false);
                    _playerAnimator.SetBool("PistolEquip", false);
                    _playerAnimator.SetBool("MeleeEquip", draw);
                    _playerAnimator.SetBool("ThrowableEquip", false);
                    selectedWeapon = SelectedWeapon.Melee;

                    switch (item.ID)
                    {
                        case 1001: //Knife
                            _playerAnimator.SetBool("KnifeEquip", draw);
                            break;
                        case 1002: //Baseball Bat
                            _playerAnimator.SetBool("BatEquip", draw);
                            break;
                    }

                    break;

                case WeaponScriptableObject.WeaponClass.Throwable:
                    _playerAnimator.SetBool("RifleEquip", false);
                    _playerAnimator.SetBool("PistolEquip", false);
                    _playerAnimator.SetBool("MeleeEquip", false);
                    _playerAnimator.SetBool("ThrowableEquip", draw);
                    selectedWeapon = SelectedWeapon.Throwable;

                    break;
            }

            item.itemLocation = ItemLocation.Player;
            drawnWeaponItem = item;
            drawWeapon = true;
            UpdateBulletCounterUI(item);
        }
        else
        {
            holsterWeapon = true;
            selectedWeapon = SelectedWeapon.None;
            item.itemLocation = ItemLocation.Inventory;
            item.weaponDrawn = false;
            drawnWeaponItem = null;
            _playerAnimator.SetBool("RifleEquip", false);
            _playerAnimator.SetBool("PistolEquip", false);
            _playerAnimator.SetBool("MeleeEquip", false);
            _playerAnimator.SetBool("ThrowableEquip", false);
            _playerAnimator.SetBool("KnifeEquip", false);
            _playerAnimator.SetBool("BatEquip", false);
            
            
        }

        drawWeapon = draw;
        item.weaponDrawn = draw;
    }


    #region Animator Events

    public void DrawWeaponAnim(string animatorMessage)
    {
        switch (animatorMessage)
        {
            case "DrawWeapon":

                ToggleDrawSelected(true);

                break;

            case "HolsterWeapon":

                ToggleDrawSelected(false);

                break;


            case "End":

                drawWeapon = false;
                holsterWeapon = false;

                break;
            default:
                Debug.LogWarning("Invalid Animator Event Message: " + animatorMessage);
                break;
        }

        void ToggleDrawSelected(bool draw)
        {
            switch (selectedWeapon)
            {
                case SelectedWeapon.Primary:
                    equippedPrimaryWeapon.weaponDrawn = draw;
                    break;
                case SelectedWeapon.Secondary:
                    equippedSecondaryWeapon.weaponDrawn = draw;
                    break;
                case SelectedWeapon.Melee:
                    equippedMeleeWeapon.weaponDrawn = draw;
                    break;
                case SelectedWeapon.Throwable:
                    equippedThrowableWeapon.weaponDrawn = draw;
                    break;
                case SelectedWeapon.None:

                    break;
                default:
                    break;
            }
        }
    }

    #endregion
}