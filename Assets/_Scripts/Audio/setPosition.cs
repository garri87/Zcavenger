using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// USED FOR CAMERA AUDIO LISTENER MATCH THE TARGET POSITION
/// </summary>

public class setPosition : MonoBehaviour
{
    public Transform targetTransform;

    private void Awake()
    {
        targetTransform = GameObject.Find("Player").transform;
    }


    private void Update()
    {
        transform.position = targetTransform.position + Vector3.up;
    }
}
