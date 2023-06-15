using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

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
    }
    [NonReorderable]  public StreetShape streetShape;

}

public class LevelBuilder : MonoBehaviour
{
    private BuildingGenerator buildGenerator;
    public int partsWidthSize;//Anchura de las partes 

    public bool randomLevelSize;
    public int minLevelSize = 250;
    public int maxLevelSize = 500;
    public int levelSize = 250;//Tamaño del nivel general

    public int minBlockWidthSize = 6;
    public int maxBlockWidthSize = 20;
    private int blockWidthSize;//Anchura de las cuadras
    

    #region Prefabs  

    public List<Street> prefabs;
    public List <GameObject> buildings;

    #endregion

    private void Awake()
    {
        if (randomLevelSize)
        {
            levelSize = Random.Range(minLevelSize, maxLevelSize + 1);
        }
        GenerateLevel(levelSize);
    }

    private void Start()
    {
        
    }

    public void GenerateLevel(int size)
    {
        for (int i = 0; i < levelSize; i++)
        {
            Debug.Log("Generating Block. Iteration " + i + " remaining space: " + levelSize);
            blockWidthSize = Random.Range(minBlockWidthSize, maxBlockWidthSize + 1);
            GenerateBlock(blockWidthSize);
            levelSize -= blockWidthSize;
        }
    }

   /// <summary>
   /// Método que genera una cuadra
   /// </summary>
   /// <param name="blockSize">Tamaño de la cuadra</param>
    public void GenerateBlock(int blockSize)
    {
        for (int i = 0; i < blockSize; i++)
        {
            //Todo: La generación debe empezar con una vereda y terminar con una calle para seguir generando
        }
    }
}
