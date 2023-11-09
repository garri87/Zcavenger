using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(FloorsPool))]
public class FloorsPoolEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); // Dibuja el inspector predeterminado.

        FloorsPool myScript = (FloorsPool)target;

        if (GUILayout.Button("Load Prefabs From Folder"))
        {
            myScript.LoadPrefabsFromFolder();
        }
        if (GUILayout.Button("Combine Children Meshes"))
        {
            myScript.CombineMeshes();
        }

       
    }
}
