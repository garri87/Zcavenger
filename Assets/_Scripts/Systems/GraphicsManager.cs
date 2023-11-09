using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using UnityEngine.UIElements;
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
    private LightRenderingMode _lightRenderingMode;
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
    
    public OptionsMenuUI optionsMenuUI;
    private GameManager _gameManager;
    private Resolution[] resolutions;
    
    private void OnValidate()
    {
        _gameManager = GameManager.Instance;
        //Switch postprocessing settings 
        if (_gameManager)
        {
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
        }
        
       

        currentRenderPipelineAsset = QualitySettings.renderPipeline;
        
    }

    
    void Awake()
    {        
        if (globalVolume != null)
        {            

        }
    }

    

    void Update()
    {
       
    }
    
    /// <summary>
    /// Gets the postprocess effects of profile
    /// </summary>
    /// <param name="profile"></param>
    public void getVolumeProfileOverrides(VolumeProfile profile)
    {
        profile.TryGet<Bloom>(out _bloom);
        profile.TryGet<DepthOfField>(out _dof);
        profile.TryGet<FilmGrain>(out _grain);
        profile.TryGet<Vignette>(out _vignette);
        profile.TryGet<ChromaticAberration>(out _chrAberr);
        profile.TryGet<LiftGammaGain>(out _liftGammaGain);
    }
    
    
    #region Graphics Settings

    public void ChangeQualitySettings(int value)
    {
        
        switch (value)
        {
            case 0://very low
                QualitySettings.renderPipeline = veryLowSettingsURPAsset;
                //textures
                optionsMenuUI.textureDropdown.index = 0;
                //shadows
                optionsMenuUI.shadowsDropdown.index = 0;
                //AA
                optionsMenuUI.antiAliasDropdown.index = 0;
                //HDR
                optionsMenuUI.hdrToggle.value = false;
                PlayerPrefs.SetInt("userTextureQuality", 0);
                PlayerPrefs.SetInt("userShadowQuality", 0);
                PlayerPrefs.SetInt("userAntiAlias", 0);
                PlayerPrefs.SetInt("userHDR",0);
                Debug.Log("changed to very low Settings");
                break;
            
            case 1://low
                //QualitySettings.renderPipeline = lowSettingsURPAsset;
                //textures
                optionsMenuUI.textureDropdown.index = 1;
                //shadows
                optionsMenuUI.shadowsDropdown.index = 1;
                //AA
                optionsMenuUI.antiAliasDropdown.index = 1;
                //HDR
                optionsMenuUI.hdrToggle.value = false;
                PlayerPrefs.SetInt("userTextureQuality", 1);
                PlayerPrefs.SetInt("userShadowQuality", 1);
                PlayerPrefs.SetInt("userAntiAlias", 1);
                PlayerPrefs.SetInt("userHDR",0);
                Debug.Log("changed to low Settings");
                break;
            
            case 2://medium
               // QualitySettings.renderPipeline = mediumSettingsURPAsset;
                //textures
                optionsMenuUI.textureDropdown.index = 2;
                //shadows
                optionsMenuUI.shadowsDropdown.index = 2;
                //AA
                optionsMenuUI.antiAliasDropdown.index = 2;
                //HDR
                optionsMenuUI.hdrToggle.value = true;
               PlayerPrefs.SetInt("userTextureQuality", 2);
               PlayerPrefs.SetInt("userShadowQuality", 2);
               PlayerPrefs.SetInt("userAntiAlias", 2);
               PlayerPrefs.SetInt("userHDR",1);

                Debug.Log("changed to medium Settings");
                break;
            
            case 3://high
               // QualitySettings.renderPipeline = highSettingsURPAsset;
                //textures
                optionsMenuUI.textureDropdown.index = 3;
                //shadows
                optionsMenuUI.shadowsDropdown.index = 3;
                //AA
                optionsMenuUI.antiAliasDropdown.index = 2;
                //HDR
                optionsMenuUI.hdrToggle.value = true;
               PlayerPrefs.SetInt("userTextureQuality", 3);
               PlayerPrefs.SetInt("userShadowQuality", 3);
               PlayerPrefs.SetInt("userAntiAlias", 2);
               PlayerPrefs.SetInt("userHDR",1);

                Debug.Log("changed to high Settings");
                break;
            
            case 4://ultra
               // QualitySettings.renderPipeline = ultraSettingsURPAsset;
                //textures
                optionsMenuUI.textureDropdown.index = 4;
                //shadows
                optionsMenuUI.shadowsDropdown.index = 4;
                //AA
                optionsMenuUI.antiAliasDropdown.index = 3;
                //HDR
                optionsMenuUI.hdrToggle.value = true;
               PlayerPrefs.SetInt("userTextureQuality", 4);
               PlayerPrefs.SetInt("userShadowQuality", 4);
               PlayerPrefs.SetInt("userAntiAlias", 3);
               PlayerPrefs.SetInt("userHDR",1);
                Debug.Log("changed to Ultra Settings");
                break;
            
            case 5://custom
               // QualitySettings.renderPipeline = customSettingsURPAsset;
                optionsMenuUI.textureDropdown.value = PlayerPrefs.GetString("userTextureQuality");
                optionsMenuUI.shadowsDropdown.value = PlayerPrefs.GetString("userShadowQuality");
                optionsMenuUI.antiAliasDropdown.value = PlayerPrefs.GetString("userAntiAlias");
                optionsMenuUI.hdrToggle.value = Convert.ToBoolean(PlayerPrefs.GetInt("userHDR"));
                
                Debug.Log("changed to custom Settings");
                break;
        }
        QualitySettings.SetQualityLevel(value);
        //optionsMenuUI.qualityDropdown.value = value.ToString();
        PlayerPrefs.SetInt("userQualitySettings", value);
    }

    public void ChangeTextureSettings(int value)
    {
        
        QualitySettings.SetQualityLevel(5);

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
        QualitySettings.SetQualityLevel(5);

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
        QualitySettings.SetQualityLevel(5);

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
        QualitySettings.SetQualityLevel(5);
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
       

    private int GetCurrentScreenModeIndex()
    {
        FullScreenMode currentMode = Screen.fullScreenMode;
        if (currentMode == FullScreenMode.FullScreenWindow)
        {
            return 0;
        }
        else if (currentMode == FullScreenMode.Windowed)
        {
            return 1;
        }
        else if (currentMode == FullScreenMode.MaximizedWindow)
        {
            return 2;
        }
        return -1;
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
