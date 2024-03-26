
using UnityEngine;
using UnityEngine.UIElements;


public class UIManager : MonoBehaviour
{
    public GameObject mainMenuUIGO;
    public GameObject difficultyOptionsUIGO;
    public GameObject loadingScreenUIGO;
    public GameObject inGameOverlayUIGO;
    public GameObject inventoryUIGO;
    public GameObject pauseMenuUIGO;
    public GameObject optionsMenuUIGO;
    public GameObject gameOverScreenUIGO;
    public GameObject itemContainerUIGO;
    public GameObject worldTextUIGO;
    
    public MainMenuUI mainMenuUI;
    public DifficultyOptionsUI difficultyOptionsUI;
    public LoadingScreenUI loadingScreenUI;
    public InGameOverlayUI inGameOverlayUI;
    public InventoryUI inventoryUI;
    public PauseMenuUI pauseMenuUI;
    public OptionsMenuUI optionsMenuUI;
    public GameOverScreenUI gameOverScreenUI;
    public ItemContainerUI itemContainerUI;
    public WorldTextUI worldTextUI;


    private UIDocument[] uiDocuments;
    
    
    private void Awake()
    {
        uiDocuments = new UIDocument[]
        {
            mainMenuUI.GetComponent<UIDocument>(),
            difficultyOptionsUI.GetComponent<UIDocument>(),
            loadingScreenUI.GetComponent<UIDocument>(),
            inGameOverlayUI.GetComponent<UIDocument>(),
            inventoryUI.GetComponent<UIDocument>(),
            pauseMenuUI.GetComponent<UIDocument>(),
            optionsMenuUI.GetComponent<UIDocument>(),
            gameOverScreenUI.GetComponent<UIDocument>(),
            itemContainerUI.GetComponent<UIDocument>(),
            worldTextUI.GetComponent<UIDocument>(),

        };


        mainMenuUI = mainMenuUIGO.GetComponent<MainMenuUI>();
        difficultyOptionsUI = difficultyOptionsUIGO.GetComponent<DifficultyOptionsUI>();
        loadingScreenUI = loadingScreenUIGO.GetComponent<LoadingScreenUI>();
        inGameOverlayUI = inGameOverlayUIGO.GetComponent<InGameOverlayUI>();
        inventoryUI = inventoryUIGO.GetComponent<InventoryUI>();
        pauseMenuUI = pauseMenuUIGO.GetComponent<PauseMenuUI>();
        optionsMenuUI = optionsMenuUIGO.GetComponent<OptionsMenuUI>();
        gameOverScreenUI = gameOverScreenUIGO.GetComponent<GameOverScreenUI>();
        itemContainerUI = itemContainerUIGO.GetComponent<ItemContainerUI>();
        worldTextUI = worldTextUIGO.GetComponent<WorldTextUI>();

        foreach (var uiDocument in uiDocuments)
        {
            uiDocument.enabled = true;
        }
    }



    public void CloseAllUI()
    {
        foreach (UIDocument uiDocument in uiDocuments)
        {
            uiDocument.rootVisualElement.style.display = DisplayStyle.None;
        }
    }

    public void ToggleUI(UIDocument uIDocument, bool enabled)
    {
        uIDocument.rootVisualElement.style.display = enabled ? DisplayStyle.Flex : DisplayStyle.None;
    }
}