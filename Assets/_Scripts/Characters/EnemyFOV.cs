
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class EnemyFOV : MonoBehaviour
{
    public CapsuleCollider FOVObject;
    
    private HealthManager agentHealthManager;
    
    private AgentController _agentController;
    
    public bool targetInRange;//target is inside the max visual range of the agent, may not be physically visible
    
    public bool targetInSight;//target is inside visual range and visible by the agent
    
    public bool targetAttackable;//target  is in the attackable area of agent
    
    private HealthManager targetHealthManager;
    
    public float targetDistance;
    public float groundDistance = 0;

    private float _attackDistance;

    public float rayLength = 10f;
    public LayerMask layersToRaycast;
    public LayerMask targetLayer;
    public RaycastHit playerHit;
    public RaycastHit[] hits;
    public List<GameObject> hitList;

    public PlayerController playerController;
    private Vector3 targetDirection;
    [HideInInspector]public Vector3 targetLastLocation;
    
    private void Start()
    {
        _agentController = GetComponentInParent<AgentController>();
        agentHealthManager = _agentController._healthManager;
        _attackDistance = _agentController.enemyScriptableObject.attackDistance;
        targetLastLocation = transform.position;
        groundDistance = Mathf.Infinity;

        FOVObject.transform.parent = _agentController._animator.GetBoneTransform(HumanBodyBones.Head);

        if (GameObject.Find("Player").TryGetComponent(out PlayerController playContrller))
        {
            playerController = playContrller;
        }
        
        
    }

    private void FixedUpdate()
    {
        if (targetInRange)
        {
            RayCastAllObjects();
        }
        Debug.DrawRay(transform.position + Vector3.up, targetLastLocation - transform.position,Color.yellow, Time.deltaTime, true);


    }
    private void Update()
    {
        targetDirection = _agentController.playerDirection;
        
        if (agentHealthManager.IsDead)
        {
            FOVObject.enabled = false;
        }
    }
    
    private void RayCastAllObjects()
    {
        Ray ray = new Ray(transform.position + Vector3.up, targetDirection);
        Debug.DrawRay(ray.origin,ray.direction,Color.green,Time.deltaTime);
        hits = Physics.RaycastAll(ray, Mathf.Infinity, layersToRaycast.value);
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].transform.gameObject.layer == LayerMask.NameToLayer("Default"))
            {
                groundDistance = hits[i].distance;
            }

            if (hits[i].transform.gameObject.layer == targetLayer)
            {
                targetDistance = hits[i].distance;
                
                if (targetDistance <= groundDistance && _agentController.currentPlayLine == playerController.currentPlayLine)
                {
                    targetInSight = true;
                }
                else
                {
                    targetInSight = false;
                }

                if (targetInSight)
                {
                    targetLastLocation = hits[i].point - Vector3.up;
                    groundDistance = Mathf.Infinity;
                }
                if (targetDistance <= _attackDistance)
                {
                    if (Physics.Raycast(transform.position + Vector3.up, targetDirection * _attackDistance,
                        out playerHit, Mathf.Infinity, LayerMask.GetMask("Player")))
                    {
                        Debug.DrawRay(ray.origin,ray.direction,Color.red,Time.deltaTime);

                        targetAttackable = true;
                    }
                }
                else
                {
                    targetAttackable = false;
                }
            }
            
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.GetComponent<HealthManager>() !=null)
            {
                targetHealthManager = other.GetComponent<HealthManager>();
            }
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
            hitList.Clear();
        }
    }

    
    

}
