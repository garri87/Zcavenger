using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class UIManager : MonoBehaviour
{
    public GameObject mainMenuUI;

    public GameObject loadingScreenUI;

    public GameObject inGameOverlayUI;

    public GameObject inventoryUI;

    public GameObject pauseMenuUI;

    public GameObject optionsMenuUI;

    public GameObject gameOverScreenUI;

    private GameObject[] uiList;

    private void OnEnable()
    {
        uiList = new GameObject[]
        {
            mainMenuUI,
            loadingScreenUI,
            inGameOverlayUI,
            inventoryUI,
            pauseMenuUI,
            optionsMenuUI,
            gameOverScreenUI
        };
    }


    public void CloseAllUI()
    {
        foreach (GameObject uIObject in uiList)
        {
            uIObject.GetComponent<UIDocument>().rootVisualElement.visible = false;
        }
    }

    public void ToggleUI(GameObject uiGameobject, bool enabled)
    {
        UIDocument uIDocument = uiGameobject.GetComponent<UIDocument>();
        uIDocument.rootVisualElement.visible = enabled;
    }
}