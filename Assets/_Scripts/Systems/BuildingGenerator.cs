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
   private RoomGenerator _roomGenerator;
   
   public List<RoomGenerator> rooms;

   private void Awake()
   {
   }

   private void Start()
   { 
       
       rightLimit.position = Vector3.right * maxWidth * 3;
       for (int i = 0; i < maxWidth; i++) 
       {
           if (spawnOrigin.position.x < rightLimit.position.x)
           {
               GameObject instRoom = Instantiate(roomTemplate, spawnOrigin.position, transform.rotation, transform); 
               _roomGenerator = instRoom.GetComponent<RoomGenerator>();
               _roomGenerator.roomWidth = Random.Range(minRoomWidth, maxRoomWidth);
               _roomGenerator.roomHeight = 1;
               _roomGenerator.gameObject.SetActive(true);
               spawnOrigin.position = instRoom.transform.position + Vector3.right * _roomGenerator.roomWidth * _roomGenerator.pieceDimension;
           }
           else
           {
               Debug.Log("Reached the Right Limit");
               return;
           }
           
           
       }
   }
}
