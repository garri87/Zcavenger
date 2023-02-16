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
    private GameObject[] _doorsPrefabs;
    private GameObject[] _wallsPrefabs;
    private GameObject[] _doorWallsPrefabs;
    private GameObject[] _basesPrefabs;
    private GameObject[] _exteriorWallsPrefabs;
    private GameObject[] _exteriorDoorWallPrefabs;
    private GameObject[] _lightsPrefabs;
    private GameObject[] _mainDoorPrefabs;
    private GameObject[] _cornicesPrefabs;
    private GameObject[] _corniceCornersPrefabs;
    private GameObject[] _stairsPrefabs;

    //Lista de prefabs del interior de la habitación
    public GameObject[] interiorPrefabs;
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

   /* public string[] roomStyles = new[]
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
    };*/
     
     //Estilos de habitación
     public enum RoomStyle
     {
         EmptyRoom,
         Stairs,
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
    
    public void Init(BuildingGenerator buildingGenerator, int roomWidth, int roomHeight)
    {
        buildingScriptableObject = buildingGenerator.buildingScriptableObject;
        
        GetResources(buildingScriptableObject);

        this.roomHeight = roomHeight;
        this.roomWidth = roomWidth;
    }

    /// <summary>
    /// Generates a room from seed
    /// </summary>
    /// <param name="spawnOrigin"></param>
    /// <param name="gOInLeft"></param>
    /// <param name="doorInLeft"></param>
    /// <param name="gOInRight"></param>
    /// <param name="doorInRight"></param>
    /// <param name="roomStyle"></param>
    /// <param name="debug">Debug the construction?</param>
    public void BuildRoom(
        Transform spawnOrigin,
        
        GameObject gOInLeft = null,
        bool doorInLeft = false,
        
        GameObject gOInRight = null,
        bool doorInRight = false,
        
        RoomStyle roomStyle = RoomStyle.EmptyRoom,
        
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

            GenerateBlock(spawnOrigin, partsWidth, partsHeight,
                _basesPrefabs[0], _wallsPrefabs[0], roomHeight);
            //Movemos el puntero a la derecha para el bloque siguiente
            spawnOrigin.position += Vector3.right * partsWidth;
        }

        //Generar extremo derecho si corresponde.
        if (gOInRight != null)
        {
            GenerateSide(gOInRight, spawnOrigin.position, doorInRight,-90);
        }

        #region Interior Decoration

        if (roomStyle != RoomStyle.EmptyRoom)
        {
            selectedInteriors = (from interior in interiorPrefabs
                where interior.name.Contains(roomStyle.ToString())
                select interior).ToArray();
            
            GameObject interiorResult = null;
            int order = 0;
            for (int i = 0; i < basesList.Count; i++)
            {
                try
                {
                    interiorResult = selectedInteriors[order];
                }
                catch
                {
                    interiorResult = selectedInteriors[0];
                    
                }
                finally
                {
                    GenerateInterior(interiorResult, basesList[i].transform);
                    order++;
                }
            }
        }
        #endregion
        
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
/// <summary>
/// Changes the interior an exterior materials of list of gameobjects
/// The GameObject must have a MaterialManager script attached
/// </summary>
/// <param name="gameObjects"></param>
/// <param name="interiorMaterial"></param>
/// <param name="exteriorMaterial"></param>
    public void ChangeMaterial(List<GameObject> gameObjects = null, Material interiorMaterial= null, Material exteriorMaterial = null)
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
    
    /// <summary>
    /// Generates a block of 1 base and n of walls where n is height of the room, places a ceiling at the end
    /// </summary>
    /// <param name="spawnOrigin"></param>
    /// <param name="partsWidth"></param>
    /// <param name="partsHeight"></param>
    /// <param name="baseGo"></param>
    /// <param name="wallGo"></param>
    /// <param name="height"></param>
    public void GenerateBlock(Transform spawnOrigin,
        int partsWidth,
        int partsHeight, 
        GameObject baseGo, 
        GameObject wallGo, 
        int height
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
            if (i < 1) //guardamos la primer pared trasera para luego eventualmente reemplazarla por puertas 
            {
                backWallsList.Add(instWall);
            }

            spawnYPoint = instWall.transform.position + (Vector3.up * partsHeight);

            if (i >= height - 1)
            {
                //Generar techo
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
        bool withDoor, float rotation = 0)
    {
        if (debugConstruction)
        {
            Debug.Log("Generating Side. with door = " + withDoor);
        }

        GameObject instSide = Instantiate(sideGameObjPrefab, spawnPoint - Vector3.right * partsWidth / 2,
            Quaternion.Euler(0, rotation, 0), wallsAndInteriorGroup);
        
        if (withDoor)
        {
            InstantiateDoor(instSide, _doorsPrefabs);
            
            doorWallsList.Add(instSide);
        }
    }

    void InstantiateDoor(GameObject targetDoorWall, GameObject[] doorprefabs)
    {
        GameObject randomDoor = null;
        GameObject instDoor = null;
        if (targetDoorWall.tag == "DoubleDoorWall")
        {
            GameObject[] doors = (from doorPrefab in doorprefabs where doorPrefab.tag == "DoubleDoor" select doorPrefab).ToArray();

            randomDoor = doors[Random.Range(0,doors.Length)];
        }
        else if (targetDoorWall.tag == "DoorWall")
        {
            GameObject[] doors = (from doorPrefab in doorprefabs where doorPrefab.tag == "Door" select doorPrefab).ToArray();
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

/// <summary>
/// Instantiates a prefab inside a room
/// </summary>
/// <param name="prefab"></param>
/// <param name="spawnPos"></param>
    public void GenerateInterior(GameObject prefab, Transform spawnPos)
    {
        bool canDecorate = true;
        RoomInterior roomInterior = prefab.GetComponent<RoomInterior>();
        //TODO: ver por que no se reconoce el script RoomInterior
        for (int i = 0; i < backDoorWallsList.Count; i++)
        {
            if (backDoorWallsList[i].transform.position.x == spawnPos.position.x)
            {
                if (!roomInterior.exitBack)
                {
                    canDecorate = false;    
                }
                else
                {
                    canDecorate = true;
                }
                
            }
        }

        if (canDecorate)
        {
            Instantiate(prefab, spawnPos.position, spawnPos.rotation,
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
        _stairsPrefabs = scriptableObject.stairsPrefabs;
        interiorMaterials = scriptableObject.interiorMaterials;
        exteriorMaterials = scriptableObject.exteriorMaterials;
        floorBaseMaterials = scriptableObject.floorBaseMaterials;

    }
    
    public void GenerateLights(int lightCount = 3) //of 10 blocks
    {
        int center = Mathf.RoundToInt(ceilingsList.Count / lightCount);
        for (int i = center; i < ceilingsList.Count; i += center)
        {
            GameObject instLight = Instantiate(_lightsPrefabs[0], ceilingsList[i].transform.position,
                    transform.rotation, transform);
        }
    }
    
    public void GenerateStairs(GameObject stairsPrefab)
    {
        Instantiate(stairsPrefab, transform.position, transform.rotation,
            transform);
    }
    
}
