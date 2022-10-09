using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class Randomizer : MonoBehaviour
{
    public float minRotation, maxRotation;
    public GameObject[] gameObjects;
    public bool selectAllChilds;
    public bool randomRotation;
    public bool randomActivation;
    public bool randomMaterial;
    public Material[] materials;
    
    
    private void Awake()
    {
        
        
        if (selectAllChilds)
        {
            if (transform.childCount > 0)
            {
                gameObjects = new GameObject[transform.childCount];
                for (int i = 0; i < transform.childCount; i++)
                {
                    gameObjects[i] = transform.GetChild(i).gameObject;
                }
            }
            else
            {
                Debug.LogWarning("No childs in transform");
            }
        }

        
            if (randomActivation)
            {
                foreach (GameObject gameObject in gameObjects)
                {
                    gameObject.SetActive(Convert.ToBoolean(Random.Range(0, 2)));
                }
            }
        
            if (randomRotation) 
            {
                foreach (GameObject gameObject in gameObjects)
                {
                    gameObject.transform.eulerAngles = new Vector3(gameObject.transform.eulerAngles.x,
                        Random.Range(minRotation, maxRotation),gameObject.transform.eulerAngles.z);
                }
            }

            if (randomMaterial)
            {
                if (materials.Length > 0)
                {
                    foreach (GameObject gameObject in gameObjects)
                    {
                        gameObject.GetComponent<MeshRenderer>().material = materials[Random.Range(0, materials.Length)];
                    }  
                }
                else
                {
                    Debug.LogWarning("Warning: No materials in array for random selection");
                }
                
            }
    }
    
}
