using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PauseMenuUI : MonoBehaviour
{
    public UIDocument pauseMenu;

    public VisualElement root;
    public Button resumeButton;
    public Button optionsButton;
    public Button restartButton;
    public Button mainMenuButton;
    private UIManager uIManager;



    private void OnEnable()
    {
        uIManager = GameManager.Instance.uiManager;
        pauseMenu = GetComponent<UIDocument>();
        root = pauseMenu.rootVisualElement;

        resumeButton = root.Q<Button>("ResumeButton");
        optionsButton = root.Q<Button>("OptionsButton");
        restartButton = root.Q<Button>("RestartButton");
        mainMenuButton = root.Q<Button>("MainMenuButton");

        resumeButton.RegisterCallback<ClickEvent>(ClosePause);
        optionsButton.RegisterCallback<ClickEvent>(OpenOptions);
        restartButton.RegisterCallback<ClickEvent>(RestartGame);
        mainMenuButton.RegisterCallback<ClickEvent>(GoToMainMenu);


    }

    private void GoToMainMenu(ClickEvent evt)
    {
    }

    private void RestartGame(ClickEvent evt)
    {
        GameManager.Instance.RestartLevel();
    }

    private void OpenOptions(ClickEvent evt)
    {
        uIManager.ToggleUI(uIManager.optionsMenuUI, true);
        OptionsMenuUI optionsUI = uIManager.optionsMenuUI.GetComponent<OptionsMenuUI>();
        optionsUI.CloseAllTabs();
        optionsUI.displayTab.style.display = DisplayStyle.Flex;
    }

    private void ClosePause(ClickEvent evt)
    {
        GameManager.Instance.PauseGame();
    }
}
