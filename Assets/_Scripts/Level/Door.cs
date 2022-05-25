using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Door : MonoBehaviour
{
    public enum DoorType
    {
        Side,
        Back,
    }

    public DoorType doorType;
    private Collider _doorZoneCollider;
    public GameObject textGameObject;
    [HideInInspector]public TextMeshPro text;
    [HideInInspector]public Transform doorTransform;
    public PlayerController.PlayLine insidePlayLine;
    public PlayerController.PlayLine outsidePlayLine;
    
    private void OnValidate()
    {
        text = textGameObject.GetComponent<TextMeshPro>();
        _doorZoneCollider = GetComponent<Collider>();
        switch (doorType)
        {
            case DoorType.Back:
                _doorZoneCollider.enabled = true;
                transform.rotation = Quaternion.Euler(0,0,0);
                //textGameObject.SetActive(true);
                //text.enabled = true;
                break;
            
            case DoorType.Side:
                _doorZoneCollider.enabled = false;
                transform.rotation = Quaternion.Euler(0,90,0);
                textGameObject.SetActive(false);
                text.enabled = false;
                break;
        }
    }

    private void Start()
    {
        text = textGameObject.GetComponent<TextMeshPro>();
        text.text = "Open Door [ " + KeyAssignments.SharedInstance.useKey.keyCode.ToString() + " ]";
        textGameObject.SetActive(false);
        doorTransform = GetComponent<Transform>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            textGameObject.SetActive(true);
            text.enabled = true;
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            textGameObject.SetActive(false);
            text.enabled = false;

        }
    }
}
