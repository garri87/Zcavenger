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
    public int activationProbabilty = 2;
    public bool randomMaterial;
    public int materialIndex = 0;
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



        foreach (GameObject gameObject in gameObjects)
        {
            if (randomActivation)
            {
                float randomNum = Mathf.RoundToInt(Random.Range(0f, activationProbabilty));
                if (randomNum == activationProbabilty)
                {
                    gameObject.SetActive(true);
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }

            if (randomRotation)
            {
                gameObject.transform.eulerAngles = new Vector3(gameObject.transform.eulerAngles.x,
                    Random.Range(minRotation, maxRotation), gameObject.transform.eulerAngles.z);
            }

            if (randomMaterial)
            {
                if (materials.Length > 0)
                {
                    MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
                    Material[] meshMaterials = meshRenderer.materials;
                    meshMaterials[materialIndex] = materials[Random.Range(0, materials.Length)];
                    meshRenderer.materials = meshMaterials;
                }
                else
                {
                    Debug.LogWarning("Warning: No materials in array for random selection");
                }
                
            }
        }
    }
}
