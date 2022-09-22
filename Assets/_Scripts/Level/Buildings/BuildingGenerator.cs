/*
-Definir área de las piezas (por defecto 3) cada pieza representa una unidad (ud) en altura, anchura y profundidad

-Definir dimensión del edificio en X, Y y Z en ud

-Definir familia del edificio

-Cargar recursos de partes según tipo de familia de edificio

-Colocar un generador de habitación en la base  en las coordenadas 0,0,0

-Establecer dimensión de la primera habitación en Y en 1ud e X según anchura del edificio

-BUCLE I: desde 0 hasta el ancho del edificio
    *SI: la habitación comienza en coordenada 0 de Y. HACER:
        *>Generar una entrada a la izquierda.
        *>BUCLE J: desde I hasta ancho del edificio
                Generar una ud de bloque de base, pared y techo según altura de la habitacion.
                -Guardar la pared en una lista
        FIN BUCLE J.<*
            
    -Generar una salida a la derecha.

Mover punto de origen en coordenada X:0 e Y en 1 ud
FIN BUCLE I.



*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;
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

   public GameObject roomSeed;
   private RoomGenerator _roomGen;
   
   public List<RoomGenerator> rooms;

   private GameObject instRoom;

   public Transform roomsTransform;

   private MeshCombiner _meshCombiner;
   
   [SerializeField] private List<GameObject> doorPrefabList;

   public BuildingArchitect architect;

 
   private void Awake()
   {
    GetResources();
   }

   private void Start()
   {
       _meshCombiner = roomsTransform.GetComponent<MeshCombiner>();
       rightLimit.position = Vector3.right * maxWidth * 3;
       
       for (int i = 0; i < maxHeight; i++)
       {
           for (int j = 0; j < maxWidth; j++)
           {
               
               if (spawnOrigin.position.x < rightLimit.position.x)
               { instRoom = Instantiate(roomSeed, spawnOrigin.position, transform.rotation, roomsTransform);
                   _roomGen = instRoom.GetComponent<RoomGenerator>();
                   _roomGen.StartGeneration(Random.Range(minRoomWidth, maxRoomWidth),1,  _roomGen.backDoorCount);
                   spawnOrigin.position = _roomGen.rightExtent.position + Vector3.right * 1.45f;
                   foreach (GameObject doorWall in _roomGen.doorWalls)
                   {
                       GameObject randDoorFile = doorPrefabList[Random.Range(0, doorPrefabList.Count)];
                       GameObject instDoor = Instantiate(randDoorFile,doorWall.transform.position,doorWall.transform.rotation,transform);
                   }
               }
           }
           _roomGen.exitRight = false;
           
           spawnOrigin.position = new Vector3(0,(i + 1) * _roomGen.pieceDimension * 1.025f,transform.position.z);
           
       }
       
       //CombineMeshes();
   }
   public void GetResources()
   {
       
       Object[] doorPrefabsFolder = Resources.LoadAll("Prefabs/Doors", typeof(GameObject));
       foreach (GameObject prefabFile in doorPrefabsFolder)
       {
           GameObject go = (GameObject) prefabFile;
            
           doorPrefabList.Add(go);
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
