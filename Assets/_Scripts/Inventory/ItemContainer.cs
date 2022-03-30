using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ItemContainer : MonoBehaviour
{
    [Range(1,15)] public int containerSize;
    private int minSize = 1;
    private int maxSize = 15;

    public int lootMultiplier = 1;
    
    public bool randomSize;
    public bool randomWeapons;
    public bool randomItems;
    
    
    
    [SerializeField]private bool interactable;
    [SerializeField]private bool containerOpen;
    public bool containerFilled;

    [Header("#Drop loot here#")]
    public List<GameObject> weaponPrefabs = new List<GameObject>();
    public List<ItemScriptableObject> itemScriptableObjects = new List<ItemScriptableObject>();
    public int[] orderedQuantity;

    private Inventory inventory;
    private UIManager _uiManager;
    
    public GameObject containerUIWindow;
    public GameObject worldUIText;
    public TextMeshPro worldUITMP;
    public Outline meshOutline;
    private PlayerController playerController;
    public GameObject itemTemplate;
    public Transform containerSlotsTransform;

    private void OnValidate()
    {
        if (orderedQuantity.Length != itemScriptableObjects.Count)
        {
            orderedQuantity = new int[itemScriptableObjects.Count];
        }
    }

    private void Awake()
    {
        containerUIWindow.SetActive(true);
        containerUIWindow.GetComponent<Canvas>().enabled = true;
        if (randomSize)
        {
            containerSize = Random.Range(minSize, maxSize);
        }
        FillContainer();
    }

    private void OnEnable()
    {
        meshOutline.enabled = false;
    }

    private void Start()
    {
       // InstantiateItems();
        worldUITMP.text = "Open [ " + KeyAssignments.SharedInstance.useKey.keyCode.ToString().ToUpper() + " ]";
    }
    
    private void Update()
    {
        meshOutline.enabled = interactable;

        if (interactable)
        {
            worldUIText.SetActive(true);

            if (Input.GetKeyDown(KeyAssignments.SharedInstance.useKey.keyCode))
            {
                ToggleContainerUI();
            }

        }
        else
        {
            worldUIText.SetActive(false);
            containerOpen = false;
        }
        if (containerOpen)
        {
            worldUIText.SetActive(false);
            containerUIWindow.SetActive(true);
            playerController.controllerType = PlayerController.ControllerType.StandByController;
            if (Input.GetKeyDown(KeyAssignments.SharedInstance.inventoryKey.keyCode) || Input.GetKeyDown(KeyCode.Escape))
            {
                containerUIWindow.SetActive(false);
                containerOpen = false;
            }
        }

        if (!containerOpen)
        {
           containerUIWindow.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerController = other.GetComponent<PlayerController>();
            interactable = true;
            inventory = other.GetComponent<Inventory>();
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


    public void FillContainer()
    {
        int selectedWeapon;
        int selectedItem;
        int quantity = 0;
        if (weaponPrefabs.Count > 0)
        {
            for (int i = 0; i < weaponPrefabs.Count; i++)
            {
                if (i > containerSize)
                {
                    return;
                }
                else
                {
                   Slot slotIndex = containerSlotsTransform.GetChild(i).GetComponent<Slot>();;
                    GameObject instantiatedWeapon;
                    if (randomWeapons)
                    {
                        selectedWeapon = Random.Range(0, weaponPrefabs.Count);
                    }
                    else
                    {
                        selectedWeapon = i;
                    }

                    instantiatedWeapon = Instantiate(weaponPrefabs[selectedWeapon], slotIndex.weaponHolderTransform);
                    WeaponItem weaponItem = instantiatedWeapon.GetComponent<WeaponItem>();
                    slotIndex.weaponItem = weaponItem;
                    weaponItem.weaponLocation = WeaponItem.WeaponLocation.Container;
                    slotIndex.UpdateWeaponSlot(weaponItem);
                }
            }
        }

        if (itemScriptableObjects.Count > 0)
        {
            for (int i = 0; i < itemScriptableObjects.Count; i++)
            {
                if (i + weaponPrefabs.Count >= containerSize)
                {
                    return;
                }
                else
                {
                    Slot slotIndex = containerSlotsTransform.GetChild(i + weaponPrefabs.Count).GetComponent<Slot>();
                    
                    GameObject instantiatedTemplate = Instantiate(itemTemplate, slotIndex.itemHolderTransform);
                    instantiatedTemplate.SetActive(false);
                    Item templateItem = instantiatedTemplate.GetComponent<Item>();

                    if (randomItems)
                    {
                        selectedItem = Random.Range(0, itemScriptableObjects.Count);
                        quantity = orderedQuantity[Random.Range(0, orderedQuantity.Length)] * lootMultiplier;
                    }
                    else
                    {
                        selectedItem = i;
                        quantity = orderedQuantity[i] * lootMultiplier;
                    }
                    templateItem.itemScriptableObject = itemScriptableObjects[selectedItem];
                    templateItem.quantity = quantity;
                    templateItem.GetItemScriptableObject(templateItem.itemScriptableObject);
                    templateItem.itemLocation = Item.ItemLocation.Container;
                    slotIndex._item = templateItem;
                    slotIndex.quantity = templateItem.quantity;
                }
            }
        }
    }
    
    
    public void ToggleContainerUI()
    {
        containerOpen = !containerOpen;

        if (containerOpen)
        {
            containerUIWindow.SetActive(true);
            inventory.showInventory = true;

        }
        if(!containerOpen)
        {
            containerUIWindow.SetActive(false);
            inventory.showInventory = false;

        }
    }
    
    public void TakeAll()
    {
        for (int i = 0; i < containerSize; i++)
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
        }
    }
}
