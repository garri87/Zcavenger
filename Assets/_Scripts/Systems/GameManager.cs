using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public static GameManager Instance
    {
        get
        {
            
            if (_instance == null)
            {
                Debug.Log("GameManager Instance started");
                _instance = FindObjectOfType<GameManager>();
                if (_instance)
                {
                    Debug.Log("GameManager Instance found");
                }
                else
                {
                    Debug.Log("No GameManager in Instance");
                }
                /*if (_instance == null)
                {
                    GameObject go = new GameObject("GameManager");
                    _instance = go.AddComponent<GameManager>();
                }  */ 
            }
            return _instance;
        }
    }
    
        
    public enum SceneType
    {
        mainTitle,
        inLevel,
    }
    public SceneType sceneType;
    
    #region Player References
    private GameObject player;
    private PlayerController playerController;
    private HealthManager playerHealthManager;
    #endregion

    public UIManager uiManager;
    private LoadingScreenUI loadingScreenUI;

    [HideInInspector] public GraphicsManager _graphicsManager;
    [HideInInspector] public AudioManager _audioManager;
    [HideInInspector] public KeyAssignments _keyAssignments;
    
    [Header("Loading Screen")] 
    public bool loadingGame;
    [SerializeField]private string[] tips;
    public float transitionSpeed = 2;
    
    [Header("Pause Menú")]
    public static bool gamePaused;
    
    [Header("Build Versión")]
    public string version;
    
    [Header("In Level Parameters")]
    public Transform startingPosition;
    public Transform endLevelPosition;
    public float startingPlayline;
    public enum GameDifficulty
    {
        Easy,
        Normal,
        Hard,
    };
    public GameDifficulty gameDifficulty;

    public bool finishedLevel = false;
    public bool gameOver;

    public static Dictionary<Floor.FloorWidth, int> floorWidhts = new Dictionary<Floor.FloorWidth, int>()
    {
        {Floor.FloorWidth.Small, 4},
        {Floor.FloorWidth.Medium, 6},
        {Floor.FloorWidth.Large, 8 }
    };

    public static Dictionary<LevelBuilder.SquareSize, int> squareSizes = new Dictionary<LevelBuilder.SquareSize, int>()
    {
        {LevelBuilder.SquareSize.Small, 5},
        {LevelBuilder.SquareSize.Medium, 7},
        {LevelBuilder.SquareSize.Large,  9}
    };

    public static Dictionary<LevelBuilder.SquareSize, Floor.FloorWidth> squareToBuildingMapping = new Dictionary<LevelBuilder.SquareSize, Floor.FloorWidth>()
    {
        {LevelBuilder.SquareSize.Small, Floor.FloorWidth.Small},
        {LevelBuilder.SquareSize.Medium, Floor.FloorWidth.Medium},
        {LevelBuilder.SquareSize.Large,  Floor.FloorWidth.Large}
    };


    public List<ScriptableObject> scriptableObjects;

    private List<ScriptableObject> GetScriptablesFromResources()
    {
        List<Object> objectList = new List<Object>();
        objectList.AddRange(Resources.LoadAll("ScriptableObjects/Items", typeof(ScriptableObject)));
        objectList.AddRange(Resources.LoadAll("ScriptableObjects/Outfits", typeof(ScriptableObject)));
        objectList.AddRange(Resources.LoadAll("ScriptableObjects/Weapons", typeof(ScriptableObject)));
        objectList.AddRange(Resources.LoadAll("ScriptableObjects/Characters", typeof(ScriptableObject)));

        List<ScriptableObject> scriptableList = objectList.OfType<ScriptableObject>().ToList();

        return scriptableList;
    }


    private void Awake()
    {
#if UNITY_EDITOR
        Debug.unityLogger.logEnabled = true;
#else
        Debug.unityLogger.logEnabled = false;
#endif
        if(!uiManager)uiManager = GetComponentInChildren<UIManager>();


        if(!_graphicsManager)_graphicsManager = GetComponent<GraphicsManager>();
        if(!_audioManager) _audioManager = GetComponent<AudioManager>();
        if(!_keyAssignments) _keyAssignments = GetComponent<KeyAssignments>();
        
    }

    void Start()
    {
        loadingScreenUI = uiManager.loadingScreenUIGO.GetComponent<LoadingScreenUI>();

        loadingGame = false;
        switch (sceneType)
        {
            case SceneType.mainTitle:
                uiManager.CloseAllUI();
                uiManager.ToggleUI(uiManager.mainMenuUI.mainMenuUIDocument,true);
              
                _graphicsManager.globalVolume.profile = _graphicsManager.mainTitlevolumeProfile;
                break;

            case SceneType.inLevel:
                uiManager.CloseAllUI();
                scriptableObjects = GetScriptablesFromResources();
                player = GameObject.Find("Player");
                playerController = player.GetComponent<PlayerController>();
                playerHealthManager = player.GetComponent<HealthManager>();

                uiManager.ToggleUI(uiManager.inGameOverlayUI.inGameOverlayUIDocument,true);

                _graphicsManager.globalVolume.profile = _graphicsManager.InGamevolumeProfile;
                break;
            
           
        }

    }

    void Update()
    {
        switch (sceneType)
        {
            case SceneType.mainTitle:
                                               
                if (Input.GetKeyDown(KeyCode.Escape) )
                {
                    if (!loadingGame)
                    {
                        uiManager.ToggleUI(uiManager.mainMenuUI.mainMenuUIDocument,true);
                    }
                }

                if (loadingGame)
                {
                    uiManager.ToggleUI(uiManager.loadingScreenUI.loadingScreenUIDocument,true);
                }
                break;

            case SceneType.inLevel:
                
                if (playerHealthManager.IsDead)
                {
                    GameOver("YOU ARE DEAD");
                }

                if (finishedLevel)
                {
                    GameOver("THANKS FOR PLAYING");
                }
                
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    PauseGame();
                }
                
                     
                break;
            
           
        }
        
    }
    

    public void StartGame(GameDifficulty difficulty)
    {
        uiManager.CloseAllUI();
        uiManager.ToggleUI(uiManager.loadingScreenUI.loadingScreenUIDocument, true);
        loadingGame = true;
        this.gameDifficulty = difficulty;
        
        StartCoroutine(LoadAsync());
    }

    public void PauseGame()
    {
        gamePaused = !gamePaused;

        if (gamePaused)
        {
            uiManager.ToggleUI(uiManager.pauseMenuUI.pauseMenuUIDocument,true);
            Time.timeScale = 0;
        }
        else
        {
            uiManager.ToggleUI(uiManager.pauseMenuUI.pauseMenuUIDocument, false);
            uiManager.ToggleUI(uiManager.optionsMenuUI.optionsMenuUIDocument, false);
            Time.timeScale = 1;
        }
    }
    
    public void RestartLevel()
    {
        Scene scene = SceneManager.GetActiveScene();
        gamePaused = false;
        Time.timeScale = 1;
        SceneManager.LoadScene(scene.name);
    }

    public void GameOver(string gameOverMessage)
    {
        if (!gameOver)
        {
            Debug.Log("GAME OVER");
        }
        gameOver = true;
        uiManager.ToggleUI(uiManager.gameOverScreenUI.gameOverScreenUIDocument, true);
        uiManager.gameOverScreenUI.ShowGameOver();

    }
    public void ExitGame()
    {
        Debug.Log("Exit Game");
        Application.Quit();
    }
    
    IEnumerator LoadAsync()
    {
        yield return new WaitForSeconds(1);
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(1);
        while (!loadOperation.isDone)
        {
            float progress = Mathf.Clamp01(loadOperation.progress / .9f);
            loadingScreenUI.progressBar.style.width = progress;
            Debug.Log("Scene Loading: " + progress);

            yield return null;
        }
    }
    
    

}

