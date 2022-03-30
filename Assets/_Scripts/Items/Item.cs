using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class Item : MonoBehaviour
{
    public ItemScriptableObject itemScriptableObject;
    
    [Header("Item ID")]
    public int ID;
    public string itemName;
    public string description;
    public Sprite itemIcon;
    public GameObject itemPrefab;
    
    [Header("Item Attributes")]
    
    public int healthRestore;
    public int foodRestore;
    public int waterRestore;
    
    public bool usable;
    public bool consumable;
    public bool isStackable;
    public int maxStack;
    
    public enum ItemLocation
    {
        World,
        Container,
        Inventory,
        
    }
    public ItemLocation itemLocation;
    
    public int quantity = 1;
    
    public bool itemPicked;

    [Header("Transform References")]
    public Transform prefabHolder;
    public Transform textTransform;
    public TextMeshPro itemText;
    private BoxCollider _boxCollider;
    public Outline outline;

    public float prefabRotationSpeed = 2f;

    [HideInInspector]public Transform itemTransform;


    private void OnValidate()
    {
        if (itemScriptableObject!=null)
        {
            itemText.text = itemScriptableObject.itemName;
        }
    }

    private void Awake()
    {
        _boxCollider = GetComponent<BoxCollider>();
    }

    private void OnEnable()
    {
        itemTransform = this.transform;
    }

    private void Start()
    {
        if (itemScriptableObject !=null)
        {
            GetItemScriptableObject(itemScriptableObject);
        }
        else
        {
            Debug.Log("No ItemScriptable Object attached on " + gameObject.name);
        }
        switch (itemLocation)
        {
            case ItemLocation.World:
                _boxCollider.enabled = true;
                itemText = textTransform.GetComponent<TextMeshPro>();
                textTransform.gameObject.SetActive(false);
                itemText.text = itemName;
                InstantiateItem(itemPrefab);
                itemPicked = false;
                break;
            
            case ItemLocation.Container:
                _boxCollider.enabled = false;
                gameObject.SetActive(false);
                itemPicked = false;
                break;
            
            case ItemLocation.Inventory:
                _boxCollider.enabled = false;
                gameObject.SetActive(false);
                itemPicked = true;
                break;
        }    }

    private void Update()
    {
        switch (itemLocation)
        {
            case ItemLocation.World:
                _boxCollider.enabled = true;
                prefabHolder.Rotate(Vector3.up * Time.deltaTime * prefabRotationSpeed);
                break;
            
            case ItemLocation.Container:
                _boxCollider.enabled = false;
                break;
            
            case ItemLocation.Inventory:
                _boxCollider.enabled = false;
                gameObject.SetActive(false);
                break;
            
        }
    }

    public void InstantiateItem(GameObject prefab)
    {
        GameObject instantiatedItem = Instantiate(prefab, prefabHolder.position, prefabHolder.rotation);
        instantiatedItem.transform.parent = prefabHolder;
        //instantiatedItem.GetComponent<Item>().itemLocation = ItemLocation.Holder;
        instantiatedItem.SetActive(true);
        instantiatedItem.AddComponent<Outline>();
        outline = instantiatedItem.GetComponent<Outline>();
        outline.OutlineWidth = 1;
        outline.OutlineMode = Outline.Mode.OutlineAll;
        outline.OutlineColor = Color.white;
        outline.enabled = false;
    }

    public void GetItemScriptableObject(ItemScriptableObject itemScriptableObject)
    {
        ID = itemScriptableObject.ID;
        itemName = itemScriptableObject.itemName;
        description = itemScriptableObject.description;
        itemIcon = itemScriptableObject.itemIcon; 
        itemPrefab = itemScriptableObject.itemPrefab;
        usable = itemScriptableObject.usable;
        consumable = itemScriptableObject.consumable;
        isStackable = itemScriptableObject.isStackable;
        maxStack = itemScriptableObject.maxStack;
        healthRestore = itemScriptableObject.healthRestore;
        foodRestore = itemScriptableObject.foodRestore;
        waterRestore = itemScriptableObject.waterRestore;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && itemLocation == ItemLocation.World)
        {
            textTransform.gameObject.SetActive(true);
            outline.enabled = true;

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && itemLocation == ItemLocation.World)
        {
            textTransform.gameObject.SetActive(false);
            outline.enabled = false;

        }
    }
}
