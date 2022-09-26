

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;
using Random = UnityEngine.Random;

public class BuildingGenerator : MonoBehaviour
{
    //Definir area de las piezas (por defecto 3) cada pieza representara una unidad (ud) en altura, anchura y profundidad
    public int partsSize = 3;
    
   //Definir dimension del edificio en X, Y y Z en ud

   public int maxBldWidth = 1;
   public int maxBldHeight = 1;
   public int maxBldDepth = 1;

   //Definir familia del edificio
   public BuildingArchitect.BuildingStyle buildingStyle;
   public List<GameObject> doorsPrefabList;
   public List<GameObject> wallsPrefabList;
   public List<GameObject> doorWallsPrefabList;
   public List<GameObject> floorBasesPrefabList;
   
   public int minRoomWidth;
   public int maxRoomWidth;
   public int roomsPerFloor = 3;
   
   public Transform spawnOrigin;
   public Transform rightLimit;

   public GameObject roomSeed;
   private RoomGenerator _roomGen;
   
   public List<RoomGenerator> rooms;

   private GameObject instRoom;

   public Transform roomsTransform;

   private MeshCombiner _meshCombiner;
   
   public BuildingArchitect architect;

   private int blocksLeft;
 
   private void Awake()
   {
       //Cargar recursos de partes segun tipo de familia de edificio
       GetResources(buildingStyle);
       
   }

   private void OnValidate()
   {
    
   }

   
   private void Start()
   {
       rightLimit.position = new Vector3(maxBldWidth * partsSize, 0, 0);
       //Establecer punto de origen de generacion en 0,0,0
      spawnOrigin.position = Vector3.zero;
      
      //Colocar un generador de habitacion en puntero de origen
      SetRoomSeed(spawnOrigin,maxBldWidth);
      for (int depth = 0; depth < maxBldDepth; depth++)
      {         
          spawnOrigin.position = new Vector3(0, 0, spawnOrigin.position.z);

          
          //BUCLE (vertical)desde 0 hasta el alto del edificio: 
          for (int height = 0; height < maxBldHeight; height++)
          {
              spawnOrigin.position = new Vector3(0, spawnOrigin.position.y, spawnOrigin.position.z);
              blocksLeft = maxBldWidth;

              //SI: el punto de origen se encuentra en coordenada Y =0, generar un lobby:
              if (spawnOrigin.position.y == 0)
              {
                  //BUCLE (horizontal): desde 0 hasta el ancho del edificio 
                  BuildRoom(doorWallsPrefabList[0], maxBldWidth, doorWallsPrefabList[0]);
                  _roomGen.CombineMeshes();
              }
              //SINO:
              else
              {
                  int roomsWidth = maxBldWidth / roomsPerFloor;
                  //BUCLE (Relleno): desde 0 hasta ancho de la habitacion
                  for (int width = 0; width < roomsPerFloor; width++)
                  {
                      //cologar un nuevo generador de habitacion y obtener su script
                      SetRoomSeed(spawnOrigin, roomsWidth);
                      if (width == 0) // crear una habitacion con pared al comienzo en la primera iteracion 
                      {
                          BuildRoom(wallsPrefabList[0], roomsWidth, doorWallsPrefabList[0]);
                      }
                      else if (width > 0 && width < roomsPerFloor - 1) //Rellenar las habitaciones del medio
                      {
                          BuildRoom(doorWallsPrefabList[0], roomsWidth, doorWallsPrefabList[0]);
                      }
                      else // crear una habitacion con pared al final en la ultima iteracion
                      {
                          if (blocksLeft > 0)
                          {
                              BuildRoom(doorWallsPrefabList[0], roomsWidth, doorWallsPrefabList[0]);
                          }
                          else
                          {
                              BuildRoom(doorWallsPrefabList[0], roomsWidth, wallsPrefabList[0]);
                          }

                      }
                  }

                  Debug.Log("Blocks Left: " + blocksLeft + " in floor: " + height);
                  if (blocksLeft > 0)
                  {
                      SetRoomSeed(spawnOrigin, blocksLeft);
                      BuildRoom(doorWallsPrefabList[0], blocksLeft, wallsPrefabList[0]);
                  }

                  _roomGen.CombineMeshes();

              }

              //mover punto de origen en coordenada X:0 e Y en 1 ud para continuar generando el siguiente edificio

              spawnOrigin.position += Vector3.up * (partsSize + 0.1f);
          }

          
          spawnOrigin.position += Vector3.forward * (partsSize + 0.1f);
      }
      
   }

   public void SetRoomSeed(Transform spawnOrigin, int roomWidht,int roomHeight = 1)
   {
       //Colocar un generador de habitacion en puntero de origen
       instRoom = Instantiate(roomSeed, spawnOrigin.position, transform.rotation, roomsTransform);
           
       //obtener el script del generador de habitacion
       _roomGen = instRoom.GetComponent<RoomGenerator>();
      
       //Establecer dimension de la primera habitacion en Y en 1ud e X segun anchura del edificio
       _roomGen.roomHeight = roomHeight;
       _roomGen.roomWidth = roomWidht;
       _roomGen._buildingGenerator = this;
   }
   
   public void BuildRoom(GameObject gOInLeft, int maxWidth, GameObject gOInRight)
   {
       //Generar extremo izquierdo.
       _roomGen.GenerateSide(gOInLeft, spawnOrigin.position);
       //BUCLE (relleno): desde 0 hasta ancho del edificio
       for (int fill = 0; fill < maxWidth; fill++) 
       { 
           //Generar una ud. de bloque (base, pared y techo) segun altura de la habitacion. Guardar la pared en una lista
           _roomGen.GenerateBlock(_roomGen.roomHeight,spawnOrigin); 
           spawnOrigin.position += Vector3.right * partsSize;
           blocksLeft -= 1;
       }
       //Generar extremo derecho.
       _roomGen.GenerateSide(gOInRight, spawnOrigin.position);

       if (spawnOrigin.position.z < maxBldDepth)
       {
           _roomGen.GenerateBackDoors(true);
       }
       

   }
   public void GetResources(BuildingArchitect.BuildingStyle buildingStyle)
   {
       string path = "Prefabs/Buildings/" + buildingStyle.ToString();
        
       Object[] basesPrefabsFolder = Resources.LoadAll(path + "/Bases", typeof(GameObject));
       Object[] wallsPrefabFolder = Resources.LoadAll(path + "/Walls", typeof(GameObject));
       Object[] doorWallsPrefabsFolder = Resources.LoadAll(path + "/DoorWalls", typeof(GameObject));
       Object[] doorPrefabsFolder = Resources.LoadAll(path + "/Doors", typeof(GameObject));
       FillPartsLists(basesPrefabsFolder,floorBasesPrefabList);
       FillPartsLists(wallsPrefabFolder,wallsPrefabList);
       FillPartsLists(doorWallsPrefabsFolder,doorWallsPrefabList);
       FillPartsLists(doorPrefabsFolder,doorsPrefabList);
   }

   private void FillPartsLists(Object[] folder, List<GameObject> list)
   {
       foreach (GameObject prefabFile in folder)
       {
           GameObject go = (GameObject) prefabFile;
            
           list.Add(go);
       }
   }
   public void CombineMeshes()
   {
       _meshCombiner = roomsTransform.GetComponent<MeshCombiner>();
       _meshCombiner.CreateMultiMaterialMesh = true;
       _meshCombiner.CombineInactiveChildren = false;
       _meshCombiner.DeactivateCombinedChildren = false;
       _meshCombiner.DeactivateCombinedChildrenMeshRenderers = true;
       _meshCombiner.GenerateUVMap = false;
       _meshCombiner.DestroyCombinedChildren = false;
       
       _meshCombiner.CombineMeshes(false);
   }
}
