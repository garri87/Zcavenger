
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class EnemyFOV : MonoBehaviour
{
    private HealthManager agentHealthManager;
    private AgentController _agentController;
    
    public CapsuleCollider FOVObject;

    public float viewRadius = 10f;

    public float viewAngle = 45f;

    public LayerMask targetLayer;
    public LayerMask obstacleLayers;
    
    public bool targetInRange;//target is inside the max visual range of the agent, may not be physically visible
    public bool targetInSight;//target is inside visual range and visible by the agent
    public bool targetAttackable;//target  is in the attackable area of agent
    
    public float targetDistance;

    private float _attackDistance;

    private Transform targetTransform;
    [HideInInspector]public Vector3 targetLastLocation;

    private void Awake()
    {
        _agentController = GetComponentInParent<AgentController>();
        agentHealthManager = _agentController._healthManager;
        FOVObject.transform.parent = _agentController._animator.GetBoneTransform(HumanBodyBones.Head);
        _attackDistance = _agentController.enemyScriptableObject.attackDistance;

    }

    private void Start()
    {
        
        targetLastLocation = transform.position;
        
    }

    private void FixedUpdate()
    {
        if (targetInRange)
        {
            RayCastForObjects();
        }
        
        Debug.DrawRay(transform.position + Vector3.up, targetLastLocation - transform.position,Color.yellow, Time.deltaTime, true);


    }
    private void Update()
    {
        
        if (agentHealthManager.IsDead)
        {
            FOVObject.enabled = false;
        }
    }
    
    private void RayCastForObjects()
    {
        Ray ray = new Ray(transform.position + Vector3.up, targetTransform.position - transform.position);
        
        Debug.DrawRay(ray.origin,ray.direction,Color.blue,Time.deltaTime);

        if (Physics.Raycast(ray,(targetTransform.position - transform.position).magnitude,targetLayer) 
            && !CheckObstacle(targetTransform))
        {
           //La Ia puede ver al jugador
           targetInSight = true;
        }
        else
        {
            //La IA no puede ver al jugador
            targetInSight = false;
        }
    }

    private bool CheckObstacle(Transform target)
    {
        Vector3 directionToTarget = target.position - transform.position; // Dirección al jugador
        float distanceToTarget = directionToTarget.magnitude; // Distancia al jugador
        if (Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstacleLayers)) // Comprueba si hay obstáculos entre la IA y el jugador
        {
            return true; // Si hay obstáculos, no puede ver al jugador
        }
        return false; // Si no hay obstáculos, puede ver al jugador
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            targetInRange = true;
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
            targetLastLocation = Vector3.Slerp(transform.position,targetLastLocation, Random.Range(0f, 1f));
            targetInRange = false;
            targetInSight = false;
            targetAttackable = false;
        }
    }

    
    

}
