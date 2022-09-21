using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorDoors : MonoBehaviour
{
   
    [SerializeField]private Transform rightDoor, leftDoor;
    
    [SerializeField]private float doorSpeed = 2;
    [SerializeField]private float aperture = 0.5f;
    public bool elevatorReady;
    public bool elevatorOnDoor;
    public int floor;
    [SerializeField]private Elevator _elevator;
    private Door _door;
    private PlayerController playerController;
   
    
    
    private void Start()
    {
        _door = GetComponent<Door>();
        _door._doorZoneCollider.enabled = true;
    }
    
    private void Update()
    {
        
        if (elevatorOnDoor)
        {
            ControlDoor(rightDoor,aperture);
            ControlDoor(leftDoor,-aperture);
        }
        else
        {
            if (_elevator != null)
            {
                if (_elevator.buttonPressed)
                {
                    ControlDoor(rightDoor,0);
                    ControlDoor(leftDoor,0);
                }
            }
            else
            {
                ControlDoor(rightDoor,0);
                ControlDoor(leftDoor,0);
            }
            
        }
                
        

    }

    public void ControlDoor(Transform doorTransform, float apertureAmount)
    {
        Vector3 doorPos = new Vector3(apertureAmount,doorTransform.localPosition.y,doorTransform.localPosition.z);
        if (doorTransform.localPosition != doorPos)
        {
            doorTransform.localPosition = Vector3.Lerp(doorTransform.localPosition, doorPos , 1 * Time.deltaTime * doorSpeed);
            elevatorReady = false;
        }
        else
        {
            elevatorReady = true;
        }

        if (Vector3.Distance(doorTransform.localPosition,doorPos) < 0.02f)
        {
            doorTransform.localPosition = doorPos;
            elevatorReady = true;
        }

        if (apertureAmount != 0)
        {
            _door.locked = false;
        }
        else
        {
            _door.locked = true;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Elevator"))
        {
            if (other.transform.TryGetComponent(out TriggerCollider triggerCollider))
            {
                _elevator = triggerCollider._elevator;
                if (!_elevator.buttonPressed)
                {
                    elevatorOnDoor = true;
                }
                _elevator.buttonPressed = false;
                _door.doorPos = other.transform.position;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Elevator"))
        {
            if (_elevator != null)
            {
                if (!_elevator.carMoving && !_elevator.buttonPressed)
                {
                    elevatorOnDoor = true; 
                }
                else
                {
                    elevatorOnDoor = false;
                } 
            }
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Elevator"))
        {
            elevatorOnDoor = false;
            _elevator = null;
        }    
    }
}
