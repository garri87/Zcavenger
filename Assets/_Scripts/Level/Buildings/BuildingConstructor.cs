using System.Collections.Generic;
using UnityEngine;

public class BuildingConstructor : MonoBehaviour
{
    private float blockHeight = 3.01f;
    public Floor.BuildingType buildingType;
    public int floorCount = 3;
    public int floorHeight = 1;
    public Floor.FloorWidth floorWidth = Floor.FloorWidth.Medium;   
    public int floorDepth = 2;

    private Floor.StairsPosition stairsPosition;
    
    public List <Floor> floorsPool;

    private Vector3 buildCursor;

    public GameObject _floorGo;
    public Transform floorPoolTransform;

    public void InitBuild()
    {
       floorsPool = GetPools(buildingType);

        buildCursor = transform.position;
        
        stairsPosition = EnumExtensions.GetRandomValue<Floor.StairsPosition>();
        
        for (int y = 0; y < floorCount+1; y++)
        {
            if (y == 0)
            {
                //Spawn Base sections
               _floorGo = GetFloorFromPool(floorsPool, Floor.FloorLocation.Base, stairsPosition, floorWidth, floorHeight, floorDepth, buildCursor);
                MoveCursor(Vector3.up, blockHeight);

            }else if (y < floorCount)
            {
                //Spawn middle sections
                _floorGo = GetFloorFromPool(floorsPool, Floor.FloorLocation.Middle, stairsPosition, floorWidth, floorHeight, floorDepth, buildCursor);
                MoveCursor(Vector3.up, blockHeight);
            }
            else if (y < floorCount+1)
            {
                //Spawn roof sections
                _floorGo = GetFloorFromPool(floorsPool, Floor.FloorLocation.Roof, stairsPosition, floorWidth, floorHeight, floorDepth, buildCursor);
                MoveCursor(Vector3.up, blockHeight);
            }
            
        }
        
    }

    private void MoveCursor(Vector3 direction, float stepSize)
    {
        buildCursor += (direction * stepSize);
    }

    /// <summary>
    ///  
    /// </summary>
    /// <param name="floorPool">List of floors</param>
    /// <param name="floorLocation"></param>
    /// <param name="stairsPosition"></param>
    /// <param name="floorWidth"></param>
    /// <param name="floorHeight"></param>
    /// <param name="floorDepth"></param>
    /// <param name="spawnPosition"></param>
    /// <returns></returns>
    public GameObject GetFloorFromPool(List<Floor> floorPool,Floor.FloorLocation floorLocation,
       Floor.StairsPosition stairsPosition, Floor.FloorWidth floorWidth,
       int floorHeight,int floorDepth,Vector3 spawnPosition)
    {
        GameObject floor = null;
        GameObject auxFloor = null;

        for (int i = 0; i < floorPool.Count; i++)
        {
            if (floorPool[i].floorLocation == floorLocation
                && floorPool[i].stairsPosition == stairsPosition
                && floorPool[i].floorWidth == floorWidth
                && floorPool[i].floorHeight == floorHeight
                && floorPool[i].floorDepth == floorDepth)
            {
                if (!floorPool[i].gameObject.activeSelf)
                {
                    floor = floorPool[i].gameObject;
                    
                    break;

                }
                else
                {
                    auxFloor = floorPool[i].gameObject;
                }

            }
        }
        if (floor == null)
        {
            floor = Instantiate(auxFloor,spawnPosition,Quaternion.identity,floorPoolTransform);
        }

        if (floor && floor.TryGetComponent<Floor>(out Floor floorComp))
        {
            switch (GameManager.Instance.gameDifficulty)
            {
                case GameManager.GameDifficulty.Easy:
                    floorComp.enemyProbability = Random.Range(0, 3);
                    floorComp.itemProbability = Random.Range(0, 11);
                    break;
                case GameManager.GameDifficulty.Normal:
                    floorComp.enemyProbability = Random.Range(4, 7);
                    floorComp.itemProbability = Random.Range(0, 8);
                    break;
                case GameManager.GameDifficulty.Hard:
                    floorComp.enemyProbability = Random.Range(7, 11);
                    floorComp.itemProbability = Random.Range(0, 5);
                    break;
                default:
                    break;
            }
        }


        floor.gameObject.SetActive(true);
        floor.transform.position = spawnPosition;
        floor.GetComponent<Floor>().buildingType = buildingType;
        
        return floor;

    }
    
    public List<Floor> GetPools(Floor.BuildingType buildingType)
    {
        List<Floor> pool = new List<Floor>();
        floorPoolTransform = GameObject.Find("FloorsPool").transform;
        for (int i = 0; i < floorPoolTransform.childCount ; i++)
        {
            Floor floor = floorPoolTransform.GetChild(i).transform.GetComponent<Floor>();
            if (floor.buildingType == buildingType)
            {
                pool.Add(floor);
            }
        }
        return pool;
    }

}
