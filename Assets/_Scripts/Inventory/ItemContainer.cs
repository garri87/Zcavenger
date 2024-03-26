using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using Array = System.Array;
using Exception = System.Exception; 

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
    private KeyAssignments _keyassignments;
    #endregion

    private VisualTreeAsset slotTemplate;
    private VisualTreeAsset statTemplate;

    public GameObject itemTemplatePrefab;
    private void OnValidate()
    {
        OrderQuantities(scriptableObjects);
        
    }

    private void Awake()
    {
        
        
    }

    private void OnEnable()
    {

        meshOutline.enabled = false;
    }
    
    private void Start()
    {
        _uiManager = GameManager.Instance.uiManager;
        if (_uiManager == null)
        {
            Debug.Log("Cannot reach GameManager Instance");
        }

        _keyassignments = GameManager.Instance._keyAssignments;


        if (_uiManager)
        {
            itemContainerUIDocument = _uiManager.itemContainerUI.itemContainerUIDocument;
            _worldTextUI = _uiManager.worldTextUI;
        }
        if (randomSize)
        {

            containerSize = Random.Range(minSize, maxSize);
        }


        int index = 0;
        if (scriptableObjects != null)
        {
            int quantity = 0;
            int order = 0;
            for (int i = 0; i < containerSize; i++)
            {
                
                if (orderedQuantity.Length > i)
                {
                    quantity = orderedQuantity[i];
                    order = 0;
                }
                else
                {
                    quantity = orderedQuantity[order];
                    order++;
                }

                if (randomItems)
                {
                    itemGOList.Add(GenerateItemGO(scriptableObjects[Random.Range(0, scriptableObjects.Count)], quantity));
                }
                else
                {
                    try
                    {
                        itemGOList.Add(GenerateItemGO(scriptableObjects[index],quantity));
                        index++;
                    }
                    catch (Exception e)
                    {
                        Debug.Log("Error creating item object" + e);
                        index = 0;
                        itemGOList.Add(GenerateItemGO(scriptableObjects[index],quantity));
                        index++;
                    }
                }
            }
            
        }
        _worldTextUI.text =  $"Open [ {_keyassignments.useKey.keyCode.ToString().ToUpper()} ]";
        _uiManager.itemContainerUI.inspectItemPanel.style.display = DisplayStyle.None;

    }

    private void Update()
    {
        meshOutline.enabled = interactable;

        if (interactable)
        {
            if (Input.GetKeyDown(_keyassignments.useKey.keyCode) || Input.GetKeyDown(_keyassignments.inventoryKey.keyCode))
            {
                containerOpen = !containerOpen;
                _uiManager.itemContainerUI.FillInventoryWithSlots(containerSize);
                _uiManager.ToggleUI(itemContainerUIDocument, containerOpen);

            }
            _uiManager.ToggleUI(_uiManager.worldTextUI.uiDocument,!containerOpen);
            playerInventory.showInventory = containerOpen;
        }
        else
        {
            containerOpen = false;
        }


        if (containerOpen)
        {
            playerController.controllerType = PlayerController.ControllerType.StandByController;
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                containerOpen = false;
            }
        }
        _uiManager.ToggleUI(itemContainerUIDocument, containerOpen);



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
    public GameObject GenerateItemGO(ScriptableObject scriptableObj, int quantity)
    {
        GameObject newItemGO = Instantiate(itemTemplatePrefab, transform);
        newItemGO.name = scriptableObj.name;
        Item item = newItemGO.GetComponent<Item>();
        item.GetScriptableObject(scriptableObj);
        item.itemLocation = Item.ItemLocation.Container;
        item.quantity = quantity;
        item.InitItem();
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
