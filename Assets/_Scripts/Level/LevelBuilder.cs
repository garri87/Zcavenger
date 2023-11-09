using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;
using System.Threading.Tasks;
using UnityEditor.Rendering;
using static UnityEngine.Rendering.PostProcessing.SubpixelMorphologicalAntialiasing;

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
    public Vector3 buildPointer;
    [SerializeField]
    public static int blocksWidth = 6;//Anchura de las partes 
    
    //CUADRAS
    public int squareQuantity = 5; // cantidad de cuadras
    private int counter = 0;
    private int[] squareWidthSizes;//Anchura de las cuadras
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

    private void Awake()
    {

        SetSquaresSize(squareQuantity, minSquareWidthSize, maxSquareWidthSize);

        GenerateLevel(squareQuantity);

    }

    private void Start()
    {
    }

    private void SetSquaresSize(int quantity, int min, int max)
    {
        squareWidthSizes = new int[quantity];
        if (randomSquareSize)
        {
            for (int i = 0; i < squareWidthSizes.Length; i++)
            {
                squareWidthSizes[i] = Random.Range(min, max + 1);
            }

        }
        else
        {
            for (int i = 0; i < squareWidthSizes.Length; i++)
            {
                squareWidthSizes[i] = minSquareWidthSize + 1 ;
            }
        }
        
    }

    
    public void GenerateLevel(int squaresCount)
    {
        buildPointer = transform.position;
        blockNumber = 0;
        for (int i = 0; i < squaresCount; i++)
        {
            int squareWidthSize = squareWidthSizes[i];
            GameObject block = GenerateBlock(squareWidthSize, buildPointer);
            blockNumber++;
        }
    }

    /// <summary>
    /// Método que genera una cuadra
    /// </summary>
    /// <param name="blockSize">Tamaño de la cuadra</param>
    public GameObject GenerateBlock(int blockSize, Vector3 origin)
    {
        GameObject block = new GameObject("Block " + blockNumber);
        block.transform.parent = transform;
        Vector3 rot = new Vector3(0, 90, 0);


        // Vereda esquina izquierda
        var leftPavCorners = SearchStreetPrefabs(Street.StreetType.Pavement, Street.StreetShape.cornerLeft);
        var middlePavements = SearchStreetPrefabs(Street.StreetType.Pavement, Street.StreetShape.straight);
        GameObject leftPavCorner = Instantiate(leftPavCorners[0].prefab, origin, Quaternion.identity, block.transform);
        origin = leftPavCorner.transform.position + (Vector3.forward * LevelBuilder.blocksWidth);

        // Vereda a 90° izquierda
        for (int i = 0; i < blockSize; i++)
        {
            Instantiate(middlePavements[0].prefab, origin, Quaternion.Euler(rot), block.transform);
            origin += Vector3.forward * LevelBuilder.blocksWidth;
        }
        origin = leftPavCorner.transform.position + (Vector3.right * LevelBuilder.blocksWidth);

        // Vereda medio
        for (int i = 1; i < blockSize; i++)
        {
            GameObject middlePavement =
                Instantiate(middlePavements[0].prefab, origin, Quaternion.identity, block.transform);
            origin = middlePavement.transform.position + (Vector3.right * LevelBuilder.blocksWidth);
        }

        // Vereda esquina derecha
        var rightCorners = SearchStreetPrefabs(Street.StreetType.Pavement, Street.StreetShape.cornerRight);
        GameObject rightPavCorner = Instantiate(rightCorners[0].prefab, origin, rightCorners[0].prefab.transform.rotation, block.transform);
        origin = rightPavCorner.transform.position + (Vector3.right * LevelBuilder.blocksWidth);

        // Calle final que intersecta a 90° y veredas a 90°
        var roads = SearchStreetPrefabs(Street.StreetType.Road, Street.StreetShape.straight);
        for (int i = 0; i < blockSize; i++)
        {
            GameObject road = Instantiate(roads[0].prefab, origin, Quaternion.Euler(rot), block.transform);
            if (i > 0)
            {
                Instantiate(middlePavements[0].prefab, origin + (Vector3.left * LevelBuilder.blocksWidth), Quaternion.Euler(rot), block.transform);
            }
            origin = road.transform.position + (Vector3.forward * LevelBuilder.blocksWidth);
        }

       
        if (withBuildings)
        {
            // Generación de edificios
            Vector3 buildingOrigin = new Vector3(leftPavCorner.transform.position.x + LevelBuilder.blocksWidth,
                leftPavCorner.transform.position.y,
                leftPavCorner.transform.position.z + LevelBuilder.blocksWidth);
            int buildingWidth = blockSize - 1;

            if (buildingsPerSquare > 1)
            {
                buildingWidth = blockSize / buildingsPerSquare;
            }
        
            int floorCount = Random.Range(minFloorCount, maxFloorCount+1);
            PlaceBuilding(buildingOrigin,floorCount,1,6,2);
            buildingOrigin = new Vector3(buildingOrigin.x + (buildingWidth * LevelBuilder.blocksWidth),
                buildingOrigin.y, buildingOrigin.z);
        }


        // Calle horizontal
        origin = leftPavCorner.transform.position + Vector3.back * LevelBuilder.blocksWidth;
        for (int i = 0; i < blockSize + 1; i++)
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


    private void PlaceBuilding(Vector3 origin,int floorCount,int floorHeight, int floorWidth, int floorDepth)
    {
        Debug.Log("Placing Building in " + origin + " FloorCount: " + floorCount + " FloorHeight: " + floorHeight + " FloorWidht: " + floorWidth + " FloorDepth: " + floorDepth);
        GameObject buildingGo = Instantiate(buildingConstructorPrefab, origin, Quaternion.identity, transform);
        BuildingConstructor buildingConstructor = buildingGo.GetComponent<BuildingConstructor>();
        buildingConstructor.floorCount = floorCount;
        buildingConstructor.floorWidth = floorWidth;
        buildingConstructor.floorHeight = floorHeight;
        buildingConstructor.floorDepth = floorDepth;
        buildingGo.SetActive(true);
        buildingConstructor.InitBuild();
        Debug.Log("Build Complete");
    }

    private void DecorateStreet()
    {

    }
}
