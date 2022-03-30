using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedgeDetector : MonoBehaviour
{
    [HideInInspector]public Transform ledge;
    public bool ledgeDetected;
    public bool ladderDetected;
    
    [HideInInspector] public Climbable climbable;
    
    [HideInInspector] public Transform ledgeTransform;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ledge"))
        {
            ledgeDetected = true;
            climbable = other.GetComponent<Climbable>();
            ledgeTransform = climbable.ledgeTransform;
            ledge = other.GetComponent<Transform>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ledge"))
        {
            ledgeDetected = false;
            climbable = null;
            ledgeTransform = null;
            ledge = null;
        }
    }
}
