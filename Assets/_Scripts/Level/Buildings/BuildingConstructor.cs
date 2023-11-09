using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuildingConstructor : MonoBehaviour
{
    private float blockHeight = 3.01f;
    public int floorCount = 3;
    public int floorHeight = 1;
    public int floorWidth = 6;   
    public int floorDepth = 2;

    private Floor.StairsPosition stairsPosition;
    
    public List <Floor> floorsPool;

    private Vector3 buildCursor;

    public GameObject _floorGo;
    public Transform floorPoolTransform;

    public void InitBuild()
    {
        GetPools();

        buildCursor = transform.position;
        
        stairsPosition = EnumExtensions.GetRandomValue<Floor.StairsPosition>();
        
        for (int y = 0; y < floorCount+1; y++)
        {
            if (y == 0)
            {
                //Spawn Base sections
               _floorGo = GetFloorFromPool(floorsPool, Floor.FloorType.Base, stairsPosition, floorWidth, floorHeight, floorDepth, buildCursor);
                MoveCursor(Vector3.up, blockHeight);

            }else if (y < floorCount)
            {
                //Spawn middle sections
                _floorGo = GetFloorFromPool(floorsPool, Floor.FloorType.Middle, stairsPosition, floorWidth, floorHeight, floorDepth, buildCursor);
                MoveCursor(Vector3.up, blockHeight);
            }
            else if (y < floorCount+1)
            {
                //Spawn roof sections
                _floorGo = GetFloorFromPool(floorsPool, Floor.FloorType.Roof, stairsPosition, floorWidth, floorHeight, floorDepth, buildCursor);
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
    /// <param name="floorType"></param>
    /// <param name="stairsPosition"></param>
    /// <param name="floorWidth"></param>
    /// <param name="floorHeight"></param>
    /// <param name="floorDepth"></param>
    /// <param name="spawnPosition"></param>
    /// <returns></returns>
    public GameObject GetFloorFromPool(List<Floor> floorPool,Floor.FloorType floorType,
       Floor.StairsPosition stairsPosition,int floorWidth,
       int floorHeight,int floorDepth,Vector3 spawnPosition)
    {
        GameObject floor = null;
        GameObject auxFloor = null;

        for (int i = 0; i < floorPool.Count; i++)
        {
            if (floorPool[i].floorType == floorType
                && floorPool[i].stairsPosition == stairsPosition
                && floorPool[i].width == floorWidth
                && floorPool[i].height == floorHeight
                && floorPool[i].depth == floorDepth)
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

        floor.gameObject.SetActive(true);
        floor.transform.position = spawnPosition;
        
        return floor;

    }
    
    public void GetPools()
    {
        floorPoolTransform = GameObject.Find("FloorsPool").transform;
        for (int i = 0; i < floorPoolTransform.childCount ; i++)
        {
            floorsPool.Add(floorPoolTransform.GetChild(i).transform.GetComponent<Floor>());
        }
    }
}
