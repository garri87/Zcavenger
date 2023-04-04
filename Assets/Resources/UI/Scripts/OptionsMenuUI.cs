using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;
using Slider = UnityEngine.UI.Slider;
using Toggle = UnityEngine.UIElements.Toggle;

public class OptionsMenuUI : MonoBehaviour
{
    public UIDocument optionsMenu;
    
    
    //Display Tab
    public Button displayButton;
    public VisualElement displayTab;
    public Dropdown resolutionDropdown;
    public Dropdown displayModeDropdown;
    public Toggle vsyncToggle;
    public Slider gammaSlider;
    
    //Graphics Tab
    public Button graphicsButton;
    public VisualElement graphicsTab;
    public Dropdown qualityDropdown;
    public Dropdown textureDropdown;
    public Dropdown shadowsDropdown;
    public Dropdown antiAliasDropdown;
    public Toggle hdrToggle;
    public Toggle bloomToggle;
    public Toggle dofToggle;
    public Toggle grainToggle;
    public Toggle vignetteToggle;
    public Toggle chrAbrrToggle;
    
    //Input Tab
    public Button inputButton;
    public VisualElement inputTab;
    public TextField[] keyAssigns;
    
    
    //Audio Tab
    public Button audioButton;
    public VisualElement audioTab;

    public Slider masterVolSlider;
    public Slider ambienceVolSlider;
    public Slider effectsVolSlider;
    public Slider weaponsVolSlider;
    public Dropdown audioModeDropdown;
    
    private void OnEnable()
    {
        
    }
}
