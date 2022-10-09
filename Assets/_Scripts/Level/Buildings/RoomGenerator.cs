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
    public int partsSize;

    public GameObject baseGo;
    public GameObject wallGo;
    public GameObject doorWallGo;
    
    public List<GameObject> bases; 
    public List<GameObject> backWalls; 
    public List<GameObject> backDoorWalls;
    public List<GameObject> doorWalls;

    public List<GameObject> decorationSetPrefabs;

    public Vector3 spawnYPoint;

    public Transform backWallGroup;
    
    public BuildingGenerator _buildingGenerator;

    public MeshCombiner _meshCombiner;
    
    public enum RoomStyle
    {
       // EmptyRoom,
        Hospital_Lobby, //ok 4
        Hospital_Reception, //ok 1
        Hospital_SurgeryRoom, //ok 2
        Hospital_Bathrooms, //ok 3
     //   Hospital_DiningRoom,
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
    
    private void Awake()
    {
    }

    public void CombineMeshes(bool combine)
    {
        if (combine)
        {
            _meshCombiner.CreateMultiMaterialMesh = true;
            _meshCombiner.CombineInactiveChildren = false;
            _meshCombiner.DeactivateCombinedChildren = false;
            _meshCombiner.DeactivateCombinedChildrenMeshRenderers = true;
            _meshCombiner.GenerateUVMap = false;
            _meshCombiner.DestroyCombinedChildren = false;
            _meshCombiner.CombineMeshes(false);
        }
        
    }
    
    public void DecorateRoom(RoomStyle roomStyle,Transform spawnPos, int order = 0)
    {
        this.roomStyle = roomStyle;
        bool canDecorate = true;
        string decorPath = "Prefabs/Buildings/" + _buildingGenerator.buildingStyle.ToString() + "/Decoration";
        Object[] decorFolder = Resources.LoadAll(decorPath);
        for (int i = 0; i < decorFolder.Length; i++)
        {
            string decorSetFilePath = (decorPath + "/" + roomStyle.ToString() + (i + 1)).ToString();
            if (Resources.Load<GameObject>(decorSetFilePath))
            {
                GameObject decorSetFile = Resources.Load<GameObject>(decorSetFilePath);
                decorationSetPrefabs.Add(decorSetFile);
            }
        }

        if (order > decorationSetPrefabs.Count)
        {
            order = 0;
        }

        for (int i = 0; i < backDoorWalls.Count; i++)
        {
            if (backDoorWalls[i].transform.position.x == spawnPos.position.x)
            {
                canDecorate = false;
            }
        }
        
        if (canDecorate && decorationSetPrefabs != null)
        {
            Instantiate(decorationSetPrefabs[order],spawnPos.position,spawnPos.rotation,transform);
        }
        
    }
    
    public void GenerateBackDoors(bool randomPos, int doorCount = 1, int backDoorPos = 0)
    {
        if (doorCount >0)
        {
            for (int i = 0; i < doorCount; i++)
            {
                if (randomPos)
                {
                    backDoorPos = Random.Range(0, backWalls.Count);
                }
            
                //SELECT THE WALL TO REPLACE FOR A DOORWALL
                
                GameObject selectedWall = backWalls[backDoorPos];
                selectedWall.SetActive(false);
            
                //CREATE A DOORWALL
                Quaternion wallDoorRot = Quaternion.Euler(0, selectedWall.transform.rotation.y, 0);
                GameObject instDoorWall = Instantiate(doorWallGo, selectedWall.transform.position, wallDoorRot,transform);
                backDoorWalls.Add(instDoorWall);

            
                switch (_buildingGenerator.buildingStyle)
                {
                    case BuildingAssets.BuildingStyle.Hospital:

                        //CREATE A DOOR IN THE SELECTED DOORWALL
                        //TODO: Mejorar
                        GameObject instDoor = 
                            Instantiate(_buildingGenerator.doorsPrefabsList[Random.Range(0,_buildingGenerator.doorsPrefabsList.Count)],
                                instDoorWall.transform.position,
                                instDoorWall.transform.rotation,
                                _buildingGenerator.transform) ;
                        Door doorScript = instDoor.GetComponent<Door>();
                        doorScript.doorOrientation = Door.DoorOrientation.Back;
                        doorScript.outsidePlayLine = (PlayerController.PlayLine)transform.position.z;
                        doorScript.insidePlayLine  = (PlayerController.PlayLine)transform.position.z +1;
                        //Debug.Log(doorScript.outsidePlayLine.ToString());
                        //Debug.Log(doorScript.insidePlayLine.ToString());
                    
                        break;
                }
                backWalls.Remove(selectedWall);

                Destroy(selectedWall);
            }
        }
        
    }

    public void GenerateBlock(int height, Transform spawnOrigin)
    {
        partsSize = _buildingGenerator.partsSize;
        //Generate Base
        GameObject instBase = Instantiate(baseGo, spawnOrigin.position, baseGo.transform.rotation, transform);
        bases.Add(instBase);
        //Set BackWall initial point for height at room's base
        spawnYPoint = instBase.transform.position - (Vector3.back * 1.45f);

        //Generate a BackWall depending Room Height
        for (int i = 0; i < height; i++)
        {
            GameObject instWall = Instantiate(wallGo, spawnYPoint, wallGo.transform.rotation, transform);
            if (i < 1)
            {
                backWalls.Add(instWall);
            }

            spawnYPoint = instWall.transform.position + (Vector3.up * partsSize);

            if (i >= height - 1)
            {
                //Generate Ceiling
                Vector3 ceilingPos = new Vector3(instBase.transform.position.x, spawnYPoint.y,
                    instBase.transform.position.z);
                GameObject instCeiling = Instantiate(baseGo, ceilingPos, baseGo.transform.rotation, transform);
                instCeiling.name = "Ceiling";
            }
        }
    }

    
    
    public GameObject GenerateSide(GameObject gameObject, Vector3 spawnPoint, bool withDoor, float rotation)
    {
        GameObject instSide = Instantiate(gameObject, spawnPoint - Vector3.right * 1.451f, Quaternion.Euler(0, rotation, 0),transform);
        if (withDoor == true)
        {
            if (gameObject.tag == "DoorWall")
            {
                InstantiateDoor(instSide, false);
            }

            if (gameObject.tag == "DoubleDoorWall")
            {
                InstantiateDoor(instSide, true);
            }
        }
        
        

        void InstantiateDoor(GameObject instGO, bool doubleDoor)
        {
            doorWalls.Add(gameObject);

            if (doubleDoor)
            {
                
                GameObject instDoor = 
                    Instantiate(_buildingGenerator.mainDoorPrefab,
                        instGO.transform.position,
                        instGO.transform.rotation,
                        _buildingGenerator.transform) ;
            }
            else
            {
                GameObject instDoor = 
                Instantiate(_buildingGenerator.doorsPrefabsList[0],
                    instGO.transform.position,
                    instGO.transform.rotation,
                    _buildingGenerator.transform) ;
                
            }
        }
        return instSide;
    }

   
    
    


    
    
}