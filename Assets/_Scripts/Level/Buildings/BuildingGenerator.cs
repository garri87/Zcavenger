

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
    private int blocksLeft;

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

    public List<GameObject> doorsPrefabs;
    public List<GameObject> wallsPrefabs;
    public List<GameObject> doorWallsPrefabs;
    public List<GameObject> basesPrefabs;
    public List<GameObject> exteriorWallsPrefab;
    public List<GameObject> exteriorDoorWallsPrefabs;
    public List<GameObject> interiorPrefabs;
    public List<GameObject> lightsPrefabs;
    public List<GameObject> mainDoorPrefabs;
    public List<GameObject> cornicesPrefabs;
    public List<GameObject> corniceCornersPrefabs;

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

        //Colocar un generador de habitacion en puntero de origen
        SetRoomSeed(spawnOrigin, maxBldWidth - stairsRoomWidth);

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
                blocksLeft = maxBldWidth - stairsRoomWidth;

                //SI: el punto de origen se encuentra en coordenada Y = 0 y Z = 0, generar un lobby:
                if (Y == 0 && Z == 0)
                {
                    BuildRoom(_roomGen, exteriorDoorWallsPrefabList[0], true, maxBldWidth - stairsRoomWidth, null,
                        false,
                        true);
                    _roomGen.isLobby = true;
                    instRoom.transform.parent = floorGroup.transform;

                    //Agregamos al final una habitacion de entrada a escaleras
                    SetRoomSeed(spawnOrigin, stairsRoomWidth);
                    blocksLeft = stairsRoomWidth;
                    BuildRoom(_roomGen, doorWallsPrefabsList[0], true, stairsRoomWidth, exteriorDoorWallsPrefabList[0],
                        true, false);
                    _roomGen.isStairsEntrace = true;
                    instRoom.transform.parent = floorGroup.transform;

                }
                //SINO:
                else
                {
                    //Dividimos las habitaciones en partes iguales segun cantidad de habitaciones por piso
                    int roomsWidth = maxBldWidth - stairsRoomWidth; 
                    roomsWidth = Mathf.RoundToInt(roomsWidth/roomsPerFloor);
                    //BUCLE (Horizontal X): desde 0 hasta la cantidad de habitaciones por piso

                    for (int X = 0; X < roomsPerFloor; X++)
                    {
                        //colocar un nuevo generador de habitacion y obtener su script
                        if (X == 0) // crear una habitacion con pared al comienzo en la primera iteracion 
                        {
                            SetRoomSeed(spawnOrigin, roomsWidth);
                            instRoom.transform.parent = floorGroup.transform;
                            BuildRoom(
                                _roomGen, 
                                exteriorWallsPrefabList[Random.Range(0, exteriorWallsPrefabList.Count)],
                                false,
                                roomsWidth, null, false, true
                                );
                        }
                        else if (X > 0 && X < roomsPerFloor) //Rellenar las habitaciones del medio
                        {
                            SetRoomSeed(spawnOrigin, roomsWidth);
                            instRoom.transform.parent = floorGroup.transform;
                            BuildRoom(_roomGen, doorWallsPrefabsList[0], true, roomsWidth, null, false, true);
                        }
                        else // crear una habitacion con pared al final en la ultima iteracion
                        {


                        }
                    }

                }

                SetRoomSeed(spawnOrigin, stairsRoomWidth);
                instRoom.transform.parent = floorGroup.transform;
                blocksLeft = stairsRoomWidth;

                if (Y != 0 && Z == 0)
                {
                    _roomGen.isStairsEntrace = true;
                    BuildRoom(_roomGen, doorWallsPrefabsList[0], true, stairsRoomWidth,
                        exteriorWallsPrefabList[Random.Range(0, exteriorWallsPrefabList.Count)],
                        true, true);
                }
                else if (Z == 1)
                {
                    _roomGen.isStairsRoom = true;
                    BuildRoom(_roomGen, wallsPrefabsList[0], false, stairsRoomWidth,
                        exteriorWallsPrefabList[Random.Range(0, exteriorWallsPrefabList.Count)],
                        false, false);
                }
                else if (Z != 0)
                {
                    BuildRoom(_roomGen, doorWallsPrefabsList[0], true, stairsRoomWidth,
                        exteriorWallsPrefabList[Random.Range(0, exteriorWallsPrefabList.Count)],
                        false, true);
                }



                //mover punto de origen en coordenada X:0 e Y en 1 ud (+ el grosor de las piezas para no interpolar)para continuar generando el siguiente edificio
                spawnOrigin.position += Vector3.up * (partsHeight + partsThickness);

            } //FIN bucle Y

            //Mover el punto de origen al siguiente punto Z en 1 ud
            spawnOrigin.position += Vector3.forward * (partsWidth);

        } //FIN bucle Z

        foreach (RoomGenerator roomGenerator in rooms)
        {
            GenerateLights(roomGenerator, roomGenerator.ceilings, roomGenerator.ceilings.Count / 2);


            //Generamos puertas traseras
            if (roomGenerator.transform.position.z < transform.position.z + ((maxBldDepth - 1) * partsWidth))
                //Si la habitación no esta en la ultima profundidad, generar puertas traseras
            {
                if (roomGenerator.isLobby) //Si la habitación es un lobby, generar puertas en partes iguales
                {
                    int doorPos = Mathf.RoundToInt(roomGenerator.backWalls.Count / roomsPerFloor);
                    for (int i = 0; i < roomsPerFloor; i++)
                    {
                        if (doorPos <= roomGenerator.backWalls.Count)
                        {
                            GenerateBackDoor(roomGenerator, false, doorPos);
                            doorPos += doorPos;
                        }

                    }
                }
                else if (roomGenerator.isStairsEntrace)
                {
                    GenerateBackDoor(roomGenerator, false, 0);
                    GenerateBackDoor(roomGenerator, false, stairsRoomWidth);
                }
                else if (!roomGenerator.isStairsRoom)
                {
                    GenerateBackDoor(roomGenerator, true);
                }
            }

            //Llenamos las habitaciones de objetos de decoracion segun estilo
            //y cambiamos el material de los interiores


            if (roomGenerator.isStairsEntrace || roomGenerator.isStairsRoom)
            {
                interiorMat = 0;
            }
            else
            {
                interiorMat = Random.Range(0, interiorMaterials.Length);

                for (int i = 0; i < roomGenerator.bases.Count; i++)
                {
                    if (roomGenerator.isLobby)
                    {
                        DecorateRoom(roomGenerator, RoomGenerator.RoomStyle.Hospital_Lobby,
                            roomGenerator.bases[i].transform, i);
                    }
                    else if (!roomGenerator.isStairsRoom)
                    {
                        DecorateRoom(roomGenerator, roomGenerator.roomStyle, roomGenerator.bases[i].transform, i);
                    }
                }

            }

            roomGenerator.ChangeMaterial(roomGenerator.bases,
                floorBaseMaterials[Random.Range(0, floorBaseMaterials.Length)]);
            roomGenerator.ChangeMaterial(roomGenerator.ceilings, interiorMaterials[interiorMat]);
            roomGenerator.ChangeMaterial(roomGenerator.backWalls, interiorMaterials[interiorMat]);
            roomGenerator.ChangeMaterial(roomGenerator.backDoorWalls, interiorMaterials[interiorMat]);
            if (roomGenerator.transform.position.x == transform.position.x)
            {
                roomGenerator.ChangeMaterial(roomGenerator.doorWalls, interiorMaterials[interiorMat],
                    exteriorMaterials[Random.Range(0, exteriorMaterials.Length)]);
            }
            else
            {
                roomGenerator.ChangeMaterial(roomGenerator.doorWalls, interiorMaterials[interiorMat]);
            }


            if (roomGenerator.isStairsRoom)
            {
                foreach (GameObject ceiling in roomGenerator.ceilings)
                {
                    ceiling.SetActive(false);
                }

                if (roomGenerator.transform.position.y > transform.position.y)
                {
                    foreach (GameObject roomBase in roomGenerator.bases)
                    {
                        roomBase.SetActive(false);
                    }
                }

                GenerateStairs(roomGenerator, buildingScriptableObject.stairsPrefabsList[0]);
            }

            for (int i = 0; i < roomGenerator.transform.childCount; i++)
            {
                if (!roomGenerator.transform.GetChild(i).gameObject.activeSelf)
                {
                    Destroy(roomGenerator.transform.GetChild(i).gameObject);
                }
            }

            roomGenerator.CombineMeshes(combineMeshesAtEnd, roomGenerator.wallsAndInteriorGroup);
            roomGenerator.CombineMeshes(combineMeshesAtEnd, roomGenerator.backWallGroup);

        }

        //Agregamos el script que oculta las paredes que estan entre la cámara y el personaje
        AddHideFrontFace(floorList, true);

        //Generamos paredes exteriores

        GenerateExteriorWalls(spawnOrigin, maxBldWidth, maxBldHeight);

        //Generamos la terraza del edificio
        GenerateRoof(maxBldWidth, spawnOrigin, roofCorniceCorners, roofCornices);
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
    public RoomGenerator SetRoomSeed(Transform spawnOrigin, int roomWidht = 1, int roomHeight = 1, string roomStyle = null)
    {
        //Colocar un generador de habitacion en punto de origen
        GameObject room = Instantiate(roomSeed, spawnOrigin.position, transform.rotation, roomsTransform);
        
        //obtener el script del generador de habitacion
        _roomGen = room.GetComponent<RoomGenerator>();
        
        //Establecer dimension de la primera habitacion en Y en 1ud e X segun anchura del edificio
        _roomGen.roomWidth = roomWidht;
        _roomGen.roomHeight = roomHeight;
    
        _roomGen.Init(this);
        
        _roomGen.roomStyle = roomStyle;
        
        //TODO: Exportar prefabs de interiores y materiales al RoomGenerator
        
        //agregamos la instancia actual de la habitación a la lista de habitaciones general
        rooms.Add(_roomGen);

        //Renombramos y numeramos la instancia de la habitación
        PlayerPrefs.SetInt("RoomNum", roomNumber + 1);
        roomNumber = PlayerPrefs.GetInt("RoomNum");
        instRoom.name = "Room N° " + roomNumber;
        
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
                        RoomGenerator roomGenerator = roomgen;
                        // Debug.Log("found roomgenerator at " + floorGO.transform.GetChild(i).gameObject.name);

                        // Debug.Log("backWallGroup count " + roomgen.backWallGroup.gameObject);
                        /*for (int j = 0; j < roomGenerator.backWallGroup.childCount; j++)
                        {
                            hideFrontFace.facesToHide.Add(roomGenerator.backWallGroup.GetChild(j));
                        }*/
                        hideFrontFace.facesToHide.Add(roomGenerator.backWallGroup);
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
                GameObject instExtWall = Instantiate(
                    exteriorWallsPrefabList[Random.Range(0, exteriorWallsPrefabList.Count)],
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

    public void GenerateRoof(int width, Transform origin, List<GameObject> corniceCorners, List<GameObject> cornices)
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
                Instantiate(corniceCorners[Random.Range(0, corniceCorners.Count)], origin.position, origin.rotation,
                    roofGroup.transform);
            }
            else if (i > 0 && i < width-1)//fill
            {
                Instantiate(cornices[Random.Range(0, cornices.Count)],
                    origin.position + (Vector3.back * partsWidth / 2), origin.rotation, roofGroup.transform);
            }
            else//right corner
            {
                Quaternion cornerRightRot =
                    Quaternion.Euler(origin.rotation.x, origin.rotation.y - 90, origin.rotation.z);
                GameObject instCorner = Instantiate(corniceCorners[Random.Range(0, corniceCorners.Count)],
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
                Instantiate(corniceCorners[Random.Range(0, corniceCorners.Count)], origin.position, cornerleftRot,
                    roofGroup.transform);
            }
            else if (i > 0 && i < width-1)//Fill
            {
                Quaternion corniceRot = Quaternion.Euler(origin.rotation.x, origin.rotation.y + 180, origin.rotation.z);

                Instantiate(cornices[Random.Range(0, cornices.Count)],
                    origin.position + (Vector3.forward * partsWidth / 2), corniceRot, roofGroup.transform);
            }
            else//Right Corner
            {
                Quaternion cornerRightRot =
                    Quaternion.Euler(origin.rotation.x, origin.rotation.y + 180, origin.rotation.z);
                Instantiate(corniceCorners[Random.Range(0, corniceCorners.Count)], origin.position, cornerRightRot,
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

            Instantiate(cornices[Random.Range(0, cornices.Count)], origin.position + Vector3.left * partsWidth / 2,
                corniceLeftRot, roofGroup.transform);

            Vector3 corniceRightPos = new Vector3(xPos + ((float) partsWidth / 2), roofGroup.transform.position.y,
                origin.position.z);

            Instantiate(cornices[Random.Range(0, cornices.Count)], corniceRightPos, corniceRightRot,
                roofGroup.transform);
        }

        CombineMeshes(roofGroup.transform);
    }

    public void GenerateStairs(RoomGenerator roomGenerator, GameObject stairsPrefab)
    {
        Instantiate(stairsPrefab, roomGenerator.transform.position, roomGenerator.transform.rotation,
            roomGenerator.transform);
    }

    public void GenerateLights(RoomGenerator roomGenerator, List<GameObject> list, int lightCount = 1)
    {
        int center = 0;
        if (!roomGenerator.isStairsRoom)
        {
            if (lightCount == 1)
            {
                center = Mathf.RoundToInt(list.Count / 2); //3
            }
            else if (lightCount > 1)
            {
                center = Mathf.RoundToInt(list.Count / lightCount);
            }
        }
        else
        {
            center = list.Count - 1;
        }



        for (int i = 0; i < lightCount; i++)
        {
            if (list.Count > 0 && center < list.Count)
            {
                GameObject instLight = Instantiate(lightsPrefabs[0], list[center].transform.position,
                    transform.rotation,
                    transform);
                center += center;
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

}
