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
    
    public GameObject[] basesPrefabs;
    public GameObject[] wallsPrefabs;
    public GameObject[] doorWallsPrefabs;
    
    
    public GameObject[] mainDoorPrefabs;
    public GameObject[] doorsPrefabs;
    
    
    public GameObject[] exteriorWallsPrefabs;
    public GameObject[] exteriorDoorWallsPrefabs;
    
    public GameObject[] roofPrefabs;
    public GameObject[] cornicesPrefabs;
    public GameObject[] corniceCornersPrefabs;

    public GameObject[] stairsPrefabs;
    public GameObject[] interiorPrefabs;
    
    public GameObject[] lightsPrefabs;

    
    
    
    public Material[] exteriorMaterials;
    public Material[] interiorMaterials;
    public Material[] floorBaseMaterials;

    
}
