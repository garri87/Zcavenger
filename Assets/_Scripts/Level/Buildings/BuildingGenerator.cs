

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor.Timeline;
using UnityEngine;
using Object = System.Object;
using Random = UnityEngine.Random;

public class BuildingGenerator : MonoBehaviour
{
    public BuildingAssets buildingScriptableObject;
    
    
   public BuildingAssets.BuildingStyle buildingStyle;

    public RoomGenerator.RoomStyle roomStyle;
    
    //Definir area de las piezas (por defecto 3) cada pieza representara una unidad (ud) en altura, anchura y profundidad
    public int partsSize = 3;
    
   //Definir dimension del edificio en X, Y y Z en ud

   public int maxBldWidth = 1;
   public int maxBldHeight = 1;
   public int maxBldDepth = 1;

   //Definir familia del edificio
   [HideInInspector] public List<GameObject> doorsPrefabsList;
   [HideInInspector] public List<GameObject> wallsPrefabsList;
   [HideInInspector] public List<GameObject> doorWallsPrefabsList;
   [HideInInspector] public List<GameObject> floorBasesPrefabsList;
   [HideInInspector] public List<GameObject> exteriorWallsPrefabList;
   [HideInInspector] public List<GameObject> exteriorDoorWallsPrefabList;
   [HideInInspector] public GameObject mainDoorPrefab;
   [HideInInspector] public GameObject roofCornice;
   [HideInInspector] public GameObject roofCorniceCorner;
   

   public List<GameObject> decorPrefabsList;

   public List<GameObject> floorList;
   public List<GameObject> extWallList;

   public int minRoomWidth;
   public int maxRoomWidth;
   public int roomsPerFloor = 3;
   public int stairsRoomWidth = 3; 
   public Transform spawnOrigin;
   public Transform rightLimit;

   public GameObject roomSeed;
   private RoomGenerator _roomGen;
   
   public List<RoomGenerator> rooms;

   private GameObject instRoom;

   public Transform roomsTransform;

   private MeshCombiner _meshCombiner;
   
   private int blocksLeft;

   public bool combineMeshesAtEnd;
   
   private void Awake()
   {
       buildingStyle = buildingScriptableObject.buildingStyle;
       //Cargar recursos de partes segun tipo de familia de edificio
       GetResources();
       
   }
   
   private void Start()
   {
     rightLimit.position = new Vector3(maxBldWidth * partsSize, 0, 0);
     //Establecer punto de origen de generacion en 0,0,0
     spawnOrigin.position = transform.position;
     
     //Colocar un generador de habitacion en puntero de origen
     SetRoomSeed(spawnOrigin,maxBldWidth);
     
     //Comenzar la generacion de habitaciones
    
     //BUCLE (Profundidad Z)
     for (int Z = 0; Z < maxBldDepth; Z++)
      {         
          //establecemos el puntero de generacion en x=0 y=0 y Z=actual
          spawnOrigin.position = new Vector3(transform.position.x, transform.position.y, spawnOrigin.position.z);
          
          //BUCLE (vertical Y)desde 0 hasta el alto del edificio: 
          for (int Y = 0; Y < maxBldHeight; Y++)
          {
              //Agrupamos todas las habitaciones del piso
              GameObject floorGroup = new GameObject("Floor " + Y);
              floorGroup.transform.position = new Vector3(transform.position.x,spawnOrigin.position.y,spawnOrigin.position.z);
              floorList.Add(floorGroup);
              floorGroup.transform.parent = transform;
              
              
              //Colocamos el puntero de generacion en X=0 para empezar a instanciar habitaciones
              spawnOrigin.position = new Vector3(transform.position.x, spawnOrigin.position.y, spawnOrigin.position.z);
              
              
              //inicializamos la cantidad restante de espacios para calcular cuanto falta para llenar el piso
              blocksLeft = maxBldWidth;

              //SI: el punto de origen se encuentra en coordenada Y =0 y Z = 0, generar un lobby:
              if (Y == 0 && Z == 0)
              {
                  //BUCLE (Horizontal X): desde 0 hasta el ancho del edificio 
                  BuildRoom(exteriorDoorWallsPrefabList[0],true, maxBldWidth, exteriorDoorWallsPrefabList[0],true);
                  _roomGen.CombineMeshes(combineMeshesAtEnd);
                  instRoom.transform.parent = floorGroup.transform;
                  _roomGen.GenerateSide(exteriorDoorWallsPrefabList[0], spawnOrigin.position, true,-90);
              }
              //SINO:
              else
              {
                  //BUCLE (Horizontal X): desde 0 hasta la cantidad de habitaciones por piso
                  for (int X = 0; X < roomsPerFloor; X++)
                  {
                     /* if (Z>0 && X == roomsPerFloor-1)
                      {
                          if (Y > 0)
                          {
                              BuildRoom(doorWallsPrefabsList[0],true,stairsRoomWidth,exteriorWallsPrefabList[0]);
                          }
                          else
                          {
                              BuildRoom(doorWallsPrefabsList[0],true,stairsRoomWidth,doorWallsPrefabsList[0],true);
                          }
                      }*/
                      
                      int roomsWidth = Mathf.RoundToInt(Random.Range(minRoomWidth,maxRoomWidth));

                      //cologar un nuevo generador de habitacion y obtener su script
                      SetRoomSeed(spawnOrigin, roomsWidth);
                      if (X == 0) // crear una habitacion con pared al comienzo en la primera iteracion 
                      {
                          BuildRoom(exteriorWallsPrefabList[Random.Range(0,exteriorWallsPrefabList.Count)],false, roomsWidth, null,false);
                      }
                      else if (X > 0 && X < roomsPerFloor - 1) //Rellenar las habitaciones del medio
                      {
                          if (blocksLeft < roomsWidth)
                          {
                              if (doorWallsPrefabsList.Count > 0)
                              {
                                  BuildRoom(doorWallsPrefabsList[0],true, blocksLeft, null,false);

                              }
                              _roomGen.GenerateSide(exteriorWallsPrefabList[Random.Range(0,exteriorWallsPrefabList.Count)], spawnOrigin.position, false,-90);

                              break;
                          }
                          else
                          {
                              BuildRoom(doorWallsPrefabsList[0],true, roomsWidth, null,false);
                          }
                      }
                      else // crear una habitacion con pared al final en la ultima iteracion
                      {
                          if (blocksLeft > 0)
                          { 
                              BuildRoom(doorWallsPrefabsList[0],true, blocksLeft, exteriorWallsPrefabList[Random.Range(0,exteriorWallsPrefabList.Count)],false);
                                //Debug.Log("Blocks Left in floor" + Y + blocksLeft);
                          }
                          
                          if(blocksLeft <1)
                          {
                              _roomGen.GenerateSide(exteriorWallsPrefabList[Random.Range(0,exteriorWallsPrefabList.Count)], spawnOrigin.position, false ,-90);
                          }

                      }
                      _roomGen.CombineMeshes(combineMeshesAtEnd);
                      instRoom.transform.parent = floorGroup.transform;

                  }

                  //Debug.Log("Blocks Left: " + blocksLeft + " in floor: " + Y);
                  if (blocksLeft > 0)
                  {
                      SetRoomSeed(spawnOrigin, blocksLeft);
                      BuildRoom(doorWallsPrefabsList[0],true, blocksLeft, wallsPrefabsList[0],false);
                      _roomGen.CombineMeshes(combineMeshesAtEnd);
                      instRoom.transform.parent = floorGroup.transform;
                  }


              }
            
             
              
              //mover punto de origen en coordenada X:0 e Y en 1 ud para continuar generando el siguiente edificio
              spawnOrigin.position += Vector3.up * (partsSize + 0.1f);
          }
          spawnOrigin.position += Vector3.forward * (partsSize);
      }

     
     
     
     AddHideFrontFace(floorList, true);
     GenerateExteriorWalls(spawnOrigin,maxBldWidth,maxBldHeight);
     GenerateRoof(maxBldWidth,spawnOrigin,roofCorniceCorner,roofCornice);


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
       rooms.Add(_roomGen);
   }
   
   public void BuildRoom(GameObject gOInLeft = null, bool doorInLeft = false,
       int maxWidth = 0, GameObject gOInRight = null, bool doorInRight = false)
   {
       //Generar extremo izquierdo.
       if (gOInLeft !=null)
       {
           _roomGen.GenerateSide(gOInLeft, spawnOrigin.position,doorInLeft,90);
       }
       
       //Definimos el estilo de habitacion
       //TODO: Crear Switch segun estilo
       roomStyle = (RoomGenerator.RoomStyle)Random.Range((int)RoomGenerator.RoomStyle.Hospital_SurgeryRoom,(int)RoomGenerator.RoomStyle.end);
      // Debug.Log("Selected " + roomStyle.ToString());
       
       //BUCLE (relleno X): desde 0 hasta ancho de la habitacion
       for (int xFill = 0; xFill < maxWidth; xFill++) 
       { 
           //Generar una ud. de bloque (base, pared y techo) segun altura de la habitacion. Guardar la pared en una lista
           _roomGen.GenerateBlock(_roomGen.roomHeight,spawnOrigin);
           spawnOrigin.position += Vector3.right * partsSize;
           blocksLeft -= 1;
       }
       //Generar extremo derecho.

       if (gOInRight != null)
       {
         // GameObject rightEnd = _roomGen.GenerateSide(gOInRight, spawnOrigin.position,doorInRight);
       }

           //Generar puertas traseras
       if (spawnOrigin.position.z < transform.position.z + ((maxBldDepth -1) * partsSize))
       {
           if (spawnOrigin.position.z == transform.position.z && spawnOrigin.position.y == transform.position.y)
           {
               _roomGen.GenerateBackDoors(true, maxBldWidth/roomsPerFloor);
               Debug.Log("Generating " + maxBldWidth/roomsPerFloor + " back doors");
           }
           else
           {
               _roomGen.GenerateBackDoors(true,Random.Range(0,2));
           }
       }
       
       
       //Llenamos las habitaciones de objetos de decoracion segun estilo
       for (int i = 0; i < _roomGen.bases.Count; i++)
       {
           if (spawnOrigin.localPosition.y == 0)
           {
               int randomNum = Random.Range(0, 2);

               if (randomNum == 0)
               {
                   _roomGen.DecorateRoom(RoomGenerator.RoomStyle.Hospital_Reception, _roomGen.bases[i].transform,i);
               }
               else
               {
                   _roomGen.DecorateRoom(RoomGenerator.RoomStyle.Hospital_Lobby, _roomGen.bases[i].transform,i);

               }
           }
           else
           {
               Vector3 asda = new Vector3(0, 0, 0);
               _roomGen.DecorateRoom(roomStyle, _roomGen.bases[i].transform,i);
           }
       }
        
   }
  
   public void GetResources()
   {
       mainDoorPrefab = buildingScriptableObject.mainDoorPrefab;
       floorBasesPrefabsList = buildingScriptableObject.floorBasesPrefabsList;
       wallsPrefabsList = buildingScriptableObject.wallsPrefabsList;
       doorWallsPrefabsList = buildingScriptableObject.doorWallsPrefabsList;
       doorsPrefabsList = buildingScriptableObject.doorsPrefabsList;
       decorPrefabsList = buildingScriptableObject.decorPrefabsList;
       exteriorWallsPrefabList = buildingScriptableObject.exteriorWallsPrefabsList;
       exteriorDoorWallsPrefabList = buildingScriptableObject.exteriorDoorWallsPrefabsList;
       
       roofCornice = buildingScriptableObject.roofCornice;
       roofCorniceCorner = buildingScriptableObject.roofCorniceCorner;
   }
   
   public void AddHideFrontFace(List<GameObject> list, bool isRoom)
   {
       foreach (GameObject floorGO in list)
       {
           BoxCollider boxCollider = floorGO.AddComponent<BoxCollider>();
           boxCollider.isTrigger = true;
           float xCenter = (maxBldWidth * partsSize) - partsSize;
           boxCollider.center = new Vector3(xCenter / 2, partsSize /2 ,partsSize);
           boxCollider.size = new Vector3(maxBldWidth * partsSize, partsSize / 2, partsSize - 1);
           
           HideFrontFace hideFrontFace = floorGO.AddComponent<HideFrontFace>();

           if (isRoom)
           {
               for (int i = 0; i < floorGO.transform.childCount; i++)
               {
                   RoomGenerator roomGenerator = floorGO.transform.GetChild(i).GetComponentInChildren<RoomGenerator>();
                   hideFrontFace.facesToHide = new List<GameObject>(roomGenerator.backWalls);
               }
           }
           else
           {
               
           }
       }
   }
   public void CombineMeshes(Transform parent)
   {
       if (combineMeshesAtEnd)
       {
           if (parent.TryGetComponent(out MeshCombiner meshComb))
           {
               _meshCombiner = meshComb;
           }
           else
           {
               _meshCombiner = parent.gameObject.AddComponent<MeshCombiner>();
           }
           _meshCombiner.CreateMultiMaterialMesh = true;
           _meshCombiner.CombineInactiveChildren = false;
           _meshCombiner.DeactivateCombinedChildren = false;
           _meshCombiner.DeactivateCombinedChildrenMeshRenderers = true;
           _meshCombiner.GenerateUVMap = false;
           _meshCombiner.DestroyCombinedChildren = false;
       
           _meshCombiner.CombineMeshes(false);
       }
   }

   public void GenerateExteriorWalls(Transform spawnOrigin, int width, int height)
   {
       GameObject exteriorGroup = new GameObject("Exterior Walls");
       exteriorGroup.transform.parent = transform;
       exteriorGroup.transform.position = transform.position;
       spawnOrigin.position = transform.position;
       for (int Y = 0; Y < height; Y++)
       {
           GameObject exteriorFloor = new GameObject("Floor " + Y);
           exteriorFloor.transform.parent = exteriorGroup.transform;
           exteriorFloor.transform.position = transform.position;
           spawnOrigin.position = new Vector3(transform.position.x, spawnOrigin.position.y,transform.position.z);
           for (int X = 0; X < width; X++)
           {
               GameObject instExtWall= Instantiate(exteriorWallsPrefabList[Random.Range(0,exteriorWallsPrefabList.Count)], 
                   spawnOrigin.position - (Vector3.forward * 1.451f), transform.rotation, 
                   exteriorFloor.transform);
               spawnOrigin.position += Vector3.right * partsSize;
           }
           if (combineMeshesAtEnd)
           {
               CombineMeshes(exteriorFloor.transform);
           }
           extWallList.Add(exteriorFloor);
           spawnOrigin.position += Vector3.up * (partsSize + 0.1f);
       }
       AddHideFrontFace(extWallList,false);
   }

   public void GenerateRoof(int width,Transform origin, GameObject corniceCorner, GameObject cornice)
   {
       GameObject roofGroup = new GameObject("Roof");
       roofGroup.transform.position = transform.position;
       roofGroup.transform.parent = transform;
       float yPos = transform.position.y + (maxBldHeight * partsSize) + 0.5f;
       
       origin.position = new Vector3(transform.position.x, yPos, transform.position.z);
       
       Instantiate(corniceCorner, origin.position, origin.rotation, roofGroup.transform);
       origin.position += Vector3.right * partsSize;
       for (int i = 0; i < maxBldWidth-2; i++)
       {
           Instantiate(cornice, origin.position + (Vector3.back * 1.5f), origin.rotation, roofGroup.transform);
           origin.position += Vector3.right * partsSize;
       }

       Quaternion cornerRightRot = Quaternion.Euler(origin.rotation.x, origin.rotation.y - 90, origin.rotation.z); 
       Instantiate(corniceCorner, origin.position, cornerRightRot, roofGroup.transform);
       
       Vector3 roofBackPos = new Vector3(transform.position.x + ((maxBldWidth * partsSize) - partsSize), transform.position.y, transform.position.z + (maxBldDepth * (partsSize-1)));
       Quaternion roofBackRot = Quaternion.Euler(transform.rotation.x,transform.rotation.y + 180,transform.rotation.z);

       GameObject roofBack = Instantiate(roofGroup, roofBackPos, roofBackRot, roofGroup.transform);

       for (int i = 0; i < maxBldDepth-2; i++)
       {
           origin.position = new Vector3(transform.position.x,origin.position.y,origin.position.z); 
           origin.position += Vector3.forward * partsSize;
           Quaternion corniceLeftRot = Quaternion.Euler(origin.rotation.x , origin.rotation.y + 90, origin.rotation.z);
           Quaternion corniceRightRot = Quaternion.Euler(origin.rotation.x, origin.rotation.y - 90, origin.rotation.z);

           Instantiate(cornice,origin.position + Vector3.left * 1.5f,corniceLeftRot,roofGroup.transform);

           Vector3 corniceRightPos = origin.position + Vector3.right * (maxBldWidth * partsSize);
           Instantiate(cornice,corniceRightPos + Vector3.left * 1.5f,corniceRightRot,roofGroup.transform);
       }
       
       CombineMeshes(roofGroup.transform);
   }
       
   
   
   
}
