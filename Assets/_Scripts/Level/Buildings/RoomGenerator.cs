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
    public float pieceDimension = 3;

    public GameObject baseGo;
    public GameObject wallGo;
    public GameObject doorWallGo;

    public int backDoorCount;

    public Transform leftExtent, rightExtent, backExtent, frontExtent;

    public GameObject leftEnd;
    public GameObject rightEnd;

    [SerializeField] private List<GameObject> baseWalls;
    [SerializeField] public List<GameObject> doorWalls;

    
    
    private GameObject instantiatedXGO;
    private GameObject instantiatedYGO;
    public Vector3 spawnXPoint;
    public Vector3 spawnYPoint;

    public bool exitLeft, exitRight, exitFront, exitBack;

    private BuildingGenerator _buildingGenerator;

    public MeshCombiner _meshCombiner;
    
    private void Awake()
    {
       
    }

    public GameObject GenerateSide(GameObject gameObject, Vector3 spawnPoint)
    {
        GameObject end = Instantiate(gameObject, spawnPoint - Vector3.right * 1.45f, Quaternion.Euler(0, 90, 0),transform);
        return end;

    }

    public GameObject GenerateWidth(GameObject gameObject, Vector3 spawnPoint, Vector3 direction, float dimension)
    {
        GameObject go = Instantiate(gameObject, spawnPoint + (direction * dimension),
            gameObject.transform.rotation);
        spawnPoint = gameObject.transform.position;
        return go;
    }

    public GameObject GenerateCeiling(GameObject gameObject, float x, float y, float z)
    {
        //Generate Ceiling
        Vector3 ceilingPos = new Vector3(x, y - 1.45f, z);
        GameObject instantiatedCeiling = Instantiate(gameObject, ceilingPos, gameObject.transform.rotation);
        return instantiatedCeiling;
    }

    public void GenerateBlock(int width, int height)
    {

        //Generate Base
        GameObject instBase = Instantiate(baseGo, spawnXPoint, baseGo.transform.rotation, transform);
        spawnXPoint = instBase.transform.position + Vector3.right * pieceDimension;

        //Set Wall initial point for height
        spawnYPoint = instBase.transform.position - (Vector3.back * 1.45f);

        //Generate Room Height
        for (int i = 0; i < height; i++)
        {
            GameObject instWall = Instantiate(wallGo, spawnYPoint, wallGo.transform.rotation, transform);
            if (i < 1)
            {
                baseWalls.Add(instWall);
            }

            spawnYPoint = instWall.transform.position + (Vector3.up * pieceDimension);

            if (i >= height - 1)
            {
                //Generate Ceiling
                Vector3 ceilingPos = new Vector3(instBase.transform.position.x, spawnYPoint.y,
                    instBase.transform.position.z);
                GameObject instCeiling = Instantiate(baseGo, ceilingPos, baseGo.transform.rotation, transform);
                instCeiling.name = "Ceiling " + i;
            }
        }
    }

    public void StartGeneration(int width, int height, int backDoors)
    {
        if (transform.parent.parent.TryGetComponent(out BuildingGenerator buildingGenerator))
        {
            _buildingGenerator = buildingGenerator;

            //set initial point 0
            spawnXPoint = transform.position;

            //Generate Left End
            if (Mathf.RoundToInt(spawnXPoint.x) < _buildingGenerator.rightLimit.position.x)
            {
                leftEnd = GenerateSide(doorWallGo, spawnXPoint); 
                doorWalls.Add(leftEnd);
                leftEnd.name += " left";
            }
            else
            {
                leftExtent.position = leftEnd.transform.position;
                rightExtent.position = rightEnd.transform.position;
                return;
            }


            //Generate Room Width
            for (int i = 0; i < width; i++)
            {
                if (Mathf.RoundToInt(spawnXPoint.x) <
                    _buildingGenerator.rightLimit.position.x) //continue generating if the room doesn't exceed the building width
                {
                    GenerateBlock(width, height);
                }
            }

            //generate right end
            if (Mathf.RoundToInt(spawnXPoint.x) < _buildingGenerator.rightLimit.position.x)
            {
                rightEnd = GenerateSide(doorWallGo, spawnXPoint);
                rightEnd.name += " right";
                rightEnd.SetActive(false);
            }
            else
            {
                rightEnd = GenerateSide(doorWallGo, new Vector3(_buildingGenerator.rightLimit.position.x, transform.position.y));
                    doorWalls.Add(rightEnd);
                    rightEnd.name += " right";
            }
            
            //Fill Side Walls

            Vector3 sideWallLeftPos = leftEnd.transform.position;
            Vector3 sideWallRightPos = rightEnd.transform.position;

            for (int i = 1; i < roomHeight; i++)
            {
                Quaternion sideWallRot = Quaternion.Euler(90, 90, 0);
                GameObject instSideLWall = Instantiate(wallGo, sideWallLeftPos + (Vector3.up * pieceDimension),
                    sideWallRot, transform);
                GameObject instSideRWall = Instantiate(wallGo, sideWallRightPos + (Vector3.up * pieceDimension),
                    sideWallRot, transform);
                sideWallLeftPos = instSideLWall.transform.position;
                sideWallRightPos = instSideRWall.transform.position;
            }

            //generate back doors
            for (int l = 0; l < backDoors; l++)
            {
                GameObject selectedWall = baseWalls[Random.Range(0, baseWalls.Count)];
                selectedWall.SetActive(false);
                Quaternion wallDoorRot = Quaternion.Euler(0, selectedWall.transform.rotation.y, 0);
                GameObject instDoorWall = Instantiate(doorWallGo, selectedWall.transform.position, wallDoorRot,transform);
                doorWalls.Add(instDoorWall);
            }

            leftExtent.position = leftEnd.transform.position;
            rightExtent.position = rightEnd.transform.position;
            
            
            CombineMeshes();
        
            
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