using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using Debug = UnityEngine.Debug;
using Image = UnityEngine.UI.Image;
using Object = UnityEngine.Object;

public class Inventory : MonoBehaviour
{
    public bool showInventory;
    
    public UIManager uIManager;
    public InventoryUI inventoryUI;
    public PlayerController _playerController;
    [HideInInspector]public Transform playerWeaponHolder;
    private Animator _playerAnimator;
    
    public List<Item> itemsList;
    
    [HideInInspector]public int currentCapacity;
    [HideInInspector]public int maxCapacity = 10;
    [HideInInspector]public int defaultMaxCapacity = 10;
    
    public bool inventoryFull;

    [HideInInspector] public bool onItem;
    [HideInInspector] public bool onWeaponItem;
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

    public WeaponItem primaryWeapon;
    public WeaponItem secondaryWeapon;
    public WeaponItem meleeWeapon;
    public WeaponItem throwableWeapon;
    
    private void Awake()
    {
        uIManager = GameManager.Instance.uiManager;
        inventoryUI = uIManager.inventoryUI;
        _playerAnimator = GetComponent<Animator>();
        playerWeaponHolder = _playerAnimator.GetBoneTransform(HumanBodyBones.RightHand).Find("WeaponHolder");
    }

    void Start()
    {
        inventoryUI.FillInventoryWithSlots(maxCapacity);

        RefreshInventoryToUI();
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
            maxCapacity = 0;
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
                if (targetItem != null)
                {
                 //   AddItemToInventory(targetItem);
                    _playerController.grabItem = true;
                    onItem = false;
                }
                
            }

            if (onWeaponItem && Input.GetKeyDown(_playerController.keyAssignments.useKey.keyCode))
            {
                if (targetWeapon != null)
                {
                   // AddWeaponToInventory(targetWeapon);
                    _playerController.grabItem = true;
                    onWeaponItem = false;
                }
            }
        }
        
        _playerAnimator.SetBool("DrawWeapon",drawWeapon);
        _playerAnimator.SetBool("HolsterWeapon", holsterWeapon);
        #region Weapon Switching
        
        if (!_playerController.isAiming && !_playerController.climbingLadder &&
            !_playerController.attacking && !_playerController.onTransition)
        {
            /*if (!drawWeapon && !holsterWeapon)
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
                
            }*/
        }

        if (_playerController._playerWpnHolderTransform.childCount == 0)
        {
            /*currentWeaponImage.sprite = emptyWeaponImage;
            bulletCounterTMPUGUI.text = null;*/
        }

        
    }
    #endregion
    public void RefreshInventoryToUI()
    {
        if (itemsList.Count > 0)
        {
            for (int i = 0; i < itemsList.Count; i++)
            {
                inventoryUI.inventorySlotList[i].name = "Slot" + i;
                IStyle style = inventoryUI.inventorySlotList[i].style;
                Label quantity = inventoryUI.inventorySlotList[i].Q<Label>("SlotQuantity");
                Item item = itemsList[i];
                style.backgroundImage = new StyleBackground(item.itemIcon);
                quantity.text = item.quantity.ToString();
            }
        }
        
    }
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
    /// Store a picked item in the inventory
    /// </summary>
    /// <param name="itemTransform"> Item Component</param>
    /// <param name="itemScriptableObject"> ScriptableObject Component </param>
    /// <param name="itemQuantity"> amount to add </param>
   public void AddItemToInventory(Transform itemTransform)
    {
        RefreshInventoryToUI(); // refresh the inventory
        Item newItem = itemTransform.GetComponent<Item>();

        itemsList.Add(newItem);

        if (newItem.isStackable)
        {
            for (int i = 0; i < itemsList.Count; i++)
            {
                if (itemsList[i].ID == newItem.ID && newItem.itemClass == Item.ItemClass.Item)
                // if item already exists in inventory, stack quantities
                {
                    itemsList[i].quantity += newItem.quantity;
                    CheckStackableItems(newItem, i);
                }
                else
                {

                }
            }

        }


        

        for (int i = 0; i < totalInventorySlots; i++) // checks every slot in inventory
        {
            Slot slotIndex = slotArray[i].GetComponent<Slot>();
           

            if (slotIndex.empty == true) // if the slot is empty, store the picked item
            {
                slotIndex.UpdateItemSlot(itemComponent); // update the slot information
                slotIndex.quantity = itemComponent.quantity;
                Debug.Log("the slot in order " + i + " was filled by " + itemComponent.itemName +
                          " with a amount of " + itemComponent.quantity);
                Debug.Log("slotIndex.UpdateItemSlot(itemComponent.itemScriptableObject)");
                itemComponent.itemPickedUp = true;
                itemComponent.itemLocation = Item.ItemLocation.Inventory;
                itemList.Add(slotArray[i].GetComponent<GameObject>());
                slotIndex.empty = false;
                CheckSlotQuantity(itemComponent, i); // check if the slot exceed the stacking limit
                RefreshInventoryToUI(); // refresh the inventory again
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
    
   public void CheckStackableItems()
    {
        for (int i = 0; i < itemsList.Count; i++)
        {
            if (itemsList[i].isStackable)

                if (itemsList[i].quantity > itemsList[i].maxStack)
                // if the quantity surpasses the max stack capacity, transfer leftover units to the next slot

                {
                    Debug.Log("The slot " + i + " has reached max stackable capacity of " 
                        + itemsList[i].maxStack + " for " + item.itemName);//example = current> 150/100 <max

                    int leftOver = itemsList[i].quantity - itemsList[i].maxStack;//get the leftover units 150-100
                    Debug.Log("remaining units: " + leftOver);//50

                    item.quantity = leftOver;

                    if (i + 1 < itemsList.Count)
                    {
                        itemsList.Add(item);

                    }
                    else
                    {
                        item.quantity = leftOver;
                        item.itemLocation = Item.ItemLocation.World;
                        item.itemPickedUp = false;
                        inventoryFull = true;
                        RefreshInventoryToUI();
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
                item.itemPickedUp = true;
                return;
            }
        }
    }*/

    
    /// <summary>
    /// Store a picked weapon item in the inventory
    /// </summary>
    /// <param name="weaponItem">weaponObject component of the picked item</param>
    /*public void AddWeaponToInventory(Transform targetTransform)
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
                RefreshInventoryToUI();
                return;
            }
        }
    }
    */
    

    /// <summary>
    /// Weapon Equipment Management
    /// </summary>
    /// <param name="equipTransform"></param>
    /// <param name="weaponClass"></param>
    /*public void ChangeWeapon(Transform equipTransform, SelectedWeapon selectedWeapon)
    {
        Slot equipSlot = equipTransform.GetComponent<Slot>();
        Transform slotWeaponHolder = equipTransform.Find("WeaponHolder");
        if (slotWeaponHolder.childCount > 0)
        {
            WeaponItem equipWeaponItem = equipSlot.weaponItem;
            _playerController.equippedWeaponItem = equipWeaponItem;
            if (playerWeaponHolderTransform.childCount <=0) 
            {
                slotWeaponHolder.GetChild(0).parent = playerWeaponHolderTransform;
                drawWeapon = true;
                Debug.Log("Changing weapon to " + equipWeaponItem.weaponName + " as " + selectedWeapon);
            }
            else if (playerWeaponHolderTransform.childCount >0)
            {
                WeaponItem playerWeaponItem = playerWeaponHolderTransform.GetChild(0).GetComponent<WeaponItem>();
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
                    _playerAnimator.SetBool("RifleEquip",false);
                    _playerAnimator.SetBool("PistolEquip",false);
                    _playerAnimator.SetBool("MeleeEquip",false);
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
                if (playerWeaponHolderTransform.childCount > 0)
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
                    if (drawWeapon) _playerAnimator.SetBool("RifleEquip", true);
                    else _playerAnimator.SetBool("RifleEquip", false);
                    break;

                case SelectedWeapon.Secondary:
                    if (drawWeapon) _playerAnimator.SetBool("PistolEquip", true);
                    else _playerAnimator.SetBool("PistolEquip", false);
                    break;

                case SelectedWeapon.Melee:
                    if (drawWeapon) _playerAnimator.SetBool("MeleeEquip", true);
                    else _playerAnimator.SetBool("MeleeEquip", false);
                    break;
                
                case SelectedWeapon.Throwable:
                    if (drawWeapon) _playerAnimator.SetBool("ThrowableEquip", true);
                    else _playerAnimator.SetBool("ThrowableEquip", false);
                    break;
            }
        }
        
    }*/

    
    /// <summary>
    /// Parent weaponToDraw transform to player Hand Holder transform (if empty)
    /// </summary>
    /// <param name="weaponToDraw"></param>
    /*public void DrawWeapon(SelectedWeapon selectedWeapon)
    {
        if (playerWeaponHolderTransform.childCount <=0)
        {
            switch (selectedWeapon)
            {
                /*case SelectedWeapon.Primary:
                    uIManager.primaryEquipSlot.Find("WeaponHolder").GetChild(0).parent = playerWeaponHolderTransform;
                    break;
            
                case SelectedWeapon.Secondary:
                   uIManager.secondaryEquipSlot.Find("WeaponHolder").GetChild(0).parent = playerWeaponHolderTransform;
                    break;
            
                case SelectedWeapon.Melee:
                    uIManager.meleeEquipSlot.Find("WeaponHolder").GetChild(0).parent = playerWeaponHolderTransform;
                    break;
                case SelectedWeapon.Throwable:
                    uIManager.throwableEquipSlot.Find("WeaponHolder").GetChild(0).parent = playerWeaponHolderTransform;
                    break;#1#
            }
        }

        if (playerWeaponHolderTransform.childCount > 0)
        {
            WeaponItem playerWeaponItem = playerWeaponHolderTransform.GetChild(0).GetComponent<WeaponItem>();
            playerWeaponItem.holderTarget.GetComponentInParent<Slot>().UpdateWeaponSlot(playerWeaponItem);
            playerWeaponItem.weaponLocation = WeaponItem.WeaponLocation.Player;
        }
        playerWeaponHolderTransform.GetChild(0).gameObject.SetActive(true);
    }*/

    /// <summary>
    /// Holster the current weapon in player hands to the corresponding parent
    /// </summary>
    /// <param name="weaponToHolster"></param>
    /*public void HolsterWeaponTo(Transform holderTransform)
    {
        if (playerWeaponHolderTransform.childCount > 0)
        {
            Transform weaponToHolster = playerWeaponHolderTransform.GetChild(0);
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
                        _playerAnimator.SetBool("RifleEquip", false);
                        break;
                
                    case WeaponScriptableObject.WeaponClass.Secondary:
                        _playerAnimator.SetBool("PistolEquip", false);
                        break;
                
                    case WeaponScriptableObject.WeaponClass.Melee:
                        _playerAnimator.SetBool("MeleeEquip", false);
                        if (_playerAnimator.GetBool("BatEquip") == true)
                        { 
                            _playerAnimator.SetBool("BatEquip", false);
                        }
                        if (_playerAnimator.GetBool("KnifeEquip") == true)
                        { 
                            _playerAnimator.SetBool("KnifeEquip", false);
                        }

                        break;
                    case WeaponScriptableObject.WeaponClass.Throwable:
                        _playerAnimator.SetBool("ThrowableEquip", false);
                        break;
                }
            }

            if (drawWeapon)
            {
                DrawWeapon(selectedWeapon);
                if (playerWeaponHolderTransform.childCount > 0)
                {
                    playerWeaponHolderTransform.GetChild(0).gameObject.SetActive(false);
                }
            }
                
        }
        else
        {
            Debug.Log("No weapon to holster in player hands");
        }
    }*/
    /*public void DrawWeaponAnim(string command) // function is triggered while "drawWeapon" is true
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
                    if (playerWeaponHolderTransform.childCount > 0)
                    {
                        WeaponItem playerWeaponItem = playerWeaponHolderTransform.GetChild(0).GetComponent<WeaponItem>();
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
    }*/
    
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
    
    
    /// <summary>
    /// Updates the bullet counter in the in-game overlay UI
    /// </summary>
    /// <param name="item"></param>
    public void UpdateBulletCounter(Item item)
    {
        Label bulletCounter = uIManager.inGameOverlayUI.bulletCountLabel;
        if (selectedWeapon == SelectedWeapon.Melee 
            || selectedWeapon == SelectedWeapon.Throwable)
        {
            bulletCounter.text = "-/-";
        }
        else
        {
            bulletCounter.text = item.bulletsInMag  + "/" + item.totalBullets;  
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
    
