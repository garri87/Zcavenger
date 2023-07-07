using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
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


    public enum AgentState
    {
        Active,
        Ontransition,
        Dead,
    }

    [Header("AI Behaviour")]
    public float
        agentAlertTime =
            5; //Tiempo en segundos que el agente esta en alerta cuando el objetivo sale de su campo de vision

    [HideInInspector] public float alertTimer;

    public AgentState agentState;
    public int animationState;
    public float disableTime;
    [SerializeField] private float disableTimer;

    #region Player references

    [HideInInspector] public GameObject player;
    private Animator playerAnimator;
    private PlayerController playerController;
    private HealthManager playerHealthManager;
    private Transform playerPosition;
    private Quaternion playerRotation;
    [HideInInspector] public Vector3 playerDirection;

    #endregion

    #region Enemy Scriptable Object Variables

    public Enemy.EnemyType enemyType;
    private float minSpeed;
    private float maxSpeed;
    [HideInInspector] public float distanceToPlayer;
    public float attackDistance;
    private int minDamage;
    private int maxDamage;
    private int bleedDamageProbability;

    #endregion

    #region Attacks Variables

    public float attackRate;
    public bool attacking;
    public bool playerCatch;
    public LayerMask attackLayerMask;
    private float attackTimer;

    #endregion

    #region Playline Variables

    public Transform originTransform;
    public float currentPlayLine;

    #endregion

    private bool growl;
    [HideInInspector] public bool hisHit;

    private void OnValidate()
    {
        try
        {
            GetEnemyType(enemyScriptableObject);
        }
        catch
        {
            Debug.LogWarning("No Scriptable Object found on " + gameObject.name);
        }
    }


    private void Awake()
    {
        originTransform = this.transform;

        _animator = GetComponent<Animator>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _checkGround = GetComponentInChildren<CheckGround>();
        _enemyFov = GetComponent<EnemyFOV>();
        _healthManager = GetComponent<HealthManager>();
        _rigidbody = GetComponent<Rigidbody>();
        _capsuleCollider = GetComponent<CapsuleCollider>();
        _animator.SetBool(enemyType.ToString(), true);

        GetEnemyType(enemyScriptableObject);

        _navMeshAgent.speed = Random.Range(minSpeed, maxSpeed);
        _navMeshAgent.updateRotation = false;
    }

    private void OnEnable()
    {
        disableTimer = disableTime;

        _animator.SetBool(enemyType.ToString(), true);
        _animator.SetInteger("State", animationState);
    }


    void Start()
    {
        currentPlayLine = transform.position.z;
        
        GetPlayer();
        _animator.SetBool("Runner", false);
        _animator.SetBool("Walker", false);
        _animator.SetBool("Crippled", false);
        _animator.SetBool("Crawler", false);
        _animator.SetBool("Brute", false);
        _animator.SetBool("Humanoid", false);
        _animator.SetBool(enemyType.ToString(), true);

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
            playerPosition = player.transform;
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


    private void Update()
    {
        _animator.SetBool("PlayerInSight", _enemyFov.targetInSight);
        _animator.SetBool("PlayerCatch", playerCatch);
        _animator.SetBool("Attack", attacking);

        if (_healthManager.IsDead)
        {
            agentState = AgentState.Dead;
        }

        if (playerHealthManager.IsDead)
        {
            playerCatch = false;
        }

        switch (agentState)
        {
            case AgentState.Active:

                if (transform.position.z != currentPlayLine)
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y,Mathf.Lerp(transform.position.z,currentPlayLine,Time.deltaTime*4));
                }
                if (_navMeshAgent.isOnNavMesh)
                {
                     _rigidbody.velocity =
                         new Vector3(_rigidbody.velocity.x, _rigidbody.velocity.y, _rigidbody.velocity.z);

                    //Moving Animation if navmeshAgent is moving

                    if (_navMeshAgent. velocity.magnitude < 0.1f ||
                        !_navMeshAgent.CalculatePath(playerPosition.position,_navMeshAgent.path))
                    {
                        _animator.SetBool("IsMoving", false);
                        
                    }
                    else
                    {
                        _animator.SetBool("IsMoving", true);
                    }
                    
                    //Stop the Agent if reach the waypoint
                    if (_navMeshAgent.remainingDistance < 0.2f)
                    {
                        _navMeshAgent.SetDestination(this.transform.position);
                    }

                    
                    
                    if (_enemyFov.targetInSight)
                    {
                        alertTimer = agentAlertTime;
                        attackTimer -= Time.deltaTime;

                        if (enemyType == Enemy.EnemyType.Crippled)
                        {
                                  _rigidbody.MoveRotation(Quaternion.Euler(new Vector3(0, 0,
                                      90 * Mathf.Sign(playerPosition.position.x - transform.position.x))));
                        }

                        if (_navMeshAgent.enabled == true)
                        {
                            //Chase Target
                            if (!attacking && !_enemyFov.targetAttackable)
                            {
                                _navMeshAgent.SetDestination(playerPosition.position);
                            }

                            //Attack Player
                            if (_enemyFov.targetAttackable && !playerHealthManager.IsDead && !playerCatch)
                            {
                                if (attackTimer <= 0)
                                {
                                    attacking = true;
                                    attackTimer = attackRate;
                                }
                            }
                            else
                            {
                                attacking = false;
                            }

                            if (!_enemyFov.targetAttackable)
                            {
                                attacking = false;
                            }

                            if (playerHealthManager.IsDead)
                            {
                                attacking = false;
                                _navMeshAgent.SetDestination(this.transform.position);
                                playerCatch = false;
                            }
                        }

                        if (!growl)
                        {
                            _animator.SetTrigger("Growl");
                            growl = true;
                        }
                    }

                    if (!_enemyFov.targetInSight || !_enemyFov.targetInRange)
                    {
                        if (_navMeshAgent.enabled == true)
                        {
                            alertTimer -= Time.deltaTime;
                            if (alertTimer <= 0)
                            {
                                Vector3 targetLastLocation = new Vector3(_enemyFov.targetLastLocation.x,
                                    transform.position.y, _enemyFov.targetLastLocation.z);
                                RaycastHit hit;
                                Ray ray = new Ray(transform.position + Vector3.up,
                                    (targetLastLocation - transform.position)+Vector3.up); 
                                if (Physics.Raycast(ray,out hit,_enemyFov.viewRadius,_enemyFov.obstacleLayers))
                                {
                                    targetLastLocation = new Vector3(hit.point.x, transform.position.y, hit.point.z);
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
                _animator.SetBool("IsMoving", false);
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
                _rigidbody.AddForce(Vector3.back * 20, ForceMode.Impulse);

                break;
        }
    }

    private void Attack()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up,
                transform.TransformDirection(Vector3.forward) * attackDistance, out hit, attackDistance,
                attackLayerMask.value))
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
                                playerController.beingBitten = true;
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
                    if (playerController._inventory.drawnWeaponItem != null)
                    {
                        playerController._inventory.drawnWeaponItem.attacking = false;
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

    /// <summary>
    /// Function called when a enemy dies
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
        playerController.trapped = false;
        playerController.beingBitten = false;
        playerController.alreadyCatched = false;

        _animator.SetBool("IsMoving", false);
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

    private void DealDamage()
    {
        int bleedingProb = Random.Range(0, 100);
        if (bleedingProb <= bleedDamageProbability)
        {
            Debug.Log("Player is Bleeding! by a chance of: " + bleedingProb + " %");
            playerHealthManager.isBleeding = true;
        }

        int damageGiven = Random.Range(minDamage, maxDamage);
        playerHealthManager.currentHealth -= damageGiven;
        Debug.Log("Player Took " + damageGiven + " damage");
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

    private void GetEnemyType(Enemy scriptableObject)
    {
        enemyType = scriptableObject.enemyType;
        maxSpeed = scriptableObject.speed;
        minSpeed = scriptableObject.minSpeed;
        minDamage = scriptableObject.minDamage;
        maxDamage = scriptableObject.maxDamage;
        bleedDamageProbability = scriptableObject.bleedDamageProbability;
        attackRate = scriptableObject.attackRate;
        attackDistance = scriptableObject.attackDistance;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawRay(transform.position + Vector3.up, transform.TransformDirection(Vector3.forward) * attackDistance);
    }
}