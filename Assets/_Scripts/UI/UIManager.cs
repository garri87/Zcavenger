using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    [Header("Main Menu")]
    public GameObject mainMenu;
    public Canvas mainMenuCanvas;
    public TextMeshProUGUI versionText;
    
    [Header("Loading Screen")]
    public GameObject loadingScreen;
    public Canvas loadingScreenCanvas;
    public CanvasGroup loadingScrnCanvasGroup;
    public GameObject progressBar;
    public Slider progressBarSlider;
    public TextMeshProUGUI tipsText;
    
    [Header("In Game Overlay")]
    public Transform inGameOverlayUI;
    public Canvas inGameOverlayCanvas;
    public Transform playerStatsPanel;
    public Transform ammoPanel;
    public Transform quickInventoryUI;
    public Canvas quickInventoryCanvas;

    [Header("Pause Menu, Options Menu & Game Over Panel")]
    public Transform pauseMenuUI;
    public Canvas pauseMenuCanvas;
    private GraphicRaycaster pauseMenuRaycaster;
    public Canvas gameOverCanvas;
    public CanvasGroup gameOverCanvasGroup;
    public TextMeshProUGUI gameOverText;
    public OptionsMenu optionsMenu;
    
    [Header("Inventory Area")]
    public Transform inventoryUI;
    public Canvas inventoryUICanvas;
    private GraphicRaycaster inventoryRaycaster;
    private CanvasGroup inventoryCanvasGroup;
    public Transform inventorySlotArea;
    public Transform capacityPanel;
    public List<Slot> inventorySlotList = new List<Slot>();
    [Space]
    public Transform primaryEquipSlot;
    public Transform secondaryEquipSlot;
    public Transform meleeEquipSlot;
    [Space]
    public Transform headEquipSlot;
    public Transform vestEquipSlot;
    public Transform lowerBodyEquipSlot;
    public Transform throwableEquipSlot;
    public Transform backpackEquipSlot;

    [Header("Inspect & Quick Info Panels")]
    public InspectPanel inspectPanel;
    public Transform quickInfoPanelUI;

    
    

    private void Awake()
    {
        for (int i = 0; i < inventorySlotArea.childCount; i++)
        {
            inventorySlotList.Add(inventorySlotArea.GetChild(i).GetComponent<Slot>());
        }
        
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }
}
