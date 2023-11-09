using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Cinemachine.DocumentationSortingAttribute;
using static UnityEditor.UIElements.ToolbarMenu;

public class FloorsPool : MonoBehaviour
{
    private GameObject[] floorPrefabs;
    public string resourcesFilePath = "Prefabs/Level/Buildings/Floors/Variants";
    
    public void LoadPrefabsFromFolder()
    {
        floorPrefabs = Resources.LoadAll<GameObject>(resourcesFilePath);

        if (floorPrefabs.Length > 0)
        {
            foreach (GameObject prefab in floorPrefabs)
            {
                GameObject go = Instantiate (prefab,transform);
                go.SetActive(false);
            }
        }
    }

    public void CombineMeshes()
    {
        if (transform.childCount > 0)
        {
            foreach (Transform child in transform)
            {
                if (child.gameObject.TryGetComponent<MeshCombiner>(out MeshCombiner meshCombiner))
                {
                    meshCombiner.CombineMeshes(true);
                }
            }
        }
        else
        {
            Debug.Log("No children to combine");
        }
        
    }
}
