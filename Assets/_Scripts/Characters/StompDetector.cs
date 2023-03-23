using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StompDetector : MonoBehaviour
{
    private RaycastHit hit;
    public float rayOffset;
    public bool canStomp;
    public bool onEnemy;
    [HideInInspector]public AgentController agentController;

    private void Awake()
    {
    }

    private void FixedUpdate()
    {
       // RayCastForEnemies();
    }

    private void Update()
    {
        if (onEnemy)
        {
            if (agentController != null)
            {
                if (!agentController._healthManager.IsDead)
                {
                    canStomp = true;
                }
                else
                {
                    canStomp = false;
                }
            }
        }
        else
        {
            canStomp = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (other.GetComponent<AgentController>() != null)
            {
                agentController = other.GetComponent<AgentController>();
            }
            else
            {
                agentController = other.GetComponentInParent<AgentController>();
            }
            
            if (agentController.enemyType == Enemy.EnemyType.Crippled && !agentController._healthManager.IsDead)
            {
                onEnemy = true;
            }
            else
            {
                onEnemy = false;
            }
        }
        else
        {
            onEnemy = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (agentController != null)
            {
                if (agentController.enemyType == Enemy.EnemyType.Crippled && !agentController._healthManager.IsDead)
                {
                    onEnemy = true;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            onEnemy = false;
            agentController = null;
        }
    }


    public void RayCastForEnemies()
    {
        Ray ray = new Ray(transform.position + Vector3.up, (transform.TransformDirection(Vector3.forward) + Vector3.down) * rayOffset);
        Debug.DrawRay(ray.origin,ray.direction * rayOffset,Color.red,Time.deltaTime);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity,LayerMask.GetMask("Enemy")))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                if (hit.collider.GetComponent<AgentController>() ==null)
                {
                    agentController = hit.collider.GetComponentInParent<AgentController>();
                }
                else
                {
                    agentController = hit.collider.GetComponent<AgentController>();
                }
                
            }
        }
        else
        {
            canStomp = false;
        }
        
    }
}
