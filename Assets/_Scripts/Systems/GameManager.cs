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
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
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
    public PlayerController.PlayLine startingPlayline;

    public bool gameOver;



    void Start()
    {
        _graphicsManager = GetComponent<GraphicsManager>();
        _audioManager = GetComponent<AudioManager>();
        _keyAssignments = GetComponent<KeyAssignments>();
        switch (sceneType)
        {
            case SceneType.mainTitle:
                uiManager.mainMenu.SetActive(true);
                uiManager.tipsText.alpha = 0;
                uiManager.loadingScreen.SetActive(false);
                uiManager.versionText.SetText("version: " + version);
                uiManager.inventoryUI.gameObject.SetActive(false);
                _graphicsManager.globalVolume.profile = _graphicsManager.mainTitlevolumeProfile;
                break;

            case SceneType.inLevel:
                player = GameObject.Find("Player");
                playerController = player.GetComponent<PlayerController>();
                playerHealthManager = player.GetComponent<HealthManager>();
                uiManager.pauseMenuUI.gameObject.SetActive(false);
                uiManager.inGameOverlayUI.gameObject.SetActive(true);
                uiManager.inventoryUI.gameObject.SetActive(true);
                _graphicsManager.globalVolume.profile = _graphicsManager.InGamevolumeProfile;
                break;
            
           
        }

    }

    void Update()
    {
        switch (sceneType)
        {
            case SceneType.mainTitle:
                
                uiManager.inventoryUI.gameObject.SetActive(false);
                
                if (Input.GetKeyDown(KeyCode.Escape) )
                {
                    uiManager.optionsMenu.gameObject.SetActive(false);
                    if (!loadingGame)
                    {
                        uiManager.mainMenu.SetActive(true);
                        if (uiManager.optionsMenu.gameObject.activeSelf == true)
                        {
                            uiManager.optionsMenu.gameObject.SetActive(false);
                        }
                    }
                }

                if (loadingGame)
                {
                    if (uiManager.loadingScrnCanvasGroup.alpha < 1)
                    {
                        uiManager.loadingScrnCanvasGroup.alpha += Time.deltaTime * transitionSpeed;
                    }
                    
                    if (uiManager.tipsText.alpha < 255)
                    {
                        uiManager.tipsText.alpha += Time.deltaTime * transitionSpeed;  
                    }
                }
                else
                {
                    uiManager.loadingScrnCanvasGroup.alpha = 0;
                    uiManager.tipsText.alpha = 0;
                }
                break;

            case SceneType.inLevel:

                loadingGame = false;
                uiManager.mainMenu.SetActive(false);
                uiManager.loadingScreen.SetActive(false);
                uiManager.inventoryUI.gameObject.SetActive(true);
                if (playerHealthManager.IsDead)
                {
                    GameOver("YOU ARE DEAD");
                }

                if (playerController.finishedLevel)
                {
                    GameOver("THANKS FOR PLAYING");
                }
                
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    if (!uiManager.optionsMenu.gameObject.activeSelf)
                    {
                        PauseGame();
                    }
                }
                
                if (gamePaused)
                {
            
                    Time.timeScale = 0;
            
                    uiManager.inventoryUICanvas.enabled = false;
                    uiManager.pauseMenuUI.gameObject.SetActive(true);
                }

                if (!gamePaused)
                {
                    Time.timeScale = 1;
                    uiManager.pauseMenuUI.gameObject.SetActive(false);
                    uiManager.optionsMenu.gameObject.SetActive(false);
                }
                
                break;
            
           
        }
        
    }
    

    public void StartGame(int sceneNumber)
    {
        uiManager.tipsText.alpha = 0;
        uiManager.loadingScrnCanvasGroup.alpha = 0;
        loadingGame = true;
        uiManager.loadingScreen.SetActive(true);
        uiManager.mainMenu.SetActive(false);
        
       
        uiManager.tipsText.text = tips[Random.Range(0, tips.Length)];
        uiManager.progressBar.SetActive(true);
        StartCoroutine(LoadAsync(sceneNumber));
    }

    public void PauseGame()
    {
        gamePaused = !gamePaused;
    }
    
    public void RestartLevel()
    {
        Scene scene = SceneManager.GetActiveScene();
        uiManager.gameObject.SetActive(false);
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
        uiManager.gameOverText.text = gameOverMessage;
        uiManager.gameOverCanvas.gameObject.SetActive(true);
        if (uiManager.gameOverCanvasGroup.alpha < 1)
        {
            uiManager.gameOverCanvasGroup.alpha += Time.deltaTime/4;
        }
        else
        {
            uiManager.gameOverCanvasGroup.alpha = 1;

        }

    }
    public void ExitGame()
    {
        Application.Quit();
    }
    
    IEnumerator LoadAsync(int sceneNumber)
    {
        yield return new WaitForSeconds(1);
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(1);
        while (!loadOperation.isDone)
        {
            float progress = Mathf.Clamp01(loadOperation.progress / .9f);
            uiManager.progressBarSlider.value = progress;
            Debug.Log("Scene Loading: " + progress);

            yield return null;
        }
    }
    
    

}

