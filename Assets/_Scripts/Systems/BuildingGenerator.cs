using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BuildingGenerator : MonoBehaviour
{
   public int maxWidth = 1;
   public int maxHeight = 1;
   public int maxDepth = 1;

   public int minRoomWidth;
   public int maxRoomWidth;
   
   public Transform spawnOrigin;
   public Transform rightLimit;

   public GameObject roomTemplate;
   private RoomGenerator _roomGen;
   
   public List<RoomGenerator> rooms;

   private GameObject instRoom;
   private void Awake()
   {
   }

   private void Start()
   { 
       
       rightLimit.position = Vector3.right * maxWidth * 3;
       
       for (int i = 0; i < maxHeight; i++)
       {
           for (int j = 0; j < maxWidth; j++)
           {
               if (spawnOrigin.position.x < rightLimit.position.x)
               {
                   instRoom = Instantiate(roomTemplate, spawnOrigin.position, transform.rotation, transform);
                   _roomGen = instRoom.GetComponent<RoomGenerator>();
                   _roomGen.StartGeneration(Random.Range(minRoomWidth, maxRoomWidth),1,  _roomGen.backDoorCount);
                   spawnOrigin.position = _roomGen.rightExtent.position + Vector3.right * 1.45f;
               }
               else
               {
                   Debug.Log("Reached the Right Limit");
                   spawnOrigin.position = new Vector3(0,(i + 1) *_roomGen.pieceDimension,transform.position.z);
               }
           }
           spawnOrigin.position = new Vector3(0,(i + 1) *_roomGen.pieceDimension,transform.position.z);
           

       }
   }
}
