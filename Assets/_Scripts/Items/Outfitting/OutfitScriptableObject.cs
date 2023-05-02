using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New_Outfit", menuName = "Outfitting/New Outfit")]
public class OutfitScriptableObject : ScriptableObject
{
    [Header("Item ID")]
    public int ID;
    public string itemName;
    public string description;
    public Sprite itemIcon;
    public Item.EquipmentSlot equipmentSlot;

    [Header("Prefabs")]
    public GameObject outfitPrefab;

    public int defense;

    public int backpackCapacity;
}
