using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlotContextMenu : MonoBehaviour
{
    public Button useButton;
    public Button equipButton;
    public Button throwButton;
    public Button inspectButton;

    public GameObject itemCollectTemplate;
    private Canvas _canvas;
    private Slot slot;
    public InspectPanel inspectPanel;
    
    

    private void Awake()
    {
        slot = GetComponentInParent<Slot>();
        _canvas = GetComponent<Canvas>();
        _canvas.enabled = false;
    }

    private void Update()
    {
        if (_canvas.enabled == true)
        {
            if (slot.itemScriptableObject == null )
            {
                useButton.interactable = false;
            }
            else
            {
                useButton.interactable = true;
            }
            

            if (slot.weaponScriptableObject == null)
            {
                equipButton.interactable = false;
            }
            else
            {
                equipButton.interactable = true;
            }
        }
    }

    public void UseButton()
    {
        if (slot.inventory.showInventory)
        {
            slot.ConsumeItem(slot._item);
        }
        slot.showSlotMenu = false;

    }

    public void EquipButton()
    {
        if (slot.weaponHolderTransform.childCount > 0)
        {
          slot.EquipWeapon(slot.weaponHolderTransform.GetChild(0));
          slot.showSlotMenu = false;
        }
        else
        {
            Debug.Log("Nothing to Equip Here!");
        }
    }

    public void ThrowButton()
    {            
        Transform playerPos = GameObject.Find("Player").GetComponent<Transform>();

        if (slot.weaponHolderTransform.childCount >0)
        {
            Transform weaponToThrow = slot.weaponHolderTransform.GetChild(0);
            WeaponItem weaponToThrowItem = weaponToThrow.GetComponent<WeaponItem>();
            weaponToThrow.position = playerPos.position + Vector3.up;
            weaponToThrow.rotation = Quaternion.Euler(0,90,0); 
            weaponToThrowItem.weaponPicked = false;
            weaponToThrow.parent = null;
            weaponToThrowItem.weaponLocation = WeaponItem.WeaponLocation.World;
            weaponToThrow.gameObject.SetActive(true);
            slot.UpdateWeaponSlot(null);

        }else if (slot._item != null)
        {
            GameObject itemToThrow = Instantiate(itemCollectTemplate, playerPos.position + Vector3.up,
                Quaternion.Euler(0, 0, 0));
            Item itemToThrowItem = itemToThrow.GetComponent<Item>();
            itemToThrowItem.itemScriptableObject = slot.itemScriptableObject;
            itemToThrowItem.GetItemScriptableObject(slot.itemScriptableObject);
            itemToThrowItem.quantity = slot.quantity;
            itemToThrowItem.itemLocation = Item.ItemLocation.World;
            slot.UpdateItemSlot(null);
        }
        
        slot.showSlotMenu = false;
        
    }

    public void InspectButton()
    {
        if (!slot.empty)
        {
            
            if (slot.itemScriptableObject != null)
            {
                inspectPanel.itemType = InspectPanel.ItemType.Item;
                inspectPanel.itemScriptableObject = slot.itemScriptableObject;
                inspectPanel.itemNameTextTMP.text = slot.itemScriptableObject.itemName;
                inspectPanel.itemDescriptionTMP.text = slot.itemScriptableObject.description;
            }
            else if (slot.weaponItem != null)
            {
                inspectPanel.itemType = InspectPanel.ItemType.Weapon;
                inspectPanel.weaponItem = slot.weaponItem;
                inspectPanel.itemNameTextTMP.text = slot.weaponItem.weaponName;
                inspectPanel.itemDescriptionTMP.text = slot.weaponItem.description;
            }

            inspectPanel.itemImage.sprite = slot.slotImage.sprite;
            inspectPanel.gameObject.SetActive(true);
        }
        slot.showSlotMenu = false;
    }

}

