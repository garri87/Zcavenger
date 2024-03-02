
using UnityEngine;
using UnityEngine.UIElements;


public class UIManager : MonoBehaviour
{
    public GameObject mainMenuUI;
    public GameObject difficultyOptionsUI;
    public GameObject loadingScreenUI;
    public GameObject inGameOverlayUI;
    public GameObject inventoryUI;
    public GameObject pauseMenuUI;
    public GameObject optionsMenuUI;
    public GameObject gameOverScreenUI;
    public GameObject itemContainerUI;

    public MainMenuUI mainMenuUIDocument;
    public DifficultyOptionsUI difficultyOptionsUIDocument;
    public LoadingScreenUI loadingScreenUIDocument;
    public InGameOverlayUI inGameOverlayUIDocument;
    public InventoryUI inventoryUIDocument;
    public PauseMenuUI pauseMenuUIDocument;
    public OptionsMenuUI optionsMenuUIDocument;
    public GameOverScreenUI gameOverScreenUIDocument;
    public ItemContainerUI itemContainerUIDocument;


    private GameObject[] uiList;


    private void OnEnable()
    {
        uiList = new GameObject[]
        {
            mainMenuUI,
            difficultyOptionsUI,
            loadingScreenUI,
            inGameOverlayUI,
            inventoryUI,
            pauseMenuUI,
            optionsMenuUI,
            gameOverScreenUI,
            itemContainerUI,

        };


        mainMenuUIDocument = mainMenuUI.GetComponent<MainMenuUI>();
        difficultyOptionsUIDocument = difficultyOptionsUI.GetComponent<DifficultyOptionsUI>();
        loadingScreenUIDocument = loadingScreenUI.GetComponent<LoadingScreenUI>();
        inGameOverlayUIDocument = inGameOverlayUI.GetComponent<InGameOverlayUI>();
        inventoryUIDocument = inventoryUI.GetComponent<InventoryUI>();
        pauseMenuUIDocument = pauseMenuUI.GetComponent<PauseMenuUI>();
        optionsMenuUIDocument = optionsMenuUI.GetComponent<OptionsMenuUI>();
        gameOverScreenUIDocument = gameOverScreenUI.GetComponent<GameOverScreenUI>();
        itemContainerUIDocument = itemContainerUI.GetComponent<ItemContainerUI>();

        foreach (var ui in uiList)
        {
            ui.GetComponent<UIDocument>().enabled = true;
        }
    }



    public void CloseAllUI()
    {
        foreach (GameObject uiObject in uiList)
        {
            uiObject.GetComponent<UIDocument>().rootVisualElement.style.display = DisplayStyle.None;
    
        }
    }

    public void ToggleUI(GameObject uiGameobject, bool enabled)
    {
        UIDocument uIDocument = uiGameobject.GetComponent<UIDocument>();

        if (enabled)
        {
            uIDocument.rootVisualElement.style.display = DisplayStyle.Flex;
        }
        else
        {
            uIDocument.rootVisualElement.style.display = DisplayStyle.None;
        }
    }
}