using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InspectPanel : MonoBehaviour
{
    public ItemScriptableObject itemScriptableObject;
    public Item weaponItem;
    
    public Transform itemAttribPanelTransform;
    private ItemAttributeField _itemAttributeField;
    
    public TextMeshProUGUI itemNameTextTMP;
    public Image itemImage;
    public TextMeshProUGUI itemDescriptionTMP;
    
    
    
    public enum ItemType
    {
        Item,
        Weapon,
    }
    public ItemType itemType;
    private void OnEnable()
    {
        switch (itemType)
        {
            case ItemType.Item:
                _itemAttributeField = itemAttribPanelTransform.GetChild(0).GetComponent<ItemAttributeField>();
                SetAttribField(_itemAttributeField,"Health Restore",itemScriptableObject.healthRestore);
                _itemAttributeField.gameObject.SetActive(true);
                
                _itemAttributeField = itemAttribPanelTransform.GetChild(1).GetComponent<ItemAttributeField>();
                SetAttribField(_itemAttributeField,"Hunger Restore",itemScriptableObject.foodRestore);
                _itemAttributeField.gameObject.SetActive(true);
                
                _itemAttributeField = itemAttribPanelTransform.GetChild(2).GetComponent<ItemAttributeField>();
                SetAttribField(_itemAttributeField,"Thirst Restore",itemScriptableObject.waterRestore);
                _itemAttributeField.gameObject.SetActive(true);
                
                for (int i = 3; i < itemAttribPanelTransform.childCount; i++)
                {
                    itemAttribPanelTransform.GetChild(i).gameObject.SetActive(false);
                }
                break;
            
            case ItemType.Weapon:
                _itemAttributeField = itemAttribPanelTransform.GetChild(0).GetComponent<ItemAttributeField>();
                SetAttribField(_itemAttributeField,"Damage",weaponItem.damage);
                _itemAttributeField.gameObject.SetActive(true);
                
                _itemAttributeField = itemAttribPanelTransform.GetChild(1).GetComponent<ItemAttributeField>();
                SetAttribField(_itemAttributeField,"Fire Rate", Mathf.RoundToInt(weaponItem.fireRate));
                _itemAttributeField.gameObject.SetActive(true);
                
                _itemAttributeField = itemAttribPanelTransform.GetChild(2).GetComponent<ItemAttributeField>();
                SetAttribField(_itemAttributeField,"Mag Capacity", weaponItem.magazineCap);
                _itemAttributeField.gameObject.SetActive(true);
                
                _itemAttributeField = itemAttribPanelTransform.GetChild(3).GetComponent<ItemAttributeField>();
                SetAttribField(_itemAttributeField,"Recoil",Mathf.RoundToInt(weaponItem.recoilMaxRotation*-1));
                _itemAttributeField.gameObject.SetActive(true);
                
                for (int i = 4; i < itemAttribPanelTransform.childCount; i++)
                {
                    itemAttribPanelTransform.GetChild(i).gameObject.SetActive(false);
                }
                break;
        }
    }

    public void SetAttribField( ItemAttributeField field,string attribName,int attribValue)
    {
        field.attributeNameTMP.text = attribName;
        field.attributeValueTMP.text = attribValue.ToString();
        field.gameObject.SetActive(true);
    }

}

