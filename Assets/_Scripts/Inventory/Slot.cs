using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using Unity.Burst;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
  /*#region Slot UI Elements references
  public enum SlotType
  {
    inventorySlot,
    EquipmentSlot,
    QuickInventorySlot,
    ContainerSlot,
  }
  public SlotType slotType;

  public Transform weaponHolderTransform;
  public Transform itemHolderTransform;
  [HideInInspector]public ItemContainer itemContainer;
  
  public TextMeshProUGUI quantityTxtMP;
  public Sprite defaultSlotSprite;

  private Transform quickInfoPanelUI;
  private TextMeshProUGUI infoText;
  
  [HideInInspector]public Inventory inventory;
  public Item _item;
  public Image slotImage;

  #endregion
  
  #region Item Scriptable Objects values
 [Header("ItemScriptableObject")]
  public ItemScriptableObject itemScriptableObject;

  
 [HideInInspector] public int itemID;
 [HideInInspector] public string itemType;
 [HideInInspector] public string description;
 [HideInInspector] public Sprite itemIcon;
 [HideInInspector] public GameObject itemPrefab;
 public int quantity;
 [HideInInspector] public bool usable;
 [HideInInspector] public bool consumable;
 [HideInInspector] public bool isStackable;
 [HideInInspector] public int maxStack;

  #endregion
  
  #region Weapon Object values
  
  [Header("WeaponObject")]
  public WeaponScriptableObject weaponScriptableObject;
  public WeaponItem weaponItem;
  public WeaponScriptableObject.WeaponClass weaponItemClass;
  public int weaponID;
  public string weaponName;
  public string weaponDescription;
  private int bulletID;
  private Sprite weaponIcon;

  #endregion

  private Button slotButton;
  
  #region Contextual Menú Variables
  public bool showSlotMenu;
  public Canvas contextMenu;
  public bool mouseOverSlot;
  
  #endregion

  public bool empty;

  private void OnEnable()
  {
    switch (slotType)
    {
      case SlotType.ContainerSlot:
        
        break;
    }
  }

  private void Awake()
  {
    slotButton = GetComponent<Button>();
    slotImage.sprite = defaultSlotSprite;
    inventory = GameObject.Find("Player").GetComponent<Inventory>();
  }

  private void Start()
  {
    /*quickInfoPanelUI = inventory.uIManager.quickInfoPanelUI;
    infoText = inventory.uIManager.quickInfoPanelUI.GetComponentInChildren<TextMeshProUGUI>();
    #1#
    
    if (weaponHolderTransform.childCount > 0)
    {
      weaponItem = weaponHolderTransform.GetChild(0).GetComponent<WeaponItem>();
    }
    switch (slotType)
    {
      case SlotType.inventorySlot:
        
        break;
      
      case SlotType.EquipmentSlot:
        
        break;
      
      case SlotType.QuickInventorySlot:
        
        break;
      
      case SlotType.ContainerSlot:
        if (itemScriptableObject != null)
        {
          
        }

        if (weaponItem !=null)
        {
          
        }
        break;
    }
    CheckSlotContent();
  }
  
  private void Update()
  {
   // CheckSlotContent();

    switch (slotType)
    {
      case SlotType.inventorySlot:

        contextMenu.enabled = showSlotMenu;

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
          if (showSlotMenu)
          {
            showSlotMenu = false;
          }
          
        }
        
        if (GameManager.gamePaused || !inventory.showInventory)
        {
          contextMenu.enabled = false;
        }
        
        if (contextMenu.enabled)
        {
          quickInfoPanelUI.gameObject.SetActive(false);
        }
        break;
    }
    
    if (mouseOverSlot)
    {
      quickInfoPanelUI.transform.position = new Vector3(Input.mousePosition.x+20,Input.mousePosition.y -20, Input.mousePosition.z);
    }
  }

  public void CheckSlotContent()
  {
    if (weaponItem == null && _item == null)
    {
      empty = true;
    }
    else if (weaponItem !=null || _item != null)
    {
      empty = false;
    }

    if (empty)
    {
      slotImage.enabled = false;
      quantityTxtMP.enabled = false;
      UpdateItemSlot(null);
      UpdateWeaponSlot(null);
    }
    else
    {
      slotImage.enabled = true;
      quantityTxtMP.enabled = true;
    }
    
    if (weaponHolderTransform.childCount > 0)
    {
      UpdateWeaponSlot(weaponHolderTransform.GetChild(0).GetComponent<WeaponItem>());
    }
    
    if (slotType == SlotType.EquipmentSlot)
    {
      if(weaponHolderTransform.childCount <= 0)
      {
        if (inventory.playerWeaponHolderTransform.childCount >0)
        {
          WeaponItem playerWeaponItem = inventory.playerWeaponHolderTransform.GetChild(0).GetComponent<WeaponItem>();
          if (playerWeaponItem.weaponItemClass == weaponItemClass)
          {
            UpdateWeaponSlot(playerWeaponItem);
          }
        }
        else if(inventory.playerWeaponHolderTransform.childCount <=0)
        {
          UpdateWeaponSlot(null);
        }
      }
    }
    
    if (slotType == SlotType.inventorySlot || slotType == SlotType.ContainerSlot)
    {
      if (_item !=null)
      {
        UpdateItemSlot(_item);
      } 
      else if (_item == null && weaponHolderTransform.childCount >0)
      {
        UpdateWeaponSlot(weaponHolderTransform.GetChild(0).GetComponent<WeaponItem>());
      }

      if (quantity <=0)
      {
        empty = true;
        UpdateItemSlot(null);
      }
      else
      {
        empty = false;
      }
    }
    
  }
  public void OnPointerClick(PointerEventData eventData)
  {
    if (eventData.button == PointerEventData.InputButton.Left)
    {
      
      for (int i = 0; i < inventory.totalInventorySlots; i++)
      {
        Slot slotIndex = inventory.slotArray[i].GetComponent<Slot>();
        slotIndex.showSlotMenu = false;
      }
      switch (slotType)
      {
        case SlotType.inventorySlot:
          
          if (!empty)
          {
            if (_item != null && quantity > 0 )
            {
              if (consumable)
              {
                ConsumeItem(_item);
              }
            }
            if (weaponItem != null)
            {
              if (weaponHolderTransform.childCount > 0)
              {
                EquipWeapon(weaponHolderTransform.GetChild(0));
              }
            }
          } 
          CheckSlotContent();
          break;

        case SlotType.EquipmentSlot: //unequip weapon and holster if is in player hands

          if (weaponHolderTransform.childCount > 0)
          {
            WeaponItem holderWeaponItem = weaponHolderTransform.GetChild(0).GetComponent<WeaponItem>();
            if (inventory.playerWeaponHolderTransform.childCount > 0)
            {
              
              /*WeaponItem playerWeaponItem = inventory.playerHandHolderTransform.GetChild(0).GetComponent<WeaponItem>();
              if (holderWeaponItem.ID != playerWeaponItem.ID)
              {#1#
                inventory.AddWeaponToInventory(weaponHolderTransform.GetChild(0));
              //} 
            }
            if (inventory.playerWeaponHolderTransform.childCount <= 0)
            {
              inventory.AddWeaponToInventory(weaponHolderTransform.GetChild(0));
            }
          }
          
          if (weaponHolderTransform.childCount <= 0)
          {
            if (empty)
            {
              Debug.Log("No weapon equipped here");
            }

            if (!empty)
            {
              if (inventory.playerWeaponHolderTransform.childCount > 0)
              {
                WeaponItem playerWeaponItem = inventory.playerWeaponHolderTransform.GetChild(0).GetComponent<WeaponItem>();
                if (playerWeaponItem.ID == weaponID)
                {
                  inventory.holsterWeapon = true;
                }
              }

              if (inventory.playerWeaponHolderTransform.childCount <= 0)
              {
                UpdateWeaponSlot(null);
              }
            }
          }
          break;

        case SlotType.QuickInventorySlot:
          
          break;
        
        case SlotType.ContainerSlot:
          if (!empty)
          {
            if (itemScriptableObject !=null)
            {
              inventory.AddItemToInventory(itemHolderTransform.GetChild(0));
              UpdateItemSlot(null);
            }
            if (itemScriptableObject == null && weaponScriptableObject !=null)
            {
              inventory.AddWeaponToInventory(weaponHolderTransform.GetChild(0));
              quantity = 0;
              UpdateWeaponSlot(null);
            }
          } 
          break;
      }
    }

    if (eventData.button == PointerEventData.InputButton.Right)
    {
      switch (slotType)
      { 
        case SlotType.ContainerSlot:
          
          break;
        
        case SlotType.inventorySlot:
          showSlotMenu = !showSlotMenu;
          ShowContextualMenu();
          break;
        
        case SlotType.EquipmentSlot:
          
          break;
        
        case SlotType.QuickInventorySlot:
          
          break;
        
      }
      
      
    }
  }

  
  public void OnPointerEnter(PointerEventData eventData)
  {
    mouseOverSlot = true;
    if (weaponItem != null)
    {
      infoText.text = weaponItem.weaponName;
    }

    if (itemScriptableObject !=null)
    {
      infoText.text = itemScriptableObject.itemName;
    }

    if (infoText.text != null)
    {
      quickInfoPanelUI.gameObject.SetActive(true);
    }
    else
    {
      quickInfoPanelUI.gameObject.SetActive(false);
    }


  }

  public void OnPointerExit(PointerEventData eventData)
  {
    mouseOverSlot = false;
    infoText.text = null;
    quickInfoPanelUI.gameObject.SetActive(false);
  }

  public void UpdateItemSlot(Item item)
  {
    _item = item;
    if (_item != null)
    {
      itemScriptableObject = item.itemScriptableObject;
      itemID = item.ID;
      itemType = item.itemName; 
      description = item.description;
      itemIcon = item.itemIcon; 
      itemPrefab = item.itemPrefab; 
      usable = item.usable; 
      consumable = item.consumable; 
      isStackable = item.isStackable; 
      maxStack = item.maxStack; 
      slotImage.enabled = true; 
      slotImage.sprite = itemIcon;
      quantityTxtMP.text = quantity.ToString();
    }
    else
    {
      _item = null;
      empty = true;
      itemScriptableObject = null;
      itemID = 0;
      itemType = null; 
      description = null;
      itemIcon = defaultSlotSprite; 
      itemPrefab = null; 
      usable = false; 
      consumable = false; 
      isStackable = false; 
      maxStack = 0; 
      slotImage.enabled = false; 
      slotImage.sprite = itemIcon;
      quantity = 0;
      quantityTxtMP.text = quantity.ToString();
    }
  }
  
  public void UpdateWeaponSlot(WeaponItem weaponScript)
  {
    weaponItem = weaponScript;
    if (weaponScript != null)
    {
      weaponScriptableObject = weaponScript.weaponScriptableObject;
      weaponItemClass = weaponScript.weaponItemClass;
      weaponID = weaponScript.ID;
      bulletID = weaponScript.bulletID;
      weaponName = weaponScript.weaponName;
      description = weaponScript.description;
      weaponIcon = weaponScript.weaponIcon;
      quantity = weaponScript.quantity;
      slotImage.sprite = weaponIcon;
      quantityTxtMP.text = quantity.ToString();
      empty = false;
    }
    else
    {
      weaponScriptableObject = null;
      empty = true;
      weaponItem = null;
      weaponItemClass = WeaponScriptableObject.WeaponClass.None;
      weaponID = 0;
      bulletID = 0;
      weaponName = null;
      description = null;
      weaponIcon = null;
      quantity = 0;
      slotImage.sprite = defaultSlotSprite;
      quantityTxtMP.text = quantity.ToString();
    }
    
  }
  
  public void ConsumeItem(Item itemScript)
  {
    ItemScriptableObject scriptableObject = itemScript.itemScriptableObject;
    Debug.Log("you used " + scriptableObject.name + "!") ;
    if (scriptableObject.usable)
    {
      if (scriptableObject.consumable)
      {
        GameObject player = GameObject.FindWithTag("Player");
        PlayerController playerController = player.GetComponent<PlayerController>();
        HealthManager healthManager = player.GetComponent<HealthManager>();
        switch (scriptableObject.ID)
        {
          //food - water
          case 7004: case 7005:
            healthManager.targetRegen += scriptableObject.healthRestore;
            healthManager.regenRate = scriptableObject.healthRestore/ Mathf.RoundToInt(healthManager.updateRate);
            healthManager.currentFood += scriptableObject.foodRestore;
            healthManager.currentWater += scriptableObject.waterRestore;
            if (scriptableObject.ID == 7004)
            {
              playerController.eating = true;
            }

            if (scriptableObject.ID == 7005)
            {
              playerController.drinking = true;
            }
            quantity -= 1;
            break;

          case 7002: case 7001://First Aid Kit - Bandage
            if (healthManager.currentHealth < healthManager.maxHealth )
            {
              playerController.bandaging = true;
              healthManager.currentHealth += scriptableObject.healthRestore;
              healthManager.isBleeding = false;
              quantity -= 1;
            }
            else
            {
              Debug.Log("Health is already full");
            }
            break;
          
          case 7003: // epinephrine
            if (healthManager.currentHealth < healthManager.maxHealth)
            {
              healthManager.currentHealth += scriptableObject.healthRestore;
              quantity -= 1;
            }
            else
            {
              Debug.Log("Health is already full");
            }
            break;
        } 
      }
    }
    
    //TODO: ***implementar efecto de item segun su clase aqui.***
    UpdateItemSlot(itemScript);
    if (quantity <=0)
    {
      empty = true;
      UpdateItemSlot(null);
      slotImage.sprite = null;
    }
    CheckSlotContent();
  }

  public void EquipWeapon(Transform weaponToEquip)
  {
    WeaponItem weaponToEquipWeaponItem = weaponToEquip.GetComponent<WeaponItem>(); // get the weapon script
    Slot slotToEquip = weaponToEquipWeaponItem.holderTarget.GetComponentInParent<Slot>(); //BUSCAR EL SLOT DE EQUIPO CORRESPONDIENTE
    Debug.Log("SlotToEquip is: " + slotToEquip.gameObject.name);

    if (slotToEquip.weaponHolderTransform.childCount > 0)
    {
      inventory.AddWeaponToInventory(slotToEquip.weaponHolderTransform.GetChild(0));
    }

    if (slotToEquip.weaponHolderTransform.childCount <= 0)
    {
      // if the selected equip slot is empty, replace the weapon, then equip
      Debug.Log("slotToEquip is NOT empty");
      if (inventory.playerWeaponHolderTransform.childCount > 0)
      {
        Debug.Log("Player HAS weapon in hands");
        WeaponItem playerWeaponItem = inventory.playerWeaponHolderTransform.GetChild(0).GetComponent<WeaponItem>();
        if (playerWeaponItem.weaponItemClass == weaponToEquipWeaponItem.weaponItemClass)
        {
          // if player have a weapon in hands, and are the same class, replace the weapon, then equip and draw
          Debug.Log("weapons Are the same class!");
          inventory.holsterWeapon = true;
          inventory.drawWeapon = true;
        }
      }
    }

    weaponToEquip.parent = slotToEquip.weaponHolderTransform;
    UpdateWeaponSlot(null);
    UpdateItemSlot(null);
    slotToEquip.UpdateWeaponSlot(weaponToEquipWeaponItem);
    slotToEquip.CheckSlotContent();
  }
  
  public void ShowContextualMenu()
  {
    contextMenu.enabled = showSlotMenu;
    contextMenu.transform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y);
  }

  public void ClearSlot()
  {
    itemScriptableObject = null;
    weaponScriptableObject = null;
    empty = true;
    slotImage.enabled = false;
    quantity = 0;
  }*/
}