using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using System;

[RequireComponent(typeof(MeshCombiner))]

public class AddMeshFilterToCombiner : MonoBehaviour
{
    public MeshCombiner meshCombiner;
    public string objectName = "DoorWall"; // Cambia esto al nombre que estás buscando.
    public List<MeshFilter> meshFiltersList;
    public Transform interiorGroup;

    public void GetMeshFilters()
    {
        if (meshCombiner)
        {
            meshFiltersList.Clear();
            GameObject[] objects = GetChildrenByName(objectName);

            //Debug.Log("Found" + objects.Length + " Objects");

            foreach (var objeto in objects)
            {
                MeshFilter meshFilter = objeto.GetComponent<MeshFilter>();
                if (meshFilter != null)
                {
                    meshFiltersList.Add(meshFilter);
                }
            }
            
        }

        if (interiorGroup)
        {
            List <MeshFilter> interiorsMeshFiltersList = interiorGroup.GetComponentsInChildren<MeshFilter>(true).ToList();
            
            foreach (MeshFilter meshFilter in interiorsMeshFiltersList)
            {
                meshFiltersList.Add(meshFilter);
            }

        }
        meshCombiner.meshFiltersToSkip = new MeshFilter[0];
        meshCombiner.meshFiltersToSkip = meshFiltersList.ToArray();
    }

    GameObject[] GetChildrenByName(string name)
    {
        return GetComponentsInChildren<Transform>(true)
            .Where(child => child.name.Contains(name))
            .Select(child => child.gameObject)
            .ToArray();
    }


}
