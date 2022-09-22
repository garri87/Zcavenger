using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomStyle
{
    public GameObject doorType;
    public GameObject floorType;
    public GameObject wallType;
    public GameObject CeilingType;
    

}

public class BuildingArchitect : MonoBehaviour
{
    public enum BuildingStyle
    {
        Residential1,
        Residential2,
        Residential3,
        ResidentialRuined,
        Office,
        Hospital,
        Market,
        PoliceStation,
        FireStation,
    }
    public BuildingStyle buildingStyle;

    public int buildingWidth;
    public int buildingHeight;
    public int floors;
    public int roomsPerFloor;
    public enum RoomType
    {
        Lobby,
        StairsUp,
        StairsDown,
        EmptyRoom,
        Kitchen,
        Laundry,
        Bedroom,
        KidBedroom,
        Living,
        Bathroom,
        OfficeBlocks,
        OfficeBoss,
        OfficeWarehouse,
        ElevatorOut,
        ElevatorIn,
    }

    public RoomType roomType;
    
 

    public RoomType[] buildOrder;

    private BuildingGenerator _buildingGenerator;
    
    [HideInInspector]public string resourcePath;

    private void OnValidate()
    {
        _buildingGenerator = GetComponent<BuildingGenerator>();
        resourcePath = "Prefabs/Buildings/" + buildingStyle.ToString();
        Debug.Log("Selected path: " + resourcePath);
        switch (buildingStyle)
        {
            case BuildingStyle.Residential1:
                
                break;
            
            case BuildingStyle.Residential2:
                
                break;
            
            case BuildingStyle.Residential3:
                
                break;
            
            case BuildingStyle.ResidentialRuined:
                
                break;
            
            case BuildingStyle.Office:
                
                break;
            
            case BuildingStyle.Hospital:
                
                break;
            
            case BuildingStyle.Market:
                
                break;
            
            case BuildingStyle.PoliceStation:
                
                break;
            
            case BuildingStyle.FireStation:
                
                break;
            
        }
    }
}
