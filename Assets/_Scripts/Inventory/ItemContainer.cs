using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using Array = System.Array;

public class ItemContainer : MonoBehaviour
{
    [Header("Container Attributes")]
    [Range(1, 15)] public int containerSize;
    private int minSize = 1;
    private int maxSize = 15;
    public bool randomSize;

    public int lootMultiplier = 1;

    public bool randomItems;

    [Header("#Drop loot here#")]

    public List<ScriptableObject> scriptableObjects = new List<ScriptableObject>();

    [HideInInspector]
    public List<GameObject> itemGOList;

    /// <summary>
    /// List of item quantities in order
    /// </summary>
    public int[] orderedQuantity;

    [SerializeField] private bool interactable;
    [SerializeField] private bool containerOpen;
    public bool containerFilled;

    #region Components

    private UIManager _uiManager;

    public Outline meshOutline;
    private PlayerController playerController;
    private Inventory playerInventory;

    public WorldTextUI _worldTextUI;
    public UIDocument itemContainerUIDocument;
    private KeyAssignments keyassignments = GameManager.Instance._keyAssignments;
    private UIManager _uIManager = GameManager.Instance.uiManager;
    #endregion

    private VisualTreeAsset slotTemplate = InventoryUI.slotTemplate;
    private VisualTreeAsset statTemplate = InventoryUI.statTemplate;

    public GameObject itemTemplatePrefab;
    private void OnValidate()
    {
        OrderQuantities(scriptableObjects);
    }

    private void Awake()
    {
        if (_uiManager)
        {
            itemContainerUIDocument = _uiManager.itemContainerUI.itemContainerUIDocument;
        }
        if (randomSize)
        {
               
            containerSize = Random.Range(minSize, maxSize);
        }


        int index = 0;
        for (int i = 0; i < containerSize; i++)
        {
            if (randomItems)
            {
                itemGOList.Add(GenerateItemGO(scriptableObjects[Random.Range(0, scriptableObjects.Count)]));
            }
            else
            {
                try
                {

                    itemGOList.Add(GenerateItemGO(scriptableObjects[index]));
                    index++;
                }
                catch
                {
                    index = 0;
                    itemGOList.Add(GenerateItemGO(scriptableObjects[index]));
                    index++;
                }
            }
        }

    }

    private void OnEnable()
    {
        meshOutline.enabled = false;
    }

    private void Start()
    {
       // worldUIText.text = "Open [ " + GameManager.Instance._keyAssignments.useKey.keyCode.ToString().ToUpper() + " ]";
    }

    private void Update()
    {
        meshOutline.enabled = interactable;

        if (interactable)
        {
            if (Input.GetKeyDown(GameManager.Instance._keyAssignments.useKey.keyCode))
            {
                containerOpen = true;
            }
            _uiManager.ToggleUI(_uiManager.WorldTextUI.uiDocument,interactable);
            playerInventory.showInventory = containerOpen;
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
            _uiManager.ToggleUI(itemContainerUIDocument, containerOpen);
        }
        else
        {
            _uiManager.ToggleUI(itemContainerUIDocument, containerOpen);
        }


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

    private void OrderQuantities(List<ScriptableObject> scriptableList)
    {
        if (scriptableList.Any())
        {
            if (orderedQuantity.Length != scriptableList.Count)
            {
                
                Array.Resize(ref orderedQuantity, scriptableList.Count);
            }
            for (int i = 0; i < orderedQuantity.Length; i++)
            {
                if (orderedQuantity[i] == 0)
                {
                    int minQuantity = 1 * lootMultiplier;
                    orderedQuantity[i] = Random.Range(minQuantity, minQuantity * lootMultiplier);
                }
            }
        }
    }

    /// <summary>
    /// Creates a item GameObject inside a container
    /// </summary>
    /// <param name="scriptableObj"></param>
    /// <returns></returns>
    public GameObject GenerateItemGO(ScriptableObject scriptableObj)
    {
        GameObject newItemGO = Instantiate(itemTemplatePrefab, transform);
        newItemGO.name = scriptableObj.name;
        Item item = newItemGO.GetComponent<Item>();
        item.scriptableObject = scriptableObj;
        item.itemLocation = Item.ItemLocation.Container;
        item.InitItem();
        newItemGO.SetActive(false);
        return newItemGO;
    }

    
    public void RandomizeQuantities(List<GameObject> list)
    {       
        foreach (GameObject itemGO in list)
        {
            Item item = itemGO.GetComponent<Item>();
            if(item.isStackable){
            item.quantity = Random.Range(1,item.maxStack/2 * lootMultiplier);}
        }
    }

    /// <summary>
    /// transfers itemlist data to UI
    /// </summary>
    public void RefreshContainerUI(List<Item> itemList)
    {
        _uiManager.itemContainerUI.containerSlotArea.Clear();

        if (randomItems)
        {
            for (int i = 0; i < containerSize; i++)
            {
                Item randomItem = itemList[Random.Range(0, itemList.Count)];
                randomItem.quantity = orderedQuantity[i];
                VisualElement slot = slotTemplate.Instantiate();
                Label slotLabel = slot.Q<Label>();
                slotLabel.text = randomItem.quantity.ToString();
                slot.style.backgroundImage = new StyleBackground(randomItem.itemIcon);

                _uiManager.itemContainerUI.containerSlotArea.Add(slot);
            }
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
