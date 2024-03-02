using Autodesk.Fbx;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSelector : MonoBehaviour
{
   
    public Floor.BuildingType buildingType = Floor.BuildingType.Residential;

    public Door.DoorOrientation doorOrientation;

    public List<Door> doors;

    private string doorTag;

    public bool doubleDoor;
    public bool stairsDoor;
    
    
    private void Awake()
    {
        if (!stairsDoor)
        {
            switch (buildingType)
            {
                case Floor.BuildingType.Hospital:
                    doorTag = "HospitalDoor";

                    break;
                case Floor.BuildingType.Residential:
                    doorTag = "ResidentialDoor";

                    break;
                default:
                    doorTag = "";
                    break;
            }
        }
        else
        {
            doorTag = "StairsDoor";
        }
       

        if (doubleDoor) doorTag += "Double";

        if (doorTag != "")
        {
            ActivateDoor(doorTag);
        }
        else
        {
            doors[Random.Range(0, doors.Count)].gameObject.SetActive(true);
        }
    }


    public void ActivateDoor(string doorTag)
    {
        foreach (Door door in doors)
        {
            door.gameObject.SetActive(false);
        }

        if (doors != null)
        {
            for (int i = 0; i < doors.Count; i++)
            {
                if (doors[i].CompareTag(doorTag) && !doors[i].gameObject.activeSelf)
                {
                    doors[i].gameObject.SetActive(true);
                    SetupDoor(doors[i]);
                    break;
                }
            }
        }
    }

    public void SetupDoor(Door door)
    {
        door.doorOrientation = doorOrientation;
        door.transform.rotation = new Quaternion(0, 0, 0, 0);
        door.SetPlaylines();
       
    }
}
