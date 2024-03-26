using System.Collections.Generic;
using System.Linq;
using UnityEngine;



public class Floor : MonoBehaviour
{
    public int floorHeight;
    public int floorDepth = 2;

    public enum BuildingType
    {
        //Factory,
        //FireDept,
        //Garage,
        Hospital,
        //Market,
        //Office,
        //PoliceDept,
        Residential,
        //Warehouse
        Exterior,
    }
    public BuildingType buildingType;

    public enum FloorWidth
    {
        Small,
        Medium,
        Large
    }
    public FloorWidth floorWidth;

    public enum FloorLocation
    {
        Base,
        Middle,
        Roof
    }
    public FloorLocation floorLocation;

    public enum StairsPosition
    {
        Left,
        Right,
        Middle,
    }
    public StairsPosition stairsPosition;

    public enum ExitDoorPosition
    {
        Front,
        LeftRight,
        FrontLeft,
        FrontRight,
        FrontLeftRight,
        None
    }
    public ExitDoorPosition exitDoorPosition;
    public MeshCollider meshCollider;
    public MeshFilter meshFilter;

    public Transform interiorsGroup;
    public SpawnManager[] enemySpawers;
    public Item[] itemSpawners;
    private List<ScriptableObject> scriptableObjects;
    private List<ScriptableObject> itemScriptableObjects;

    [Range(0,10)]
    public int enemyProbability = 0, itemProbability = 0;

    private void OnValidate()
    {

    }

    private void Awake()
    {
        meshCollider = GetComponent<MeshCollider>();
        meshFilter = GetComponent<MeshFilter>();
        if (meshCollider)
        {
            if (meshFilter)
            {
                meshCollider.sharedMesh = meshFilter.sharedMesh;
            }
        }
        SetFloorType(buildingType);
    }

    private void Start()
    {
        switch (GameManager.Instance.gameDifficulty)
        {
            case GameManager.GameDifficulty.Easy:
                itemProbability = 7;
                enemyProbability = 3;
                break;
            case GameManager.GameDifficulty.Normal:
                itemProbability = 5;
                enemyProbability = 5;
                break;
            case GameManager.GameDifficulty.Hard:
                itemProbability = 3;
                enemyProbability = 7;
                break;
            default:
                itemProbability = 5;
                enemyProbability = 5;
                break;
        }
        scriptableObjects = GameManager.Instance.scriptableObjects;
        itemScriptableObjects = scriptableObjects.Where(obj => obj.GetType() == typeof(ItemScriptableObject) || obj.GetType() == typeof(WeaponScriptableObject)).ToList();


        SetSpawners(itemSpawners, itemProbability);
        SetSpawners(enemySpawers, enemyProbability);

    }

    public void SetFloorType(BuildingType buildingType)
    {
        if (interiorsGroup)
        {
            foreach (Transform group in interiorsGroup)
            {
                if (group.name == buildingType.ToString())
                {
                    group.gameObject.SetActive(true);
                }
                else
                {
                    group.gameObject.SetActive(false);
                }
            }
        }
     
    }

    public void SetSpawners(Object[] spawnersTransform, int probability)
    {
        foreach(var spawner in spawnersTransform)
        {
            RaycastHit backHit;
            RaycastHit forwardHit;

            int rand = Random.Range(1, 10);
            if (rand > 0 && rand < probability)
            {
                if (spawner is SpawnManager)
                {
                    SpawnManager spawnManager = (SpawnManager)spawner;
                    spawnManager.Init(buildingType);
                    spawnManager.gameObject.SetActive(true);

                }

                if (spawner is Item)
                {
                    Item item = (Item)spawner;

                    Ray backRay = new Ray(item.transform.position, item.transform.TransformDirection(Vector3.back) * (LevelBuilder.blocksWidth / 2));
                    Ray forwardRay = new Ray(item.transform.position, item.transform.TransformDirection(Vector3.forward) * (LevelBuilder.blocksWidth / 2));

                    if (Physics.Raycast(backRay, out backHit, (LevelBuilder.blocksWidth / 2))&& 
                        Physics.Raycast(forwardRay, out forwardHit, (LevelBuilder.blocksWidth / 2)))
                    {
                        if (backHit.transform.gameObject.layer == LayerMask.NameToLayer("Door") || forwardHit.transform.gameObject.layer == LayerMask.NameToLayer("Door"))
                        {
                            Debug.Log("Door detected, aborting item spawning");
                        }
                        else
                        {
                            if (scriptableObjects.Count > 0)
                            {
                                ScriptableObject scriptableItem = itemScriptableObjects[Random.Range(0, scriptableObjects.Count)];
                                item.scriptableObject = scriptableItem;

                                item.gameObject.SetActive(true);

                            }
                        }
                    }

                    
                         

                }

            }
        }
    }

  

}
