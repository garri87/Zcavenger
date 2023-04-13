using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Slider = UnityEngine.UI.Slider;

public class UIManager : MonoBehaviour
{
    public MainMenuUI mainMenuUI;

    public LoadingScreenUI loadingScreenUI;

    public InGameOverlayUI inGameOverlayUI;

    public InventoryUI inventoryUI;

    public PauseMenuUI pauseMenuUI;

    public OptionsMenuUI optionsMenuUI;

    public GameOverScreenUI gameOverScreenUI;

    private GameObject[] uiList;

    private void OnEnable()
    {
        uiList = new GameObject[]
        {
            mainMenuUI.gameObject,
            loadingScreenUI.gameObject,
            inGameOverlayUI.gameObject,
            inventoryUI.gameObject,
            pauseMenuUI.gameObject,
            optionsMenuUI.gameObject,
            gameOverScreenUI.gameObject
        };
    }


    public void CloseAllUI()
    {
        foreach (GameObject gameObject in uiList)
        {
            gameObject.SetActive(false);
        }
    }

    public void ToggleUI(GameObject uiGameobject, bool enabled)
    {
        uiGameobject.SetActive(enabled);
    }
}