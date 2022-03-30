using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleCollider : MonoBehaviour
{
    public Collider _collider;
    public string targeTag;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targeTag))
        {
            _collider.enabled = false;
        }
        else
        {
            _collider.enabled = true;
        }
    }
}
