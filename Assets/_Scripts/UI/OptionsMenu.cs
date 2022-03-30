using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    
    public Canvas optionsCanvas;
    public Canvas displayCanvas;
    public Canvas graphicsCanvas;
    public Canvas audioCanvas;
    public Canvas inputCanvas;
    private GraphicRaycaster optionsgraphRayCaster;
    private GraphicRaycaster displaygraphRayCaster;
    private GraphicRaycaster graphicsgraphRayCaster;
    private GraphicRaycaster audiographRayCaster;
    private GraphicRaycaster inputgraphRayCaster;
    

    [Header("Display Settings")] 
    public TMP_Dropdown screenResDropdown;
    public TMP_Dropdown displayModeDropdown;
    public Toggle vSyncToggle;
    public Slider gammaSlider;
    

    [Space]
    [Header("Graphics Settings")] 
    public TMP_Dropdown qualityDropdown;
    public TMP_Dropdown texturesDropdown;
    public TMP_Dropdown shadowsDropdown;
    public TMP_Dropdown antiAliasDropdown;
    public Toggle hdrToggle;
    

    public GraphicsManager graphicsManager;
    public Toggle bloomToggle;
    public Toggle dOFToggle;
    public Toggle filmGrainToggle;
    public Toggle vignetteToggle;
    public Toggle chromaAberrToggle;
    [Space] 
    [Header("Audio Settings")] 
    public Slider masterVol;
    public Slider ambientVol;
    public Slider effectVol;
    public TMP_Dropdown audioMode;

    [Space]
    [Header("Input Settings")]
    public KeyAssignments keyAssignments;

    private void OnValidate()
    {

    }

    private void Start()
    {
        optionsgraphRayCaster = optionsCanvas.GetComponent<GraphicRaycaster>();
        displaygraphRayCaster = displayCanvas.GetComponent<GraphicRaycaster>();
        graphicsgraphRayCaster = graphicsCanvas.GetComponent<GraphicRaycaster>();
        audiographRayCaster = audioCanvas.GetComponent<GraphicRaycaster>();
        inputgraphRayCaster = inputCanvas.GetComponent<GraphicRaycaster>();
        
    }

    private void Update()
    {
        optionsgraphRayCaster.enabled = optionsCanvas.enabled;
        displaygraphRayCaster.enabled = displayCanvas.enabled;
        graphicsgraphRayCaster.enabled = graphicsCanvas.enabled;
        audiographRayCaster.enabled = audioCanvas.enabled;
        inputgraphRayCaster.enabled = inputCanvas.enabled;
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
           gameObject.SetActive(false);
        }
        
        
    }

    public void CloseOptionsCanvases()
    {
    optionsCanvas.enabled = false;
    displayCanvas.enabled = false;
    graphicsCanvas.enabled = false;
    audioCanvas.enabled = false;
    inputCanvas.enabled = false;
    
    }

    public void SetGraphicsRaycaster(bool enabled)
    {
        
    }




}
