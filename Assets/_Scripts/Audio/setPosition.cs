using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class setPosition : MonoBehaviour
{
    //USED FOR AUDIO LISTENER MATCH THE TARGET POSITION
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
