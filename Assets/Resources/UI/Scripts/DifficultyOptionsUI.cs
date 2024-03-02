
using UnityEngine;
using UnityEngine.UIElements;

public class DifficultyOptionsUI : MonoBehaviour
{
    public UIDocument difficultyOptions;

    public Button easyButton,normalButton,hardButton,backButton;

    public VisualElement root;

    private UIManager uiManager;
    private GameManager gameManager;

    private void OnEnable()
    {
        gameManager = GameManager.Instance;
        uiManager = gameManager.uiManager;

        difficultyOptions = GetComponent<UIDocument>();

        root = difficultyOptions.rootVisualElement;

        easyButton = root.Q<Button>("EasyButton");
        normalButton = root.Q<Button>("NormalButton");
        hardButton = root.Q<Button>("HardButton");
        backButton = root.Q<Button>("BackButton");

        easyButton.RegisterCallback<ClickEvent,GameManager.GameDifficulty>(StartGame,GameManager.GameDifficulty.Easy);
        normalButton.RegisterCallback<ClickEvent,GameManager.GameDifficulty>(StartGame, GameManager.GameDifficulty.Normal);
        hardButton.RegisterCallback<ClickEvent, GameManager.GameDifficulty>(StartGame, GameManager.GameDifficulty.Hard);
        backButton.RegisterCallback<ClickEvent>(MainMenu);


    }

    public void StartGame(ClickEvent evt, GameManager.GameDifficulty gameDifficulty)
    {
        gameManager.StartGame(gameDifficulty);
    }

    public void MainMenu(ClickEvent evt)
    {
        uiManager.ToggleUI(uiManager.mainMenuUI, true);
    }

}
