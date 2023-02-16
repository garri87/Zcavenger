using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomInterior : MonoBehaviour
{
    [Header("Room Settings")]
    public bool exitLeft; 
    public bool exitRight; 
    public bool exitBack; 
    public bool exitFront;

    [Header("Item & container spawn Locations")]
    public Transform[] SpawnLocations;

}
