using Autodesk.Fbx;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSelector : MonoBehaviour
{
    public enum DoorType
    {
        Residential,
        Hospital,
        Office,
        Stairs,
        Any,
    }
    public DoorType doorType;

    public Door.DoorOrientation doorOrientation;

    public List<Door> doors;

    private string doorTag;

    public bool doubleDoor;
    
    private void Start()
    {
        switch (doorType)
        {
            case DoorType.Residential:
                doorTag = "ResidentialDoor";
                break;
            case DoorType.Hospital:
                doorTag = "HospitalDoor";

                break;
            case DoorType.Office:
                doorTag = "OfficeDoor";

                break;
            case DoorType.Stairs:
                doorTag = "StairsDoor";
                break;
            default:
                doorTag = "";
                break;
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

        if (doorOrientation == Door.DoorOrientation.Back)
        {
           // door.outsidePlayLine = Mathf.RoundToInt(door.transform.parent.parent.parent.position.z);
           // door.insidePlayLine = Mathf.RoundToInt(door.transform.parent.parent.parent.position.z + LevelBuilder.blocksWidth);
        }
            
       
    }
}
