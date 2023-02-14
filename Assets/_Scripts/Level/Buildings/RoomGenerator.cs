using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.Serialization;
using Object = System.Object;
using Random = UnityEngine.Random;

public class RoomGenerator : MonoBehaviour
{
    //Referencia al Constructor de edificio
    public BuildingAssets buildingScriptableObject;
    
    //Dimensiones de la habitación
    public int roomWidth;
    public int roomHeight;
    
    //Dimensiones de las partes
    public int partsWidth = 6;
    public int partsHeight = 3;
    public float partsThickness = 0.1f;
    
    
    //Punto de generación en coordenada Y
    public Vector3 spawnYPoint;
    
    //Listas de gameobjects de las partes
    public List<GameObject> basesList; 
    public List<GameObject> ceilingsList; 
    public List<GameObject> backWallsList; 
    public List<GameObject> backDoorWallsList;
    public List<GameObject> doorWallsList;
    
    //Listas de prefabs para utilizar en construcción
    private List<GameObject> _doorsPrefabs;
    private List<GameObject> _wallsPrefabs;
    private List<GameObject> _doorWallsPrefabs;
    private List<GameObject> _basesPrefabs;
    private List<GameObject> _exteriorWallsPrefabs;
    private List<GameObject> _exteriorDoorWallPrefabs;
    private List<GameObject> _lightsPrefabs;
    private List<GameObject> _mainDoorPrefabs;
    private List<GameObject> _cornicesPrefabs;
    private List<GameObject> _corniceCornersPrefabs;
    
    //Lista de prefabs del interior de la habitación
    public List<GameObject> interiorPrefabs;
    public GameObject[] selectedInteriors;
    
    //Materiales
    public Material[] exteriorMaterials;
    public Material[] interiorMaterials;
    public Material[] floorBaseMaterials;
    
    //Agrupadores de Gameobjects
    public Transform backWallGroup;
    public Transform wallsAndInteriorGroup;
    
    //Referencia al Generador de edificio padre
    public BuildingGenerator _buildingGenerator;

    public bool combineMeshesAtEnd = false;

    public string[] roomStyles = new[]
    {
        "EmptyRoom",
        
        "Hospital_Lobby", //ok 4
        "Hospital_Reception", //ok 1
        "Hospital_SurgeryRoom", //ok 2
        "Hospital_Bathrooms", //ok 3
        "Hospital_DiningRoom", //ok 3
        "Hospital_PatientRooms", //ok 1
        "Hospital_Pharmacy",
        "Hospital_Warehouse",

        "Residential_Bathroom",
        "Residential_Bedroom",
        "Residential_ElevatorIn",
        "Residential_ElevatorOut",
        "Residential_KidBedroom",
        "Residential_Kitchen",
        "Residential_Laundry",
        "Residential_Living",
        "Residential_Lobby",

        "Office_Lobby",
        "Office_Blocks",
        "Office_Boss",
        "Office_Warehouse",
        "Office_Reception",
        "Office_Bathrooms"
    };
     
     //Estilos de habitación
     public enum RoomStyle
     {
         EmptyRoom,
         Hospital_Lobby, //ok 4
         Hospital_Reception, //ok 1
         Hospital_SurgeryRoom, //ok 2
         Hospital_Bathrooms, //ok 3
         Hospital_DiningRoom,//ok 3
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

    public bool isLobby = false;
    public bool isStairsEntrace = false;
    public bool isStairsRoom = false;

    public bool debugConstruction;
    
    public void Init(BuildingGenerator buildingGenerator, string roomstyle)
    {
        buildingScriptableObject = buildingGenerator.buildingScriptableObject;
        
        GetResources(buildingScriptableObject);

        selectedInteriors = (from interior in interiorPrefabs
            where interior.name.Contains(roomstyle)
            select interior).ToArray();
    }

    /// <summary>
    /// Generates a room from seed
    /// </summary>
    /// <param name="gOInLeft"></param>
    /// <param name="doorInLeft"></param>
    /// <param name="maxWidth"></param>
    /// <param name="gOInRight"></param>
    /// <param name="doorInRight"></param>
    /// <param name="withDecoration"></param>
    public void BuildRoom(
        Transform spawnOrigin,
        
        GameObject gOInLeft = null,
        bool doorInLeft = false,
        
        GameObject gOInRight = null,
        bool doorInRight = false,
        
        string roomStyle = null,
        
        int blockCount = 0,
        
        bool debug = false
        )
    {
        if (debug)
        {
            debugConstruction = true;
            
            Debug.Log("Generating Room." + 
                      " gOInLeft = " + gOInLeft + 
                      ", doorInLeft = " + doorInLeft + 
                      ", roomWidth = " + roomWidth + 
                      ", gOInRight = " + gOInRight + 
                      ", doorInRight = " + doorInRight + 
                      ", withDecoration= " + roomStyle);  
        }
        //Definimos el estilo de habitacion
        //TODO: Crear Switch segun estilo
        //roomStyle = (RoomGenerator.RoomStyle) Random.Range(
        //    (int) RoomGenerator.RoomStyle.Hospital_SurgeryRoom,
        //    (int) RoomGenerator.RoomStyle.end);
        // Debug.Log("Selected " + roomStyle.ToString());  

        
        
        //Generar extremo izquierdo.
        if (gOInLeft != null)
        {
            GenerateSide(gOInLeft, spawnOrigin.position, doorInLeft, 90);
        }
        
        //BUCLE (relleno X): desde 0 hasta ancho de la habitacion
        for (int xFill = 0; xFill < roomWidth; xFill++)
        {

            //Generar una ud. de bloque (base, pared y techo) segun altura de la habitacion. Guardar la pared en una lista

            GenerateBlock(partsWidth,partsHeight, _basesPrefabs[0], _wallsPrefabs[0], roomHeight,
                spawnOrigin);
            
            //Restamos un bloque para llevar control y Movemos el punto de origen a la derecha en 1 ud para el siguiente bloque
            blockCount -= 1;
            spawnOrigin.position += Vector3.right * partsWidth;



        }

        //Generar extremo derecho si corresponde.
        if (gOInRight != null)
        {
            GenerateSide(gOInRight, spawnOrigin.position, doorInRight, -90);
        }

        #region Interior Decoration

        if (roomStyle != null)
        {
            GameObject[] interiorQuery = (from interior in interiorPrefabs 
                where interior.name.Contains(roomStyle) select interior).ToArray();
            GameObject interiorResult = null;
            for (int i = 0; i < basesList.Count; i++)
            {
                try
                {
                    interiorResult = interiorQuery[i];
                }
                catch
                {
                    interiorResult = interiorQuery[0];
                }
                finally
                {
                    GenerateInterior(interiorResult, basesList[i].transform);
                }
            }
        }
        #endregion
        
        GameObject[] queryPrefabsByBuilding = (from interior in interiorPrefabs
            where interior.name.Contains(buildingScriptableObject.buildingStyle.ToString()) 
            select interior).ToArray(); //Ejemplo: selecciona todos los prefabs que empiezan con "Hospital"

        
        //Hospital_Lobby
        
        var roomPrefabs = from interior in interiorPrefabs where interior.name ==         
        
                                                                 
                                                                 
    }
    
    
    public void CombineMeshes(bool combine, Transform transform)
    {
        if (combine)
        {
            MeshFilter meshFilter = transform.gameObject.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = transform.gameObject.AddComponent<MeshRenderer>();
            MeshCombiner meshCombiner = transform.gameObject.AddComponent<MeshCombiner>();
            meshCombiner.CreateMultiMaterialMesh = true;
            meshCombiner.CombineInactiveChildren = false;
            meshCombiner.DeactivateCombinedChildren = false;
            meshCombiner.DeactivateCombinedChildrenMeshRenderers = true;
            meshCombiner.GenerateUVMap = false;
            meshCombiner.DestroyCombinedChildren = false;
            meshCombiner.CombineMeshes(false);
        }
        
    }

    public void ChangeMaterial(List<GameObject> gameObjects, Material interiorMaterial, Material exteriorMaterial = null)
    {
        foreach (GameObject gameObject in gameObjects)
        {
            MaterialManager materialManager = gameObject.GetComponent<MaterialManager>();
            
            materialManager.meshRenderer = gameObject.GetComponent<MeshRenderer>();
            
            Material[] materials = materialManager.meshRenderer.materials;
            materials[materialManager.interiorFaceMaterialIndex] = interiorMaterial;
            if (exteriorMaterial != null)
            {
                materials[materialManager.exteriorFaceMaterialIndex] = exteriorMaterial;
            }
            else
            {
                materials[materialManager.exteriorFaceMaterialIndex] = interiorMaterial;
            }
            materialManager.meshRenderer.materials = materials;

        }
        
        
    }
    
    public void GenerateBlock(Transform spawnOrigin,
        int partsWidth,
        int partsHeight, GameObject baseGo, GameObject wallGo, int height
        )
    {
        //Generate Base
        GameObject instBase = Instantiate(baseGo, spawnOrigin.position, baseGo.transform.rotation,
            wallsAndInteriorGroup);
        basesList.Add(instBase);
        //Set BackWall initial point for height at room's base
        spawnYPoint = instBase.transform.position - (Vector3.back * (partsWidth / 2));

        //Generate a BackWall depending Room Height
        for (int i = 0; i < height; i++)
        {
            GameObject instWall = Instantiate(wallGo, spawnYPoint, wallGo.transform.rotation,
                backWallGroup);
            if (i < 1)
            {
                backWallsList.Add(instWall);
            }

            spawnYPoint = instWall.transform.position + (Vector3.up * partsHeight);

            if (i >= height - 1)
            {
                //Generate Ceiling
                Vector3 ceilingPos = new Vector3(instBase.transform.position.x, spawnYPoint.y,
                    instBase.transform.position.z);
                GameObject instCeiling = Instantiate(baseGo, ceilingPos, baseGo.transform.rotation,
                    wallsAndInteriorGroup);
                instCeiling.name = "Ceiling";
                ceilingsList.Add(instCeiling);
            }
        }
    }

    public void GenerateSide(GameObject sideGameObjPrefab, Vector3 spawnPoint,
        bool withDoor, float rotation)
    {
        if (debugConstruction)
        {
            Debug.Log("Generating Side. with door = " + withDoor);
        }

        GameObject instSide = Instantiate(sideGameObjPrefab, spawnPoint - Vector3.right * partsWidth / 2,
            Quaternion.Euler(0, rotation, 0), wallsAndInteriorGroup);
        
        if (withDoor)
        {
            InstantiateDoor(instSide);
            
            doorWallsList.Add(instSide);

        }
    }

    void InstantiateDoor(GameObject targetDoorWall)
    {
        GameObject randomDoor = null;
        GameObject instDoor = null;
        if (targetDoorWall.tag == "DoubleDoorWall")
        {
            GameObject[] doors = (from doorPrefab in _doorsPrefabs where doorPrefab.tag == "DoubleDoor" select doorPrefab).ToArray();

            randomDoor = doors[Random.Range(0,doors.Length)];
        }
        else if (targetDoorWall.tag == "DoorWall")
        {
            GameObject[] doors = (from doorPrefab in _doorsPrefabs where doorPrefab.tag == "Door" select doorPrefab).ToArray();
            randomDoor = doors[Random.Range(0,doors.Length)];

        }
        else
        {
            Debug.LogWarning("DoorWall is not Tagged, no door instantiated");
        }

        if (randomDoor != null)
        {
            instDoor = Instantiate(randomDoor,
                targetDoorWall.transform.position,
                targetDoorWall.transform.rotation,
                transform); 
        }
        
        if (instDoor.TryGetComponent(out Door door))
        {
            door.doorOrientation = Door.DoorOrientation.Side;
        }
    }
    public void GenerateBackDoor(GameObject doorWallPrefab, GameObject doorPrefab, bool randomPos = true, int doorPos = 0)
    {
        //Debug.Log("DoorPos is: "+doorPos );
        int selectedWall = doorPos - 1;
        if (selectedWall < 0)
        {
            selectedWall = 0;
        }

        if (randomPos)
        {
            selectedWall = Random.Range(0, backWallsList.Count);
        }

        if (doorPos > backWallsList.Count)
        {
            if (debugConstruction)
            {
                Debug.Log("doorPos " + doorPos + " is greater than backwalls " +
                          backWallsList.Count + " in " + gameObject.name);
            }

        }
        else
        {
            if (backWallsList.Count > 0)
            {
                //Seleccionar la pared a reemplazar por un marco
//                Debug.Log("BackWall Count: " + roomGenerator.backWalls.Count + " " + roomGenerator.gameObject.name);
                GameObject backWall = backWallsList[selectedWall];

                //Desactivar la pared seleccionada y ponerla en un grupo que no sea de las paredes traseras.
                backWall.transform.parent = wallsAndInteriorGroup;
                backWall.SetActive(false);

                //Obtener rotacion de la pared
                Quaternion wallDoorRot = Quaternion.Euler(0, backWall.transform.rotation.y, 0);
                //Crear un marco
                GameObject instDoorWall = Instantiate(doorWallPrefab, backWall.transform.position,
                    wallDoorRot, backWallGroup);
                //Agregar el marco a la lista
                backDoorWallsList.Add(instDoorWall);

                //CREATE A DOOR IN THE SELECTED DOORWALL
                //TODO: Mejorar
                GameObject instDoor = null;
                if (isStairsEntrace)
                {
                    //TODO: SELECCIONAR PUERTAS DE TIPO PARA HABITACION DE ESCALERAS
                    instDoor =
                        Instantiate(doorPrefab,
                            instDoorWall.transform.position,
                            instDoorWall.transform.rotation,
                            transform);
                }
                else
                {
                    instDoor =
                        Instantiate(doorPrefab,
                            instDoorWall.transform.position,
                            instDoorWall.transform.rotation,
                            transform);
                }

                Door doorScript = instDoor.GetComponent<Door>();
                doorScript.doorOrientation = Door.DoorOrientation.Back;
                doorScript.outsidePlayLine = transform.position.z;
                doorScript.insidePlayLine = transform.position.z + partsWidth;

                //roomGenerator.backWalls.Remove(backWall);
                //Destroy(backWall);
            }

        }
    }


    public void GenerateInterior(GameObject prefab, Transform spawnPos,
        int order = 0)
    {
        bool canDecorate = true;
        
        if (order > interiorPrefabs.Count)
        {
            order = 0;
        }

        for (int i = 0; i < backDoorWallsList.Count; i++)
        {
            if (backDoorWallsList[i].transform.position.x == spawnPos.position.x)
            {
                canDecorate = false;
            }
        }

        if (canDecorate && interiorPrefabs != null)
        {
            Instantiate(interiorPrefabs[order], spawnPos.position, spawnPos.rotation,
                backWallGroup);
        }

    }

    public void GetResources(BuildingAssets scriptableObject)
    {
        _mainDoorPrefabs = scriptableObject.mainDoorPrefabs;
        _basesPrefabs = scriptableObject.basesPrefabs;
        _wallsPrefabs = scriptableObject.wallsPrefabs;
        _doorWallsPrefabs = scriptableObject.doorWallsPrefabs;
        _doorsPrefabs = scriptableObject.doorsPrefabs;
        interiorPrefabs = scriptableObject.interiorPrefabs;
        _exteriorWallsPrefabs = scriptableObject.exteriorWallsPrefabs;
        _exteriorDoorWallPrefabs = scriptableObject.exteriorDoorWallsPrefabs;
        _lightsPrefabs = scriptableObject.lightsPrefabs;
        _cornicesPrefabs = scriptableObject.cornicesPrefabs;
        _corniceCornersPrefabs = scriptableObject.corniceCornersPrefabs;

        interiorMaterials = scriptableObject.interiorMaterials;
        exteriorMaterials = scriptableObject.exteriorMaterials;
        floorBaseMaterials = scriptableObject.floorBaseMaterials;

    }

    

    
    
    

   
    
    


    
    
}