using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class HideFrontFace : MonoBehaviour
{
    public List<Transform> facesToHide;
    //private Color frontSideColor;

    public List<MeshRenderer> _meshRenderers;

    private void Start()
    {
        _meshRenderers = new List<MeshRenderer>();
        
        for (int i = 0; i < facesToHide.Count; i++)
        {
            if (facesToHide[i].TryGetComponent(out MeshRenderer renderer))
            {
                _meshRenderers.Add(renderer);
                if (facesToHide[i].childCount > 0)
                {
                    for (int j = 0; j < facesToHide[i].childCount; j++)
                    {
                        _meshRenderers.AddRange(facesToHide[i].GetComponentsInChildren<MeshRenderer>());
                    }
                }
            }
            else
            {
                for (int j = 0; j < facesToHide[i].childCount; j++)
                {
                   _meshRenderers.AddRange(facesToHide[i].GetComponentsInChildren<MeshRenderer>());
                }
            }
        }

        for (int i = 0; i < _meshRenderers.Count; i++)
        {
            if (_meshRenderers[i] == null)
            {
                _meshRenderers.Remove(_meshRenderers[i]);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Faces Hidden!");
           /* foreach (Transform face in facesToHide)
            {
                face.gameObject.SetActive(false);
            }*/

           foreach (MeshRenderer renderer in _meshRenderers)
           {
               renderer.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
           }
        }

    }


    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            /*foreach (Transform face in facesToHide)
            {
                face.gameObject.SetActive(true);
            }*/
            
            foreach (MeshRenderer renderer in _meshRenderers)
            {
                renderer.shadowCastingMode = ShadowCastingMode.On;
            }
        }
    }
}

    
 