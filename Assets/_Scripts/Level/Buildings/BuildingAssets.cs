using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;


[CreateAssetMenu(fileName = "New_BuildingStyle", menuName = "Building/New Building Style")]
public class BuildingAssets : ScriptableObject
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
    
    public List<GameObject> doorsPrefabsList;
    public List<GameObject> wallsPrefabsList;
    public List<GameObject> doorWallsPrefabsList;
    public List<GameObject> floorBasesPrefabsList;
    public List<GameObject> exteriorWallsPrefabsList;
    public List<GameObject> exteriorDoorWallsPrefabsList;
    public List<GameObject> roofPrefabsList;
    
    public List<GameObject> decorPrefabsList;

    public GameObject mainDoorPrefab;
    public GameObject roofCornice;
    public GameObject roofCorniceCorner;
    
}
