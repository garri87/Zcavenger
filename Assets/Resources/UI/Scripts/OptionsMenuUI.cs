using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;
using Slider = UnityEngine.UIElements.Slider;
using Toggle = UnityEngine.UIElements.Toggle;
using DropdownField = UnityEngine.UIElements.DropdownField;



public class OptionsMenuUI : MonoBehaviour
{
    public UIDocument optionsMenu;

    public VisualElement root;

    //Display Tab
    public Button displayButton;
    public VisualElement displayTab;
    public DropdownField resolutionDropdown;
    public DropdownField displayModeDropdown;
    public Toggle vsyncToggle;
    public Slider gammaSlider;

    //Graphics Tab
    public Button graphicsButton;
    public VisualElement graphicsTab;
    public DropdownField qualityDropdown;
    public DropdownField textureDropdown;
    public DropdownField shadowsDropdown;
    public DropdownField antiAliasDropdown;
    public Toggle hdrToggle;
    public Toggle bloomToggle;
    public Toggle dofToggle;
    public Toggle grainToggle;
    public Toggle vignetteToggle;
    public Toggle chrAbrrToggle;

    //Input Tab
    public Button inputButton;
    public VisualElement inputTab;
    public VisualElement keyAssignPanel;
    public VisualTreeAsset keyAssignTemplate;
    public List<VisualElement> keyAssignList;

    //Audio Tab
    public Button audioButton;
    public VisualElement audioTab;

    public Slider masterVolSlider;
    public Slider ambienceVolSlider;
    public Slider effectsVolSlider;
    public Slider weaponsVolSlider;
    public DropdownField audioModeDropdown;


    //General
    public Button closeButton;
    private GameManager gameManager;
    private GraphicsManager graphicsManager;

    private bool waitForKey;
    private float timer;
    private float keyWaitTime = 10f;
    public KeyCode currentKeyCode;
    private KeyAssignments keyAssignments;
    private KeyStr selectedKey;

    private List<String> qualitiyChoices = new List<string>
        {
        "Very Low",
        "Low",
        "Medium",
        "High",
        "Ultra"
        };

    private List<String> antiAliasChoices = new List<string>
        {
        "Disabled",
        "2X",
        "4X",
        "8X"
        };
    private List<string> textureChoices = new List<string>
        {
         "Quarter","Half","Total"
        };

    private void OnEnable()
    {
        optionsMenu = GetComponent<UIDocument>();
        root = optionsMenu.rootVisualElement;

        gameManager = GameManager.Instance;
        graphicsManager = gameManager._graphicsManager;
        KeyAssignments = KeyAssignments.Instance;

        //Display Tab
        displayButton = root.Q<Button>("DisplayButton");
        displayButton.RegisterCallback<ClickEvent, VisualElement>(OpenTab, displayTab);
        displayTab = root.Q<VisualElement>("DisplayPanel");

        resolutionDropdown = displayTab.Q<DropdownField>("ResolutionDropdown");
        resolutionDropdown.choices.Clear();
        GetResolutions(resolutionDropdown);
        resolutionDropdown.RegisterValueChangedCallback(ChangeResolution);

        displayModeDropdown = displayTab.Q<DropdownField>("ModeDropdown");
        displayModeDropdown.Clear();
        List<String> displayModeChoices = new List<string> { "FullScreen", "Windowed", "Borderless" };
        displayModeDropdown.choices = displayModeChoices;
        displayModeDropdown.RegisterValueChangedCallback(ChangeDisplayMode);

        vsyncToggle = displayTab.Q<Toggle>("VsyncToggle");
        vsyncToggle.RegisterValueChangedCallback(ToggleVsync);

        gammaSlider = displayTab.Q<Slider>("GammaSlider");
        gammaSlider.RegisterValueChangedCallback(SetGamma);



        //Graphics Tab
        graphicsButton = root.Q<Button>("GraphicsButton");
        graphicsButton.RegisterCallback<ClickEvent, VisualElement>(OpenTab, graphicsTab);
        graphicsTab = root.Q<VisualElement>("GraphicsPanel");

        qualityDropdown = graphicsTab.Q<DropdownField>("QualityDropdown");
        qualityDropdown.choices = qualitiyChoices;
        qualityDropdown.choices.Add("Custom");
        qualityDropdown.RegisterValueChangedCallback(ChangeQuality);

        textureDropdown = graphicsTab.Q<DropdownField>("TextureDropdown");

        textureDropdown.choices = textureChoices;
        textureDropdown.RegisterValueChangedCallback(ChangeTextureQuality);


        shadowsDropdown = graphicsTab.Q<DropdownField>("ShadowsDropdown");
        shadowsDropdown.choices = qualitiyChoices;
        shadowsDropdown.RegisterValueChangedCallback(ChangeShadowsQuality);


        antiAliasDropdown = graphicsTab.Q<DropdownField>("AntiAliasDropDown");
        antiAliasDropdown.choices = ;
        antiAliasDropdown.RegisterValueChangedCallback(ChangeAntiAliasQuality);

        hdrToggle = graphicsTab.Q<Toggle>("HDRToggle");
        hdrToggle.RegisterValueChangedCallback(ToggleHDR);

        bloomToggle = graphicsTab.Q<Toggle>("BloomToggle");
        bloomToggle.RegisterValueChangedCallback(ToggleBloom);

        dofToggle = graphicsTab.Q<Toggle>("DOFToggle");
        dofToggle.RegisterValueChangedCallback(ToggleDOF);


        grainToggle = graphicsTab.Q<Toggle>("GrainToggle");
        grainToggle.RegisterValueChangedCallback(ToggleGrain);

        vignetteToggle = graphicsTab.Q<Toggle>("VignetteToggle");
        vignetteToggle.RegisterValueChangedCallback(ToggleVignette);


        chrAbrrToggle = graphicsTab.Q<Toggle>("ChrAbrrToggle");
        chrAbrrToggle.RegisterValueChangedCallback(ToggleChrAbrr);


        //Input Tab
        inputButton = root.Q<Button>("InputButton");
        inputButton.RegisterCallback<ClickEvent, VisualElement>(OpenTab, inputTab);
        inputTab = root.Q<VisualElement>("InputPanel");

        keyAssignPanel = root.Q<VisualElement>("KeyAssignPanel");


        //Audio Tab
        audioButton = root.Q<Button>("AudioButton");
        audioButton.RegisterCallback<ClickEvent, VisualElement>(OpenTab, audioTab);

        audioTab = root.Q<VisualElement>("AudioPanel");

        masterVolSlider = root.Q<Slider>("MasterSlider");
        ambienceVolSlider = root.Q<Slider>("AmbienceSlider");
        effectsVolSlider = root.Q<Slider>("EffectsSlider");
        weaponsVolSlider = root.Q<Slider>("WeaponsSlider");
        audioModeDropdown = root.Q<DropdownField>("AudioModeDropDown");

        //General
        closeButton = root.Q<Button>("CloseButton");
        closeButton.RegisterCallback<ClickEvent>(CloseOptions);
    }



    private void Start()
    {
        //Input Tab
        GetKeyAssigns();
    }

    private void OnGUI()
    {
        if (waitForKey)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {                               
                timer = keyWaitTime;
                waitForKey = false;
            }
            if (Event.current.isKey)
            {
                ListenForKeyCode();
            }
        }

    }

    private void ListenForKeyCode()
    {
        KeyCode newKeyCode = Event.current.keyCode;
        if (newKeyCode != KeyCode.Escape)
        {
            currentKeyCode = newKeyCode;
            KeyAssignments.UpdateKeyBinding(currentKeyName);
            timer = cancelTime;
            waitForKey = false;
        }
        else
        {
            timer = cancelTime;
            waitForKey = false;
        }
    }

    /// <summary>
    /// Populates the ui with key assigns boxes from KeyCode List
    /// </summary>
    public void GetKeyAssigns()
    {
        for (int i = 0; i < keyAssignments.keyCodes.Length; i++)
        {
            VisualElement keyAssign = keyAssignTemplate.Instantiate().Q<VisualElement>("KeyAssign");
            keyAssign.name = "KeyAssign_" + i;

            Label keyValue = keyAssign.Q<Label>("KeyValue");
            Label keyName = keyAssign.Q<Label>("KeyName");
            keyName.text = keyAssignments.keyCodes[i].keyName;

            keyValue.text = keyAssignments.keyCodes[i].keyCode.ToString();
            
            keyValue.RegisterCallback<ClickEvent, string, Label>(UpdateKeycode, keyAssignments.keyCodes[i].keyName, keyValue);
            
            keyAssignPanel.Add(keyAssign);
            keyAssignList.Add(keyAssign);

        }
    }

    private void UpdateKeycode(ClickEvent evt, string keyName, Label label)
    {
        if (evt.button == (int)MouseButton.LeftMouse)
        {
            label.text = "Press any key";
            waitForKey = true;
            selectedKey = key;
        }     
    }

   


    public void CloseOptions(ClickEvent evt)
    {
        root.visible = false;
    }

    public void CloseAllTabs()
    {
        displayTab.visible = false;
        graphicsTab.visible = false;
        inputTab.visible = false;
        audioTab.visible = false;
    }

    public void OpenTab(ClickEvent evt, VisualElement tab)
    {
        CloseAllTabs();
        tab.visible = true;
    }

    public void GetResolutions(DropdownField dropdown)
    {
        // Obtener resoluciones de pantalla soportadas
        Resolution[] resolutions = Screen.resolutions;

        // Limpiar opciones del dropdown de resoluciones
        dropdown.choices.Clear();

        // Agregar cada resolución como opción en el dropdown

        foreach (Resolution resolution in resolutions)
        {
            dropdown.choices.Add(resolution.width.ToString() + "X" + resolution.height.ToString());
        }

        // Establecer resolución actual como opción seleccionada en el dropdown
        int currentResolutionIndex = GetCurrentResolution();
        dropdown.index = currentResolutionIndex;

        int GetCurrentResolution()
        {
            Resolution currentResolution = Screen.currentResolution;
            for (int i = 0; i < resolutions.Length; i++)
            {
                if (resolutions[i].width == currentResolution.width && resolutions[i].height == currentResolution.height)
                {
                    return i;
                }
            }
            return 0;
        }


    }


    public void ChangeResolution(ChangeEvent<string> evt)
    {
        Resolution resolution = new Resolution();
        String[] newResolution = evt.newValue.ToString().Split("X");
        resolution.width = int.Parse(newResolution[0]);
        resolution.height = int.Parse(newResolution[1]);

        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);

    }

    private void ChangeDisplayMode(ChangeEvent<string> evt)
    {
        switch (evt.newValue)
        {
            case "Fullscreen":
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                break;
            case "Windowed":
                Screen.fullScreenMode = FullScreenMode.Windowed;
                break;
            case "Borderless":
                Screen.fullScreenMode = FullScreenMode.MaximizedWindow;
                break;
            default:
                Debug.LogError("Invalid screen mode: " + evt.newValue);
                break;
        }
    }
    private void ToggleVsync(ChangeEvent<bool> evt)
    {
        graphicsManager.ToggleVsync(evt.newValue);
    }

    private void SetGamma(ChangeEvent<float> evt)
    {
        graphicsManager.SetGammaValue(evt.newValue);
    }

    private void ChangeAntiAliasQuality(ChangeEvent<string> evt)
    {
        switch (evt.newValue)
        {
            case "Disabled":
                graphicsManager.ChangeAntiAliasSettings(0);
                break;
            case "2X":
                graphicsManager.ChangeAntiAliasSettings(1);
                break;

            case "4X":
                graphicsManager.ChangeAntiAliasSettings(2);

                break;

            case "8X":
                graphicsManager.ChangeAntiAliasSettings(3);
                break;

            default:
                Debug.Log("Invalid Anti-Alias Index Value: "+ evt.newValue);
                break;
        }
    }

    private void ChangeShadowsQuality(ChangeEvent<string> evt)
    {
        switch (evt.newValue)
        {
            case "Very Low":
                graphicsManager.ChangeShadowSettings(0);
                break;
            case "Low":
                graphicsManager.ChangeShadowSettings(1);
                break;
            case "Medium":
                graphicsManager.ChangeShadowSettings(2);

                break;
            case "High":
                graphicsManager.ChangeShadowSettings(3);
                break;
            case "Ultra":
                graphicsManager.ChangeShadowSettings(4);
                break;
            default:
                Debug.Log("Invalid Shadows Index Value:" + evt.newValue);
                break;
        }
    }

    private void ChangeTextureQuality(ChangeEvent<string> evt)
    {
        switch (evt.newValue)
        {
            case "Quarter":
                graphicsManager.ChangeTextureSettings(0);
                break;
            case "Half":
                graphicsManager.ChangeShadowSettings(1);
                break;
            case "Total":
                graphicsManager.ChangeShadowSettings(2);

                break;
           
            default:
                Debug.Log("Invalid Texture Index Value: " + evt.newValue);
                break;
        }
    }

    private void ChangeQuality(ChangeEvent<string> evt)
    {
        switch (evt.newValue)
        {
            case "Very Low":
                graphicsManager.ChangeQualitySettings(0);
                break;
            case "Low":
                graphicsManager.ChangeQualitySettings(1);
                break;
            case "Medium":
                graphicsManager.ChangeQualitySettings(2);
                break;
            case "High":
                graphicsManager.ChangeQualitySettings(3);
                break;
            case "Ultra":
                graphicsManager.ChangeQualitySettings(4);
                break;
            case "Custom":
                graphicsManager.ChangeQualitySettings(5);
                break;
            default:
                Debug.Log("Invalid Quality Index Value:" + evt.newValue);
                break;
        }
    }
    private void ToggleChrAbrr(ChangeEvent<bool> evt)
    {
        graphicsManager.ToggleChrAberr(evt.newValue);
    }

    private void ToggleVignette(ChangeEvent<bool> evt)
    {
        graphicsManager.ToggleVignette(evt.newValue);

    }

    private void ToggleGrain(ChangeEvent<bool> evt)
    {
        graphicsManager.ToggleGrain(evt.newValue);

    }

    private void ToggleDOF(ChangeEvent<bool> evt)
    {
        graphicsManager.ToggleDoF(evt.newValue);

    }

    private void ToggleBloom(ChangeEvent<bool> evt)
    {
        graphicsManager.ToggleBloom(evt.newValue);

    }

    private void ToggleHDR(ChangeEvent<bool> evt)
    {
        graphicsManager.ToggleHDR(evt.newValue);

    }


}
