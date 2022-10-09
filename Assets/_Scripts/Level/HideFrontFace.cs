using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

public class HideFrontFace : MonoBehaviour
{
    public List<GameObject> facesToHide;
    //private Color frontSideColor;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Faces Hidden!");
            foreach (GameObject face in facesToHide)
            {
                face.SetActive(false);
            }
        }

    }


    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (GameObject face in facesToHide)
            {
                face.SetActive(true);
            }
        }
    }
}

    
 