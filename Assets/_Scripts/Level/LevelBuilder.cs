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
    public Transform buildStartPoint;
    public Transform buildPointer;
    public GameObject buildingGeneratorPrefab;
    public int partsWidthSize;//Anchura de las partes 

    //TAMAÑO DEL NIVEL
    public bool randomLevelSize;
    public int levelSize = 250;//Tamaño del nivel general medido en bloques
    public int minLevelSize = 250;
    public int maxLevelSize = 500;

    //CUADRAS
    public int squareQuantity = 5; // cantidad de cuadras
    private int[] squareWidthSizes;//Anchura de las cuadras
    public int minSquareWidthSize = 6;
    public int maxSquareWidthSize = 20;
    
    //EDIFICIOS
    public int buildingsPerSquare = 1;
    public int minBuildingHeight = 2;
    public int maxBuildingHeight = 8;
    
    //PREFABS
    public List<Street> streetPrefabs;
    public List <BuildingAssets> buildingsScriptables;
    private void Awake()
    {
        if (randomLevelSize)
        {
            levelSize = Random.Range(minLevelSize, maxLevelSize + 1);
        }

        squareWidthSizes = new int [squareQuantity];
        for (int i = 0; i < squareWidthSizes.Length; i++)
        {
            squareWidthSizes[i] = Random.Range(minSquareWidthSize, maxSquareWidthSize + 1);
        }
        
        GenerateLevel(levelSize);
    }

    private void Start()
    {
    }

    private void GenerateLevel(int size)
    {
            
            for (int i = 0; i < squareQuantity; i++)
            {
               int squareWidthSize = squareWidthSizes[i];
                Debug.Log("Generating Block. Iteration " + i);
                GenerateBlock(squareWidthSize,buildStartPoint);
            }
    }

    public List<Street> SearchStreetPrefabs(Street.StreetType streetType, Street.StreetShape streetShape)
    {
        var streets = streetPrefabs.Where(street =>
            street.streetType == streetType && street.streetShape == streetShape).ToList();
        return streets;
    }
    
    
   /// <summary>
   /// Método que genera una cuadra
   /// </summary>
   /// <param name="blockSize">Tamaño de la cuadra</param>
    public void GenerateBlock(int blockSize, Transform origin)
    {
        buildPointer.position = origin.position;//Establece el puntero en origen de generación
        Quaternion yRot = Quaternion.Euler(buildPointer.eulerAngles.x, 90, buildPointer.eulerAngles.z); 
        //Vereda esquina izquierda
        var leftPavCorners = SearchStreetPrefabs(Street.StreetType.Pavement,Street.StreetShape.cornerLeft);
        var middlePavements = SearchStreetPrefabs(Street.StreetType.Pavement, Street.StreetShape.straight);
        GameObject leftPavCorner = Instantiate(leftPavCorners[0].prefab,buildPointer.position,buildPointer.rotation,transform);
        buildPointer.position = leftPavCorner.transform.position + (Vector3.forward * partsWidthSize);
        //Vereda a 90° izquierda
        for (int i = 0; i < blockSize; i++)
        {
            Instantiate(middlePavements[0].prefab, buildPointer.position,yRot,transform);
            buildPointer.position += Vector3.forward * partsWidthSize;  
        }
        buildPointer.position = leftPavCorner.transform.position + (Vector3.right * partsWidthSize);
        
        //Vereda medio
        
        for (int i = 1; i < blockSize; i++)
        {
            GameObject middlePavement =
                Instantiate(middlePavements[0].prefab, buildPointer.position, buildPointer.rotation,transform);
            buildPointer.position = middlePavement.transform.position + (Vector3.right * partsWidthSize);
        }

        //Vereda esquina derecha
        var rightCorners = SearchStreetPrefabs(Street.StreetType.Pavement, Street.StreetShape.cornerRight);
        GameObject rightPavCorner = Instantiate(rightCorners[0].prefab, buildPointer.position, buildPointer.rotation,transform);
        buildPointer.position = rightPavCorner.transform.position + (Vector3.right * partsWidthSize);
        
        //Calle final que intersecta a 90° y veredas a 90°
        var roads = SearchStreetPrefabs(Street.StreetType.Road, Street.StreetShape.straight);
        Vector3 rot = new Vector3(0, 90, 0);
        for (int i = 0; i < blockSize; i++)
        {
            
            GameObject road = Instantiate(roads[0].prefab,buildPointer.position,Quaternion.Euler(rot),transform);
            if (i > 0)
            {
                Instantiate(middlePavements[0].prefab, buildPointer.position + (Vector3.left * partsWidthSize), Quaternion.Euler(rot),transform);
            }
            buildPointer.position = road.transform.position + (Vector3.forward * partsWidthSize);
        }
        
        //Generación de edificios
        Vector3 buildingOrigin = new Vector3(leftPavCorner.transform.position.x + partsWidthSize,
            leftPavCorner.transform.position.y,
            leftPavCorner.transform.position.z + partsWidthSize);

        int buildingWidth = blockSize;  


        if (buildingsPerSquare > 1)
        {
            buildingWidth = blockSize / buildingsPerSquare;
        }

        for (int i = 0; i < buildingsPerSquare; i++)
        {
            int height = Random.Range(minBuildingHeight, maxBuildingHeight);
            var buildingsScriptable = buildingsScriptables[Random.Range(0, buildingsScriptables.Count)];
            /*Task.Run(async () =>
            {
                await PlaceBuilding(buildingsScriptable, buildingOrigin, buildingWidth, height);
                buildingOrigin = new Vector3(buildingOrigin.x + (buildingWidth * partsWidthSize),
                    buildingOrigin.y, buildingOrigin.z);
                Debug.Log("PlaceBuilding() completado");

            });*/
        }
        

        //Calle horizontal
        buildPointer.position = leftPavCorner.transform.position + Vector3.back * partsWidthSize;
        for (int i = 0; i < blockSize+1; i++)
        {
            GameObject road = Instantiate(roads[0].prefab,buildPointer.position,buildPointer.rotation,transform);
            buildPointer.position = road.transform.position + (Vector3.right * partsWidthSize);
        }
        
        //Intersección
        var intersections = SearchStreetPrefabs(Street.StreetType.Road, Street.StreetShape.intersection);
        GameObject roadIntersection = Instantiate(intersections[0].prefab,buildPointer.position,buildPointer.rotation,transform);
        buildPointer.position = roadIntersection.transform.position + (Vector3.right * partsWidthSize);

        origin.position = buildPointer.position + (Vector3.forward * partsWidthSize);
    }

   /// <summary>
   /// Generates a Building in specified origin
   /// </summary>
   /// <param name="buildingScriptable"></param>
   /// <param name="origin"></param>
   /// <param name="width"></param>
   /// <param name="height"></param>
   private void PlaceBuilding(BuildingAssets buildingScriptable,Vector3 origin, int width, int height)
   {
       GameObject buildingGo = Instantiate(buildingGeneratorPrefab,origin,Quaternion.identity,transform);
       BuildingGenerator buildingGenerator = buildingGo.GetComponent<BuildingGenerator>();
       buildingGenerator.maxBldWidth = width;
       buildingGenerator.maxBldHeight = height;
       buildingGenerator.InitBuilding(buildingScriptable);
   }
}
