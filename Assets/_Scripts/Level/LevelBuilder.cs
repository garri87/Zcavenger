using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Street 
{

     [NonReorderable] public GameObject prefab;
   public enum StreetType
    {
        Road,
        Pavement,
        Decoration,
    }

    [NonReorderable]  public StreetType streetType;

    public enum StreetShape
    {
        straight,
        cornerLeft,
        cornerRight,
        intersection,
    }
    [NonReorderable]  public StreetShape streetShape;

}

public class LevelBuilder : MonoBehaviour
{
    private Vector3 buildPointer;
    [SerializeField]
    public static int blocksWidth = 6;//Anchura de las partes 
    
    //CUADRAS
    public int squareQuantity = 5; // cantidad de cuadras
    private List<SquareSize> squareSizes;//Anchura de las cuadras
    public int minSquareWidthSize = 6;
    public int maxSquareWidthSize = 20;
    
    //EDIFICIOS
    public int buildingsPerSquare = 1;
    public int minFloorCount = 2;
    public int maxFloorCount = 8;

    //PREFABS
    public GameObject buildingConstructorPrefab;
    public List<Street> streetPrefabs;

    public bool withBuildings;
    public bool randomSquareSize;
    private int blockNumber;

    private List<Street> leftPavCorners;
    private List<Street> middlePavements;

    private Vector3 leftRot;
    private Vector3 middleRot;
    private Vector3 rightRot;

    public enum SquareSize
    {
        Small,
        Medium,
        Large
    }
    public static SquareSize squareSize;


    private void Awake()
    {
        
    leftRot = new Vector3(0,90,0);
    middleRot = Vector3.zero;
    rightRot = new Vector3(0,-90,0);

    squareSizes = SetSquaresSizes(squareQuantity, randomSizes: randomSquareSize);

        GenerateLevel(squareQuantity);

    }

    private void Start()
    {
    }


    private List <SquareSize> SetSquaresSizes(int squareCount, SquareSize defaultSize = SquareSize.Medium, bool randomSizes = false)
    {
        List<SquareSize> sizes = new List<SquareSize>();
        if (randomSizes)
        {
            for (int i = 0; i < squareCount; i++)
            {
                sizes.Add(EnumExtensions.GetRandomValue<SquareSize>());
            }

        }
        else
        {
            for (int i = 0; i < squareCount; i++)
            {
                sizes.Add(defaultSize);
            }
        }
        return sizes;  
    }

    /// <summary>
    /// Método principal que genera el nivel
    /// </summary>
    /// <param name="squaresCount"></param>
    public void GenerateLevel(int squaresCount)
    {
        buildPointer = transform.position;
        blockNumber = 0;
        for (int i = 0; i < squaresCount; i++)
        {
           GenerateBlock(squareSizes[i], buildPointer);
            blockNumber++;
        }
    }

    /// <summary>
    /// Método que genera una cuadra
    /// </summary>
    /// <param name="squareSize">Tamaño de la cuadra</param>
    public GameObject GenerateBlock(SquareSize squareSize, Vector3 origin)
    {
        GameObject block = new GameObject("Block " + blockNumber);
        block.transform.parent = transform;
        
        int size = GameManager.squareSizes[squareSize];

        // Vereda esquina izquierda
        leftPavCorners = SearchStreetPrefabs(Street.StreetType.Pavement, Street.StreetShape.cornerLeft);
        middlePavements = SearchStreetPrefabs(Street.StreetType.Pavement, Street.StreetShape.straight);
        GameObject leftPavCorner = Instantiate(leftPavCorners[0].prefab, origin, Quaternion.identity, block.transform);
        origin = leftPavCorner.transform.position + (Vector3.forward * LevelBuilder.blocksWidth);

        // Vereda a 90° izquierda
        for (int i = 0; i < size; i++)
        {
            Instantiate(middlePavements[0].prefab, origin, Quaternion.Euler(leftRot), block.transform);
            origin += Vector3.forward * LevelBuilder.blocksWidth;
        }
        origin = leftPavCorner.transform.position + (Vector3.right * LevelBuilder.blocksWidth);

        // Vereda medio
        for (int i = 1; i < size; i++)
        {
            GameObject middlePavement =
                Instantiate(middlePavements[0].prefab, origin, Quaternion.Euler(middleRot), block.transform);
            origin = middlePavement.transform.position + (Vector3.right * LevelBuilder.blocksWidth);
        }

        // Vereda esquina derecha
        var rightCorners = SearchStreetPrefabs(Street.StreetType.Pavement, Street.StreetShape.cornerRight);
        GameObject rightPavCorner = Instantiate(rightCorners[0].prefab, origin, rightCorners[0].prefab.transform.rotation, block.transform);
        origin = rightPavCorner.transform.position + (Vector3.right * LevelBuilder.blocksWidth);

        // Calle final que intersecta a 90° y veredas a 90°
        var roads = SearchStreetPrefabs(Street.StreetType.Road, Street.StreetShape.straight);
        for (int i = 0; i < size; i++)
        {
            GameObject road = Instantiate(roads[0].prefab, origin, Quaternion.Euler(rightRot), block.transform);
            if (i > 0)
            {
                Instantiate(middlePavements[0].prefab, origin + (Vector3.left * LevelBuilder.blocksWidth), Quaternion.Euler(rightRot), block.transform);
            }
            origin = road.transform.position + (Vector3.forward * LevelBuilder.blocksWidth);
        }

       
        if (withBuildings)
        {
            // Generación de edificios
            Vector3 buildingOrigin = new Vector3(leftPavCorner.transform.position.x + LevelBuilder.blocksWidth,
                leftPavCorner.transform.position.y,
                leftPavCorner.transform.position.z + LevelBuilder.blocksWidth);
           
            int buildingWidth = size - 1;

            if (buildingsPerSquare > 1)
            {
                buildingWidth = size / buildingsPerSquare;
            }
        
            int floorCount = Random.Range(minFloorCount, maxFloorCount+1);


            Floor.BuildingType buildingType = EnumExtensions.GetRandomValue<Floor.BuildingType>();
            if (buildingType != Floor.BuildingType.Exterior)
            {
                PlaceBuilding(buildingType, buildingOrigin, floorCount, floorWidth: GameManager.squareToBuildingMapping[squareSize]);
            }
            
            buildingOrigin = new Vector3(buildingOrigin.x + (buildingWidth * LevelBuilder.blocksWidth),
                buildingOrigin.y, buildingOrigin.z);
        }


        // Calle horizontal
        origin = leftPavCorner.transform.position + Vector3.back * LevelBuilder.blocksWidth;
        for (int i = 0; i < size + 1; i++)
        {
            GameObject road = Instantiate(roads[0].prefab, origin,Quaternion.identity, block.transform);
            origin = road.transform.position + (Vector3.right * LevelBuilder.blocksWidth);
        }

        // Intersección
        var intersections = SearchStreetPrefabs(Street.StreetType.Road, Street.StreetShape.intersection);
        GameObject roadIntersection = Instantiate(intersections[0].prefab, origin, Quaternion.identity, block.transform);
        origin = roadIntersection.transform.position + (Vector3.right * LevelBuilder.blocksWidth);

        origin = rightPavCorner.transform.position + (Vector3.right * 2) * LevelBuilder.blocksWidth ;

        buildPointer = origin;

        return block;
    }

    public List<Street> SearchStreetPrefabs(Street.StreetType streetType, Street.StreetShape streetShape)
    {
        var streets = streetPrefabs.Where(street =>
            street.streetType == streetType && street.streetShape == streetShape).ToList();
        return streets;
    }



    private void PlaceBuilding(Floor.BuildingType buildingType, Vector3 origin,int floorCount,int floorHeight = 1,Floor.FloorWidth floorWidth = Floor.FloorWidth.Medium, int floorDepth = 2)
    {
        Debug.Log("Placing Building " + buildingType + " in " + origin + " FloorCount: " + floorCount + " FloorHeight: " + floorHeight + " FloorWidht: " + floorWidth + " FloorDepth: " + floorDepth);
        GameObject buildingGo = Instantiate(buildingConstructorPrefab, origin, Quaternion.identity, transform);
        BuildingConstructor buildingConstructor = buildingGo.GetComponent<BuildingConstructor>();
        buildingConstructor.buildingType = buildingType;
        buildingConstructor.floorCount = floorCount;
        buildingConstructor.floorWidth = floorWidth;
        buildingConstructor.floorHeight = floorHeight;
        buildingConstructor.floorDepth = floorDepth;
        buildingGo.SetActive(true);
        buildingConstructor.InitBuild();
        Debug.Log("Build Complete");
    }


}
