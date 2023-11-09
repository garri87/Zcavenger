using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(AddMeshFilterToCombiner))]
[CanEditMultipleObjects]
public class AddMeshFilterToCombinerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); // Dibuja el inspector predeterminado.

        AddMeshFilterToCombiner myScript = (AddMeshFilterToCombiner)target;

        if (GUILayout.Button("Obtener MeshFilters por Nombre"))
        {
            myScript.GetMeshFilters();
        }
    }
}
