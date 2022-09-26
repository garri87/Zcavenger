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

    public int backDoorCount;

    public Transform leftExtent, rightExtent, backExtent, frontExtent;

    public GameObject leftEnd;
    public GameObject rightEnd;

    [SerializeField] private List<GameObject> backWalls;
    [SerializeField] public List<GameObject> doorWalls;

    
    
    private GameObject instantiatedXGO;
    private GameObject instantiatedYGO;
    public Vector3 spawnXPoint;
    public Vector3 spawnYPoint;

    public bool exitLeft, exitRight, exitFront, exitBack;

    public BuildingGenerator _buildingGenerator;

    public MeshCombiner _meshCombiner;
    
    private void Awake()
    {
    }

    public GameObject GenerateSide(GameObject gameObject, Vector3 spawnPoint)
    {
        GameObject end = Instantiate(gameObject, spawnPoint - Vector3.right * 1.45f, Quaternion.Euler(0, 90, 0),transform);
        if (gameObject.tag == "DoorWall")
        {
            doorWalls.Add(gameObject);
            switch (_buildingGenerator.buildingStyle)
            {
                case BuildingArchitect.BuildingStyle.Hospital:
                    GameObject instDoor = Instantiate(_buildingGenerator.doorsPrefabList[0],end.transform.position,end.transform.rotation,_buildingGenerator.transform) ;
                    
                    break;
            }
        }
        return end;
        
    }
    
    public void GenerateBlock(int height, Transform spawnOrigin)
    {
        partsSize = _buildingGenerator.partsSize;
        //Generate Base
        GameObject instBase = Instantiate(baseGo, spawnOrigin.position, baseGo.transform.rotation, transform);

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
    
    //generate back doorWalls
    public void GenerateBackDoors(bool randomPos, int doorCount = 1, int backDoorPos = 0)
    {
        if (randomPos)
        {
            backDoorPos = Random.Range(0, backWalls.Count);
        }

        for (int i = 0; i < doorCount; i++)
        {
            GameObject selectedWall = backWalls[backDoorPos];
            selectedWall.SetActive(false);
            Quaternion wallDoorRot = Quaternion.Euler(0, selectedWall.transform.rotation.y, 0);
            GameObject instDoorWall = Instantiate(doorWallGo, selectedWall.transform.position, wallDoorRot,transform);
            doorWalls.Add(instDoorWall);

            switch (_buildingGenerator.buildingStyle)
            {
                case BuildingArchitect.BuildingStyle.Hospital:

                    GameObject instDoor = Instantiate(_buildingGenerator.doorsPrefabList[1],instDoorWall.transform.position,instDoorWall.transform.rotation,_buildingGenerator.transform) ;
                    
                    break;
            }
            
        }
        
        
        
    }
    

    public void CombineMeshes()
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