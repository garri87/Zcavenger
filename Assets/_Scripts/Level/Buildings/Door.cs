 using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(AudioSource))]

public class Door : MonoBehaviour
{
    public enum DoorOrientation
    {
        Side,
        Back,
    }
    public DoorOrientation doorOrientation;
    
    public string doorID;
    public Transform doorTransform;
    [HideInInspector]public Vector3 doorPos;
    public float sideDoorRotation = 90;
    public GameObject textGameObject;
    
    public bool locked;
    private bool doorOpen;
    private bool doorClose;
    
    [HideInInspector]public Collider _doorZoneCollider;
    [HideInInspector]public TextMeshPro text;
    private HingeJoint _hingeJoint;
    
    public float insidePlayLine;
    public float outsidePlayLine;
   
    
    private AudioSource _audioSource;
    public AudioClip[] doorOpenSounds;
    public AudioClip[] doorCloseSounds;
    public bool debug = false;

    public static string DoorLayer = "Door";

    private void OnValidate()
    {
        SetPlaylines();
    }

    private void Awake()
    {
        _doorZoneCollider = GetComponent<Collider>();
        switch (doorOrientation)
        {
            case DoorOrientation.Back:
                _doorZoneCollider.enabled = true;
               //transform.rotation = Quaternion.Euler(0,0,0);

                break;
            
            case DoorOrientation.Side:
                
                _doorZoneCollider.enabled = false;
                //transform.rotation = Quaternion.Euler(0,sideDoorRotation,0);
                if (textGameObject)
                {
                    textGameObject.SetActive(false);
                    text.enabled = false;
                }
                                
                break;
        }
    }

    private void Start()
    {
        
        text = textGameObject.GetComponent<TextMeshPro>();
        textGameObject.SetActive(false);
        _audioSource = GetComponent<AudioSource>();
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).TryGetComponent(out HingeJoint hingeJoint))
            {
                _hingeJoint = hingeJoint; 
                break;
            }
        }
        doorPos = doorTransform.position;

        SetPlaylines();

    }


    /*private void OnEnable()
    {
        
    }*/

    private void Update()
    {
        if (_hingeJoint)
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

        if (doorOrientation == DoorOrientation.Side)
        {
            _doorZoneCollider.enabled = false;
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (doorOrientation == DoorOrientation.Back)
            {
                textGameObject.SetActive(true);
                text.enabled = true;  
                
                text.text = " Open " + "[ " + GameManager.Instance._keyAssignments.useKey.keyCode.ToString() + " ]";

            }
            if (TryGetComponent(out ElevatorDoors elevatorDoors))
            {
                if (doorOrientation == DoorOrientation.Side)
                {
                    textGameObject.transform.rotation = Quaternion.LookRotation(Vector3.forward);
                    if (elevatorDoors.elevatorOnDoor)
                    {
                        textGameObject.SetActive(false);
                        text.enabled = false; 
                    }
                }
                if (!elevatorDoors.elevatorOnDoor)
                {
                    textGameObject.SetActive(true);
                    text.enabled = true; 
                    text.text = "Call Elevator";
                }
            }
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


    public void SetPlaylines()
    {
        if (debug)
        {
            //  Debug.Log("Setting Play Lines...");
            //  Debug.Log("Level Builder blocksWidth is : " + LevelBuilder.blocksWidth);
        }

        int num = LevelBuilder.blocksWidth / 2;


        outsidePlayLine = Mathf.RoundToInt(this.transform.position.z - num);

        insidePlayLine = Mathf.RoundToInt(this.transform.position.z + num);
        if (debug)
        {
            // Debug.Log("Num is " + num);
            // Debug.Log("Outside Playline is: " + outsidePlayLine);
            // Debug.Log("Inside Playline is: " + insidePlayLine);
        }

    }
}
