using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

public class HideFrontFace : MonoBehaviour
{
    public GameObject frontSide;
    //private Color frontSideColor;

    public bool fadeOut;
    public bool fadeIn;
    private float fadeAmount;
    public float transitionSpeed;
    public float currentValue;

    void Start()
    {
    }

    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }

    }


    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(true);

            }
        }
    }
}

    
 