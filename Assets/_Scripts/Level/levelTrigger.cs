using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class levelTrigger : MonoBehaviour
{
    public GameObject bossGameObject;
    public HealthManager bossHealthManager;
    public Rigidbody truckRigidbody;    
    public Rigidbody[] targetRigidbodies;
    public float force = 80;
    public bool activateTrigger;

    private void Awake()
    {

        activateTrigger = false;
    }

    private void Update()
    {
        if (bossHealthManager.IsDead)
        {
            activateTrigger = true;
        }

        if (activateTrigger)
        {
            BossDeath();
        }
    }


    public void BossDeath()
    {
        foreach (Rigidbody targetRB in targetRigidbodies)
        {
            targetRB.isKinematic = false;
            targetRB.useGravity = true;
        }
        
        truckRigidbody.AddForce(transform.TransformDirection(Vector3.forward) * force,ForceMode.Force);
        
    }
}
