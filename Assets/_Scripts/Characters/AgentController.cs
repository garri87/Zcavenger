using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;


public class AgentController : MonoBehaviour
{
    
    public Enemy enemyScriptableObject;

    #region Components
    [HideInInspector] public Rigidbody _rigidbody;
    [HideInInspector] public Animator _animator;
    [HideInInspector] public NavMeshAgent _navMeshAgent;
    [HideInInspector] private CapsuleCollider _capsuleCollider;
    [HideInInspector] public HealthManager _healthManager;
    [HideInInspector] public CheckGround _checkGround;
    [HideInInspector] public EnemyFOV enemyFov;
    [HideInInspector] public IKManager iKManager;
    public CapsuleCollider hipsCollider;
    #endregion

    
    
    public enum AgentState
    {
        Active,
        Ontransition,
        Dead,
    }

    [Header("AI Behaviour")]
    public float agentAlertTime = 5;
    private float alertTimer;
    
    public AgentState agentState;
    public int animationState;
    private string selectedEnemyType;
    public float disableTime;
    [SerializeField]private float disableTimer;
    
    #region Player references

    [HideInInspector]public GameObject player;
    private Animator playerAnimator;
    private PlayerController playerController;
    private HealthManager playerHealtManager;
    private Transform playerPosition;
    private Quaternion playerRotation;
    [HideInInspector]public Vector3 playerDirection;
    
    #endregion
    
    #region Enemy Scriptable Object Variables
    public Enemy.EnemyType enemyType;
    private float minSpeed;
    private float maxSpeed;
    [HideInInspector]public float distanceToPlayer;
    public float attackDistance;
    private int minDamage;
    private int maxDamage;
    private int bleedDamageProbability;
    #endregion

    #region Attacks Variables

    public bool attacking;
    public bool playerCatch;
    public LayerMask attackLayerMask;

    #endregion
    
    #region Playline Variables

    public enum PlayLine
    {
        playLine0,
        playLine1,
        playLine2,
    }

    public PlayLine playLine;
    public float currentPlayLine;

    
    
    #endregion

    private bool growl;
    [HideInInspector]public bool hisHit;
    
    private void OnValidate()
    {
        player = GameObject.Find("Player");
        _animator = GetComponent<Animator>();
        if (enemyScriptableObject!=null)
        {
            enemyType = enemyScriptableObject.enemyType;
            maxSpeed = enemyScriptableObject.speed;
            minSpeed = enemyScriptableObject.minSpeed;
            minDamage = enemyScriptableObject.minDamage;
            maxDamage = enemyScriptableObject.maxDamage;
            bleedDamageProbability = enemyScriptableObject.bleedDamageProbability;
            attackDistance = enemyScriptableObject.attackDistance;
            
        }
    }

    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _checkGround = GetComponentInChildren<CheckGround>();
        enemyFov = GetComponent<EnemyFOV>();
        _healthManager = GetComponent<HealthManager>();
        _rigidbody = GetComponent<Rigidbody>();
        _capsuleCollider = GetComponent<CapsuleCollider>();
        selectedEnemyType = enemyScriptableObject.enemyType.ToString();
        _animator.SetBool(selectedEnemyType,true);
        
        playerController = player.GetComponent<PlayerController>();
        playerHealtManager = player.GetComponent<HealthManager>();
        playerAnimator = player.GetComponent<Animator>();

        enemyType = enemyScriptableObject.enemyType;
        maxSpeed = enemyScriptableObject.speed;
        minSpeed = enemyScriptableObject.minSpeed;
        minDamage = enemyScriptableObject.minDamage;
        maxDamage = enemyScriptableObject.maxDamage;
        bleedDamageProbability = enemyScriptableObject.bleedDamageProbability;
        attackDistance = enemyScriptableObject.attackDistance;
        _navMeshAgent.speed = Random.Range(minSpeed,maxSpeed);
        _navMeshAgent.updateRotation = false;
    }

    private void OnEnable()
    {
        disableTimer = disableTime;
        
        _animator.SetBool(selectedEnemyType,true);
        _animator.SetInteger("State", animationState);
    }

    
    void Start()
    {
        _animator.SetBool("Runner",false);
        _animator.SetBool("Walker",false);
        _animator.SetBool("Crippled",false);
        _animator.SetBool("Crawler",false);
        _animator.SetBool("Brute",false);
        _animator.SetBool("Humanoid",false);
        _animator.SetBool(selectedEnemyType,true);

        
        
        
        if (enemyType == Enemy.EnemyType.Crippled)
        { 
            _capsuleCollider.enabled = false; 
            hipsCollider.enabled = true;
        }
        else 
        {
            _capsuleCollider.enabled = true;
            hipsCollider.enabled = false;
        }

            switch (playLine)
        {
            case PlayLine.playLine0:
                currentPlayLine = PlayerController.playLine0;
                break;
            
            case PlayLine.playLine1:
                currentPlayLine = PlayerController.playLine1;
                break;        
            case PlayLine.playLine2:
                currentPlayLine = PlayerController.playLine2;
                break;
        }
    }

    private void FixedUpdate()
    {
        playerPosition = player.transform;
        playerDirection = (player.transform.position - transform.position).normalized;
        if (_checkGround.isGrounded)
        {
            _navMeshAgent.enabled = true;
        }
        else 
        { 
            _navMeshAgent.enabled = false; 
        }
        
    }

    private void Update()
    {
        _animator.SetBool("PlayerInSight", enemyFov.playerInSight);
        _animator.SetBool("PlayerCatch", playerCatch);
        _animator.SetBool("Attack", attacking);

        if (_healthManager.IsDead)
        {
            if (enemyType != Enemy.EnemyType.Crippled)
            {
                int randomValue = Random.Range(1, 2);
                _animator.SetBool("IsDead"+randomValue,true);
                _rigidbody.velocity = Vector3.zero;
            }
            else
            {
                _animator.SetBool("IsDead1",true);
            }
            
            playerCatch = false;
            attacking = false;
            _capsuleCollider.isTrigger = true;
            agentState = AgentState.Dead;
        }

        if (playerHealtManager.IsDead)
        {
            playerCatch = false;
        }
        if (!_healthManager.IsDead)
        {
            
        }
        
        switch (agentState)
        {
           case AgentState.Active:
               if (!_healthManager.IsDead)
               {
                  if (_navMeshAgent.isOnNavMesh) 
                {
                    _rigidbody.velocity = new Vector3(_rigidbody.velocity.x,_rigidbody.velocity.y,_rigidbody.velocity.z);
                    if (transform.position.z != currentPlayLine) 
                    { 
                        transform.position = new Vector3(transform.position.x, transform.position.y, currentPlayLine); 
                    }
                    
                    if (enemyFov.playerInSight)
                    {
                       
                        
                        alertTimer = agentAlertTime;
                        
                        if (enemyType == Enemy.EnemyType.Crippled) 
                        { 
                            _rigidbody.MoveRotation(Quaternion.Euler(new Vector3(0, 0,
                                90 * Mathf.Sign(playerPosition.position.x - transform.position.x)))); 
                        }
                        
                        if (_navMeshAgent.enabled == true) 
                        { 
                           
                            if (enemyFov.playerAttackable && !playerHealtManager.IsDead && !playerCatch)
                            { 
                                attacking = true;
                            }
                    
                            else
                            {
                                attacking = false;
                            }

                            if (!enemyFov.playerAttackable)
                            {
                                attacking = false;
                            }
                            

                            if (!attacking)
                            {
                                _navMeshAgent.SetDestination(playerPosition.position); 
                                _animator.SetBool("IsMoving", true); 
                            }
                            if (playerHealtManager.IsDead)
                            {
                                attacking = false;
                                _navMeshAgent.SetDestination(this.transform.position);
                                _animator.SetBool("IsMoving", false);
                                playerCatch = false;
                            }
                        }
                        if (!growl)
                        {
                            _animator.SetTrigger("Growl");
                            growl = true;
                        }
                    }
                    
                    if (!enemyFov.playerInSight || !enemyFov.playerInRange)
                    {
                        if (_navMeshAgent.enabled == true)
                        { 
                            alertTimer -= Time.deltaTime;
                            if (alertTimer <= 0)
                            {
                                _navMeshAgent.SetDestination(enemyFov.playerLastLocation);
                                _animator.SetBool("IsMoving", true);
                                growl = false;
                            }
                            
                            if (_navMeshAgent.remainingDistance < 0.2f)
                            {
                                _navMeshAgent.SetDestination(transform.position);
                                _animator.SetBool("IsMoving", false); 
                            }
                        }
                       
                    }
                }
               }
               if (_healthManager.IsDead)
               {
                   disableTimer = disableTime;
                   agentState = AgentState.Dead;
               }
                break;
            
            case AgentState.Ontransition:

                if (_navMeshAgent.enabled || _navMeshAgent.isOnNavMesh)
                {
                    _navMeshAgent.SetDestination(transform.position);
                }
                
                _animator.SetBool("IsMoving", false);
                _rigidbody.velocity = new Vector3(0, _rigidbody.velocity.y, 0);
                
                if (!attacking)
                {
                    if (_healthManager.IsDead)
                    {
                        disableTimer = disableTime;
                        agentState = AgentState.Dead;
                    }
                    else
                    {
                        disableTimer = disableTime;
                        agentState = AgentState.Active;
                    }
                }
                break;
            
            case AgentState.Dead:
                EnemyDeath();
                break;
        }
    }

    private void LateUpdate()
    {
        if (agentState == AgentState.Active)
        {
            if (_navMeshAgent.enabled == true && _navMeshAgent.isOnNavMesh)
            {
                if (_navMeshAgent.velocity.sqrMagnitude > Mathf.Epsilon)
                {
                    transform.rotation = Quaternion.LookRotation(_navMeshAgent.velocity.normalized);
                }
            }
        }
        
    }
    
    public void AgentAnimationEvent(string command)
    {
        switch (command)
        {
            case "TransitionStart":
                agentState = AgentState.Ontransition;
                break;
            
            case  "TransitionEnd":

                if (!_healthManager.IsDead)
                {
                    agentState = AgentState.Active;
                }
                break;
            
            case "!isDead":
                _animator.SetBool("IsDead",false); 
                break;
            
            case "CatchPlayer": 
                Attack(); 
                break;
            
            case "DoDamage":
                Attack();
                break;
            
            case "AttackEnd":
                attacking = false;
                _animator.SetBool("Attack", false);
                disableTimer = disableTime;
                agentState = AgentState.Active;
                break;
            
            case "InstantiateBlood":
                GameObject enemyImpactParticle = ObjectPool.SharedInstance.GetPooledObject("EnemyImpactParticle");
                if (enemyImpactParticle != null)
                {

                    if (enemyType == Enemy.EnemyType.Walker)
                    {
                        enemyImpactParticle.transform.position = playerAnimator.GetBoneTransform(HumanBodyBones.Neck).position;
                    }

                    if (enemyType == Enemy.EnemyType.Crippled)
                    {
                        enemyImpactParticle.transform.position = playerAnimator.GetBoneTransform(HumanBodyBones.LeftFoot).position;
                    }
                    enemyImpactParticle.SetActive(true);
                }
                break;
            
            case "PushBack":
                _rigidbody.AddForce(Vector3.back * 20 ,ForceMode.Impulse);
                
                break;
        }
    }
   
    
    
    private void Attack()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up, transform.TransformDirection(Vector3.forward) * attackDistance, out hit, attackDistance, attackLayerMask.value))
        { 
            if (hit.collider.CompareTag("Player"))
            {
                if (!playerController.blocking)
                {
                    switch (enemyType)
                    {
                        case Enemy.EnemyType.Humanoid:
                            break;
            
                        case Enemy.EnemyType.Walker:

                            if (!playerController.alreadyCatched)
                            {
                                playerCatch = true;
                                playerController.bitten = true;
                                playerController.alreadyCatched = true;
                            }
                            break;
            
                        case Enemy.EnemyType.Runner:
                            break;
            
                        case Enemy.EnemyType.Crawler:
                            break;
            
                        case Enemy.EnemyType.Crippled:
                            if (!playerController.alreadyCatched)
                            {
                                playerCatch = true;
                                playerController.trapped = true;
                                playerController.alreadyCatched = true; 
                            }
                            break;
            
                        case Enemy.EnemyType.Brute:
                            break; 
                    
                    }
                    DealDamage();
                    attacking = false;
                    if (playerController.equippedWeaponItem != null)
                    {
                        playerController.equippedWeaponItem.attacking = false;  
                    }
                    playerAnimator.SetBool("MeleeAttack1", false);
                    playerAnimator.SetBool("MeleeAttack2", false);
                    playerAnimator.SetBool("MeleeAttack3", false);
                    playerAnimator.SetBool("OnLedge", false);
                    playerAnimator.SetBool("OnWall", false);
                    playerAnimator.SetBool("ClimbingLadder", false);
                    playerAnimator.SetBool("Jump", false);
                    playerController.reloadingWeapon = false;
                    if (enemyType != Enemy.EnemyType.Crippled)
                    {
                        playerAnimator.SetTrigger("Hit");
                    }
                    
                    playerController.controllerType = PlayerController.ControllerType.DefaultController;

                    playerController.DefaultController();
                }
                else
                {
                    playerAnimator.SetTrigger("BlockHit");
                }
            }

            if (hit.collider.CompareTag("Crashable"))
            {
                hit.collider.GetComponent<CrashObject>().Crash();
            }
        }
    }

    private void EnemyDeath()
    {
        if (_navMeshAgent.enabled == true && _navMeshAgent.isOnNavMesh)
        {
            _navMeshAgent.SetDestination(this.transform.position);
        }

        playerCatch = false;
        playerController.trapped = false;
        playerController.bitten = false;
        if (playerController.alreadyCatched)
        {
            playerController.alreadyCatched = false;
        }
        _animator.SetBool("IsMoving", false);
        attacking = false;
        enemyFov.enabled = false;
        _capsuleCollider.isTrigger = true;
        if (enemyType==Enemy.EnemyType.Crippled)
        {
            if (hipsCollider != null)
            {
                hipsCollider.isTrigger = true;
            }
        }
        disableTimer -= Time.deltaTime;
        if (disableTimer <=0)
        {
            playerController._stompDetector.onEnemy = false;
            gameObject.SetActive(false);
            disableTimer = disableTime;
        }
    }
    private void DealDamage()
    {
        int bleedingProb = Random.Range(0,100);
        if (bleedingProb <=bleedDamageProbability)
        {
            
            Debug.Log("Player is Bleeding! by a chance of: " + bleedingProb + " %");
            playerHealtManager.isBleeding = true;
        }
        int damageGiven = Random.Range(minDamage, maxDamage);
        playerHealtManager.currentHealth -= damageGiven;
        Debug.Log("Player Took " + damageGiven + " damage");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawRay(transform.position + Vector3.up, transform.TransformDirection(Vector3.forward) * attackDistance);
    }
    
}

