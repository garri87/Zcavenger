using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using Object = System.Object;
using Random = UnityEngine.Random;

public class RoomGenerator : MonoBehaviour
{
    public int roomWidth;
    public int roomHeight;
    
    public List<GameObject> bases; 
    public List<GameObject> ceilings; 
    public List<GameObject> backWalls; 
    public List<GameObject> backDoorWalls;
    public List<GameObject> doorWalls;

    public List<GameObject> decorationSetPrefabs;

    public Material exteriorMaterial;
    public Material interiorMaterial;
    public Material floorBaseMaterial;
    
    public Vector3 spawnYPoint;

    public Transform backWallGroup;
    public Transform wallsAndInteriorGroup;
    
    public BuildingGenerator _buildingGenerator;
    
    public enum RoomStyle
    {
       // EmptyRoom,
        Hospital_Lobby, //ok 4
        Hospital_Reception, //ok 1
        Hospital_SurgeryRoom, //ok 2
        Hospital_Bathrooms, //ok 3
        Hospital_DiningRoom,//ok 3
        Hospital_PatientRooms, //ok 1
     //  Hospital_Pharmacy,
      //  Hospital_Warehouse,
        
     /*  Residential_Bathroom,
        Residential_Bedroom,
        Residential_ElevatorIn,
        Residential_ElevatorOut,
        Residential_KidBedroom,
        Residential_Kitchen,
        Residential_Laundry,
        Residential_Living,
        Residential_Lobby,*/
        
       /* Office_Lobby,
        Office_Blocks,
        Office_Boss,
        Office_Warehouse,
        Office_Reception,
        Office_Bathrooms,
        
        StairsDown,
        StairsUp,*/
        end,
    }
    public RoomStyle roomStyle;

    public bool isLobby = false;
    public bool isStairsEntrace = false;
    public bool isStairsRoom = false;
    
    
    public void CombineMeshes(bool combine, Transform transform)
    {
        if (combine)
        {
            MeshFilter meshFilter = transform.gameObject.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = transform.gameObject.AddComponent<MeshRenderer>();
            MeshCombiner meshCombiner = transform.gameObject.AddComponent<MeshCombiner>();
            meshCombiner.CreateMultiMaterialMesh = true;
            meshCombiner.CombineInactiveChildren = false;
            meshCombiner.DeactivateCombinedChildren = false;
            meshCombiner.DeactivateCombinedChildrenMeshRenderers = true;
            meshCombiner.GenerateUVMap = false;
            meshCombiner.DestroyCombinedChildren = false;
            meshCombiner.CombineMeshes(false);
        }
        
    }

    public void ChangeMaterial(List<GameObject> gameObjects, Material interiorMaterial, Material exteriorMaterial = null)
    {
        foreach (GameObject gameObject in gameObjects)
        {
            MaterialManager materialManager = gameObject.GetComponent<MaterialManager>();
            
            materialManager.meshRenderer = gameObject.GetComponent<MeshRenderer>();
            
            Material[] materials = materialManager.meshRenderer.materials;
            materials[materialManager.interiorFaceMaterialIndex] = interiorMaterial;
            if (exteriorMaterial != null)
            {
                materials[materialManager.exteriorFaceMaterialIndex] = exteriorMaterial;
            }
            else
            {
                materials[materialManager.exteriorFaceMaterialIndex] = interiorMaterial;
            }
            materialManager.meshRenderer.materials = materials;

        }
        
        
    }
    
    
    

    

    
    
    

   
    
    


    
    
}