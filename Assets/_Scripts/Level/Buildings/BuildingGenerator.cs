using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Unity.Mathematics;
using UnityEditor.Timeline;
using UnityEngine;
using Object = System.Object;
using Random = UnityEngine.Random;

[RequireComponent(typeof(MeshCombiner))]

public class BuildingGenerator : MonoBehaviour
{
    //Definir familia del edificio
    public BuildingAssets buildingScriptableObject;
    public BuildingAssets.BuildingStyle buildingStyle;
    
    //Definir area de las piezas (default width: 6, height: 3, thickness: 0.1f)
    //cada pieza representara una unidad (ud) en altura, en anchura y profundidad
    public int partsWidth = 6;
    public int partsHeight = 3;
    public float partsThickness = 0.1f;

    //Definir dimension del edificio en X, Y y Z en ud
    public int maxBldWidth = 1;
    public int maxBldHeight = 1;
    public int maxBldDepth = 1;
    public int roomsPerFloor = 3;

    //Definir punto de origen de generación
    public Transform spawnOrigin;

    //Definimos una lista de los pisos que se van a generar
    public List<GameObject> floorList;
    
    //Definimos una lista de las paredes externas para luego ocultar
    [HideInInspector] public List<GameObject> extWallList;

    #region Rooms
    
    //Referencia al gameobject de generador de habitación, su script y su instancia actual
    public GameObject roomSeed;
    private RoomGenerator _roomGen;
    private GameObject instRoom;
    
    //Definimos una lista de las habitaciones generadas y su gameobject padre en la jerarquia
    public List<RoomGenerator> rooms;
    public Transform roomsTransform;

    //Definimos la anchura de la habitación de escaleras y elevadores que conecten los pisos
    
    public int stairsRoomWidth = 3;

    public int elevatorRoomWidth = 3;
    
    #endregion

    //Variable de control de espacio libre en el edificio

    //Referencia al script del mesh combiner
    private MeshCombiner _meshCombiner;
    public bool combineMeshesAtEnd;
    
    //Debug opcional
    public bool debugConstruction;

    //variable de control de cantidad de habitaciones
    [SerializeField]
    private int roomNumber;
    
    //TODO: Pasar esta variable a RoomGenerator
    private int interiorMat;
    
    
    //Listas de prefabs para utilizar en construcción
    #region Building prefabs

    public GameObject[] doorsPrefabs;
    public GameObject[] wallsPrefabs;
    public GameObject[] doorWallsPrefabs;
    public GameObject[] basesPrefabs;
    public GameObject[] exteriorWallsPrefab;
    public GameObject[] exteriorDoorWallsPrefabs;
    public GameObject[] interiorPrefabs;
    public GameObject[] lightsPrefabs;
    public GameObject[] mainDoorPrefabs;
    public GameObject[] cornicesPrefabs;
    public GameObject[] corniceCornersPrefabs;
    public GameObject[] stairsPrefabs;

    #endregion

    //Directivas de los materiales a utilizar en el edificio  
    #region Materials

    public Material[] exteriorMaterials;
    public Material[] interiorMaterials;
    public Material[] floorBaseMaterials;

    #endregion
    
    
    
    private void Awake()
    {
        
        buildingStyle = buildingScriptableObject.buildingStyle;
        //Cargar recursos de partes segun tipo de familia de edificio
        GetResources();

    }

    private void Start()
    {
        //Establecer punto de origen de generacion en x0,y0,z0
        spawnOrigin.position = transform.position;

        

        //Comenzar la generacion de habitaciones

        //BUCLE (Profundidad Z)
        for (int Z = 0; Z < maxBldDepth; Z++)
        {
            //establecemos el puntero de generacion en x=0 y=0 y Z=actual
            spawnOrigin.position = new Vector3(transform.position.x, transform.position.y, spawnOrigin.position.z);
            
            //BUCLE (vertical Y)desde 0 hasta el alto del edificio: 
            for (int Y = 0; Y < maxBldHeight; Y++)
            {
                //Creamos un GameObject vacio para agrupar las habitaciones del piso actual
                GameObject floorGroup = new GameObject("Floor " + Y + " depth " + Z);
                floorGroup.transform.position =
                    new Vector3(transform.position.x, spawnOrigin.position.y, spawnOrigin.position.z);
                floorList.Add(floorGroup);
                floorGroup.transform.parent = transform;

                //Colocamos el puntero de generacion en X=0 para empezar a instanciar habitaciones de izquierda a derecha
                spawnOrigin.position =
                    new Vector3(transform.position.x, spawnOrigin.position.y, spawnOrigin.position.z);

                
                
                
                //SI: el punto de origen se encuentra en coordenada Y = 0 y Z = 0, generar un lobby:
                if (Y == 0 && Z == 0)
                {  
                    //Colocar un generador de habitacion en puntero de origen
                    RoomGenerator room = SetRoomSeed(spawnOrigin, maxBldWidth - stairsRoomWidth);
                    
                    string buildingName = buildingStyle.ToString();
                    RoomGenerator.RoomStyle roomName = (RoomGenerator.RoomStyle)Enum.Parse(typeof(RoomGenerator.RoomStyle),
                        buildingName + "_" + "Lobby");
                    room.isLobby = true;
                    room.BuildRoom(spawnOrigin, exteriorDoorWallsPrefabs[0], true, doorWallsPrefabs[0], true, roomName , debugConstruction);
                    rooms.Add(room);
                    
                    room.transform.parent = floorGroup.transform;

                    //Agregamos al final una habitacion de entrada a escaleras
                    room = SetRoomSeed(spawnOrigin, stairsRoomWidth);
                    room.isStairsEntrace = true;
                    room.BuildRoom(spawnOrigin, doorWallsPrefabs[0], true, exteriorDoorWallsPrefabs[0],
                        true,RoomGenerator.RoomStyle.EmptyRoom,debugConstruction);
                    rooms.Add(room);
                    room.transform.parent = floorGroup.transform;

                }
                //SINO:
                else
                {
                    //Dividimos las habitaciones en partes iguales segun cantidad de habitaciones por piso
<<<
                    //BUCLE (Horizontal X): desde 0 hasta la cantidad de habitaciones por piso  
                    for (int X = 0; X < roomsWidth.Length; X++)
                    {
                        //colocar un nuevo generador de habitacion y obtener su script
<<<
                        RoomGenerator.RoomStyle roomName = (RoomGenerator.RoomStyle)Enum.Parse(
                            typeof(RoomGenerator.RoomStyle),
                            interiorPrefabs[Random.Range(0, interiorPrefabs.Length)].name);
                        if (X == 0) // crear una habitacion con pared al comienzo en la primera iteracion 
                        {
                            room.BuildRoom(spawnOrigin,exteriorWallsPrefab[0],
                                false, doorWallsPrefabs[0],true,roomName,debugConstruction);
                            room.transform.parent = floorGroup.transform;
                            rooms.Add(room);
                        }
                        else if (X > 0 && X < roomsPerFloor) //Rellenar las habitaciones del medio
                        {
                            room = SetRoomSeed(spawnOrigin, roomsWidth[X]);
                            
                            room.BuildRoom(spawnOrigin, 
                                doorWallsPrefabs[0], true,
                                doorWallsPrefabs[0], true,
                                roomName, debugConstruction);
                            rooms.Add(room);
                            room.transform.parent = floorGroup.transform;
                            
                        }
                        else // crear una habitacion con pared al final en la ultima iteracion
                        {
                            room = SetRoomSeed(spawnOrigin, stairsRoomWidth);
                            
                            room.BuildRoom(spawnOrigin, 
                                doorWallsPrefabs[0], true,
                                doorWallsPrefabs[0], true,
                                roomName, debugConstruction);
                            rooms.Add(room);
                            room.transform.parent = floorGroup.transform;
                        }
                    }

                }

                RoomGenerator stairsRoom = SetRoomSeed(spawnOrigin, stairsRoomWidth);
                stairsRoom.transform.parent = floorGroup.transform;
                rooms.Add(stairsRoom);

                if (Y != 0 && Z == 0)
                {
                    stairsRoom.isStairsEntrace = true;
                    stairsRoom.BuildRoom(spawnOrigin,
                        doorWallsPrefabs[0], true,
                        doorsPrefabs[0], true,
                        RoomGenerator.RoomStyle.EmptyRoom, debugConstruction);
                    
                }
                else if (Z == 1)
                {
                    stairsRoom.isStairsRoom = true;
                    stairsRoom.BuildRoom(spawnOrigin,
                        wallsPrefabs[0], false,
                        wallsPrefabs[0] ,false,
                        RoomGenerator.RoomStyle.Stairs,debugConstruction);
                }
                else if (Z != 0)
                {
                    RoomGenerator.RoomStyle roomName = (RoomGenerator.RoomStyle)Enum.Parse(
                        typeof(RoomGenerator.RoomStyle),
                        interiorPrefabs[Random.Range(0, interiorPrefabs.Length)].name);
                    stairsRoom.BuildRoom(spawnOrigin, doorWallsPrefabs[0], true, exteriorWallsPrefab[0],
                        false,roomName, debugConstruction);
                }
                
                //mover punto de origen en coordenada X:0 e Y en 1 ud (+ el grosor de las piezas para no interpolar)para continuar generando el siguiente edificio
                spawnOrigin.position += Vector3.up * (partsHeight + partsThickness);

            } //FIN bucle Y

            //Mover el punto de origen al siguiente punto Z en 1 ud
            spawnOrigin.position += Vector3.forward * (partsWidth);

        } //FIN bucle Z

        foreach (RoomGenerator room in rooms)
        {
             room.GenerateLights(room.ceilingsList.Count / 2);
             
            //Generamos puertas traseras
            if (room.transform.position.z < transform.position.z + ((maxBldDepth - 1) * partsWidth))
                //Si la habitación no esta en la ultima profundidad, generar puertas traseras
            {
                if (room.isLobby) //Si la habitación es un lobby, generar puertas en partes iguales
                {
                    int doorPos = Mathf.RoundToInt(room.backWallsList.Count / roomsPerFloor);
                    for (int i = 0; i < roomsPerFloor; i += doorPos)
                    {
                        if (doorPos <= room.backWallsList.Count)
                        {
                            room.GenerateBackDoor(doorWallsPrefabs[0], doorsPrefabs[Random.Range(0,doorsPrefabs.Length)],false,doorPos);
                        }

                    }
                }
                else if (room.isStairsEntrace)
                {
                    GameObject[] stairsDoors = (from door in doorsPrefabs where door.tag == "StairsDoor" select door).ToArray(); 
                    room.GenerateBackDoor(doorWallsPrefabs[0], 
                        stairsDoors[Random.Range(0,stairsDoors.Length)], false,0);
                    room.GenerateBackDoor(doorWallsPrefabs[0], 
                        stairsDoors[Random.Range(0,stairsDoors.Length)], false, stairsRoomWidth);
                }
                else if (!room.isStairsRoom)
                {
                    room.GenerateBackDoor(doorWallsPrefabs[0], doorsPrefabs[Random.Range(0,doorsPrefabs.Length)],true);
                }
            }

            //Llenamos las habitaciones de objetos de decoracion segun estilo
            //y cambiamos el material de los interiores


            if (room.isStairsEntrace || room.isStairsRoom)
            {
                interiorMat = 0;
            }
            else
            {
                interiorMat = Random.Range(0, interiorMaterials.Length);

                for (int i = 0; i < room.basesList.Count; i++)
                {
                    if (room.isLobby)
                    {
                        GameObject[] lobbyinteriors = (from interior in interiorPrefabs 
                            where interior.name.Contains(buildingStyle.ToString() + "_" + "Lobby") 
                            select interior).ToArray();
                        room.GenerateInterior(lobbyinteriors[Random.Range(0,lobbyinteriors.Length)], room.basesList[i].transform);
                    }
                    else if (!room.isStairsRoom)
                    {
                        room.GenerateInterior(interiorPrefabs[Random.Range(0,interiorPrefabs.Length)], room.basesList[i].transform);
                    }
                }

            }

            room.ChangeMaterial(room.basesList,
                floorBaseMaterials[Random.Range(0, floorBaseMaterials.Length)]);
            room.ChangeMaterial(room.ceilingsList, interiorMaterials[interiorMat]);
            room.ChangeMaterial(room.backWallsList, interiorMaterials[interiorMat]);
            room.ChangeMaterial(room.backDoorWallsList, interiorMaterials[interiorMat]);
            if (room.transform.position.x == transform.position.x)
            {
                room.ChangeMaterial(room.doorWallsList, interiorMaterials[interiorMat],
                    exteriorMaterials[Random.Range(0, exteriorMaterials.Length)]);
            }
            else
            {
                room.ChangeMaterial(room.doorWallsList, interiorMaterials[interiorMat]);
            }


            if (room.isStairsRoom)
            {
                foreach (GameObject ceiling in room.ceilingsList)
                {
                    ceiling.SetActive(false);
                }

                if (room.transform.position.y > transform.position.y)
                {
                    foreach (GameObject roomBase in room.basesList)
                    {
                        roomBase.SetActive(false);
                    }
                }

                room.GenerateStairs(stairsPrefabs[0]);
            }

            for (int i = 0; i < room.transform.childCount; i++)
            {
                if (!room.transform.GetChild(i).gameObject.activeSelf)
                {
                    Destroy(room.transform.GetChild(i).gameObject);
                }
            }

            room.CombineMeshes(combineMeshesAtEnd, room.wallsAndInteriorGroup);
            room.CombineMeshes(combineMeshesAtEnd, room.backWallGroup);

        }

        //Agregamos el script que oculta las paredes que estan entre la cámara y el personaje
        AddHideFrontFace(floorList, true);

        //Generamos paredes exteriores

        GenerateExteriorWalls(spawnOrigin, maxBldWidth, maxBldHeight);

        //Generamos la terraza del edificio
        GenerateRoof(maxBldWidth, spawnOrigin, corniceCornersPrefabs, cornicesPrefabs);
    }


    public void GetResources()
    {
        mainDoorPrefabs = buildingScriptableObject.mainDoorPrefabs;
        basesPrefabs = buildingScriptableObject.basesPrefabs;
        wallsPrefabs = buildingScriptableObject.wallsPrefabs;
        doorWallsPrefabs = buildingScriptableObject.doorWallsPrefabs;
        doorsPrefabs = buildingScriptableObject.doorsPrefabs;
        interiorPrefabs = buildingScriptableObject.interiorPrefabs;
        exteriorWallsPrefab = buildingScriptableObject.exteriorWallsPrefabs;
        exteriorDoorWallsPrefabs = buildingScriptableObject.exteriorDoorWallsPrefabs;
        lightsPrefabs = buildingScriptableObject.lightsPrefabs;

        cornicesPrefabs = buildingScriptableObject.cornicesPrefabs;
        corniceCornersPrefabs = buildingScriptableObject.corniceCornersPrefabs;

        interiorMaterials = buildingScriptableObject.interiorMaterials;
        exteriorMaterials = buildingScriptableObject.exteriorMaterials;
        floorBaseMaterials = buildingScriptableObject.floorBaseMaterials;

    }

    /// <summary>
    /// Places a room seed in a origin
    /// </summary>
    /// <param name="spawnOrigin">Origin of the room</param>
    /// <param name="roomWidht">Max room Width</param>
    /// <param name="roomHeight">Max room Height</param>
    public RoomGenerator SetRoomSeed(Transform spawnOrigin, int roomWidht = 1, int roomHeight = 1)
    {
        //Colocar un generador de habitacion en punto de origen
        GameObject room = Instantiate(roomSeed, spawnOrigin.position, transform.rotation, roomsTransform);
        
        //obtener el script del generador de habitacion
        _roomGen = room.GetComponent<RoomGenerator>();
        
        //Configurar la habitación en estilo y dimensiones 
     
        _roomGen.Init(this,roomWidht,roomHeight);
        
        //TODO: Exportar prefabs de interiores y materiales al RoomGenerator
        
        //agregamos la instancia actual de la habitación a la lista de habitaciones general
        rooms.Add(_roomGen);

        //Renombramos y numeramos la instancia de la habitación
        PlayerPrefs.SetInt("RoomNum", roomNumber + 1);
        roomNumber = PlayerPrefs.GetInt("RoomNum");
        instRoom.name = "Room N° " + roomNumber;

        return _roomGen;
    }
    
    public void AddHideFrontFace(List<GameObject> list, bool isRoom)
    {
        foreach (GameObject floorGO in list)
        {
            BoxCollider boxCollider = floorGO.AddComponent<BoxCollider>();
            boxCollider.isTrigger = true;
            float xCenter = (maxBldWidth * partsWidth) - partsWidth;

            //0z center 4.5
            //0z size 6
            //1z center 3
            //1z size 3


            HideFrontFace hideFrontFace = floorGO.AddComponent<HideFrontFace>();
            hideFrontFace.facesToHide = new List<Transform>();
            if (isRoom)
            {
                boxCollider.center = new Vector3(xCenter / 2, partsWidth / 2,
                    (((partsWidth * maxBldDepth) - floorGO.transform.position.z) / 2));

                boxCollider.size = new Vector3(maxBldWidth * partsWidth, partsWidth / 2,
                    ((maxBldDepth - 1) * partsWidth) - floorGO.transform.position.z);

                //Debug.Log( floorGO.name + " child Count: " + floorGO.transform.childCount);
                for (int i = 0; i < floorGO.transform.childCount; i++)
                {

                    if (floorGO.transform.GetChild(i).TryGetComponent(out RoomGenerator roomgen))
                    {
                        
                        // Debug.Log("found roomgenerator at " + floorGO.transform.GetChild(i).gameObject.name);

                        // Debug.Log("backWallGroup count " + roomgen.backWallGroup.gameObject);
                        /*for (int j = 0; j < roomGenerator.backWallGroup.childCount; j++)
                        {
                            hideFrontFace.facesToHide.Add(roomGenerator.backWallGroup.GetChild(j));
                        }*/
                        hideFrontFace.facesToHide.Add(roomgen.backWallGroup);
                    }
                    //   
                }
            }
            else
            {
                boxCollider.center = new Vector3(xCenter / 2, partsWidth / 2,
                    ((partsWidth * maxBldDepth) - partsWidth - floorGO.transform.position.z) / 2);

                boxCollider.size = new Vector3(maxBldWidth * partsWidth, partsWidth / 2,
                    (maxBldDepth * partsWidth) - floorGO.transform.position.z);
                
                hideFrontFace.facesToHide.Add(floorGO.transform);
            }
        }
    }


    /// <summary>
    /// Generates exterior walls at front of the building
    /// </summary>
    /// <param name="spawnOrigin"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
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
            exteriorFloor.transform.position = new Vector3(transform.position.x, Y * partsWidth, transform.position.z);
            
            spawnOrigin.position = new Vector3(transform.position.x, spawnOrigin.position.y, transform.position.z);
            for (int X = 0; X < width; X++)
            {
                GameObject instExtWall = Instantiate(exteriorWallsPrefab[Random.Range(0, exteriorWallsPrefab.Length)],
                    spawnOrigin.position - (Vector3.forward * partsWidth / 2), transform.rotation,
                    exteriorFloor.transform);
                
                spawnOrigin.position += Vector3.right * partsWidth;
            }
            
            extWallList.Add(exteriorFloor);
            
            spawnOrigin.position += Vector3.up * (partsHeight + 0.1f);

            if (combineMeshesAtEnd)
            {
                CombineMeshes(exteriorFloor.transform);
            }
        }

        AddHideFrontFace(extWallList, false);
    }

    public void GenerateRoof(int width, Transform origin, GameObject[] corniceCorners, GameObject[] cornices)
    {
        GameObject roofGroup = new GameObject("Roof");
        roofGroup.transform.position = transform.position + Vector3.up * (maxBldHeight * partsHeight);
        roofGroup.transform.parent = transform;
        float yPos = transform.position.y + (maxBldHeight * partsWidth) + 0.5f;
        float xPos = roofGroup.transform.position.x;
        origin.position = roofGroup.transform.position;


        for (int i = 0; i < width; i++)
        {
            //Generate front Cornices
            if (i == 0)//left corner
            {
                Instantiate(corniceCorners[Random.Range(0, corniceCorners.Length)], origin.position, origin.rotation,
                    roofGroup.transform);
            }
            else if (i > 0 && i < width-1)//fill
            {
                Instantiate(cornices[Random.Range(0, cornices.Length)],
                    origin.position + (Vector3.back * partsWidth / 2), origin.rotation, roofGroup.transform);
            }
            else//right corner
            {
                Quaternion cornerRightRot =
                    Quaternion.Euler(origin.rotation.x, origin.rotation.y - 90, origin.rotation.z);
                GameObject instCorner = Instantiate(corniceCorners[Random.Range(0, corniceCorners.Length)],
                    origin.position, cornerRightRot, roofGroup.transform);
                xPos = instCorner.transform.position.x;
            }

            origin.position += Vector3.right * partsWidth;
        }
        //Generate Back Cornices
        origin.position = roofGroup.transform.position + Vector3.forward * ((maxBldDepth * partsWidth) - partsWidth);

        for (int i = 0; i < width; i++)
        {
            if (i == 0)//Left corner
            {
                Quaternion cornerleftRot =
                    Quaternion.Euler(origin.rotation.x, origin.rotation.y + 90, origin.rotation.z);
                Instantiate(corniceCorners[Random.Range(0, corniceCorners.Length)], origin.position, cornerleftRot,
                    roofGroup.transform);
            }
            else if (i > 0 && i < width-1)//Fill
            {
                Quaternion corniceRot = Quaternion.Euler(origin.rotation.x, origin.rotation.y + 180, origin.rotation.z);

                Instantiate(cornices[Random.Range(0, cornices.Length)],
                    origin.position + (Vector3.forward * partsWidth / 2), corniceRot, roofGroup.transform);
            }
            else//Right Corner
            {
                Quaternion cornerRightRot =
                    Quaternion.Euler(origin.rotation.x, origin.rotation.y + 180, origin.rotation.z);
                Instantiate(corniceCorners[Random.Range(0, corniceCorners.Length)], origin.position, cornerRightRot,
                    roofGroup.transform);

            }

            origin.position += Vector3.right * partsWidth;
        }


        origin.position = roofGroup.transform.position;
        for (int i = 0; i < maxBldDepth - 2; i++)
        {

            origin.position += Vector3.forward * partsWidth;
            Quaternion corniceLeftRot = Quaternion.Euler(origin.rotation.x, origin.rotation.y + 90, origin.rotation.z);
            Quaternion corniceRightRot = Quaternion.Euler(origin.rotation.x, origin.rotation.y - 90, origin.rotation.z);

            Instantiate(cornices[Random.Range(0, cornices.Length)], origin.position + Vector3.left * partsWidth / 2,
                corniceLeftRot, roofGroup.transform);

            Vector3 corniceRightPos = new Vector3(xPos + ((float) partsWidth / 2), roofGroup.transform.position.y,
                origin.position.z);

            Instantiate(cornices[Random.Range(0, cornices.Length)], corniceRightPos, corniceRightRot,
                roofGroup.transform);
        }

        CombineMeshes(roofGroup.transform);
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

    public int[] SortRooms(int buildingWidth, int rmsPerFloor)
    {
        int widthleft = buildingWidth; 
        int[] sort = new int[rmsPerFloor];

        for (int i = 0; i < sort.Length; i++)
        {
            int randomWidth = Random.Range(1, widthleft);
            sort[i] = randomWidth;
            widthleft -= randomWidth;
        }

        return sort;
    }
}
