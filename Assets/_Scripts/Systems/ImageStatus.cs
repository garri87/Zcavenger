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
        bleedingIcon.gameObject.SetActive(_healthManager.isBleeding);
        injuredIcon.gameObject.SetActive(_healthManager.isInjured);
        sickIcon.gameObject.SetActive(_healthManager.isSick);
    }
}