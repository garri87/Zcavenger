using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class Randomizer : MonoBehaviour
{
    public List<GameObject> gameObjects;

    public bool selectAllChilds;

    public bool activateOne;

    public bool randomRotation;
    public float minRotation, maxRotation;

    public bool randomActivation;
    [Range(0,10)]
    public int activationProbabilty = 7;

    public bool randomMaterial;
    public int materialIndex = 0;
    public Material[] materials;


    private void OnValidate()
    {
        activationProbabilty = 8;
    }

    private void Awake()
    {
        if (selectAllChilds)
        {
            SelectAllChilds();
        }
        if (gameObjects != null)
        {
            if (activateOne)
            {
                ActivateOne(gameObjects);
            }

            foreach (GameObject gameObject in gameObjects)
            {
                if (randomActivation)
                {
                    RandomActivation(gameObject);
                }

                if (randomRotation)
                {
                    RandomRotation(gameObject);
                }

                if (randomMaterial)
                {
                    RandomMaterial(gameObject);
                }
            }
        }
    }

    public void SelectAllChilds()
    {
        if (transform.childCount > 0)
        {
            foreach (Transform child in transform)
            {
                gameObjects.Add(child.gameObject);
            }
        }
        else
        {
            Debug.LogWarning("No childs in transform");
        }
    }

    public void RandomActivation(GameObject gameObject)
    {
        float randomNum = Mathf.RoundToInt(Random.Range(0,11));
        if (randomNum <= activationProbabilty) 
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public void RandomRotation(GameObject gameObject)
    {
        gameObject.transform.eulerAngles = new Vector3(gameObject.transform.eulerAngles.x,
            Random.Range(minRotation, maxRotation), gameObject.transform.eulerAngles.z);
    }

    public void RandomMaterial(GameObject gameObject)
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

    /// <summary>
    /// activates a random object on the list
    /// </summary>
    /// <param name="gameObjects"></param>
    public void ActivateOne(List <GameObject> gameObjects)
    {
        if (gameObjects != null)
        {
            foreach(GameObject go in gameObjects)
            {
                go.SetActive(false);
            }

            gameObjects[Random.Range(0, gameObjects.Count)].SetActive(true);
        }
    }
}