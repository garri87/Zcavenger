using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;
using Unity.Mathematics;


public class ItemContainer : MonoBehaviour
{

    [Range(1,15)] public int containerSize;
    private int minSize = 1;
    private int maxSize = 15;
    public bool randomSize;

    public int lootMultiplier = 1;
    
    public bool randomItems;

    [Header("#Drop loot here#")]
    public List<WeaponScriptableObject> weaponScriptableObjects = new List<WeaponScriptableObject>();
    public List<ItemScriptableObject> itemScriptableObjects = new List<ItemScriptableObject>();
    public List<OutfitScriptableObject> outfitScriptableObjects = new List<OutfitScriptableObject>();

    [HideInInspector]
    public List<Item> itemList;

    /// <summary>
    /// List of item quantities in order
    /// </summary>
    public int[] orderedQuantity;

    [SerializeField] private bool interactable;
    [SerializeField] private bool containerOpen;

    private UIManager _uiManager;
    
    public ItemContainerUI itemContainerUI;
    public TextMeshPro worldUIText;
    public Outline meshOutline;
    private PlayerController playerController;
    private Inventory playerInventory;


    public bool containerFilled;

    private VisualElement slotTemplate;


    private void OnValidate()
    {
        if (itemScriptableObjects.Count > 0 ||
            weaponScriptableObjects.Count > 0||
            outfitScriptableObjects.Count > 0)
        {
            int itemCount = itemScriptableObjects.Count +
                                    weaponScriptableObjects.Count +
                                    outfitScriptableObjects.Count;

            if (orderedQuantity.Length != itemCount)
            {
                orderedQuantity = new int[itemCount];
            }
        }
        
    }

    private void Awake()
    {
        if (randomSize)
        {
            containerSize = Random.Range(minSize, maxSize);
        }

        _uiManager = GameManager.Instance.uiManager;
        itemContainerUI = _uiManager.itemContainerUI.GetComponent<ItemContainerUI>();
       
        GetScriptableObjectsToItem();
        if (randomItems)
        {
            itemList = RandomizeItems(itemList);
        }

        
    }

    private void OnEnable()
    {
        meshOutline.enabled = false;
    }

    private void Start()
    {
        slotTemplate = itemContainerUI.containerSlot;
        worldUIText.text = "Open [ " + GameManager.Instance._keyAssignments.useKey.keyCode.ToString().ToUpper() + " ]";
        
    }
    
    private void Update()
    {
        meshOutline.enabled = interactable;
        worldUIText.gameObject.SetActive(interactable);
        if (interactable)
        {
            if (Input.GetKeyDown(GameManager.Instance._keyAssignments.useKey.keyCode))
            {
                containerOpen = true;
            }

        }
        else
        {
            containerOpen = false;
        }


        if (containerOpen)
        {
            playerController.controllerType = PlayerController.ControllerType.StandByController;
            if (Input.GetKeyDown(GameManager.Instance._keyAssignments.inventoryKey.keyCode) || Input.GetKeyDown(KeyCode.Escape))
            {
                containerOpen = false;
            }
            itemContainerUI.root.style.display = DisplayStyle.Flex;
        }
        else
        {
            itemContainerUI.root.style.display = DisplayStyle.None;

        }

        playerInventory.showInventory = containerOpen;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            interactable = true;

            playerController = other.GetComponent<PlayerController>();
            playerInventory = other.GetComponent<Inventory>();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            interactable = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            interactable = false;
        }
    }


    /// <summary>
    /// Gets scriptable objects and converts to Item Objects in a list
    /// </summary>
    public void GetScriptableObjectsToItem()
    {
        //Generate items list

        
        if(itemScriptableObjects.Count > 0) 
            foreach (ItemScriptableObject itemScriptableObject in itemScriptableObjects)
        {
            Item item = new Item();
            item.GetItemScriptableObject(itemScriptableObject);
            itemList.Add(item);
        }

        if (weaponScriptableObjects.Count > 0) foreach (WeaponScriptableObject weaponScriptableObject in weaponScriptableObjects)
        {
            Item item = new Item();
            item.GetWeaponScriptableObject(weaponScriptableObject);
            itemList.Add(item);
        }

        if (outfitScriptableObjects.Count > 0) foreach (OutfitScriptableObject outfitScriptableObject in outfitScriptableObjects)
        {
            Item item = new Item();
            item.GetOutfitScriptableObject(outfitScriptableObject);
            itemList.Add(item);
        }
    }

    public List<Item> RandomizeItems (List<Item> list)
    {
       List<Item> items = new List<Item>();
        for (int i = 0; i < containerSize; i++)
        {
            Item randomItem = list[Random.Range(0, list.Count)];
            if (randomItem.isStackable)
            {
                randomItem.quantity = Random.Range(1, lootMultiplier);
            }
            items.Add(randomItem);
        }
        return items;   
    }

    /// <summary>
    /// transfers itemlist data to UI
    /// </summary>
    public void RefreshContainerUI(List<Item> itemList)
    { 
        itemContainerUI.containerSlotArea.Clear();

        if (randomItems)
        {
            for (int i = 0; i < containerSize; i++)
            {
                Item randomItem = itemList[Random.Range(0, itemList.Count)];
                randomItem.quantity = orderedQuantity[i];
                VisualElement slot = slotTemplate;
                Label slotLabel = slot.Q<Label>();
                slotLabel.text = randomItem.quantity.ToString();
                slot.style.backgroundImage = new StyleBackground(randomItem.itemIcon);

                itemContainerUI.containerSlotArea.Add(slot);                
            }
        }

    }
    
    
    public void ToggleContainerUI()
    {
        containerOpen = !containerOpen;

        if (containerOpen)
        {
            playerInventory.showInventory = true;
        }
        if(!containerOpen)
        {
            playerInventory.showInventory = false;
        }
    }
    
    public void TakeAll()
    {
       /* for (int i = 0; i < containerSize; i++)
        {
            if (!inventory.inventoryFull)
            {
                Slot contSlotIndex = containerSlotsTransform.GetChild(i).GetComponent<Slot>();

                if (!contSlotIndex.empty && contSlotIndex.itemHolderTransform.childCount > 0)
                {      
                    Item contItemIndex = contSlotIndex.itemHolderTransform.GetChild(0).GetComponent<Item>();
                    inventory.AddItemToInventory(contItemIndex.itemTransform);
                    contSlotIndex.UpdateItemSlot(null);
                } else if (!contSlotIndex.empty && contSlotIndex.weaponHolderTransform.childCount >0)
                {      
                    inventory.AddWeaponToInventory(contSlotIndex.weaponHolderTransform.GetChild(0));
                    contSlotIndex.UpdateWeaponSlot(null);
                }
                else
                {
                    Debug.Log("The container is empty!");
                }
            }
        }*/
    }
}
