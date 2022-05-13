using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Door : MonoBehaviour
{
    public enum DoorType
    {
        Side,
        Back,
    }

    public DoorType doorType;
    public string doorID;
    private Collider _doorZoneCollider;
    public GameObject textGameObject;
    [HideInInspector]public TextMeshPro text;
    public Transform doorTransform;
    public PlayerController.PlayLine insidePlayLine;
    public PlayerController.PlayLine outsidePlayLine;
    private HingeJoint _hingeJoint;
    
    private AudioSource _audioSource;
    public AudioClip[] doorOpenSounds;
    public AudioClip[] doorCloseSounds;

    private bool doorOpen;
    private bool doorClose;

    public bool locked;
    
    
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
        _audioSource = GetComponent<AudioSource>();
        _hingeJoint = doorTransform.GetComponent<HingeJoint>();
    }

    private void Update()
    {
        if (_hingeJoint.angle <-0.2f || _hingeJoint.angle >0.2f)
        {
            if (doorClose)
            {
                _audioSource.PlayOneShot(doorOpenSounds[Random.Range(0,doorOpenSounds.Length)]);  
            }
            doorClose = false;
            doorOpen = true;
        }
        else
        {
            
            if (doorOpen)
            {
                _audioSource.PlayOneShot(doorCloseSounds[Random.Range(0,doorCloseSounds.Length)]);
            }
            doorClose = true;
            doorOpen = false;
        }
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
