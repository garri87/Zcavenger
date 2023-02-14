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
        Hospital, //ok
        Market,
        PoliceStation,
        FireStation,
    }
    public BuildingStyle buildingStyle;
    
    public List<GameObject> basesPrefabs;
    public List<GameObject> wallsPrefabs;
    public List<GameObject> doorWallsPrefabs;
    
    
    public List<GameObject> mainDoorPrefabs;
    public List<GameObject> doorsPrefabs;
    
    
    public List<GameObject> exteriorWallsPrefabs;
    public List<GameObject> exteriorDoorWallsPrefabs;
    
    public List<GameObject> roofPrefabs;
    public List<GameObject> cornicesPrefabs;
    public List<GameObject> corniceCornersPrefabs;

    public List<GameObject> stairsPrefabs;
    public List<GameObject> interiorPrefabs;
    
    public List<GameObject> lightsPrefabs;

    
    
    
    public Material[] exteriorMaterials;
    public Material[] interiorMaterials;
    public Material[] floorBaseMaterials;

    
}
