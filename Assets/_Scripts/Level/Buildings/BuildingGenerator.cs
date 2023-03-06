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
    [Header("Part size")] public int partsWidth = 6;
    public int partsHeight = 3;
    public float partsThickness = 0.1f;

    //Definir dimension del edificio en X, Y y Z en ud
    [Header("Building Dimensions")] public int maxBldWidth = 1;
    public int maxBldHeight = 1;
    public int maxBldDepth = 1;
    public int roomsPerFloor = 3;

    //Definir Salidas del edificio
    [Header("Entrace doors")] public bool entraceSides;
    public bool entraceFront;

    //Definir punto de origen de generación
    [HideInInspector] public Transform spawnOrigin;

    [Space]
    //Definimos una lista de los pisos que se van a generar
    [HideInInspector]
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

    public int stairsRoomWidth = 2;

    public int elevatorRoomWidth = 1;

    #endregion

    //Variable de control de espacio libre en el edificio

    //Referencia al script del mesh combiner
    private MeshCombiner _meshCombiner;
    public bool combineMeshesAtEnd;

    //Debug opcional
    public bool debugConstruction;

    //variable de control de cantidad de habitaciones
    [SerializeField] private int roomNumber;

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
        spawnOrigin = transform.Find("SpawnOrigin");
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

                //inicializamos la cantidad restante de espacios para calcular cuanto falta para llenar el piso


                //SI: el punto de origen se encuentra en coordenada Y = 0 y Z = 0, generar un lobby:
                if (Y == 0 && Z == 0)
                {
                    //Colocar un generador de habitacion en puntero de origen
                    RoomGenerator room = SetRoomSeed(spawnOrigin, maxBldWidth - stairsRoomWidth);

                    string buildingName = buildingStyle.ToString();
                    RoomGenerator.RoomStyle roomName = (RoomGenerator.RoomStyle)Enum.Parse(
                        typeof(RoomGenerator.RoomStyle),
                        buildingName + "_" + "Lobby");
                    room.isLobby = true;
                    if (entraceSides)
                    {
                        room.BuildRoom(spawnOrigin, exteriorDoorWallsPrefabs[0], true, null, false, roomName,
                            debugConstruction);
                    }
                    else
                    {
                        room.BuildRoom(spawnOrigin, exteriorWallsPrefab[0], false, null, false, roomName,
                            debugConstruction);
                    }


                    room.transform.parent = floorGroup.transform;

                    //Agregamos al final una habitacion de entrada a escaleras
                    RoomGenerator stairsRoom = SetRoomSeed(spawnOrigin, stairsRoomWidth);
                    stairsRoom.isStairsEntrace = true;
                    if (entraceSides)
                    {
                        stairsRoom.BuildRoom(spawnOrigin, doorWallsPrefabs[0], true, exteriorDoorWallsPrefabs[0],
                            true, RoomGenerator.RoomStyle.EmptyRoom, debugConstruction);
                    }
                    else
                    {
                        stairsRoom.BuildRoom(spawnOrigin, doorWallsPrefabs[0], false, exteriorWallsPrefab[0],
                            false, RoomGenerator.RoomStyle.EmptyRoom, debugConstruction);
                    }

                    stairsRoom.transform.parent = floorGroup.transform;
                }
                //SINO:
                else
                {
                    //Dividimos las habitaciones en partes iguales segun cantidad de habitaciones por piso


                    int[] roomsWidth = SortRooms(maxBldWidth - stairsRoomWidth, roomsPerFloor);
                    //BUCLE (Horizontal X): desde 0 hasta la cantidad de habitaciones por piso  
                    for (int X = 0; X < roomsPerFloor; X++)
                    {
                        //colocar un nuevo generador de habitacion y obtener su script
                        RoomGenerator room = SetRoomSeed(spawnOrigin, roomsWidth[X]);
                        var styles = Enum.GetNames(typeof(RoomGenerator.RoomStyle)).ToArray();
                        string[] queryStyles =
                            (from style in styles where style.Contains(buildingStyle.ToString()) select style)
                            .ToArray();
                        RoomGenerator.RoomStyle roomName = (RoomGenerator.RoomStyle)Enum.Parse(
                            typeof(RoomGenerator.RoomStyle),
                            queryStyles[Random.Range(0, queryStyles.Length)]);


                        if (X == 0) // crear una habitacion con pared al comienzo en la primera iteracion 
                        {
                            room.BuildRoom(spawnOrigin, exteriorWallsPrefab[0],
                                false, null, false, roomName, debugConstruction);
                        }
                        else if (X > 0 && X < roomsPerFloor - 1) //Rellenar las habitaciones del medio
                        {
                            room.BuildRoom(spawnOrigin,
                                doorWallsPrefabs[0], true, null
                                , false,
                                roomName, debugConstruction);
                        }
                        else // crear una habitacion con pared al final en la ultima iteracion
                        {
                            room.BuildRoom(spawnOrigin,
                                doorWallsPrefabs[0], true,
                                null, false,
                                roomName, debugConstruction);
                        }

                        room.transform.parent = floorGroup.transform;
                    }

                    RoomGenerator stairsRoom = SetRoomSeed(spawnOrigin, stairsRoomWidth);

                    //Verificar si corresponde generar una habitacion de entrada de escalera o escaleras

                    if (Z == 0) // si Z está en 0 generar una entrada a escaleras
                    {
                        stairsRoom.isStairsEntrace = true;
                        stairsRoom.BuildRoom(spawnOrigin,
                            doorWallsPrefabs[0], true, exteriorWallsPrefab[0]
                            , false,
                            RoomGenerator.RoomStyle.EmptyRoom, debugConstruction);
                    }
                    else if (Z == 1) // Si Z esta en 1 generar habitacion de escaleras
                    {
                        stairsRoom.isStairsRoom = true;
                        stairsRoom.BuildRoom(spawnOrigin,
                            wallsPrefabs[0], false,
                            exteriorWallsPrefab[0], false,
                            RoomGenerator.RoomStyle.Stairs, debugConstruction);
                    }
                    else if (Z > 1) // si Z es mayor a uno generar una habitación normal
                    {
                        var styles = Enum.GetNames(typeof(RoomGenerator.RoomStyle)).ToArray();
                        string[] queryStyles =
                            (from style in styles where style.Contains(buildingStyle.ToString()) select style)
                            .ToArray();
                        RoomGenerator.RoomStyle roomName = (RoomGenerator.RoomStyle)Enum.Parse(
                            typeof(RoomGenerator.RoomStyle),
                            queryStyles[Random.Range(0, queryStyles.Length)]);

                        stairsRoom.BuildRoom(spawnOrigin, doorWallsPrefabs[0], true, exteriorWallsPrefab[0],
                            false, roomName, debugConstruction);
                    }
                    stairsRoom.transform.parent = floorGroup.transform;
                }

                //mover punto de origen en coordenada X:0 e Y en 1 ud (+ el grosor de las piezas para no interpolar)para continuar generando el siguiente edificio
                spawnOrigin.position += Vector3.up * (partsHeight + partsThickness);
            } //FIN bucle Y

            //Mover el punto de origen al siguiente punto Z en 1 ud
            spawnOrigin.position += Vector3.forward * (partsWidth);
        } //FIN bucle Z

        foreach (RoomGenerator room in rooms) // por cada habitación creada
        {
            if (room.roomWidth > 0)
            {
                //Generamos luces
                room.GenerateLights(room.ceilingsList.Count / 2);

                //Generamos puertas traseras
                if (room.transform.position.z < transform.position.z + ((maxBldDepth - 1) * partsWidth))
                    //Si la habitación no esta en la ultima profundidad, generar puertas traseras
                {
                    if (room.isLobby) //Si la habitación es un lobby, generar puertas en partes iguales
                    {
                        int doorPos = Mathf.RoundToInt(room.backWallsList.Count / roomsPerFloor);
                        Debug.Log("doorpos: " + doorPos);
                        Debug.Log("room.backWallsList.Count in " + room.name + ": " + room.backWallsList.Count);
                        for (int i = doorPos; i <= room.backWallsList.Count + 1; i += doorPos)
                        {
                            try
                            {
                                Debug.Log("Generating Lobby backdoors");
                                room.GenerateBackDoor(room.backWallsList[i]);
                                Debug.Log("Done");
                            }
                            catch
                            {
                                Debug.Log("index " + i + " is out of range of array in " + room.name);
                            }
                        }
                    }

                    else if (room.isStairsEntrace)
                    {
                        room.GenerateBackDoor(room.backWallsList[0]);
                        room.GenerateBackDoor(room.backWallsList.Last());
                    }

                    else if (!room.isStairsRoom)
                    {
                        Debug.Log("Generating Back Doors on: " + room.name);
                        int randomBackWall = Random.Range(0, room.backWallsList.Count() - 1);

                        RaycastHit hit;
                        LayerMask mask = LayerMask.GetMask("Door");
                        if (Physics.Raycast(
                                room.backWallsList[randomBackWall].transform.position + Vector3.up,
                                room.backWallsList[randomBackWall].transform.TransformDirection(Vector3.back),
                                out hit,
                                partsWidth,mask.value)
                           )
                        {
                            if (hit.collider.CompareTag("Door"))
                            {
                                randomBackWall = Random.Range(0, room.backWallsList.Count() - 1);
                                //TODO: HACER QUE DETECTE LA PUERTA. NO FUNCIONA
                                Debug.Log("Door Detected!");
                            }
                        }

                        room.GenerateBackDoor(room.backWallsList[randomBackWall]);
                    }
                }

                //Si hay un interior que obstruye una puerta,este se desactiva
                foreach (GameObject interior in room.interiorsList)
                {
                    for (int i = 0; i < room.backDoorWallsList.Count; i++)
                    {
                        if (interior.transform.position.x == room.backDoorWallsList[i].transform.position.x)
                        {
                            RoomInterior roomInterior = interior.GetComponent<RoomInterior>();
                            if (!roomInterior.exitBack)
                            {
                                interior.SetActive(false);
                            }
                        }
                    }
                }

                //y seteamos el material de los interiores

                if (room.isStairsEntrace || room.isStairsRoom)
                {
                    interiorMat = 0;
                }
                else
                {
                    interiorMat = Random.Range(0, interiorMaterials.Length);
                }

                room.ChangeMaterial(room.basesList,
                    floorBaseMaterials[Random.Range(0, floorBaseMaterials.Length)]);
                room.ChangeMaterial(room.ceilingsList, interiorMaterials[interiorMat]);
                room.ChangeMaterial(room.backWallsList, interiorMaterials[interiorMat]);
                room.ChangeMaterial(room.backDoorWallsList, interiorMaterials[interiorMat]);

                if (room.transform.position.x ==
                    transform.position
                        .x) // Si la habitación esta al frente incluir material de exterior en los laterales
                {
                    room.ChangeMaterial(room.doorWallsList, interiorMaterials[interiorMat],
                        exteriorMaterials[Random.Range(0, exteriorMaterials.Length)]);
                }
                else
                {
                    room.ChangeMaterial(room.doorWallsList, interiorMaterials[interiorMat]);
                }


                if (room.isStairsRoom) // Desactivar el techo y suelo de cada habitación de escalera para poder moverse,
                    // deja solo el prefab de escalera
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
                }

                for (int i = 0; i < room.transform.childCount; i++)
                {
                    if (!room.transform.GetChild(i).gameObject.activeSelf) //Destruir todos los gameobjects desactivados
                    {
                        Destroy(room.transform.GetChild(i).gameObject);
                    }
                }

                room.CombineMeshes(combineMeshesAtEnd, room.wallsAndInteriorGroup);
                room.CombineMeshes(combineMeshesAtEnd, room.backWallGroup);
            }
        }

        //Agregamos el script que oculta las paredes que estan entre la cámara y el personaje
        AddHideFrontFace(floorList, true);

        //Generamos paredes exteriores

        GenerateExteriorWalls(spawnOrigin, maxBldWidth, maxBldHeight);

        //Generamos la terraza del edificio
        GenerateRoof(maxBldWidth, spawnOrigin);
    }


    public void GetResources()
    {
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
        stairsPrefabs = buildingScriptableObject.stairsPrefabs;
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

        _roomGen.Init(this, roomWidht, roomHeight);

        //TODO: Exportar prefabs de interiores y materiales al RoomGenerator

        //agregamos la instancia actual de la habitación a la lista de habitaciones general
        rooms.Add(_roomGen);

        //Renombramos y numeramos la instancia de la habitación
        PlayerPrefs.SetInt("RoomNum", roomNumber + 1);
        roomNumber = PlayerPrefs.GetInt("RoomNum");
        room.name = "Room N° " + roomNumber;
        Debug.Log("Creating " + room.name);
        return _roomGen;
    }

    public void AddHideFrontFace(List<GameObject> list, bool isRoom)
    {
        foreach (GameObject floorGO in list)
        {
            BoxCollider boxCollider = floorGO.AddComponent<BoxCollider>();
            boxCollider.isTrigger = true;
            float xCenter = (maxBldWidth * partsWidth) - partsWidth;
            float yCenter = (Convert.ToSingle(partsHeight) / 2);
            //0z center 4.5
            //0z size 6
            //1z center 3
            //1z size 3


            HideFrontFace hideFrontFace = floorGO.AddComponent<HideFrontFace>();
            hideFrontFace.facesToHide = new List<Transform>();
            if (isRoom)
            {
                boxCollider.center = new Vector3(xCenter / 2, yCenter,
                    (partsWidth*maxBldDepth)/2);

                boxCollider.size = new Vector3(maxBldWidth * partsWidth, partsHeight,
                    (partsWidth * maxBldDepth) - transform.position.z);

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
                boxCollider.center = new Vector3(xCenter / 2, yCenter,
                    partsWidth);

                boxCollider.size = new Vector3(maxBldWidth * partsWidth, partsHeight,
                    (maxBldDepth * partsWidth));

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
        //Creamos un Gameobject vacio para agrupar las paredes 
        GameObject exteriorGroup = new GameObject("Exterior Walls");
        exteriorGroup.transform.parent = transform;
        exteriorGroup.transform.position = transform.position;

        //Movemos el puntero de generación en el inicio del edificio
        spawnOrigin.position = transform.position;
        for (int Y = 0; Y < height; Y++)
        {
            //Creamos otro GameObject hijo vacio para dividir por piso
            //y lo colocamos a la altura segun que piso correponda 
            GameObject exteriorFloor = new GameObject("Floor " + Y);
            exteriorFloor.transform.parent = exteriorGroup.transform;
            exteriorFloor.transform.position =
                new Vector3(transform.position.x, spawnOrigin.position.y, transform.position.z);

            //Movemos el puntero de generación al extremo izquierdo para empezar a generar
            spawnOrigin.position = new Vector3(transform.position.x, spawnOrigin.position.y, transform.position.z);
            for (int X = 0; X < width; X++)
            {
                //Instanciamos un GameObject del array de paredes exteriores de manera aleatoria
                GameObject instExtWall = Instantiate(exteriorWallsPrefab[Random.Range(0, exteriorWallsPrefab.Length)],
                    spawnOrigin.position - (Vector3.forward * partsWidth / 2), transform.rotation,
                    exteriorFloor.transform);

                //Movemos el puntero a la siguiente posición en X 
                spawnOrigin.position += Vector3.right * partsWidth;
            }

            //Agregamos el piso generado a la lista    
            extWallList.Add(exteriorFloor);

            //Si estamos en la primer iteración en coordenada Y,
            //y se definió con puertas al frente, reemplazar una de las paredes por una puerta
            if (Y == 0 && entraceFront)
            {
                GameObject randomWall = exteriorFloor.transform
                    .GetChild(Random.Range(0, exteriorFloor.transform.childCount)).gameObject;
                GameObject instDoorWall = Instantiate(exteriorDoorWallsPrefabs[0],
                    randomWall.transform.position, randomWall.transform.rotation,
                    exteriorFloor.transform);
                randomWall.SetActive(false);


                GameObject[] entraceDoors =
                    (from mainDoor in doorsPrefabs where mainDoor.tag == "DoubleDoor" select mainDoor).ToArray();

                GameObject instDoor = Instantiate(entraceDoors[Random.Range(0, entraceDoors.Length)],
                    instDoorWall.transform.position,
                    instDoorWall.transform.rotation, transform);

                if (instDoor.TryGetComponent(out Door door))
                {
                    door.doorOrientation = Door.DoorOrientation.Back;
                    door.outsidePlayLine = transform.position.z - partsWidth;
                    door.insidePlayLine = transform.position.z;
                }
                else
                {
                    Debug.Log("No doorscript found on " + instDoor.name);
                }
            }

            //Movemos el puntero de generación un espacio arriba
            spawnOrigin.position += Vector3.up * (partsHeight + 0.1f);

            //Combinamos los meshes al finalizar el piso
            if (combineMeshesAtEnd)
            {
                CombineMeshes(exteriorFloor.transform);
            }
        }

        //Agregamos el script de ocultar paredes en cada piso 
        AddHideFrontFace(extWallList, false);
    }

    public void GenerateRoof(int width, Transform origin)
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
            if (i == 0) //left corner
            {
                Instantiate(corniceCornersPrefabs[Random.Range(0, corniceCornersPrefabs.Length)], origin.position,
                    origin.rotation,
                    roofGroup.transform);
            }
            else if (i > 0 && i < width - 1) //fill
            {
                Instantiate(cornicesPrefabs[Random.Range(0, cornicesPrefabs.Length)],
                    origin.position + (Vector3.back * partsWidth / 2), origin.rotation, roofGroup.transform);
            }
            else //right corner
            {
                Quaternion cornerRightRot =
                    Quaternion.Euler(origin.rotation.x, origin.rotation.y - 90, origin.rotation.z);
                GameObject instCorner = Instantiate(
                    corniceCornersPrefabs[Random.Range(0, corniceCornersPrefabs.Length)],
                    origin.position, cornerRightRot, roofGroup.transform);
                xPos = instCorner.transform.position.x;
            }

            origin.position += Vector3.right * partsWidth;
        }

        //Generate Back Cornices
        origin.position = roofGroup.transform.position + Vector3.forward * ((maxBldDepth * partsWidth) - partsWidth);

        for (int i = 0; i < width; i++)
        {
            if (i == 0) //Left corner
            {
                Quaternion cornerleftRot =
                    Quaternion.Euler(origin.rotation.x, origin.rotation.y + 90, origin.rotation.z);
                Instantiate(corniceCornersPrefabs[Random.Range(0, corniceCornersPrefabs.Length)], origin.position,
                    cornerleftRot,
                    roofGroup.transform);
            }
            else if (i > 0 && i < width - 1) //Fill
            {
                Quaternion corniceRot = Quaternion.Euler(origin.rotation.x, origin.rotation.y + 180, origin.rotation.z);

                Instantiate(cornicesPrefabs[Random.Range(0, cornicesPrefabs.Length)],
                    origin.position + (Vector3.forward * partsWidth / 2), corniceRot, roofGroup.transform);
            }
            else //Right Corner
            {
                Quaternion cornerRightRot =
                    Quaternion.Euler(origin.rotation.x, origin.rotation.y + 180, origin.rotation.z);
                Instantiate(corniceCornersPrefabs[Random.Range(0, corniceCornersPrefabs.Length)], origin.position,
                    cornerRightRot,
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

            Instantiate(cornicesPrefabs[Random.Range(0, cornicesPrefabs.Length)],
                origin.position + Vector3.left * partsWidth / 2,
                corniceLeftRot, roofGroup.transform);

            Vector3 corniceRightPos = new Vector3(xPos + ((float)partsWidth / 2), roofGroup.transform.position.y,
                origin.position.z);

            Instantiate(cornicesPrefabs[Random.Range(0, cornicesPrefabs.Length)], corniceRightPos, corniceRightRot,
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
        int[] sort = new int[rmsPerFloor];
        int widthleft = buildingWidth;


        for (int i = 0; i < rmsPerFloor; i++)
        {
            int randomWidth = Random.Range(1, widthleft / 2);
            sort[i] = randomWidth;
            widthleft -= randomWidth;
            Debug.Log(sort[i]);
        }

        if (widthleft > 0)
        {
            int random = Random.Range(1, sort.Length);
            sort[random] += widthleft;
            widthleft -= sort[random];
        }

        return sort;
    }
}