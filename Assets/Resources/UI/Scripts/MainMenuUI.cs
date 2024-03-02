using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuUI : MonoBehaviour
{
    public UIDocument mainMenu;

    public Button startGameButton;
    public Button optionsButton;
    public Button exitButton;
    public Label versionLabel;

    public VisualElement root;
    private GameManager gameManager;
    private UIManager uiManager;


    private void OnEnable()
    {
        gameManager = GameManager.Instance;
        uiManager = gameManager.uiManager;
        mainMenu = GetComponent<UIDocument>();
        root = mainMenu.rootVisualElement;


        startGameButton = root.Q<Button>("StartGameButton");
        optionsButton = root.Q<Button>("OptionsButton");
        exitButton = root.Q<Button>("ExitButton");
        versionLabel = root.Q<Label>("Version");

        startGameButton.RegisterCallback<ClickEvent>(DifficultyOptions);
        optionsButton.RegisterCallback<ClickEvent>(OptionsMenu);
        exitButton.RegisterCallback<ClickEvent>(ExitGame);

        versionLabel.text = "Version: " + GameManager.Instance.version;

    }

    public void DifficultyOptions(ClickEvent evt)
    {
        uiManager.CloseAllUI();
        uiManager.ToggleUI(uiManager.difficultyOptionsUI, true);
    }

    public void ExitGame(ClickEvent evt)
    {
       gameManager.ExitGame();
    }

    public void OptionsMenu(ClickEvent evt)
    {
        uiManager.ToggleUI(uiManager.optionsMenuUI,true);
    }
}
