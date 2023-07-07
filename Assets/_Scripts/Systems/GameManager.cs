using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();
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

    public bool finishedLevel = false;
    public bool gameOver;


    private void Awake()
    {
#if UNITY_EDITOR
        Debug.unityLogger.logEnabled = true;
#else
        Debug.unityLogger.logEnabled = false;
#endif
        uiManager = GetComponentInChildren<UIManager>();


        _graphicsManager = GetComponent<GraphicsManager>();
        _audioManager = GetComponent<AudioManager>();
        _keyAssignments = GetComponent<KeyAssignments>();
    }

    void Start()
    {
        loadingScreenUI = uiManager.loadingScreenUI.GetComponent<LoadingScreenUI>();

        loadingGame = false;
        switch (sceneType)
        {
            case SceneType.mainTitle:
                uiManager.CloseAllUI();
                uiManager.ToggleUI(uiManager.mainMenuUI,true);
              
                _graphicsManager.globalVolume.profile = _graphicsManager.mainTitlevolumeProfile;
                break;

            case SceneType.inLevel:
                uiManager.CloseAllUI();
                player = GameObject.Find("Player");
                playerController = player.GetComponent<PlayerController>();
                playerHealthManager = player.GetComponent<HealthManager>();

                uiManager.ToggleUI(uiManager.inGameOverlayUI,true);

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
                        uiManager.ToggleUI(uiManager.mainMenuUI,true);
                    }
                }

                if (loadingGame)
                {
                    uiManager.ToggleUI(uiManager.loadingScreenUI,true);
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
    

    public void StartGame()
    {
        uiManager.CloseAllUI();
        uiManager.ToggleUI(uiManager.loadingScreenUI, true);
        loadingScreenUI.hintsLabel.text = tips[Random.Range(0, tips.Length)];
        loadingGame = true;
              
        StartCoroutine(LoadAsync());
    }

    public void PauseGame()
    {
        gamePaused = !gamePaused;

        if (gamePaused)
        {
            uiManager.ToggleUI(uiManager.pauseMenuUI,true);
            Time.timeScale = 0;
        }
        else
        {
            uiManager.ToggleUI(uiManager.pauseMenuUI, false);
            uiManager.ToggleUI(uiManager.optionsMenuUI, false);
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
        uiManager.ToggleUI(uiManager.gameOverScreenUI, true);
        uiManager.gameOverScreenUIDocument.ShowGameOver();

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

