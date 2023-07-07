using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New_Item", menuName = "Items/New Item")]
public class ItemScriptableObject : ScriptableObject
{
    
    [Header("Item ID")]
    public int ID;
    public string itemName;
    public string description;
    public Sprite itemIcon;
    
    [Header("Prefabs")]
    public GameObject itemPrefab;

    [Header("Properties")]
    public bool usable;
    public bool consumable;
    public bool isStackable;
    public int maxStack;
    public int minLootQuantity;
    public int maxLootQuantity;
    
    public int healthRestore;
    public int foodRestore;
    public int waterRestore;
    
}
