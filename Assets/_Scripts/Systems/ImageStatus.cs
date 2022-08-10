using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;


public class ImageStatus : MonoBehaviour
{
    
    public Image bleedingIcon;
    public Image injuredIcon;
    public Image sickIcon;
    public HealthManager _healthManager;
    
    void Start()
    {
        _healthManager = GameObject.Find("Player").GetComponent<HealthManager>();
    }

    void Update()
    {
        if (_healthManager.isBleeding)
        {
            bleedingIcon.gameObject.SetActive(true);
        }
        else
        {
            bleedingIcon.gameObject.SetActive(false);
        }

        if (_healthManager.isInjured)
        {
            injuredIcon.gameObject.SetActive(true);
        }
        else
        {
            injuredIcon.gameObject.SetActive(false);
        }

        if (_healthManager.isSick)
        {
            sickIcon.gameObject.SetActive(true);
        }
        else
        {
            sickIcon.gameObject.SetActive(false);
        }
    }
}
