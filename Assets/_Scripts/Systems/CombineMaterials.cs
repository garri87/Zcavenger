using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEditor;

public class CombineMaterials : MonoBehaviour
{
    [SerializeField] private Material combinedMaterial;

    [SerializeField] private bool exportMaterial = false;
    [SerializeField] private string materialSavePath = "Assets/CombinedMaterial.mat";

#if UNITY_EDITOR
    [CustomEditor(typeof(CombineMaterials))]
    public class CombineMaterialsEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            CombineMaterials combineMaterials = (CombineMaterials)target;

            if (GUILayout.Button("Combine Materials"))
            {
                combineMaterials.Combine();
            }

            if (GUILayout.Button("Export Material"))
            {
                combineMaterials.ExportMaterial();
            }
        }
    }
#endif

    public void Combine()
    {
        // Obtener los renderers del GameObject
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        // Crear un nuevo material combinado compatible con URP
        combinedMaterial = new Material(UniversalRenderPipeline.asset.defaultShader);

        // Combinar todos los materiales en uno solo
        foreach (Renderer renderer in renderers)
        {
            foreach (Material material in renderer.sharedMaterials)
            {
                // Copiar las propiedades del material al material combinado
                combinedMaterial.CopyPropertiesFromMaterial(material);
            }
        }

        // Asignar el material combinado a todos los renderers del GameObject
        foreach (Renderer renderer in renderers)
        {
            Material[] materials = new Material[renderer.sharedMaterials.Length];
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = combinedMaterial;
            }
            renderer.sharedMaterials = materials;
        }
    }

    public void ExportMaterial()
    {
#if UNITY_EDITOR
        if (combinedMaterial != null)
        {
            string path = EditorUtility.SaveFilePanel("Export Material", "Assets", "CombinedMaterial", "mat");
            if (!string.IsNullOrEmpty(path))
            {
                path = FileUtil.GetProjectRelativePath(path);
                AssetDatabase.CreateAsset(combinedMaterial, path);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                Debug.Log("Material exported to: " + path);

                // Modificar el mesh para requerir solo un material
                MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
                if (meshRenderer != null)
                {
                    meshRenderer.sharedMaterials = new Material[] { combinedMaterial };
                }
            }
        }
#endif
    }
}