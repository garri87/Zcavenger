using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using Bloom = UnityEngine.Rendering.Universal.Bloom;
using ChromaticAberration = UnityEngine.Rendering.Universal.ChromaticAberration;
using DepthOfField = UnityEngine.Rendering.Universal.DepthOfField;
using ShadowQuality = UnityEngine.ShadowQuality;
using Vignette = UnityEngine.Rendering.Universal.Vignette;

public class GraphicsManager : MonoBehaviour
{
    
    public UniversalRenderPipelineAsset 
        ultraSettingsURPAsset,
        highSettingsURPAsset,
        mediumSettingsURPAsset,
        lowSettingsURPAsset,
        veryLowSettingsURPAsset,
        customSettingsURPAsset;
    
    public RenderPipelineAsset currentRenderPipelineAsset;
    
    #region Postprocessing
    public Volume globalVolume;
    public VolumeProfile mainTitlevolumeProfile;
    public VolumeProfile InGamevolumeProfile;
    
    private Bloom _bloom = null;
    private DepthOfField _dof = null;
    private FilmGrain _grain = null;
    private Vignette _vignette = null;
    private ChromaticAberration _chrAberr = null;
    private LiftGammaGain _liftGammaGain;

    #endregion
    
    public OptionsMenu optionsMenu;
    private GameManager _gameManager;
    
    private void OnValidate()
    {
        _gameManager = GetComponent<GameManager>();
        switch (_gameManager.sceneType)
        {
            case GameManager.SceneType.inLevel:
                globalVolume.profile = InGamevolumeProfile;
                getVolumeProfileOverrides(InGamevolumeProfile);
                break;
            
            case GameManager.SceneType.mainTitle:
                globalVolume.profile = mainTitlevolumeProfile;
                getVolumeProfileOverrides(mainTitlevolumeProfile);
                break;
        }
        
        optionsMenu.qualityDropdown.value = QualitySettings.GetQualityLevel();
        currentRenderPipelineAsset = QualitySettings.renderPipeline;
        
    }

    public void getVolumeProfileOverrides(VolumeProfile profile)
    {
        profile.TryGet<Bloom>(out _bloom);
        profile.TryGet<DepthOfField>(out _dof);
        profile.TryGet<FilmGrain>(out _grain);
        profile.TryGet<Vignette>(out _vignette);
        profile.TryGet<ChromaticAberration>(out _chrAberr);
        profile.TryGet<LiftGammaGain>(out _liftGammaGain);
    }
    void Awake()
    {
        optionsMenu.screenResDropdown.value = PlayerPrefs.GetInt("userResolution");
        optionsMenu.displayModeDropdown.value = PlayerPrefs.GetInt("userDisplayMode");
        
        optionsMenu.qualityDropdown.value = PlayerPrefs.GetInt("userQualitySettings");
        
        optionsMenu.texturesDropdown.value = PlayerPrefs.GetInt("userTextureQuality");
        optionsMenu.antiAliasDropdown.value = PlayerPrefs.GetInt("userAntiAlias");
        optionsMenu.hdrToggle.isOn = Convert.ToBoolean(PlayerPrefs.GetInt("userHDR"));
        

        if (globalVolume != null)
        {
            _bloom.active = Convert.ToBoolean(PlayerPrefs.GetInt("userBloom"));
            _dof.active = Convert.ToBoolean(PlayerPrefs.GetInt("userDof"));
            _grain.active = Convert.ToBoolean(PlayerPrefs.GetInt("userGrain"));
            _vignette.active = Convert.ToBoolean(PlayerPrefs.GetInt("userVignette"));
            _chrAberr.active = Convert.ToBoolean(PlayerPrefs.GetInt("userChrAberr"));
            optionsMenu.gammaSlider.value = PlayerPrefs.GetFloat("userGamma");

            _liftGammaGain.gamma.overrideState = true;
            optionsMenu.bloomToggle.isOn = _bloom.active;
            optionsMenu.dOFToggle.isOn = _dof.active;
            optionsMenu.filmGrainToggle.isOn = _grain.active;
            optionsMenu.vignetteToggle.isOn = _vignette.active;
            optionsMenu.chromaAberrToggle.isOn = _chrAberr.active;

        }
    }

    // Update is called once per frame
    void Update()
    {
        //currentRenderPipelineAsset = GraphicsSettings.renderPipelineAsset;
    }

    #region Graphics Settings

    public void ChangeQualitySettings(int value)
    {
        
        switch (value)
        {
            case 0://very low
                //QualitySettings.renderPipeline = veryLowSettingsURPAsset;
                //textures
                optionsMenu.texturesDropdown.value = 0;
                //shadows
                optionsMenu.shadowsDropdown.value = 0;
                //AA
                optionsMenu.antiAliasDropdown.value = 0;
                //HDR
                optionsMenu.hdrToggle.isOn = false;
                PlayerPrefs.SetInt("userTextureQuality", 0);
                PlayerPrefs.SetInt("userShadowQuality", 0);
                PlayerPrefs.SetInt("userAntiAlias", 0);
                PlayerPrefs.SetInt("userHDR",0);
                Debug.Log("changed to very low Settings");
                break;
            
            case 1://low
                //QualitySettings.renderPipeline = lowSettingsURPAsset;
                //textures
                optionsMenu.texturesDropdown.value = 1;
                //shadows
                optionsMenu.shadowsDropdown.value = 1;
                //AA
                optionsMenu.antiAliasDropdown.value = 1;
                //HDR
                optionsMenu.hdrToggle.isOn = false;
                PlayerPrefs.SetInt("userTextureQuality", 1);
                PlayerPrefs.SetInt("userShadowQuality", 1);
                PlayerPrefs.SetInt("userAntiAlias", 1);
                PlayerPrefs.SetInt("userHDR",0);
                Debug.Log("changed to low Settings");
                break;
            
            case 2://medium
               // QualitySettings.renderPipeline = mediumSettingsURPAsset;
                //textures
                optionsMenu.texturesDropdown.value = 2;
                //shadows
                optionsMenu.shadowsDropdown.value = 2;
                //AA
                optionsMenu.antiAliasDropdown.value = 2;
                //HDR
                optionsMenu.hdrToggle.isOn = true;
               PlayerPrefs.SetInt("userTextureQuality", 2);
               PlayerPrefs.SetInt("userShadowQuality", 2);
               PlayerPrefs.SetInt("userAntiAlias", 2);
               PlayerPrefs.SetInt("userHDR",1);

                Debug.Log("changed to medium Settings");
                break;
            
            case 3://high
               // QualitySettings.renderPipeline = highSettingsURPAsset;
                //textures
                optionsMenu.texturesDropdown.value = 3;
                //shadows
                optionsMenu.shadowsDropdown.value = 3;
                //AA
                optionsMenu.antiAliasDropdown.value = 2;
                //HDR
                optionsMenu.hdrToggle.isOn = true;
               PlayerPrefs.SetInt("userTextureQuality", 3);
               PlayerPrefs.SetInt("userShadowQuality", 3);
               PlayerPrefs.SetInt("userAntiAlias", 2);
               PlayerPrefs.SetInt("userHDR",1);

                Debug.Log("changed to high Settings");
                break;
            
            case 4://Very High
              //  QualitySettings.renderPipeline = ultraSettingsURPAsset;
                //textures
                optionsMenu.texturesDropdown.value = 4;
                //shadows
                optionsMenu.shadowsDropdown.value = 4;
                //AA
                optionsMenu.antiAliasDropdown.value = 3;
                //HDR
                optionsMenu.hdrToggle.isOn = true;
              PlayerPrefs.SetInt("userTextureQuality", 4);
              PlayerPrefs.SetInt("userShadowQuality", 4);
              PlayerPrefs.SetInt("userAntiAlias", 3);
              PlayerPrefs.SetInt("userHDR",1);
                Debug.Log("changed to Ultra Settings");
                break;
            
            case 5://ultra
               // QualitySettings.renderPipeline = ultraSettingsURPAsset;
                //textures
                optionsMenu.texturesDropdown.value = 4;
                //shadows
                optionsMenu.shadowsDropdown.value = 4;
                //AA
                optionsMenu.antiAliasDropdown.value = 3;
                //HDR
                optionsMenu.hdrToggle.isOn = true;
               PlayerPrefs.SetInt("userTextureQuality", 4);
               PlayerPrefs.SetInt("userShadowQuality", 4);
               PlayerPrefs.SetInt("userAntiAlias", 3);
               PlayerPrefs.SetInt("userHDR",1);
                Debug.Log("changed to Ultra Settings");
                break;
            
            case 6://custom
               // QualitySettings.renderPipeline = customSettingsURPAsset;
                optionsMenu.texturesDropdown.value = PlayerPrefs.GetInt("userTextureQuality");
                optionsMenu.shadowsDropdown.value = PlayerPrefs.GetInt("userShadowQuality");
                optionsMenu.antiAliasDropdown.value = PlayerPrefs.GetInt("userAntiAlias");
                optionsMenu.hdrToggle.isOn = Convert.ToBoolean(PlayerPrefs.GetInt("userHDR"));
                
                Debug.Log("changed to custom Settings");
                break;
        }
        QualitySettings.SetQualityLevel(value);
        optionsMenu.qualityDropdown.value = value;
        PlayerPrefs.SetInt("userQualitySettings", value);
    }

    public void ChangeTextureSettings(int value)
    {
        
        QualitySettings.SetQualityLevel(6);

        switch (value)
        {
            case 0: //Quarter
                QualitySettings.masterTextureLimit = 2;
                Debug.Log("Texture Resolution set to Quarter");

                break;
            
            case 1: //Half
                    
                QualitySettings.masterTextureLimit = 1;
                Debug.Log("Texture Resolution set to Half");

                break;
            
            case 2: //Total
                QualitySettings.masterTextureLimit = 0;
                Debug.Log("Texture Resolution set to Total");
                break;
        }

        PlayerPrefs.SetInt("userTextureQuality", value);
       // optionsMenu.qualityDropdown.value = 6;
    }

    public void ChangeShadowSettings(int value)
    {
        QualitySettings.SetQualityLevel(6);

        switch (value)
        {
            case 0: //Disabled
                customSettingsURPAsset.shadowDistance = veryLowSettingsURPAsset.shadowDistance;
                Debug.Log("Shadows set to disabled" );
                break;
            
            case 1: //Low
                customSettingsURPAsset.shadowDistance = lowSettingsURPAsset.shadowDistance;
                Debug.Log("Changed shadow Quality to Low Settings" );
                break;
            
            case 2: //Medium
                customSettingsURPAsset.shadowDistance = mediumSettingsURPAsset.shadowDistance;
                Debug.Log("Changed shadow Quality to Medium Settings" );
                break;
            
            case 3: //High
                customSettingsURPAsset.shadowDistance = highSettingsURPAsset.shadowDistance;
                Debug.Log("Changed shadow Quality to High Settings" );
                break;
            
            case 4: //Ultra
                customSettingsURPAsset.shadowDistance = ultraSettingsURPAsset.shadowDistance;
                Debug.Log("Changed shadow Quality to Ultra Settings" );
                break;
        }

        PlayerPrefs.SetInt("userShadowQuality", value);
        


        //optionsMenu.qualityDropdown.value = 6;
    }
    
    public void ChangeAntiAliasSettings(int value)
    {
        QualitySettings.SetQualityLevel(6);

        switch (value)
        {
            case 0://Disabled
                customSettingsURPAsset.msaaSampleCount = veryLowSettingsURPAsset.msaaSampleCount;
                break;
            case 1: //2x
                customSettingsURPAsset.msaaSampleCount = lowSettingsURPAsset.msaaSampleCount;
                break;
            case 2: //4x
                customSettingsURPAsset.msaaSampleCount = mediumSettingsURPAsset.msaaSampleCount;
                break;
            case 3: //8x
                customSettingsURPAsset.msaaSampleCount = ultraSettingsURPAsset.msaaSampleCount;
                break;
        }
        PlayerPrefs.SetInt("userAntiAlias", value);
        
        


        //optionsMenu.qualityDropdown.value = 6;
    }
    public void ToggleHDR(bool enabled)
    {
        QualitySettings.SetQualityLevel(6);
        customSettingsURPAsset.supportsHDR = enabled;
        PlayerPrefs.SetInt("userHDR",Convert.ToInt32(enabled));
       // optionsMenu.qualityDropdown.value = 6;
        Debug.Log("HDR: " + enabled);

    }
    public void ToggleBloom(bool enabled)
    {
        _bloom.active = enabled;
        PlayerPrefs.SetInt("userBloom",Convert.ToInt32(enabled));
        Debug.Log("Bloom: " + enabled);
    }

    public void ToggleDoF(bool enabled)
    {
        _dof.active = enabled;
        PlayerPrefs.SetInt("userDof",Convert.ToInt32(enabled));
        Debug.Log("DoF: " + enabled);

    }
    
    public void ToggleGrain(bool enabled)
    {
        _grain.active = enabled;
        PlayerPrefs.SetInt("userGrain",Convert.ToInt32(enabled));
        Debug.Log("FilmGrain: " + enabled);

    }
    
    public void ToggleVignette(bool enabled)
    {
        _vignette.active = enabled;
        PlayerPrefs.SetInt("userVignette",Convert.ToInt32(enabled));
        Debug.Log("Vignette: " + enabled);

    }

    public void ToggleChrAberr(bool enabled)
    {
        _chrAberr.active = enabled;
        PlayerPrefs.SetInt("userChrAberr",Convert.ToInt32(enabled));
        Debug.Log("Chromatic Aberration: " + enabled);
    }
    #endregion

    #region Display Settings

    public void SetScreenSize(int value)
    {
        switch (value)
        {
            case 0:
                Screen.SetResolution(640,480,Screen.fullScreen);
                break;
            case 1:
                Screen.SetResolution(800,600,Screen.fullScreen);
                break;
            case 2:
                Screen.SetResolution(1024,768,Screen.fullScreen);
                break;
            case 3:
                Screen.SetResolution(1280,720,Screen.fullScreen);
                break;
            case 4:
                Screen.SetResolution(1366,768,Screen.fullScreen);
                break;
            case 5:
                Screen.SetResolution(1600,900,Screen.fullScreen);
                break;
            case 6:
                Screen.SetResolution(1920,1080,Screen.fullScreen);
                break;
            case 7:
                Screen.SetResolution(1920,1200,Screen.fullScreen);
                break;
            case 8:
                Screen.SetResolution(2560,1440,Screen.fullScreen);
                break;
            case 9:
                Screen.SetResolution(2560,1600,Screen.fullScreen);
                break;
            case 10:
                Screen.SetResolution(3840,2160,Screen.fullScreen);
                break;
        }
        PlayerPrefs.SetInt("userResolution", value);
        Debug.Log("userResolution: " +  PlayerPrefs.GetInt("userResolution"));
        Debug.Log("Changing resolution to: " + Screen.currentResolution);
    }

    public void SetDisplayMode(int value)
    {
        switch (value)
        {
            case 0://fullscreen
                Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                break;
            case 1: //windowed
                Screen.fullScreenMode = FullScreenMode.Windowed;
                break;
            
            case 2://borderless
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                break;
        }
        PlayerPrefs.SetInt("userDisplayMode", value);
        Debug.Log("Setting display mode to: " + Screen.fullScreenMode);
    }

    public void SetGammaValue(float value)
    {
        _liftGammaGain.gamma.Override(new Vector4(value, value, value, value)); 
        
        PlayerPrefs.SetFloat("userGamma", value);
        Debug.Log("Gamma set to: " + _liftGammaGain.gamma.value);
    }

    public void ToggleVsync(bool enabled)
    {
        QualitySettings.vSyncCount = Convert.ToInt32(enabled);
        Debug.Log("Vertical Sync: " + QualitySettings.vSyncCount);
    }
    
    #endregion

    

}
