using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UIElements;
using Debug = UnityEngine.Debug;
using Image = UnityEngine.UI.Image;

public class Inventory : MonoBehaviour
{
    public bool showInventory;

    [HideInInspector] public int totalInventorySlots;

    [Header("UI Transforms")] 
    public UIManager uIManager;
    [HideInInspector]public Canvas inventoryUICanvas;
    public Transform playerHandHolderTransform;
    public Image currentWeaponImage;
    public Sprite emptyWeaponImage;
    [HideInInspector]public TextMeshProUGUI bulletCounterTMPUGUI;
    [HideInInspector]public TextMeshProUGUI capacityText;
        
    [HideInInspector]public GameObject[] slotArray;
    [HideInInspector]public GameObject[] quickSlotCount;
    public List<GameObject> itemList = new List<GameObject>();

    private int totalQuickSlots;
    private int activeSlots;
    
    private PlayerController _playerController;
    private Animator _animator;
    
    [HideInInspector]public int currentCapacity;
    [HideInInspector]public int maxCapacity;
    public bool inventoryFull;

    private bool onItem;
    private bool onWeaponItem;
    private Transform targetItem;
    private Transform targetWeapon;

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

    private void Awake()
    {
        capacityText = uIManager.capacityPanel.GetComponentInChildren<TextMeshProUGUI>();
    }

    void Start()
    {
        _playerController = GetComponent<PlayerController>();
        _animator = GetComponentInChildren<Animator>();
        inventoryUICanvas = uIManager.inventoryUI.GetComponent<Canvas>();
        bulletCounterTMPUGUI = uIManager.ammoPanel.Find("AmmoCount").GetComponent<TextMeshProUGUI>();

       // totalQuickSlots = uIManager.quickInventoryUI.childCount; // get the number of quickslots
        totalInventorySlots = uIManager.inventorySlotArea.childCount; // get the number of inventory slots
        slotArray = new GameObject[totalInventorySlots];
        quickSlotCount = new GameObject[totalQuickSlots];

        CheckInventorySlots();
       // CheckInventorySlots(quickSlotCount, totalQuickSlots, uIManager.quickInventoryUI);
    }
    public void CheckInventorySlots()
    {
        for (int i = 0; i < totalInventorySlots; i++)
        {
            slotArray[i] = uIManager.inventorySlotArea.GetChild(i).gameObject;
            Slot slot = slotArray[i].GetComponent<Slot>();
            slot.CheckSlotContent();
            if (slot.empty)
            {
                inventoryFull = false;
            }
        }
    }
    
    private void FixedUpdate()
    {
        
    }

    void Update()
    {
        InventoryToggle();
        if (!inventoryFull)
        {
            if (onItem && Input.GetKeyDown(_playerController.keyAssignments.useKey.keyCode))
            {
                if (targetItem != null)
                {
                    AddItemToInventory(targetItem);
                    _playerController.grabItem = true;
                    onItem = false;
                }
                
            }

            if (onWeaponItem && Input.GetKeyDown(_playerController.keyAssignments.useKey.keyCode))
            {
                if (targetWeapon != null)
                {
                    AddWeaponToInventory(targetWeapon);
                    _playerController.grabItem = true;
                    onWeaponItem = false;
                }
            }
        }
        
        _animator.SetBool("DrawWeapon",drawWeapon);
        _animator.SetBool("HolsterWeapon", holsterWeapon);
        #region Weapon Switching
        
        if (!_playerController.isAiming && !_playerController.climbingLadder &&
            !_playerController.attacking && !_playerController.onTransition)
        {
            if (!drawWeapon && !holsterWeapon)
            {
                if (Input.GetKeyDown(_playerController.keyAssignments.primaryKey.keyCode))
                {
                    selectedWeapon = SelectedWeapon.Primary;
                    ChangeWeapon(uIManager.primaryEquipSlot,SelectedWeapon.Primary);
                }

                if (Input.GetKeyDown(_playerController.keyAssignments.secondaryKey.keyCode))
                {
                    selectedWeapon = SelectedWeapon.Secondary;

                    ChangeWeapon(uIManager.secondaryEquipSlot,SelectedWeapon.Secondary);
                }

                if (Input.GetKeyDown(_playerController.keyAssignments.meleeKey.keyCode))
                {
                    selectedWeapon = SelectedWeapon.Melee;

                    ChangeWeapon(uIManager.meleeEquipSlot,SelectedWeapon.Melee);
                }
                if (Input.GetKeyDown(_playerController.keyAssignments.throwableKey.keyCode))
                {
                    selectedWeapon = SelectedWeapon.Throwable;

                    ChangeWeapon(uIManager.throwableEquipSlot,SelectedWeapon.Throwable);
                }
                
            }
        }

        if (_playerController._weaponHolderTransform.childCount == 0)
        {
            currentWeaponImage.sprite = emptyWeaponImage;
            bulletCounterTMPUGUI.text = null;
        }

        
    }
    #endregion
    
    private void OnTriggerEnter(Collider other)
    {
        if (!inventoryFull)
        {
            if (other.CompareTag("Item"))
            {
                targetItem = other.transform;
                onItem = true;
                
            }

            if (other.CompareTag("Weapon"))
            {
                targetWeapon = other.transform;
                onWeaponItem = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            targetItem = null;
            onItem = false;
        }

        if (other.CompareTag("Weapon"))
        {
            onWeaponItem = false;
            targetWeapon = null;
        }
    }

    public void InventoryToggle()
    {
        inventoryUICanvas.enabled = showInventory;
        

        if (Input.GetKeyDown(_playerController.keyAssignments.inventoryKey.keyCode))
        {
            showInventory = !showInventory;
            if (showInventory == true)
            {
                CheckInventorySlots();
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            showInventory = false;
        }

        if (showInventory == true)
        {
            _playerController.OnUIController();
            currentCapacity = itemList.Count;
            maxCapacity = totalInventorySlots;
            capacityText.SetText(currentCapacity + " / " + maxCapacity);
        }

        if (showInventory == false)
        {
            uIManager.inspectPanel.gameObject.SetActive(false);
        }
    }
    
    
    
    
    /// <summary>
    /// Store a picked item in the inventory
    /// </summary>
    /// <param name="itemTransform"> Item Component</param>
    /// <param name="itemScriptableObject"> ScriptableObject Component </param>
    /// <param name="itemQuantity"> amount to add </param>
    public void AddItemToInventory(Transform itemTransform)
    {
        CheckInventorySlots(); // refresh the inventory
        for (int i = 0; i < totalInventorySlots; i++) // checks every slot in inventory
        {
            Slot slotIndex = slotArray[i].GetComponent<Slot>();
            Item itemComponent = itemTransform.GetComponent<Item>();

            if (slotIndex.empty == true) // if the slot is empty, store the picked item
            {
                slotIndex.UpdateItemSlot(itemComponent); // update the slot information
                slotIndex.quantity = itemComponent.quantity;
                Debug.Log("the slot in order " + i + " was filled by " + itemComponent.itemName +
                          " with a amount of " + itemComponent.quantity);
                Debug.Log("slotIndex.UpdateItemSlot(itemComponent.itemScriptableObject)");
                itemComponent.itemPicked = true;
                itemComponent.itemLocation = Item.ItemLocation.Inventory;
                itemList.Add(slotArray[i].GetComponent<GameObject>());
                slotIndex.empty = false;
                CheckSlotQuantity(itemComponent, i); // check if the slot exceed the stacking limit
                CheckInventorySlots(); // refresh the inventory again
                return;
            }

            if (slotIndex.empty == false && slotIndex.weaponItem == null)
            {
                if (slotIndex._item.itemName == itemComponent.itemName)
                {
                    Debug.Log("an existing " + itemComponent.itemName + " was found in slot: " + i);
                    
                    if (slotIndex.quantity < slotIndex.maxStack)
                    {
                        slotIndex.quantity += itemComponent.quantity;
                        Debug.Log("added " + itemComponent.quantity + " units to slot " + i);
                        Debug.Log("slotIndex.UpdateItemSlot()");
                        itemComponent.itemLocation = Item.ItemLocation.Inventory;
                        slotIndex.UpdateItemSlot(itemComponent); // update the slot information
                        CheckSlotQuantity(itemComponent, i); // check the quantity of the slot to determine if the item is picked or not
                        Debug.Log("added " + itemComponent.quantity + " " + itemComponent.itemName + " in the slot " + i);
                        return;
                    }
                    else
                    {
                        Debug.Log("The slot in index " + i + " is full");
                    }
                }
            }
            
        }
    }
    
    public void CheckSlotQuantity(Item item, int slotOrder)
    {
        for (int i = slotOrder; i < totalInventorySlots; i++)
        {
            Slot slotIndex = slotArray[i].GetComponent<Slot>();

            if (slotIndex.isStackable && slotIndex.quantity > slotIndex.maxStack)
                // if the quantity surpasses the max stack capacity, transfer leftover units to the next slot
            {
                Debug.Log("The slot " + i + " has reached max stackable capacity of " + slotIndex.maxStack + " for " +
                          item.itemName);//example = current> 150/100 <max
                int leftOver = slotIndex.quantity - slotIndex.maxStack;//150-100
                Debug.Log("remaining units: " + leftOver);//50
                
                if (i + 1 < totalInventorySlots)
                {
                    for (int j = i+1; j < totalInventorySlots; j++)
                    {
                        Slot nextSlot = slotArray[j].GetComponent<Slot>();
                        if (nextSlot.empty)
                        {
                            item.quantity = 0;
                            nextSlot._item = slotIndex._item; // copy the information to the next slot
                            nextSlot.UpdateItemSlot(slotIndex._item); //update the next slot information
                            Debug.Log("Updated next Slot Information");
                            slotIndex.UpdateItemSlot(item); //update the current slot information
                            Debug.Log("Updated original Slot information");
                            Debug.Log("the next slot in order " + (i + 1) + " was filled by " + item.itemName);
                            nextSlot.quantity = leftOver; // transfer the units to next slot
                            Debug.Log("the amount of next slot " + (i + 1) + " is " + nextSlot.quantity);
                            slotIndex.quantity -= leftOver; // substract the surplus to match the max stack capacity
                            Debug.Log(" slotIndex.quantity: " + slotIndex.quantity);

                            nextSlot.empty = false; // tell the next slot is not empty
                            Debug.Log(" nextSlot.empty: " + nextSlot.empty);
                            Debug.Log("transferred " + leftOver + " units to the next slot");
                            itemList.Add(slotArray[i + 1].GetComponent<GameObject>());
                            CheckSlotQuantity(item,slotOrder+1);
                            return;
                        }
                    }
                    
                    
                }
                else
                {
                    item.quantity = leftOver;
                    item.itemLocation = Item.ItemLocation.World;
                    item.itemPicked = false;
                    inventoryFull = true;
                    CheckInventorySlots();
                    slotIndex.quantity = slotIndex.maxStack;
                    //slotIndex.UpdateItemSlot(item);
                    Debug.Log("Inventory is Full");
                    return;
                }

            }
            else
            {
                item.quantity = 0;
                item.itemLocation = Item.ItemLocation.Inventory;
                item.itemTransform = slotIndex.itemHolderTransform;
                item.itemPicked = true;
                return;
            }
        }
    }

    
    /// <summary>
    /// Store a picked weapon item in the inventory
    /// </summary>
    /// <param name="weaponItem">weaponObject component of the picked item</param>
    public void AddWeaponToInventory(Transform targetTransform)
    {
        for (int i = 0; i < totalInventorySlots; i++)
        {
            Slot slotIndex = slotArray[i].GetComponent<Slot>();
            WeaponItem targetWeaponItem = targetTransform.GetComponent<WeaponItem>();
            if (slotIndex.empty)
            {
                targetTransform.parent = slotIndex.weaponHolderTransform;
                targetWeaponItem.weaponPicked = true;
                slotIndex.UpdateWeaponSlot(targetWeaponItem);
                targetWeaponItem.weaponLocation = WeaponItem.WeaponLocation.Inventory;
                Debug.Log("the slot in order " + i + " was filled by " + targetWeaponItem.weaponName);
                slotIndex.empty = false;
                itemList.Add(targetTransform.gameObject);
                CheckInventorySlots();
                return;
            }
        }
    }
    

    /// <summary>
    /// Weapon Equipment Management
    /// </summary>
    /// <param name="equipTransform"></param>
    /// <param name="weaponClass"></param>
    public void ChangeWeapon(Transform equipTransform, SelectedWeapon selectedWeapon)
    {
        Slot equipSlot = equipTransform.GetComponent<Slot>();
        Transform slotWeaponHolder = equipTransform.Find("WeaponHolder");
        if (slotWeaponHolder.childCount > 0)
        {
            WeaponItem equipWeaponItem = equipSlot.weaponItem;
            if (playerHandHolderTransform.childCount <=0) 
            {
                slotWeaponHolder.GetChild(0).parent = playerHandHolderTransform;
                drawWeapon = true;
                Debug.Log("Changing weapon to " + equipWeaponItem.weaponName + " as " + selectedWeapon);
            }
            else if (playerHandHolderTransform.childCount >0)
            {
                WeaponItem playerWeaponItem = playerHandHolderTransform.GetChild(0).GetComponent<WeaponItem>();
                if (equipWeaponItem.ID == playerWeaponItem.ID)
                {
                    Debug.Log("Holstering Weapon...");
                    holsterWeapon = true;
                    drawWeapon = false;
                }
                else
                {
                    Debug.Log("Swapping Weapons...");
                    holsterWeapon = true;
                    drawWeapon = true;
                    _animator.SetBool("RifleEquip",false);
                    _animator.SetBool("PistolEquip",false);
                    _animator.SetBool("MeleeEquip",false);
                }
            }
        }
        else if (slotWeaponHolder.childCount <= 0)
        {
            if (equipSlot.empty)
            {
                Debug.Log("No Weapon Equipped on " + selectedWeapon);
            }
            if (!equipSlot.empty)
            {
                if (playerHandHolderTransform.childCount > 0)
                {
                    holsterWeapon = true;
                    drawWeapon = false;
                }
            }
        }

        if (drawWeapon)
        {
            switch (selectedWeapon)
            {
                case SelectedWeapon.Primary:
                    if (drawWeapon) _animator.SetBool("RifleEquip", true);
                    else _animator.SetBool("RifleEquip", false);
                    break;

                case SelectedWeapon.Secondary:
                    if (drawWeapon) _animator.SetBool("PistolEquip", true);
                    else _animator.SetBool("PistolEquip", false);
                    break;

                case SelectedWeapon.Melee:
                    if (drawWeapon) _animator.SetBool("MeleeEquip", true);
                    else _animator.SetBool("MeleeEquip", false);
                    break;
                
                case SelectedWeapon.Throwable:
                    if (drawWeapon) _animator.SetBool("ThrowableEquip", true);
                    else _animator.SetBool("ThrowableEquip", false);
                    break;
            }
        }
        
    }

    
    /// <summary>
    /// Parent weaponToDraw transform to player Hand Holder transform (if empty)
    /// </summary>
    /// <param name="weaponToDraw"></param>
    public void DrawWeapon(SelectedWeapon selectedWeapon)
    {
        if (playerHandHolderTransform.childCount <=0)
        {
            switch (selectedWeapon)
            {
                case SelectedWeapon.Primary:
                    uIManager.primaryEquipSlot.Find("WeaponHolder").GetChild(0).parent = playerHandHolderTransform;
                    break;
            
                case SelectedWeapon.Secondary:
                   uIManager.secondaryEquipSlot.Find("WeaponHolder").GetChild(0).parent = playerHandHolderTransform;
                    break;
            
                case SelectedWeapon.Melee:
                    uIManager.meleeEquipSlot.Find("WeaponHolder").GetChild(0).parent = playerHandHolderTransform;
                    break;
                case SelectedWeapon.Throwable:
                    uIManager.throwableEquipSlot.Find("WeaponHolder").GetChild(0).parent = playerHandHolderTransform;
                    break;
            }
        }

        if (playerHandHolderTransform.childCount > 0)
        {
            WeaponItem playerWeaponItem = playerHandHolderTransform.GetChild(0).GetComponent<WeaponItem>();
            playerWeaponItem.holderTarget.GetComponentInParent<Slot>().UpdateWeaponSlot(playerWeaponItem);
            playerWeaponItem.weaponLocation = WeaponItem.WeaponLocation.Player;
        }
        playerHandHolderTransform.GetChild(0).gameObject.SetActive(true);
    }

    /// <summary>
    /// Holster the current weapon in player hands to the corresponding parent
    /// </summary>
    /// <param name="weaponToHolster"></param>
    public void HolsterWeaponTo(Transform holderTransform)
    {
        if (playerHandHolderTransform.childCount > 0)
        {
            Transform weaponToHolster = playerHandHolderTransform.GetChild(0);
            WeaponItem weaponToHolsterWeaponItem = weaponToHolster.GetComponent<WeaponItem>();
            
            if (holderTransform.childCount > 0)
            {
                if (!inventoryFull)
                {
                    AddWeaponToInventory(weaponToHolster);
                    weaponToHolsterWeaponItem.weaponLocation = WeaponItem.WeaponLocation.Inventory;
                }
                else
                {
                    weaponToHolster.parent = null;
                    weaponToHolsterWeaponItem.weaponLocation = WeaponItem.WeaponLocation.World;
                }
            }
            
            if (holderTransform.childCount <= 0)
            {
                weaponToHolster.parent = holderTransform;
                weaponToHolsterWeaponItem.weaponLocation = WeaponItem.WeaponLocation.Inventory;
            }
            
            if (!drawWeapon)
            {
                switch (weaponToHolsterWeaponItem.weaponItemClass)
                {
                    case WeaponScriptableObject.WeaponClass.Primary:
                        _animator.SetBool("RifleEquip", false);
                        break;
                
                    case WeaponScriptableObject.WeaponClass.Secondary:
                        _animator.SetBool("PistolEquip", false);
                        break;
                
                    case WeaponScriptableObject.WeaponClass.Melee:
                        _animator.SetBool("MeleeEquip", false);
                        if (_animator.GetBool("BatEquip") == true)
                        { 
                            _animator.SetBool("BatEquip", false);
                        }
                        if (_animator.GetBool("KnifeEquip") == true)
                        { 
                            _animator.SetBool("KnifeEquip", false);
                        }

                        break;
                    case WeaponScriptableObject.WeaponClass.Throwable:
                        _animator.SetBool("ThrowableEquip", false);
                        break;
                }
            }

            if (drawWeapon)
            {
                DrawWeapon(selectedWeapon);
                if (playerHandHolderTransform.childCount > 0)
                {
                    playerHandHolderTransform.GetChild(0).gameObject.SetActive(false);
                }
            }
                
        }
        else
        {
            Debug.Log("No weapon to holster in player hands");
        }
    }
    public void DrawWeaponAnim(string command) // function is triggered while "drawWeapon" is true
    {
        switch (command)
        {
            case "WeaponDraw":

                if (drawWeapon)
                {
                    DrawWeapon(selectedWeapon);
                }
                
                break;
            
            case "WeaponHolster":

                if (holsterWeapon)
                {
                    if (playerHandHolderTransform.childCount > 0)
                    {
                        WeaponItem playerWeaponItem = playerHandHolderTransform.GetChild(0).GetComponent<WeaponItem>();
                        HolsterWeaponTo(playerWeaponItem.holderTarget);
                    }
                }
                
                break;
            
            case "DrawEnd":
                drawWeapon = false;
                holsterWeapon = false;
                break;
            
            case "HolsterEnd":
                holsterWeapon = false;
                break;
        }
    }
    
    public int CheckItemsLeft(int id, int total)
    {                    
        //Debug.Log("seeking inventory for ID " + id);

        total = 0;
        for (int i = 0; i < totalInventorySlots; i++)
        {
            Slot slotIndex = slotArray[i].GetComponent<Slot>();
            if (!slotIndex.empty && slotIndex.itemScriptableObject != null)
            {
                if (slotIndex.itemScriptableObject.ID == id)
                {
                    // Debug.Log("Found " + slotIndex.quantity + " units of ID" + id);
                    total += slotIndex.quantity;
                }
            }
        }
        //Debug.Log("return" + counter);
        return total;
    }
    public void UpdateBulletCounter(WeaponItem playerWeaponItem)
    {
        if (selectedWeapon == SelectedWeapon.Melee 
            || selectedWeapon == SelectedWeapon.Throwable)
        {
            bulletCounterTMPUGUI.SetText("-/-");
        }
        else
        {
            bulletCounterTMPUGUI.SetText(playerWeaponItem.bulletsInMag  + "/" + playerWeaponItem.totalBullets);  
        }
        
        if (playerWeaponItem.bulletsInMag < 5)
        {
            bulletCounterTMPUGUI.color = Color.red;
        }
        else
        {
            bulletCounterTMPUGUI.color = Color.white;
        }    
    }
    
    
    
}
    
