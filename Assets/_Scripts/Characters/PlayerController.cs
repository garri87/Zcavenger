using System;
using TMPro;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region ControllerTypes

    public KeyAssignments keyAssignments;
    public enum ControllerType
    {
        DefaultController,
        OnLadderController,
        OnUIController,
        StandByController,
        OnLedgeController,
    }

    public ControllerType controllerType;

    #endregion

    #region Player States

    public enum PlayerState
    {
        OnUI,

        IsAlive,
        IsDead,

        Default,
        IsIdle,
        IsMoving,
        IsWalking,
        IsCrouching,
        IsProne,
        IsRolling,

        IsOnLedge,
        IsClimbingLedge,
        IsClimbingLadder,
        
        IsAiming,
        IsReloading,
        
        IsHidden,
        IsBleeding,
        IsInjured,
        IsSick,
    }

    public PlayerState playerState;

    #endregion

    #region Movement Variables

    [HideInInspector]public float horizontalInput, verticalInput;
    [HideInInspector] private Vector3 lastPosition;
    public bool ascending;
    public bool descending;
    public bool jump;
    public bool hardLanded;
    private bool walking;
    private bool crouch;
    public bool prone;
    public bool roll;
    public bool blocking;
    public bool trapped;
    public bool bitten;
    public bool alreadyCatched;
    public bool onConduct;
    public bool onTransition;
   
    
    
    private float _crouchColliderHeight;
    private Vector3 _crouchColliderCenter;
    private Vector3 _proneColliderCenter;

    public float currentSpeed;
    public float jumpSpeed;
    public float crouchWalkSpeed;
    public float injuredSpeed = 1;
    public float acceleration = 4;
    public float normalSpeed;
    public float struggleForce = 3; 
    public float hitDistance;
    

    private bool doubleTap;
    private float doubleTapTime = 0.5f;
    private int tapCount = 0;

    [HideInInspector]
    public int FacingSign
    {
        get
        {
            Vector3 perp = Vector3.Cross(transform.forward, Vector3.forward);
            float dir = Vector3.Dot(perp, transform.up);
            return dir > 0f ? -1 : dir < 0f ? 1 : 0;
        }
    }

    #endregion

    #region Unarmed Attacks variables

    public bool canStomp = false;
    public int stompDamage = 10;
    public GameObject bloodSplatterParticle;

    #endregion
    
    #region Ladders & Ledge Climb variables

    private Climber _climber;
    private Ladder _ladder;
    private Vector3 ladderPosition;
    private Transform ladderTransform;
    public float ladderOffset;
    public float climbVelocity = 2f;
    public float transitionSpeed;
    public bool nextToLadder;
    public bool climbingLadder;
    public bool upLadder;
    public bool climbingToTop;

    #endregion

    #region Weapon Handling Variables

    public WeaponItem equippedWeaponItem;
    public bool weaponEquipped;
    public Transform _weaponHolderTransform;
    [HideInInspector]public bool isAiming;
    [HideInInspector]public bool attacking;
    [HideInInspector] public bool reloadingWeapon;
    public Transform targetTransform;
    private SpriteRenderer crosshair;
    public LayerMask mouseAimMask;

    #endregion

    #region Components
    public GameManager gameManager;
    [HideInInspector] public HealthManager _healthManager;
    [HideInInspector] public ParticleSystem _particleSystem;
    [HideInInspector] public Rigidbody _rigidbody;
    [HideInInspector] public Animator _animator;
    public CapsuleCollider _collider;
    [HideInInspector] public CheckGround _checkGround;
    private Camera mainCamera;
    private PlayerAudio _playerAudio;
    [HideInInspector]public AgentController stompTargetAgentController;
    public StompDetector _stompDetector;

    #endregion

    #region PlayLine variables

    public enum PlayLine
    {
        PlayLine0,
        PlayLine1,
        PlayLine2,
        PlayLine3,
    }

    public PlayLine playLine;
    public bool onDoor;
    [HideInInspector]public bool inBackPlayLine;
    public static float playLine0 = 0;
    public static float playLine1 = 4;
    public static float playLine2 = 8;
    public static float playLine3 = 12;
    [HideInInspector]public static float currentPlayLine = playLine0;
    private Transform doorPos;
    [HideInInspector]public Door doorScript;
    public Transform mouseTargetLayer;

    #endregion

    #region Inventory Variables

    [HideInInspector] public Inventory _inventory;
    private static readonly int Speed = Animator.StringToHash("Speed");

    #endregion

    public bool finishedLevel = false;

    private void OnValidate()
    {
    }

    void Start()
    {
        mainCamera = Camera.main;
        _healthManager = GetComponent<HealthManager>();
        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        _inventory = GetComponent<Inventory>();
        _checkGround = GetComponentInChildren<CheckGround>();
        _playerAudio = GetComponent<PlayerAudio>();
        _climber = GetComponent<Climber>();
        currentSpeed = normalSpeed;
        crosshair = targetTransform.GetComponent<SpriteRenderer>();
        
        transform.position = gameManager.startingPosition.position;
        SwitchPlayLine(gameManager.startingPlayline);
    }

    private void FixedUpdate()
    {
        
    }

    void Update()
    {

        #region axis inputs
        
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        _animator.SetFloat("VerticalInput", verticalInput);

        #endregion

        CheckFallingState();
        

        if (_weaponHolderTransform.childCount > 0)
        {
            weaponEquipped = true;
            equippedWeaponItem = _weaponHolderTransform.GetChild(0).GetComponent<WeaponItem>();
            attacking = equippedWeaponItem.attacking;
        }
        else
        {
            weaponEquipped = false;
            attacking = false;
            equippedWeaponItem = null;
        }
        
        #region PlayLine placement

        if (transform.position.z != currentPlayLine && !climbingLadder)
        {
            Vector3 newPosition = new Vector3(transform.position.x, transform.position.y, currentPlayLine);
            transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * transitionSpeed);
        }
        #endregion

        #region Controller Switch

        switch (controllerType)
        {
            case ControllerType.DefaultController:
                DefaultController();
                MouseAim();
                break;

            case ControllerType.OnLadderController:
                OnLadderController();
                break;
            
            case ControllerType.OnLedgeController:
                OnLedgeController();
                break;
            
            case ControllerType.StandByController:
                StandByController();
                break;
            
            case ControllerType.OnUIController:
                OnUIController();
                break;
            
        }
        
        if (_healthManager.IsDead)
        {
            controllerType = ControllerType.StandByController;
        }

        if (!_healthManager.IsDead)
        {
            if (!_inventory.showInventory)
            {
                if (!onTransition)
                {
                    if (!climbingLadder || 
                        !_climber.attachedToLedge || 
                        !_climber.climbingLedge || 
                        !climbingToTop||
                        !roll || 
                        !blocking ||
                        !trapped || 
                        !bitten || 
                        !hardLanded || 
                        !blocking||
                        !finishedLevel)
                    {
                        controllerType = ControllerType.DefaultController;
                    }
                }
            }

            if (hardLanded || blocking || trapped || bitten || finishedLevel)
            {
                controllerType = ControllerType.StandByController;
            }
            
            //STOMP ENEMIES
            if (canStomp && !bitten && Input.GetKeyDown(keyAssignments.attackKey.keyCode))
            {
                _animator.SetTrigger("Stomp");
            }

            #region Ladder Climbing

            if (nextToLadder)
            {
                //player climbing from bottom side
                if (verticalInput > 0 && !upLadder)
                {
                    climbingLadder = true;
                    nextToLadder = false;
                    AttachOnLadder("DownLadder");
                }

                //player climbing from bottom upper side
                if (verticalInput < 0 && nextToLadder && upLadder)
                {
                    climbingLadder = true;
                    nextToLadder = false;
                    AttachOnLadder("UpLadder");
                    controllerType = ControllerType.OnLadderController;
                }
            }

            if (climbingLadder || climbingToTop)
            {
                controllerType = ControllerType.OnLadderController;
            }
            
            

            #endregion

            #region Ledge Climbing

            if (_climber.attachedToLedge || _climber.climbingLedge)
            {
                controllerType = ControllerType.OnLedgeController;
            }
            #endregion
        }
        #endregion

        #region UI Management

        if (_inventory.showInventory == true || GameManager.gamePaused)
        {
            playerState = PlayerState.OnUI;
            controllerType = ControllerType.OnUIController;
        }

        #endregion

        #region Animator parameters

        _animator.SetBool("ClimbingLadder", climbingLadder);
        _animator.SetBool("UpLadder", upLadder);
        _animator.SetBool("ClimbingToTop", climbingToTop);
        _animator.SetBool("Aim", isAiming);
        _animator.SetBool("Jump", jump);
        _animator.SetBool("Walking", walking);
        _animator.SetBool("Crouch", crouch);
        _animator.SetBool("Prone", prone);
        _animator.SetBool("Roll", roll);
        _animator.SetBool("IsDead", _healthManager.IsDead);
        _animator.SetBool("Blocking", blocking);
        _animator.SetBool("Trapped",trapped);
        _animator.SetBool("Bitten", bitten);

        #endregion
    }



    #region Collider OnTrigger Functions

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ladder"))
        {
            nextToLadder = true;
            ladderTransform = other.GetComponent<Transform>();
            _ladder = other.GetComponent<Ladder>();
        }

        if (other.CompareTag("UpLadder") && climbingLadder)
        {
            nextToLadder = false;
            upLadder = true;
        }

        if (other.CompareTag("UpLadder") && !climbingLadder)
        {
            nextToLadder = true;
            upLadder = true;
            ladderTransform = other.GetComponent<Transform>();
        }

        if (other.CompareTag("Door"))
        {
            doorScript = other.GetComponent<Door>();
            doorPos = other.GetComponent<Transform>();
            onDoor = true;
        }

        if (other.CompareTag("Finish"))
        {
            finishedLevel = true;
            controllerType = ControllerType.OnUIController;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Conduct"))
        {
            onConduct = true;
        }

        if (other.CompareTag("Ladder") && !climbingLadder)
        {
            nextToLadder = true;
        }

        if (other.CompareTag("UpLadder") && !climbingLadder)
        {
            nextToLadder = true;
        }
        
        if (other.CompareTag("Door"))
        {
            onDoor = true;
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Conduct"))
        {
            onConduct = false;
        }

        if (other.CompareTag("Ladder"))
        {
            nextToLadder = false;
        }

        if (other.CompareTag("UpLadder"))
        {
            upLadder = false;
            nextToLadder = false;
        }

        if (other.CompareTag("Door"))
        {
            onDoor = false;
        }

        
    }

    #endregion

    #region Controllers Functions

    public void DefaultController()
    {
        _animator.applyRootMotion = false;
        _rigidbody.useGravity = true;
        _collider.isTrigger = false;
        
        #region BASIC MOVEMENT

        //Determines the horizontal facing direction of the player towards the mouse

        _rigidbody.MoveRotation(Quaternion.Euler(new Vector3(0,
            90 * Mathf.Sign(targetTransform.position.x - transform.position.x))));

        //Set the horizontal keys to move player
        _rigidbody.velocity = new Vector3(horizontalInput * currentSpeed,
            _rigidbody.velocity.y, 0);

        //replicate velocity data to animator
        _animator.SetFloat("Speed",
            (FacingSign * horizontalInput * currentSpeed));
        _animator.SetFloat("VerticalVelocity", _rigidbody.velocity.y);

        //CALCULATE DISTANCE TO GROUND
        CalculateDistance(
            Vector3.Lerp(_animator.GetBoneTransform(HumanBodyBones.LeftFoot).position,
                _animator.GetBoneTransform(HumanBodyBones.RightFoot).position, 0.5f) + Vector3.down / 8, Vector3.down,
            "Ground");

        //MOVING ANIMATION
        if (horizontalInput != 0)
        {
            _animator.SetBool("IsMoving", true);
            playerState = PlayerState.IsMoving;
            currentSpeed += Time.deltaTime * acceleration;
            if (currentSpeed > normalSpeed )
            {
                currentSpeed = normalSpeed;
            }

            if (currentSpeed > crouchWalkSpeed)
            {
                if (crouch || walking)
                {
                    if (currentSpeed > crouchWalkSpeed)
                    {
                        currentSpeed -= Time.deltaTime * acceleration;
                    }
                    else
                    {
                        currentSpeed = crouchWalkSpeed;
                    }
                } 
            }
        }
        else if (horizontalInput == 0)
        {
            playerState = PlayerState.IsIdle;
            currentSpeed = 0;
            _animator.SetBool("IsMoving", false);
        }
        
        #endregion

        

        #region PLAYER ACTIONS


        //MOVEMENT SPEED
        
        if (prone)
        {
            currentSpeed = crouchWalkSpeed / 4;
        }
        
        //WALKING TOGGLE
        if (Input.GetKey(keyAssignments.walkKey.keyCode) && !crouch && !prone)
        {
            walking = true;
            playerState = PlayerState.IsWalking;
            if (currentSpeed > crouchWalkSpeed)
            {
                currentSpeed -= Time.deltaTime * acceleration;
            }

            _animator.SetFloat("Speed", FacingSign * horizontalInput * currentSpeed);
        }
        else if (!crouch && !prone)
        {
            walking = false;
            if (currentSpeed < normalSpeed)
            {
                currentSpeed += Time.deltaTime * acceleration;
            }else if (currentSpeed > normalSpeed)
            {
                currentSpeed = normalSpeed;
            }
            playerState = PlayerState.IsMoving;
        }

        //AIMING

        if (isAiming)
        {
            crosshair.enabled = true;
        }
        else
        {
            crosshair.enabled = false;
        }


        if (weaponEquipped )
        {
            if (Input.GetKey(keyAssignments.aimBlockKey.keyCode))
            {
                if (equippedWeaponItem.weaponItemClass != WeaponScriptableObject.WeaponClass.Melee)
                {
                    if (_checkGround.isGrounded && !_climber.attachedToLedge && !trapped && !bitten)
                    {
                        playerState = PlayerState.IsAiming;
                        isAiming = true;   
                    }
                    else
                    {
                        isAiming = false;
                    }
                }
                if (equippedWeaponItem.weaponItemClass == WeaponScriptableObject.WeaponClass.Melee && equippedWeaponItem.ID != 1001 && !bitten && !trapped) // 1001: knife
                {
                    if (_healthManager.currentStamina >= _healthManager.blockHitPenalty)
                    {
                        blocking = true;
                    }
                    else
                    {
                        blocking = false;
                    }
                }
                else
                {
                    blocking = false;
                }
            }
            else
            {
                isAiming = false;
            }
            
            
        }
        
        
        
        if (weaponEquipped && Input.GetKeyUp(keyAssignments.aimBlockKey.keyCode))
        {
            isAiming = false;
            blocking = false;
        }

        //JUMPING
        if (Input.GetKeyDown(keyAssignments.jumpKey.keyCode) 
            && _checkGround.isGrounded 
            && _healthManager.currentStamina >= _healthManager.jumpPenalty
            && !_healthManager.isInjured)
        {
            jump = true;
            _rigidbody.velocity = new Vector3(0, 0, 0);
        }
        else
        {
            jump = false;
        }

        //CROUCH AND PRONE

        _crouchColliderCenter = new Vector3(0, 0.68f, 0);
        _crouchColliderHeight = 1.33f;

        _proneColliderCenter = new Vector3(0, 0.18f, 0f);

        if (_checkGround.isGrounded)
        {
            if (Input.GetKeyDown(keyAssignments.crouchKey.keyCode) && !onConduct && _animator.GetFloat("Speed") <= crouchWalkSpeed)
            {
                crouch = !crouch;
                if (prone)
                {
                    prone = false;
                    currentSpeed = crouchWalkSpeed;
                }

                if (crouch)
                {
                    playerState = PlayerState.IsCrouching;
                    crouch = true;
                    currentSpeed = crouchWalkSpeed;
                    SetColliderShape(PlayerState.IsCrouching);
                }
                
                if (!crouch)
                {
                    currentSpeed = normalSpeed;
                    SetColliderShape(PlayerState.Default);
                }
            }
            
            if (Input.GetKeyDown(keyAssignments.proneKey.keyCode) && !onConduct)
            {
                prone = !prone;
                if (crouch)
                {
                    crouch = false;
                    currentSpeed = crouchWalkSpeed / 2;
                }

                if (!prone)
                {
                    currentSpeed = normalSpeed;
                }

                if (prone)
                {
                    SetColliderShape(PlayerState.IsProne);
                    
                }
            }

            //ROLLING
            if (Input.GetKeyDown(keyAssignments.crouchKey.keyCode))
            {
                if (doubleTapTime > 0 && tapCount == 1 /*Number of Taps you want Minus One*/)
                {
                    //Has double tapped
                       
                    if (_animator.GetFloat("Speed") > 0.1f 
                        && _healthManager.currentStamina >= _healthManager.rollPenalty
                        && !_healthManager.isInjured)
                    {
                        _animator.SetBool("Crouch", false);
                        roll = true;
                        SetColliderShape(PlayerState.IsRolling);
                    }

                }
                else
                {
                    doubleTapTime = 0.5f;
                    tapCount += 1;
                }
            }

            if (roll)
            {
                _animator.applyRootMotion = true;
            }
            if (doubleTapTime > 0)
            {
                doubleTapTime -= 1 * Time.deltaTime;
            }
            else
            {
                tapCount = 0;
            }
        }
        
        
        
        //INTERACTION
        if (Input.GetKeyDown(keyAssignments.useKey.keyCode) && onDoor)
        {
            transform.position = new Vector3(doorPos.position.x, transform.position.y, transform.position.z);
            if (doorPos.position.z > transform.position.z)
            {
                
                SwitchPlayLine(doorScript.insidePlayLine);
            }
            else
            {
                SwitchPlayLine(doorScript.outsidePlayLine);
            }
        }
        #endregion

        
        
        #region PUSH OBJECTS

        if (IKManager.pushingObject == true)
        {
            walking = true;
            currentSpeed = crouchWalkSpeed;
        }

        #endregion

    }

    public void OnUIController()
    {
        _animator.SetFloat("Speed",0);
        
        if (Input.GetKeyDown(KeyCode.Escape) || _inventory.showInventory == false)
        {
            _inventory.showInventory = false;
            controllerType = ControllerType.DefaultController;

        }
    }

    public void OnLadderController()
    {
        if (climbingLadder & !climbingToTop)
        {
            _rigidbody.velocity = new Vector3(0, verticalInput * climbVelocity, 0);
            
            //Cancel Climbing if press down key and player touch ground
            if (_checkGround.isGrounded && verticalInput < -0.2f && !upLadder)
            {
                climbingLadder = false;
                _rigidbody.useGravity = true;
                _animator.applyRootMotion = false;
                climbingToTop = false;
                controllerType = ControllerType.DefaultController;
            }

            if (upLadder && verticalInput > 0)
            {
                //If player is climbing and reach the upperladder when pressing up
                _animator.applyRootMotion = true;
                climbingToTop = true;
            }

            //If player is climbing and reach the upperladder when pressing down
            if (upLadder == true && verticalInput < 0)
            {
                _animator.applyRootMotion = false;
            }
        }
        else if (!climbingToTop)
        {
            controllerType = ControllerType.DefaultController;
        }
        if (!climbingToTop && !upLadder)
        {
            _animator.applyRootMotion = false;
        }

        if (climbingToTop)
        {
            
            Quaternion ladderRot = Quaternion.LookRotation(-ladderTransform.right, Vector3.up);
            transform.rotation = ladderRot;
            _rigidbody.useGravity = false;
            _animator.applyRootMotion = true;
        }
    }

    public void OnLedgeController()
    {
        playerState = PlayerState.IsOnLedge;
        currentSpeed = crouchWalkSpeed;
        _animator.applyRootMotion = true;
        _rigidbody.useGravity = false;
        _collider.isTrigger = true;
        Climbable.Orientation ledgeOrientation = _climber.climbable.orientation;
        if (ledgeOrientation == Climbable.Orientation.Right || ledgeOrientation == Climbable.Orientation.Left)
        {
            _rigidbody.velocity = Vector3.zero;
            _animator.SetFloat("Speed",0);
        }
        if (ledgeOrientation == Climbable.Orientation.Back)
        {
            _animator.SetFloat("Speed",horizontalInput * currentSpeed);
            _rigidbody.velocity = new Vector3(horizontalInput * currentSpeed,
                0, 0);
        }
    }

    public void StandByController()
    {
        _rigidbody.velocity = Vector3.zero;
        _animator.SetFloat("Speed", 0);
        
        if (weaponEquipped && Input.GetKeyUp(keyAssignments.aimBlockKey.keyCode))
        {
            isAiming = false;
            blocking = false;
        }
    }

    #endregion

    #region Animator Event Functions

    public void AnimationEvent(string message)
    {
        switch (message)
        {
            case "TransitionStart":
                controllerType = ControllerType.StandByController;
                onTransition = true;
                _animator.applyRootMotion = true;
                break;

            case "TransitionEnd":
                if (climbingLadder)
                {
                    controllerType = ControllerType.OnLadderController;
                }
                else
                {
                    climbingToTop = false;
                    hardLanded = false;
                    controllerType = ControllerType.DefaultController;
                }


                if (roll)
                {
                    roll = false;
                    SetColliderShape(PlayerState.IsRolling);
                }
                
                

                _animator.applyRootMotion = false;
                onTransition = false;
                break;
        }
    }

    public void OnTopLadderAnim(float instance) //Used for Animator events
    {
        switch (instance)
        {
            case 0: //Start
                transform.position= new Vector3(ladderTransform.position.x, transform.position.y,
                    ladderTransform.position.z);
                _rigidbody.useGravity = false;
                _rigidbody.velocity = Vector3.zero;
                _animator.applyRootMotion = true;
                _collider.enabled = false;
                break;

            case 1: //End
                
                
                _rigidbody.useGravity = true;
                _animator.applyRootMotion = false;
                climbingToTop = false;
                climbingLadder = false;
                _collider.enabled = true;
                controllerType = ControllerType.DefaultController;
                break;
        }
    }
    
    public void SetLayerWeight(int index, float value)
    {
        _animator.SetLayerWeight(index,value);
    }//Used for Animator events

    public void JumpAnim(float value)//Used for Animator events
        {
            if (value == 0)//jump start
            {
                _checkGround.isGrounded = false;
            }
            if (value == 1)//jump impulse
            { 
                _checkGround.isGrounded  = false;
                _rigidbody.AddForce(Vector3.up * Mathf.Sqrt(jumpSpeed * -1 * Physics.gravity.y), ForceMode.VelocityChange);
                _healthManager.ConsumeStamina(30);
            }
            if (value == 2)//jump end normal or before hard lading
            {
                hardLanded = false; 
                _checkGround.isGrounded  = true;
            }
            if (value == 3) //Hard Landing
            {
            hardLanded = true;
            _checkGround.isGrounded = false;
            }
        }

    public void WeaponAnimEvent(string command)
    {
        Transform leftHandTransform = _animator.GetBoneTransform(HumanBodyBones.LeftMiddleProximal).transform;
        switch (command)
        {
            case "ReloadStart":
                equippedWeaponItem.ReloadWeaponAnim("ReloadStart");
                break;
            
            case "GrabMag":
                if (equippedWeaponItem.magGameObject != null)
                {
                    equippedWeaponItem.magGameObject.transform.parent = leftHandTransform;
                    equippedWeaponItem.magGameObject.transform.position = leftHandTransform.position;
                }
                
                break;
            
            case "HideMag":

                if (equippedWeaponItem.magGameObject != null)
                {
                    equippedWeaponItem.magGameObject.SetActive(false); 
                }
                
                break;
            
            case "ShowMag":
                if (equippedWeaponItem.magGameObject != null)
                {
                    equippedWeaponItem.magGameObject.SetActive(true); 
                }
                break;
            
            case "AttachMag":
                
                if (equippedWeaponItem.magGameObject != null)
                {
                    equippedWeaponItem.magGameObject.transform.parent = equippedWeaponItem.magHolder;
                    equippedWeaponItem.magGameObject.transform.localPosition = new Vector3(0,0,0);
                    equippedWeaponItem.magGameObject.transform.localEulerAngles = new Vector3(0,0,0);
                }
                
                break;
            
            case "ReloadEnd":
                equippedWeaponItem.ReloadWeaponAnim("ReloadEnd");
                break;
            
            case "MeleeStart":
                equippedWeaponItem.MeleeAttackAnim("Start");
                break;
            
            case "DoDamage":
                _healthManager.ConsumeStamina(_healthManager.meleeAttackPenalty);
                equippedWeaponItem.MeleeAttackAnim("DoDamage");
                break;
            
            case "MeleeEnd":
                equippedWeaponItem.MeleeAttackAnim("MeleeEnd");
                break;
        }
    }

    public void RollAnim(string command)
    {
        switch (command)
        {
            case "ConsumeStamina":
                _healthManager.ConsumeStamina(_healthManager.rollPenalty);
                break;
        }
    }

    public void StompAnim(string command)
    {
        switch (command)
        {
            case "doDamage":

                if (stompTargetAgentController != null)
                {
                    stompTargetAgentController._healthManager.currentHealth -= stompDamage;
                    stompTargetAgentController._animator.SetTrigger("Hit");
                    if (stompTargetAgentController._healthManager.currentHealth <=0)
                    {
                        _playerAudio.StompHeadSound();
                        Instantiate(bloodSplatterParticle, stompTargetAgentController._animator.GetBoneTransform(HumanBodyBones.Head).position,
                            _animator.GetBoneTransform(HumanBodyBones.RightFoot).rotation);
                    }
                    else
                    {
                        _playerAudio.StompHitSound();
                    }
                    
                }
                
                break;
        }
    }
    
    #endregion

    #region Actions Functions

    private void MouseAim()
    {
        
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
       
        RaycastHit hit;
        mouseAimMask = LayerMask.GetMask("MouseTarget");
        
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, mouseAimMask.value))
        {
            targetTransform.position = hit.point;
        }
        
    }
    public void AttachOnLadder(string startPoint)
    {
        switch (startPoint)
        {
            case "UpLadder":
                ladderPosition = new Vector3(ladderTransform.position.x, transform.position.y - 0.5f,
                    ladderTransform.position.z);
                break;
            case "DownLadder":
                ladderPosition = new Vector3(ladderTransform.position.x, transform.position.y,
                    ladderTransform.position.z);
                break;
        }
        
        playerState = PlayerController.PlayerState.IsClimbingLedge;
        //set position and rotation of the Player towards the ladder
        transform.position =  ladderPosition;
        Quaternion ladderRot = Quaternion.LookRotation(-ladderTransform.right, Vector3.up);
        transform.rotation = ladderRot; 
        _rigidbody.useGravity = false;
        //_animator.applyRootMotion = true;
    }
    
    /// <summary>
    /// Changes the object Z position to determined playline
    /// </summary>
    /// <param name="playLineTarget"></param>
    public void SwitchPlayLine(PlayLine playLineTarget)
    {
        switch (playLineTarget)
        {
            case PlayLine.PlayLine0:
                currentPlayLine = playLine0;
                break;
            case PlayLine.PlayLine1:
                currentPlayLine = playLine1;
                break;
            case PlayLine.PlayLine2:
                currentPlayLine = playLine2;
                break;
            case PlayLine.PlayLine3:
                currentPlayLine = playLine3;
                break;
        }
        mouseTargetLayer.transform.position = new Vector3(transform.position.x,transform.position.y, currentPlayLine);
    }

    #endregion
    
    /// <summary>
    /// Checks if the object is falling or ascending
    /// </summary>
    private void CheckFallingState()
    {
        Vector3 currentPosition = transform.position;

        if (lastPosition.y != currentPosition.y)
        {
            if (currentPosition.y > lastPosition.y)
            {
                ascending = true;
                descending = false;
            }
            else
            {
                ascending = false;
                descending = true;
            }
        }
        else
        {
            descending = false;
            ascending = false;
        }

        lastPosition = currentPosition;
    }
    
    /// <summary>
    /// Used for calculate distance to certain object
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="direction"></param>
    /// <param name="targetTag"></param>
    private void CalculateDistance(Vector3 origin , Vector3 direction, string targetTag)
    {
        RaycastHit Hit = new RaycastHit();
        if (Physics.Raycast(origin, direction, out Hit))
        {
            if (Hit.collider.CompareTag(targetTag))
            {
                hitDistance = Hit.distance;
                Debug.DrawRay(origin, direction * hitDistance, Color.yellow);
                
                _animator.SetFloat("GroundDistance", hitDistance);
            }
        }
    }

    public void SetColliderShape(PlayerState state)
    {
        switch (state)
        {
            
            case PlayerState.IsClimbingLedge:
                _collider.direction = 1; //y
                _collider.height = 1.76f;
                _collider.center = new Vector3(0, _climber.climbable.ledgeTransform.position.y, 0);
                break;
            
            case PlayerState.IsClimbingLadder:
                _collider.direction = 1; //y
                _collider.height = 1.76f;
                _collider.center = new Vector3(0, _ladder.topPosition.position.y, 0);
                break;
            
            case PlayerState.IsRolling:

                _collider.direction = 1; //y
                _collider.height = _crouchColliderHeight;
                _collider.center = _crouchColliderCenter;
                break;
            
            case PlayerState.IsCrouching:
                _collider.direction = 1; //y
                _collider.height = _crouchColliderHeight;
                _collider.center = _crouchColliderCenter;
                break;
            
            case PlayerState.IsProne:
                _collider.direction = 2; //z
                _collider.center = _proneColliderCenter;
                break;
            
            case PlayerState.Default:
                _collider.direction = 1; //y
                _collider.height = 1.76f;
                _collider.center = new Vector3(0, 0.9f, 0);
                break;
        }
        
    }

    

}
