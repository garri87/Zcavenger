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
        if (!targetTransform)
        {
            SearchForPlayer("Player");
        }
        else
        {
           ParentListener(targetTransform);
        }

    }

 private void ParentListener(Transform parent)
    {
        transform.parent = parent;
        transform.position = parent.position + Vector3.up;
    }

private void SearchForPlayer(string name)
    {
        try
        {
            Transform player = GameObject.Find(name).transform;
            ParentListener(player);
        }
        catch (Exception e)
        {
            Debug.Log("No Player Found. Exception: " + e);
        }
    }
}

