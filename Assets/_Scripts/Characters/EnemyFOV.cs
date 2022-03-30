
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyFOV : MonoBehaviour
{
    
    
    public CapsuleCollider FOVObject;
    private HealthManager enemyHealth;
    private AgentController _agentController;
    
    public bool playerInRange;//Player is inside the max visual range of the agent
    public bool playerInSight;//Player is inside visual range and visible by the agent
    public bool playerAttackable;//Player is in the attackable area of agent
    private HealthManager playerHealthManager;
    
    public float targetDistance;
    public float playerDistance = 1;
    public float groundDistance = 0;

    private float _attackDistance;

    public float rayLength = 10f;
    public LayerMask layer;
    public RaycastHit playerHit;
    public RaycastHit[] hits;
    public List<GameObject> hitList;

    private Transform playerPosition;
    private Vector3 playerDirection;
    [HideInInspector]public Vector3 playerLastLocation;
    
    private void Start()
    {
        _agentController = GetComponentInParent<AgentController>();
        enemyHealth = _agentController._healthManager;
        playerPosition = _agentController.player.transform;
        _attackDistance = _agentController.enemyScriptableObject.attackDistance;
        playerLastLocation = transform.position;
        groundDistance = Mathf.Infinity;
    }

    private void FixedUpdate()
    {
        if (playerInRange)
        {
            RayCastAllObjects();
        }
        Debug.DrawRay(transform.position + Vector3.up, playerLastLocation - transform.position,Color.yellow, Time.deltaTime, true);


    }
    private void Update()
    {
        playerDirection = _agentController.playerDirection;
        
        if (enemyHealth.IsDead)
        {
            FOVObject.enabled = false;
        }
    }

    public void GetDistanceToPlayer()
    {
        if (Physics.Raycast(transform.position + Vector3.up,playerDirection, out playerHit, Mathf.Infinity,LayerMask.GetMask("Player")))
        {
            if (playerHit.collider.CompareTag("Player"))
            {
                playerDistance = playerHit.distance;
            }
        }
    }

    private void RayCastAllObjects()
    {
        Ray ray = new Ray(transform.position + Vector3.up, playerDirection);
        Debug.DrawRay(ray.origin,ray.direction,Color.green,Time.deltaTime);
        hits = Physics.RaycastAll(ray, Mathf.Infinity, layer.value);
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].transform.gameObject.layer == LayerMask.NameToLayer("Default"))
            {
                groundDistance = hits[i].distance;
            }

            if (hits[i].transform.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                targetDistance = hits[i].distance;
                
                if (targetDistance <= groundDistance && _agentController.currentPlayLine == PlayerController.currentPlayLine)
                {
                    playerInSight = true;
                }
                else
                {
                    playerInSight = false;
                }

                if (playerInSight)
                {
                    playerLastLocation = hits[i].point - Vector3.up;
                    groundDistance = Mathf.Infinity;
                }
                if (targetDistance <= _attackDistance)
                {
                    if (Physics.Raycast(transform.position + Vector3.up, playerDirection * _attackDistance,
                        out playerHit, Mathf.Infinity, LayerMask.GetMask("Player")))
                    {
                        Debug.DrawRay(ray.origin,ray.direction,Color.red,Time.deltaTime);

                        playerAttackable = true;
                    }
                }
                else
                {
                    playerAttackable = false;
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
                playerHealthManager = other.GetComponent<HealthManager>();
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerLastLocation = Vector3.Slerp(transform.position,playerLastLocation, Random.Range(0f, 1f));
            playerInRange = false;
            playerInSight = false;
            playerAttackable = false;
            hitList.Clear();
        }
    }

    
    

}
