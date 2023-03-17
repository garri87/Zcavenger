
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class EnemyFOV : MonoBehaviour
{
    private HealthManager agentHealthManager;
    private AgentController _agentController;
    
    public Collider FOVObject;

    public float viewRadius = 10f;

    public float viewAngle = 45f;

    public LayerMask targetLayer;
    public LayerMask obstacleLayers;
    
    public bool targetInRange;//target is inside the max visual range of the agent, may not be physically visible
    public bool targetInSight;//target is inside visual range and visible by the agent
    public bool targetAttackable;//target  is in the attackable area of agent
    
    public float targetDistance;

    private float _attackDistance;

    public Transform targetTransform;
    [HideInInspector]public Vector3 targetLastLocation;

    private void Awake()
    {
        _agentController = GetComponent<AgentController>();
        agentHealthManager = GetComponent<HealthManager>();
        _attackDistance = _agentController.enemyScriptableObject.attackDistance;

    }

    private void Start()
    {
        
        targetLastLocation = transform.position;
        
    }

    private void FixedUpdate()
    {
        if (targetInRange) // Si el objetivo esta dentro del campo visual, determinar si este esta directamente visible
        {
            targetInSight = RayCastForTarget(targetTransform);
        }
        else
        {
            targetInSight = false;
            targetAttackable = false;
        }

        if (targetInSight)
        {
            targetLastLocation = targetTransform.position;
            if ((targetTransform.position - transform.position).magnitude <= _attackDistance)
            {
                targetAttackable = true;
            }
            else
            {
                targetAttackable = false;
            }
            
            //Debug.DrawRay(transform.position + Vector3.up, targetTransform.position - transform.position,Color.red, Time.deltaTime, true);
        }
        else
        {
            Vector3 lastLocation = new Vector3(targetLastLocation.x,transform.position.y,targetLastLocation.z);
            Debug.DrawRay(transform.position + Vector3.up, lastLocation - transform.position,Color.yellow, Time.deltaTime, true);
        }




    }
    private void Update()
    {
        
        if (agentHealthManager.IsDead)
        {
            FOVObject.enabled = false;
        }
    }
    
    private bool RayCastForTarget(Transform target)
    {
        Ray ray = new Ray(transform.position + Vector3.up, target.position - transform.position);
        if (Physics.Raycast(ray,(target.position - transform.position).magnitude,targetLayer) 
            && CheckObstacle(targetTransform)) //Si no hay obstaculos
        {
           //La Ia puede ver al jugador
           return true;
        }
        else
        {
            //La IA no puede ver al jugador
            return false;
        }
    }

    private bool CheckObstacle(Transform target)
    {
        Vector3 directionToTarget = target.position - transform.position; // Dirección al jugador
        float distanceToTarget = directionToTarget.magnitude; // Distancia al jugador
        Debug.DrawRay(transform.position + Vector3.up, directionToTarget,Color.blue, Time.deltaTime, true);

        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up, directionToTarget, out hit,  distanceToTarget, obstacleLayers)) // Comprueba si hay obstáculos entre la IA y el jugador
        {
            if (!hit.collider.isTrigger)
            {
                return false; // Si hay obstáculos, no puede ver al jugador   
            }
        }
        return true; // Si no hay obstáculos, puede ver al jugador
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            targetInRange = true;
            targetTransform = other.transform;
        }
    }

    /*private void OnTriggerStay(Collider other)
    {
        if (!playerInRange)
        {
            if (other.CompareTag("Player"))
            {
                playerInRange = true;
            }  
        }
    }*/

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            
            targetLastLocation = Vector3.Slerp(transform.position,other.transform.position, Random.Range(0.8f, 1f));
            targetInRange = false;
        }
    }

   /* private void OnDrawGizmosSelected()
    {
        
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position + Vector3.up, targetTransform.position - transform.position);
        
    }*/
}
