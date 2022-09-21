using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Elevator : MonoBehaviour
{
    [SerializeField] private GameObject floorGO;
    [SerializeField]private GameObject carGO;
    [SerializeField]private GameObject buttonGO;
    [SerializeField]private GameObject doorGO;
    
    [SerializeField] private int floorCount;
    [SerializeField] private float floorSeparation;
    [SerializeField] private List<Transform> floorTransforms;
    [SerializeField] private List<TextMeshProUGUI> buttonTextList;

    public int currentFloor = 0;
    [SerializeField]private float elevatorSpeed;
    
    [SerializeField] private float acceleration,elevatorMaxAcceleration;
    public bool carMoving;
    
    private Button elevatorButton;
    private TextMeshProUGUI elevatorButtonText;

    [SerializeField]private Canvas buttonPanelUI;

    public Transform playerTransform;
    
    public bool playerInside;

    public bool buttonPressed;

    [SerializeField]private float startTimer = 3;
    private float timer;

    private ElevatorDoors _elevatorDoors;
    void Start()
    {
        floorGO.name = "Floor 0";
        buttonGO.name = "Button 0";
        doorGO.name = "Door 0";
        elevatorButton = buttonGO.GetComponent<Button>();
        elevatorButtonText = elevatorButton.transform.GetComponentInChildren<TextMeshProUGUI>();
        elevatorButtonText.text = 0.ToString();
        _elevatorDoors = doorGO.GetComponent<ElevatorDoors>();
        _elevatorDoors.floor = 0;
        floorTransforms.Add(floorGO.transform);
        
        
        buttonTextList.Add(elevatorButtonText);
        
        
        
        GenerateFloors(floorCount, floorSeparation);
        
        carGO.transform.position = floorTransforms[0].position;

        timer = startTimer;
    }

    
    void Update()
    {
        ElevatorMovement();
        
        UIToggle();
        
    }

    private void GenerateFloors(int floors, float separation)
    {
        for (int i = 1; i < floors; i++)
        {
            GameObject instantiatedFloor = Instantiate(floorGO, floorGO.transform.position + Vector3.up * separation, floorGO.transform.rotation,transform);
            floorGO = instantiatedFloor;
            instantiatedFloor.name = "Floor " + i;
            floorTransforms.Add(instantiatedFloor.transform);
            
            GameObject instantiatedButton = Instantiate(buttonGO, buttonGO.transform.parent);
            instantiatedButton.name = "Button " + i;
            Button button = instantiatedButton.GetComponent<Button>();
            TextMeshProUGUI buttonText = instantiatedButton.GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = i.ToString();
            buttonTextList.Add(buttonText);

            GameObject instantiatedDoor = Instantiate(doorGO, doorGO.transform.position + Vector3.up * separation, 
                doorGO.transform.rotation, transform);
            doorGO = instantiatedDoor;
            instantiatedDoor.name = "Door " + i;
            _elevatorDoors = instantiatedDoor.GetComponent<ElevatorDoors>();
            _elevatorDoors.floor = i;
        }
    }

    public void ChangeFloor()
    {
        TextMeshProUGUI buttonText =
            EventSystem.current.currentSelectedGameObject.GetComponentInChildren<TextMeshProUGUI>();
        if (!carMoving)
        {
            foreach (TextMeshProUGUI text in buttonTextList)
            {
                text.color = Color.white;
            }
            int floor = Int32.Parse(buttonText.text);
            
            Debug.Log("Selected floor " + floor);
            if (carGO.transform.position.y != floorTransforms[floor].transform.position.y)
            {
                GoToFloor(floor);
                buttonText.color = Color.red;
            }
        }
    }

    public void GoToFloor(int targetFloor)
    {
        timer = startTimer;
        buttonPressed = true;
        currentFloor = targetFloor;
        carMoving = true;
    }
    private void ElevatorMovement()
    {
        if (carMoving)
        {
            Vector3 carDirection = floorTransforms[currentFloor].position - carGO.transform.position;
            float targetFloorDistance =
                Vector3.Distance(carGO.transform.localPosition, floorTransforms[currentFloor].localPosition);
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                if (targetFloorDistance >= 0.5f)
                {
                    if (elevatorSpeed < elevatorMaxAcceleration)
                    {
                        elevatorSpeed += Time.deltaTime * acceleration;
                    }
                }
                else
                {
                    if (elevatorSpeed > 0)
                    {
                        elevatorSpeed -= Time.deltaTime;
                    }
                    else
                    {
                        if (targetFloorDistance > 0.1f)
                        {
                            elevatorSpeed = + Time.deltaTime * acceleration *20 ;
                        }
                        else
                        {
                            carMoving = false;
                            elevatorSpeed = 0;   
                        }
                        
                    }
                }

                carGO.transform.Translate(carDirection.normalized * elevatorSpeed * Time.deltaTime);
            }
        }

        if (!carMoving)
            {
                
                if (carGO.transform.position != floorTransforms[currentFloor].position)
                {
                  /*  carGO.transform.position = Vector3.Lerp(carGO.transform.position,
                        floorTransforms[currentFloor].position, 1 * Time.deltaTime * 2);*/
                  
                    foreach (TextMeshProUGUI text in buttonTextList)
                    {
                        text.color = Color.white;
                    }
                }
                elevatorSpeed = 0;
            }
        }
    

    private void UIToggle()
    {
        if (playerInside)
        {
            buttonPanelUI.enabled = true;
        }
        else
        {
            buttonPanelUI.enabled = false;
        }
    }
}
