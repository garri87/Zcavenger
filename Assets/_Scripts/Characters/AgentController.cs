using Autodesk.Fbx;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using UnityEngine.UI;
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
    [HideInInspector] public EnemyFOV _enemyFov;
    [HideInInspector] public IKManager iKManager;
    public CapsuleCollider hipsCollider;

    #endregion

    

    [Header("AI Behaviour")]
    public float
        agentAlertTime =
            5; //Time in seconds that the agent is on alert state when the target goes off sight

    [HideInInspector] public float alertTimer;

    public enum AgentState
    {
        Active,
        Ontransition,
        Dead,
    }

    public AgentState agentState;

    public int animationState;

    public float disableTime;//Time in seconds that the agent's gameobject will be disabled
    [SerializeField] private float disableTimer;

    #region Player references

    [HideInInspector] public GameObject player;
    private Animator playerAnimator;
    private PlayerController playerController;
    [SerializeField]
    private HealthManager playerHealthManager;
    private Transform playerTransform;
    private Quaternion playerRotation;
    [HideInInspector] public Vector3 playerDirection;

    #endregion

    #region Enemy Scriptable Object Variables

    public Enemy.EnemyType enemyType;
    [SerializeField]
    private float minSpeed, maxSpeed;
    [HideInInspector] public float distanceToPlayer;
    public float attackRange;
    private int minDamage;
    private int maxDamage;
    private int bleedDamageProbability;
    public float attackRate;

    #endregion

    private bool isMoving;
    private bool canAttack;
    public bool attacking;
    public bool playerCatch;
    public LayerMask attackLayerMask;
    private float attackTimer;
    private RaycastHit attackHit;


    #region Playline Variables

    public Transform originTransform;
    public float currentPlayLine;

    #endregion

    private bool growl = false;

    [HideInInspector] public bool hisHit;

    public Floor.BuildingType[] buildingTypes; //Types of building where the agent usually appears

    public static string AgentTag = "Enemy";
    
    private void OnValidate()
    {
        try
        {
            GetEnemyAttr(enemyScriptableObject);
        }
        catch
        {
            Debug.LogWarning("No Scriptable Object found on " + gameObject.name);
        }
    }

    

    private void Awake()
    {
        originTransform = this.transform;

        GetComponents();

        GetEnemyAttr(enemyScriptableObject);


        _navMeshAgent.updateRotation = false;
    }

    private void OnEnable()
    {

        GetEnemyAttr(enemyScriptableObject);

        _navMeshAgent.speed = Random.Range(minSpeed, maxSpeed);

        disableTimer = disableTime;

        SetEnemyDifficulty(GameManager.Instance.gameDifficulty);
    }

   
    void Start()
    {
        currentPlayLine = transform.position.z;
        
        GetPlayer();

        if(_animator)SetAnimatorParam(_animator);

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

        currentPlayLine = transform.position.z;
    }

    private void FixedUpdate()
    {
        if (player != null)
        {
            playerTransform = player.transform;
            playerDirection = (player.transform.position - transform.position).normalized;
        }

        if (_checkGround.isGrounded)
        {
            _navMeshAgent.enabled = true;
        }
        else
        {
            _navMeshAgent.enabled = false;
        }
    }

    /// <summary>
    /// Forces the agent z position to current playline
    /// </summary>
    
    public bool TargetReachable(NavMeshAgent navMeshAgent, Vector3 targetPos)
    {
        
        return navMeshAgent.CalculatePath(targetPos, navMeshAgent.path);

    }


    private void Update()
    {
        _animator.SetBool("PlayerInSight", _enemyFov.targetInSight);
        _animator.SetBool("PlayerCatch", playerCatch);
        _animator.SetBool("Attack", attacking);
        _animator.SetBool("IsMoving", isMoving);


        agentState = _healthManager.IsDead ? AgentState.Dead : agentState;
        
        if (playerHealthManager.IsDead)
        {
            canAttack = false;
            attacking = false;
            playerCatch = false;
            _navMeshAgent.SetDestination(this.transform.position);
        }
        

        switch (agentState)
        {
            case AgentState.Active:

                SetAgentPlayline(currentPlayLine);
               
                if (_navMeshAgent.isOnNavMesh)
                {
                     _rigidbody.velocity = new Vector3(
                         x:_rigidbody.velocity.x,
                         y: _rigidbody.velocity.y, 
                         z: _rigidbody.velocity.z);

                    //Moving Animation if navmeshAgent is moving
                    if (_navMeshAgent.velocity.magnitude < 0.1f || !TargetReachable(_navMeshAgent,playerTransform.position))
                    {//Stop the Agent if velocity is less than 0 or there's no possible path to player
                        isMoving = false;
                    }
                    else
                    {
                        isMoving = true;
                    }
                                        
                    //Stop the Agent if reach the waypoint
                    if (_navMeshAgent.remainingDistance < 0.2f)
                    {
                        _navMeshAgent.SetDestination(this.transform.position);
                    }
                    
                    if (_enemyFov.targetInSight)
                    {
                        alertTimer = agentAlertTime; //resets the alert time
                        attackTimer -= Time.deltaTime; //starts the attack timer

                        if (enemyType == Enemy.EnemyType.Crippled)
                        {
                            Vector3 rot = new Vector3(
                                x: 0, 
                                y: 0, 
                                z: 90 * Mathf.Sign(playerTransform.position.x - transform.position.x));

                            _rigidbody.MoveRotation(rot:Quaternion.Euler(euler:rot)); //Rotates towards the target
                        }

                        if (_navMeshAgent.enabled)
                        {
                            //Chase Target
                            if (!attacking && !_enemyFov.targetAttackable)
                            {
                                _navMeshAgent.SetDestination(playerTransform.position);
                            }

                            //Attack Player
                            if (_enemyFov.targetAttackable && !playerHealthManager.IsDead && !playerCatch)
                            {
                                if (attackTimer <= 0)
                                {
                                    canAttack= true;
                                    attacking = true;
                                    attackTimer = attackRate;
                                }
                                else
                                {
                                    canAttack= false;
                                }
                            }
                            else
                            {
                                attacking = false;
                                canAttack= false;
                            }

                            if (!_enemyFov.targetAttackable)
                            {
                                attacking = false;
                            }
                        }

                        if (!growl)
                        {
                            _animator.SetTrigger("Growl");
                            growl = true;
                        }
                    }

                    if (!_enemyFov.targetInSight || !_enemyFov.targetInRange)
                    {//If target not in sigth or range
                        if (_navMeshAgent.enabled == true)
                        {
                            alertTimer -= Time.deltaTime; //start alert timer
                            if (alertTimer <= 0)
                            {//Go to player last known location
                                Vector3 targetLastLocation = new Vector3(_enemyFov.targetLastLocation.x,
                                    transform.position.y, _enemyFov.targetLastLocation.z);
                                RaycastHit hit;
                                Ray ray = new Ray(transform.position + Vector3.up,
                                    (targetLastLocation - transform.position)+Vector3.up); 
                                if (Physics.Raycast(
                                    ray:ray,
                                    hitInfo: out hit,
                                    maxDistance:_enemyFov.viewRadius,
                                    layerMask:_enemyFov.obstacleLayers))
                                {
                                    targetLastLocation = new Vector3(
                                        x:hit.point.x,
                                        y: transform.position.y,
                                        z:hit.point.z);
                                }
                                
                                _navMeshAgent.SetDestination(targetLastLocation);
                                growl = false;
                            }
                        }
                    }
                }

                break;

            case AgentState.Ontransition:

                if (_navMeshAgent.enabled || _navMeshAgent.isOnNavMesh)
                {
                    _navMeshAgent.SetDestination(transform.position);
                }

                isMoving = false;
                //_rigidbody.velocity = new Vector3(0, _rigidbody.velocity.y, 0);

                break;

            case AgentState.Dead:
                EnemyDeath();
                break;
        }
    }

    private void LateUpdate()
    {
        switch (agentState)
        {
            case AgentState.Active:
                if (_navMeshAgent.enabled == true && _navMeshAgent.isOnNavMesh)
                {
                    if (_navMeshAgent.velocity.sqrMagnitude > Mathf.Epsilon)
                    {
                        transform.rotation = Quaternion.LookRotation(_navMeshAgent.velocity.normalized);
                    }
                }

                break;
        }
    }

    public void AgentAnimationEvent(string command)
    {
        switch (command)
        {
            case "TransitionStart":
                agentState = AgentState.Ontransition;
                break;

            case "TransitionEnd":

                if (!_healthManager.IsDead)
                {
                    agentState = AgentState.Active;
                }

                break;

            case "!isDead":
                _animator.SetBool("IsDead", false);
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
                        enemyImpactParticle.transform.position =
                            playerAnimator.GetBoneTransform(HumanBodyBones.Neck).position;
                    }

                    if (enemyType == Enemy.EnemyType.Crippled)
                    {
                        enemyImpactParticle.transform.position =
                            playerAnimator.GetBoneTransform(HumanBodyBones.LeftFoot).position;
                    }

                    enemyImpactParticle.SetActive(true);
                }

                break;

            case "PushBack":
                
                _rigidbody.AddForce(Vector3.back * 40, ForceMode.Impulse);
                break;
        }
    }

    /// <summary>
    /// Performs validations if attack hits something
    /// </summary>
    private void Attack()
    {
        if (Physics.Raycast(transform.position + Vector3.up,
                transform.TransformDirection(Vector3.forward) * attackRange, out attackHit, attackRange,
                attackLayerMask.value))
        {
            if (attackHit.collider.CompareTag("Player"))
            {
                if (playerController)
                {
                    if (!playerController.isBlocking)
                    {
                        switch (enemyType)
                        {
                            case Enemy.EnemyType.Humanoid:
                                break;

                            case Enemy.EnemyType.Walker:
                            case Enemy.EnemyType.Crippled:

                                if (!playerController.alreadyCatched)
                                {
                                    playerCatch = true;
                                    playerController.beingBitten = true;
                                    playerController.alreadyCatched = true;
                                }

                                break;

                            case Enemy.EnemyType.Runner:

                                break;

                            case Enemy.EnemyType.Crawler:

                                break;

                            case Enemy.EnemyType.Brute:
                                break;
                        }

                        if (playerHealthManager)
                        { 
                            DealDamage(playerHealthManager);
                        }
                        else
                        {
                            Debug.Log("No health manager found");
                        }
                        
                        
                        attacking = false;

                        if (playerController._inventory.drawnWeaponItem != null)
                        {//Cancel player attack if receiving damage
                            playerController._inventory.drawnWeaponItem.attacking = false;
                        }

                        playerAnimator.SetBool("AxeAttack", false);
                        playerAnimator.SetBool("BatAttack", false);
                        playerAnimator.SetBool("KnifeAttack", false);
                        playerAnimator.SetBool("OnLedge", false);
                        playerAnimator.SetBool("OnWall", false);
                        playerAnimator.SetBool("ClimbingLadder", false);
                        playerAnimator.SetBool("Jump", false);
                        playerController.isReloadingWeapon = false;
                        if (enemyType != Enemy.EnemyType.Crippled)
                        {
                            playerAnimator.SetTrigger("Hit");
                        }
                    }
                    else
                    {
                        //If player is blocking activate block recoil animation
                        playerAnimator.SetTrigger("BlockHit");
                    }
                }
                
            }

            if (attackHit.collider.CompareTag("Crashable"))
            {
                //if hits a destructible object, activate object crash method 
                attackHit.collider.GetComponent<CrashObject>().Crash();
            }
        }
    }

    /// <summary>
    /// Function called once when agent dies
    /// </summary>
    private void EnemyDeath()
    {
        if (enemyType != Enemy.EnemyType.Crippled)
        {
            int randomValue = Random.Range(1, 2);
            _animator.SetBool("IsDead" + randomValue, true);
            // _rigidbody.velocity = Vector3.zero;
        }
        else
        {
            _animator.SetBool("IsDead1", true);
            if (hipsCollider != null)
            {
                hipsCollider.isTrigger = true;
            }
        }


        _capsuleCollider.isTrigger = true;

        if (_navMeshAgent.enabled == true && _navMeshAgent.isOnNavMesh)
        {
            _navMeshAgent.SetDestination(this.transform.position);
        }

        attacking = false;
        playerCatch = false;
        playerController.isTrapped = false;
        playerController.beingBitten = false;
        playerController.alreadyCatched = false;

        isMoving = false;
        _enemyFov.enabled = false;

        disableTimer -= Time.deltaTime;
        if (disableTimer <= 0)
        {
            playerController._stompDetector.onEnemy = false;
            gameObject.SetActive(false);
            disableTimer = disableTime;
            transform.position = originTransform.position;
        }
    }

    private void DealDamage(HealthManager targetHealthMngr)
    {
        
        int bleedingProb = Random.Range(0, 100);
        if (bleedingProb <= bleedDamageProbability)
        {
            targetHealthMngr.isBleeding = true;
        }

        int damageGiven = Random.Range(minDamage, maxDamage);
        targetHealthMngr.currentHealth -= damageGiven;
    }


    /// <summary>
    /// Search for a Player GameObject and get references to PlayerController, HealthManager & Animator
    /// </summary>
    private void GetPlayer()
    {
        try
        {
            player = GameObject.Find("Player");
        }
        catch
        {
            Debug.LogWarning("No Player Gameobject Found");
        }

        if (player != null)
        {
            playerController = player.GetComponent<PlayerController>();
            playerHealthManager = player.GetComponent<HealthManager>();
            playerAnimator = player.GetComponent<Animator>();
        }
    }


    private void GetEnemyAttr(Enemy scriptableObject)
    {
        enemyType = scriptableObject.enemyType;
        maxSpeed = scriptableObject.speed;
        minSpeed = scriptableObject.minSpeed;
        minDamage = scriptableObject.minDamage;
        maxDamage = scriptableObject.maxDamage;
        bleedDamageProbability = scriptableObject.bleedDamageProbability;
        attackRate = scriptableObject.attackRate;
        attackRange = scriptableObject.attackDistance;
    }

    private void SetAgentPlayline(float playLine)
    {
        if (transform.position.z != playLine)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, Mathf.Lerp(transform.position.z, playLine, Time.deltaTime * 4));
        }
    }

    private void GetComponents()
    {
        _animator = GetComponent<Animator>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _checkGround = GetComponentInChildren<CheckGround>();
        _enemyFov = GetComponent<EnemyFOV>();
        _healthManager = GetComponent<HealthManager>();
        _rigidbody = GetComponent<Rigidbody>();
        _capsuleCollider = GetComponent<CapsuleCollider>();
    }

    public void SetEnemyDifficulty(GameManager.GameDifficulty difficulty)
    {
        switch (difficulty)
        {
            case GameManager.GameDifficulty.Easy:
                _healthManager.currentHealth /= 4;
                //Quarter health
                break;
            case GameManager.GameDifficulty.Normal:
                _healthManager.currentHealth /= 2;
                //Half health
                break;

            case GameManager.GameDifficulty.Hard:
                //Full health
                break;
        }
    }

    private void SetAnimatorParam(Animator animator)
    {
        animator.SetInteger("State", animationState);
        animator.SetBool("Runner", false);
        animator.SetBool("Walker", false);
        animator.SetBool("Crippled", false);
        animator.SetBool("Crawler", false);
        animator.SetBool("Brute", false);
        animator.SetBool("Humanoid", false);
        animator.SetBool(enemyType.ToString(), true);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawRay(transform.position + Vector3.up, transform.TransformDirection(Vector3.forward) * attackRange);
    }
}