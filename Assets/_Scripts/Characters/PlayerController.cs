
using UnityEngine;
using UnityEngine.Serialization;

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


    private static readonly string Horizontal = "Horizontal";
    private static readonly string Vertical = "Vertical";
    [HideInInspector] public float horizontalInput, verticalInput;

    [HideInInspector] private Vector3 lastPosition;
    [Header("Movement")]
    public float currentSpeed;
    public float normalSpeed;
    public float jumpForce;
    public float crouchWalkSpeed;
    public float injuredSpeed = 1;
    public float acceleration = 4;


    [HideInInspector] public bool jump;
    private bool walking;

    [Header("Conditions")]
    public bool isAscending;
    public bool isDescending;
    public bool canMove;
    public bool hardLanded;
    public bool isCrouched;
    public bool isProne;
    public bool isRolling;
    private float rollTime = 0.1f;
    private float rollTimer;
    public bool isBlocking;
    public bool isTrapped;
    public bool beingBitten;
    public bool alreadyCatched;
    public bool onConduct;
    public bool onTransition;
    public bool isBandaging;
    public bool isDrinking;
    public bool isEating;
    public bool grabbingItem;
    public bool playerIsBusy;

    [Header("Collider Shapes")]
    private float _crouchColliderHeight;
    private Vector3 _crouchColliderCenter;
    private Vector3 _proneColliderCenter;

    [Header("Attributes")]
    public float struggleForce = 3;
    public float throwForce = 5;

    #region Unarmed Attacks variables

    [HideInInspector]
    public bool canStomp = false;

    public int stompDamage = 10;
    public GameObject bloodSplatterParticle;

    #endregion

    private Vector2 gravity;
    public float hitDistance;

    #region DoubleTappingCheck
    private bool doubleTap;
    private float doubleTapTime = 0.5f;
    private int tapCount = 0;
    #endregion


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

    #region Ladders & Ledge Climb variables

    private Climber _climber;
    private Ladder _ladder;
    private Vector3 ladderPosition;
    private Transform ladderTransform;
    public float climbVelocity = 2f;
    public float transitionSpeed;
    public bool nextToLadder;
    public bool climbingLadder;
    public bool upLadder;
    public bool climbingToTop;

    #endregion

    #region Weapon Handling Variables

    public Transform crosshairTransform; //Transform reference of the player's crosshair Gameobject
    private SpriteRenderer crosshairSprtRenderer;
    public Transform _weaponHolder; //Transform reference to place Child Gameobjects
    
    public bool isAiming;
    public bool isAttacking;
    public bool isReloadingWeapon;


    #endregion

    #region Components Variables

    [HideInInspector] public GameManager gameManager;
    [HideInInspector] public HealthManager _healthManager;
    [HideInInspector] public ParticleSystem _particleSystem;
    [HideInInspector] public Rigidbody _rigidbody;
    [HideInInspector] public Animator _animator;
    [HideInInspector] public CapsuleCollider _collider;
    [HideInInspector] public CheckGround _checkGround;
    private Camera mainCamera;
    private PlayerAudio _playerAudio;
    private SoundSensor _soundSensor;
    [HideInInspector] public AgentController stompTargetAgentController;
    [HideInInspector] public StompDetector _stompDetector;
    private IKAimer _ikAimer;
    #endregion

    #region PlayLine variables
    [Header("Playline and Mouse Aim")]
    public float currentPlayLine = 0;
    private float currentTransitionSpeed = 0f;
    public float transitionAcceleration = 2f;

    public LayerMask mouseAimMask;
    public Transform mouseTargetLayerTransform;

    #endregion

    #region DoorsInteraction
    [Header("Doors Interaction")]
    public bool onDoor;
    private LayerMask doorLayer;
    public Door doorScript;

    #endregion

    #region Inventory Variables

    [HideInInspector] public Inventory _inventory;

    #endregion

    #region Animator references
    private static readonly int AnimatorSpeed = Animator.StringToHash("Speed");
    private Transform leftFoot, rightFoot;
    #endregion


    private void Awake()
    {
        
        mainCamera = Camera.main;
        gameManager = GameManager.Instance;
        keyAssignments = GameManager.Instance._keyAssignments;
        mouseTargetLayerTransform = GameObject.Find("MouseTargetLayer").transform;
        doorLayer = LayerMask.NameToLayer("Door");
        #region GetComponents

        keyAssignments = gameManager.GetComponent<KeyAssignments>();
        _healthManager = GetComponent<HealthManager>();
        _rigidbody = GetComponent<Rigidbody>();
        
        _inventory = GetComponent<Inventory>();
        _checkGround = GetComponentInChildren<CheckGround>();
        _playerAudio = GetComponent<PlayerAudio>();
        _climber = GetComponent<Climber>();
        _collider = GetComponent<CapsuleCollider>();
        crosshairSprtRenderer = crosshairTransform.GetComponent<SpriteRenderer>();
        _soundSensor = GetComponent<SoundSensor>();
        _animator = GetComponent<Animator>();
        #endregion

        _stompDetector = gameObject.transform.Find("StompDetector").GetComponent<StompDetector>();
        _ikAimer = GetComponent<IKAimer>();
    }

    void Start()
    {
        gravity = Physics2D.gravity;

        currentSpeed = normalSpeed;

        if (gameManager)
        {
            transform.position = gameManager.startingPosition.position;
            SwitchPlayLine(gameManager.startingPlayline);
        }
        rollTimer = rollTime;

    }

   
    void Update()
    {
        //   transform.eulerAngles = new Vector3(transform.eulerAngles.x,transform.eulerAngles.y,0); 
        #region axis inputs

        horizontalInput = Input.GetAxis(Horizontal);
        verticalInput = Input.GetAxis(Vertical);

        #endregion

        #region Animator parameters

        leftFoot = _animator.GetBoneTransform(HumanBodyBones.LeftFoot);
        rightFoot = _animator.GetBoneTransform(HumanBodyBones.RightFoot);
        AnimatorParameters();

        #endregion

        CheckFallingState();


       // CheckWeaponEquipped();

        #region PlayLine placement

        //Put the mouselayer on the player Z playline
        mouseTargetLayerTransform.transform.position =
            new Vector3(mouseTargetLayerTransform.position.x, transform.position.y, currentPlayLine);
        Vector3 newPosition = new Vector3(transform.position.x, transform.position.y, currentPlayLine);
        if (transform.position.z != currentPlayLine && !climbingLadder)
        {
            currentTransitionSpeed += Time.deltaTime * transitionAcceleration;
            if (currentTransitionSpeed >= transitionSpeed)
            {
                currentTransitionSpeed = transitionSpeed;
            }
            transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * transitionSpeed);

        }
        else
        {
            transform.position = newPosition;
            currentTransitionSpeed = 0f;
        }

        #endregion

        #region Controller Switch

        bool[] cantMoveConditions ={
            _healthManager.IsDead,
            _inventory.showInventory,
            onTransition,
            climbingLadder,
            _climber.attachedToLedge,
            _climber.climbingLedge,
            climbingToTop,
            isRolling,
            isBlocking,
            isTrapped,
            beingBitten,
            hardLanded,
            isBlocking
        };

     
        switch (controllerType)
        {
            case ControllerType.DefaultController:
                DefaultController();
                SetCrosshairPosition();
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

        canMove = true;

        for (int i = 0; i < cantMoveConditions.Length; i++)
        {
            if (cantMoveConditions[i] == true)
            {
                canMove = false;
                break;
            }
 
        }

        if (!canMove)
        {
            if (climbingLadder)
            {
                controllerType = ControllerType.OnLadderController;
            }
            else if (_climber.attachedToLedge || _climber.climbingLedge)
            {
                controllerType = ControllerType.OnLedgeController;
            }
            else
            {
                controllerType = ControllerType.StandByController;
            }
        }

        if (canMove)
        {
            controllerType = ControllerType.DefaultController;
        }

        PlayerRoll();
               
        #endregion

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

        #region UI Management

        if (_inventory.showInventory == true || GameManager.gamePaused)
        {
            playerState = PlayerState.OnUI;
            controllerType = ControllerType.OnUIController;
        }

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

        if (other.gameObject.layer == doorLayer)
        {
            if (other.TryGetComponent(out Door door))
            {
                doorScript = door;
                onDoor = true;
            }
        }

        if (other.CompareTag("Conduct"))
        {
            onConduct = true;
        }

        if (other.CompareTag("Finish"))
        {
            gameManager.finishedLevel = true;
            controllerType = ControllerType.OnUIController;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        /*if (other.CompareTag("Elevator"))
        {
            transform.position = new Vector3(transform.position.x,other.transform.position.y,transform.position.z);
        }*/
         if (other.gameObject.layer == doorLayer)
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

        if (other.gameObject.layer == doorLayer)
        {
            onDoor = false;
        }
    }

    #endregion

    #region Controllers Functions
    
    
    /// <summary>
    /// 
    /// </summary>
    private void WeaponSelectionController()
    {
        if (Input.GetKeyDown(keyAssignments.primaryKey.keyCode) && _inventory.equippedPrimaryWeapon)
        {
            if (_inventory.selectedWeapon != Inventory.SelectedWeapon.Primary)
                {
                    _inventory.DrawWeapon(_inventory.equippedPrimaryWeapon, true);
                    
                }
                else
                {
                    _inventory.DrawWeapon(_inventory.equippedPrimaryWeapon, false);
                }
        }

        if (Input.GetKeyDown(keyAssignments.secondaryKey.keyCode) && _inventory.equippedSecondaryWeapon)
        {
            if (_inventory.selectedWeapon != Inventory.SelectedWeapon.Secondary)
            {

                _inventory.DrawWeapon(_inventory.equippedSecondaryWeapon, true);
            }
            else
            {
                _inventory.DrawWeapon(_inventory.equippedSecondaryWeapon, false);
            }
        }

        if (Input.GetKeyDown(keyAssignments.meleeKey.keyCode) && _inventory.equippedMeleeWeapon)
        {
            if (_inventory.selectedWeapon != Inventory.SelectedWeapon.Melee)
            {
                _inventory.DrawWeapon(_inventory.equippedMeleeWeapon, true);
            }
            else
            {
                _inventory.DrawWeapon(_inventory.equippedMeleeWeapon, false);
            }
        }

        if (Input.GetKeyDown(keyAssignments.throwableKey.keyCode) && _inventory.equippedThrowableWeapon)
        {
            if (_inventory.selectedWeapon != Inventory.SelectedWeapon.Throwable)
            {
                _inventory.DrawWeapon(_inventory.equippedThrowableWeapon, true);
            }
            else
            {
                _inventory.DrawWeapon(_inventory.equippedThrowableWeapon, false);
            }
        }
    }

    public void AimBlock()
    {
        if (isAiming)
        {
            crosshairSprtRenderer.enabled = true;
        }
        else
        {
            crosshairSprtRenderer.enabled = false;
        }

        if (_inventory.drawnWeaponItem)
        {
            if (_inventory.drawnWeaponItem.weaponDrawn)
            {
                //Aiming toggle
                if (Input.GetKey(keyAssignments.aimBlockKey.keyCode) && !PlayerBusy())
                {
                    if (_inventory.drawnWeaponItem && _inventory.drawnWeaponItem.weaponClass != WeaponScriptableObject.WeaponClass.Melee)
                    {
                        if (_checkGround.isGrounded && !_climber.attachedToLedge && !isTrapped && !beingBitten)
                        {
                            playerState = PlayerState.IsAiming;
                            isAiming = true;
                            if (_inventory.drawnWeaponItem.weaponClass == WeaponScriptableObject.WeaponClass.Throwable)
                            {
                                ShowThrowTrayectory(crosshairTransform,throwForce);
                            }
                        }
                        else
                        {
                            isAiming = false;
                        }
                    }

                    if (_inventory.drawnWeaponItem.weaponClass == WeaponScriptableObject.WeaponClass.Melee &&
                        _inventory.drawnWeaponItem.ID != 1001 && !beingBitten && !isTrapped) // 1001: knife
                    {
                        if (_healthManager.currentStamina >= _healthManager.blockHitPenalty)
                        {
                            isBlocking = true;
                        }
                        else
                        {
                            isBlocking = false;
                        }
                    }
                    else
                    {
                        isBlocking = false;
                    }
                }
                else
                {
                    isAiming = false;
                }
            }
        }

        if (Input.GetKeyUp(keyAssignments.aimBlockKey.keyCode))
        {
            isAiming = false;
            isBlocking = false;
        }

    }

    /// <summary>
    ///  Determines the horizontal facing direction of the player towards the mouse
    /// </summary>

    public int trayectoryPoints = 10;
    public float timePoint = 0.1f;
    private void ShowThrowTrayectory(Transform targetTransform, float throwForce)
    {
        Transform throwable = _inventory.drawnWeaponItem.transform;

        Vector2 direction = targetTransform.position - throwable.position;
        float tiempo = timePoint;

        Vector2 initialVelocity = direction * (throwForce / direction.magnitude);
        Vector2 impulse = initialVelocity * _rigidbody.mass;

        for (int i = 0; i < trayectoryPoints; i++)
        {
            Vector2 position = CalculatePosition(tiempo, impulse);
            tiempo += timePoint;
            Debug.DrawLine(throwable.position, position, Color.red, timePoint);
        }
    }
    private Vector2 CalculatePosition(float time, Vector2 initialVelocity)
    {
        Vector2 position = _inventory.drawnWeaponItem.transform.position;
        return position + initialVelocity * time + gravity * (0.5f * time * time);
    }
    public void DefaultController()
    {
        if (!isAiming && !climbingLadder &&
                    !isAttacking && !onTransition)
        {
            WeaponSelectionController();//Allow weapon change
        }

        _animator.applyRootMotion = false;
        _rigidbody.useGravity = true;
        _collider.isTrigger = false;

        #region BASIC MOVEMENT

        SetFacingDirection();


        //Set the horizontal keys to move player
        _rigidbody.velocity = new Vector3(horizontalInput * currentSpeed,
            _rigidbody.velocity.y, 0);

        //replicate velocity data to animator
        _animator.SetFloat(AnimatorSpeed,
            (Mathf.Sign(FacingSign) * horizontalInput * currentSpeed));
        _animator.SetFloat("VerticalVelocity", _rigidbody.velocity.y);



        //MOVING ANIMATION
        if (horizontalInput != 0)
        {
            _animator.SetBool("IsMoving", true);
            playerState = PlayerState.IsMoving;

            //Gradual moving acceleration
            currentSpeed += Time.deltaTime * acceleration;
            if (currentSpeed > normalSpeed)
            {
                currentSpeed = normalSpeed;
            }

            if (currentSpeed > crouchWalkSpeed)
            {
                if (isCrouched || walking)
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

        //PRONE MOVEMENT SPEED
        if (isProne)
        {
            currentSpeed = crouchWalkSpeed / 4;
        }

        #endregion

        #region PLAYER ACTIONS

        //WALKING TOGGLE
        if (Input.GetKey(keyAssignments.walkKey.keyCode) && !isCrouched && !isProne)
        {
            walking = true;
            playerState = PlayerState.IsWalking;
            if (currentSpeed > crouchWalkSpeed)
            {
                currentSpeed -= Time.deltaTime * acceleration;
            }

            _animator.SetFloat(AnimatorSpeed, Mathf.Sign(FacingSign) * horizontalInput * currentSpeed);
        }
        else if (!isCrouched && !isProne)
        {
            walking = false;
            if (currentSpeed < normalSpeed)
            {
                currentSpeed += Time.deltaTime * acceleration;
            }
            else if (currentSpeed > normalSpeed)
            {
                currentSpeed = normalSpeed;
            }

            playerState = PlayerState.IsMoving;
        }

        //AIMING AND BLOCKING

        AimBlock();

        

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
            if (Input.GetKeyDown(keyAssignments.crouchKey.keyCode) && !onConduct &&
                _animator.GetFloat(AnimatorSpeed) <= crouchWalkSpeed)
            {
                isCrouched = !isCrouched;
                if (isProne)
                {
                    isProne = false;
                    currentSpeed = crouchWalkSpeed;
                }

                if (isCrouched)
                {
                    playerState = PlayerState.IsCrouching;
                    isCrouched = true;
                    currentSpeed = crouchWalkSpeed;
                    SetColliderShape(PlayerState.IsCrouching);
                }

                if (!isCrouched)
                {
                    currentSpeed = normalSpeed;
                    SetColliderShape(PlayerState.Default);
                }
            }

            if (Input.GetKeyDown(keyAssignments.proneKey.keyCode) && !onConduct)
            {
                isProne = !isProne;
                if (isCrouched)
                {
                    isCrouched = false;
                    currentSpeed = crouchWalkSpeed / 2;
                }

                if (!isProne)
                {
                    currentSpeed = normalSpeed;
                }

                if (isProne)
                {
                    SetColliderShape(PlayerState.IsProne);

                }
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

        //ATTACKS
        
        canStomp = _stompDetector.canStomp;
        
        if (!beingBitten && Input.GetKeyDown(keyAssignments.attackKey.keyCode))
        {
            //STOMP
            if (canStomp)
            {
                _animator.SetTrigger("Stomp");
            }
        }

        if (!canStomp && !beingBitten && Input.GetKey(keyAssignments.attackKey.keyCode))
        {
            //FIRE WEAPON
            if (_inventory.drawnWeaponItem)
            {
                if (_inventory.drawnWeaponItem.meleeAttackTimer <= 0)
                {
                    if (_inventory.drawnWeaponItem.weaponClass == WeaponScriptableObject.WeaponClass.Primary ||
                        _inventory.drawnWeaponItem.weaponClass == WeaponScriptableObject.WeaponClass.Secondary)
                    {
                        if (isAiming)
                        {
                            _inventory.drawnWeaponItem.FireWeapon();    
                        }
                        
                    }

                    //MELEE ATTACK
                    if (_inventory.drawnWeaponItem.weaponClass == WeaponScriptableObject.WeaponClass.Melee)
                    {
                        switch (_inventory.drawnWeaponItem.ID)
                        {
                            case 1006: // Fire Axe
                                _animator.SetBool("AxeAttack",true);
                                break;
                            case 1002: //Baseball Bat
                                _animator.SetBool("BatAttack",true);
                                break;
                            case 1001: //Knife
                                _animator.SetBool("KnifeAttack",true);
                                break;

                        }
                        _inventory.drawnWeaponItem.attacking = true;
                    }
                    //THROW OBJECT
                    if (_inventory.drawnWeaponItem.weaponClass == WeaponScriptableObject.WeaponClass.Throwable)
                    {
                        if (isAiming)
                        {
                            _animator.SetTrigger("Throw");
                        }
                        
                    }
                    _inventory.drawnWeaponItem.meleeAttackTimer = _inventory.drawnWeaponItem.fireRate;
                }
            }
        }
        
        
        

        //INTERACTION
        if (onDoor && Input.GetKeyDown(keyAssignments.useKey.keyCode))
        {
            if (!_inventory.onItem)
            {
                if (!doorScript.locked && doorScript.doorOrientation == Door.DoorOrientation.Back)
                {
                    Debug.Log("doorScript.doorPos.z: " + doorScript.doorPos.z);
                    Debug.Log("player transform.position.z: " + transform.position.z);
                    if (doorScript.doorTransform.position.z > transform.position.z)
                    {
                        SwitchPlayLine(doorScript.insidePlayLine);
                        Debug.Log("Switching to insidePlayLine");
                    }
                    else
                    {
                        SwitchPlayLine(doorScript.outsidePlayLine);
                        Debug.Log("Switching to outsidePlayLine");
                    }

                }
                else
                {
                    if (doorScript.transform.TryGetComponent(out ElevatorDoors elevatorDoors))
                    {
                        Elevator elevator = elevatorDoors.GetComponentInParent<Elevator>();
                        if (!elevator.playerInside && elevator.currentFloor != elevatorDoors.floor)
                        {
                            elevator.GoToFloor(elevatorDoors.floor);
                        }
                    }
                }
            }
        }

        #endregion

        //WEAPONS
        if (Input.GetKeyDown(keyAssignments.reloadKey.keyCode))
        {
            if (_inventory.drawnWeaponItem && !isReloadingWeapon && _inventory.drawnWeaponItem.itemClass == Item.ItemClass.Weapon)
            {
                if (_inventory.drawnWeaponItem.bulletsInMag < _inventory.drawnWeaponItem.magazineCap &&
                    _inventory.CheckItemsLeft(_inventory.drawnWeaponItem.bulletID) > 0)
                {
                    isReloadingWeapon = true;
                }
                else
                {
                    Debug.Log("No Ammo found for " + _inventory.drawnWeaponItem.itemName);
                    isReloadingWeapon = false;
                }
            }
        }

        if (Input.GetKey(keyAssignments.attackKey.keyCode) && !canStomp)
        {
            
            
        }
        
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

        _animator.SetFloat(AnimatorSpeed, 0);

        if (Input.GetKeyDown(KeyCode.Escape) || _inventory.showInventory == false)
        {
            _inventory.showInventory = false;
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
            _animator.SetFloat(AnimatorSpeed, 0);
        }

        if (ledgeOrientation == Climbable.Orientation.Back)
        {
            _animator.SetFloat(AnimatorSpeed, horizontalInput * currentSpeed);
            _rigidbody.velocity = new Vector3(horizontalInput * currentSpeed,
                0, 0);
        }
    }

    public void StandByController()
    {
        _rigidbody.velocity = Vector3.zero;
        _animator.SetFloat(AnimatorSpeed, 0);

        if (Input.GetKeyUp(keyAssignments.aimBlockKey.keyCode))
        {
            isAiming = false;
            isBlocking = false;
        }
    }

    #endregion

    public void AnimatorParameters()
    {

        _animator.SetFloat("VerticalInput", verticalInput);
        _animator.SetBool("ClimbingLadder", climbingLadder);
        _animator.SetBool("UpLadder", upLadder);
        _animator.SetBool("ClimbingToTop", climbingToTop);
        _animator.SetBool("Aim", isAiming);
        _animator.SetBool("Jump", jump);
        _animator.SetBool("Walking", walking);
        _animator.SetBool("Crouch", isCrouched);
        _animator.SetBool("Prone", isProne);
        _animator.SetBool("Roll", isRolling);
        _animator.SetBool("IsDead", _healthManager.IsDead);
        _animator.SetBool("Blocking", isBlocking);
        _animator.SetBool("Trapped", isTrapped);
        _animator.SetBool("Bitten", beingBitten);
        _animator.SetBool("Bandage", isBandaging);
        _animator.SetBool("Drink", isDrinking);
        _animator.SetBool("Eat", isEating);
        _animator.SetBool("GrabItem", grabbingItem);
        _animator.SetBool("Reloading", isReloadingWeapon);
        

        _animator.SetFloat("GroundDistance", CalculateDistance(
            Vector3.Lerp(leftFoot.position, rightFoot.position, 0.5f) + Vector3.down / 8, Vector3.down,
            "Ground"));
    }

    #region Animator Event Functions

    public void AnimationEvent(string animatorMessage)
    {
        switch (animatorMessage)
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
                }


                if (isRolling)
                {
                    isRolling = false;
                    SetColliderShape(PlayerState.Default);
                }



                _animator.applyRootMotion = false;
                onTransition = false;
                break;

            case "BandageEnd":
                isBandaging = false;
                break;

            case "DrinkEnd":
                isDrinking = false;
                break;

            case "EatingEnd":
                isEating = false;
                break;

            case "GrabItemEnd":
                grabbingItem = false;
                break;

            default:
                Debug.LogWarning("Invalid Animator Event Message: " + animatorMessage);
                break;
        }
    }

    public void AnimationSound(string animatorMessage)
    {
        switch (animatorMessage)
        {
            case "DrawWeaponSound":

                if (_inventory.drawnWeaponItem)
                {
                    _inventory.drawnWeaponItem._weaponSound.DrawWeaponSound();
                }
                
                break;
            
            case "MeleeAttackSound":
                
                if (_inventory.drawnWeaponItem)
                {
                    _inventory.drawnWeaponItem._weaponSound.MeleeAttackSound();
                }
                
                break;
            
            case "ExplosiveSound":
                if (_inventory.drawnWeaponItem)
                {
                    _inventory.drawnWeaponItem._weaponSound.ExplosiveSound();
                }
                break;
            
            
            case "MagOut":
            case "MagIn":
            case "ReloadEnd":

                if (_inventory.drawnWeaponItem)
                {
                    _inventory.drawnWeaponItem._weaponSound.ReloadSound(animatorMessage);
                }
                
                break;
                
            
        }
        
    }
    public void OnTopLadderAnim(float instance) //Used for Animator events
    {
        switch (instance)
        {
            case 0: //Start
                transform.position = new Vector3(ladderTransform.position.x, transform.position.y,
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
                break;
        }
    }

    public void SetLayerWeight(int index, float value)
    {
        _animator.SetLayerWeight(index, value);
    } //Used for Animator events

    private bool PlayerBusy()
    {
        if (isBandaging || isDrinking || isEating || grabbingItem)
        {
            playerIsBusy = true;
        }
        else if (!isBandaging && !isDrinking && !isEating && !grabbingItem)
        {
            playerIsBusy = false;

        }

        return playerIsBusy;
    }

   /* private void CheckWeaponEquipped()
    {
        
        if (_WeaponHolder.childCount > 0)
        {
            weaponDrawn = true;
            if (!drawnWeaponItem)
            {
                drawnWeaponItem = _WeaponHolder.GetComponentInChildren<Item>(); //TODO: CACHEAR
            }
            else
            {
                attacking = drawnWeaponItem.attacking;
            }
        }
        else
        {
            weaponDrawn = false;
            attacking = false;
            drawnWeaponItem = null;
        }
    }*/

    public void JumpAnim(float value) //Used for Animator events
    {
        if (value == 0) //jump start
        {
            _checkGround.isGrounded = false;
        }

        if (value == 1) //jump impulse
        {
            _checkGround.isGrounded = false;
            _rigidbody.AddForce(Vector3.up * Mathf.Sqrt(jumpForce * -1 * Physics.gravity.y), ForceMode.VelocityChange);
            _healthManager.ConsumeStamina(30);
        }

        if (value == 2) //jump end normal or before hard lading
        {
            hardLanded = false;
            _checkGround.isGrounded = true;
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
                _inventory.drawnWeaponItem.ReloadWeaponAnim("ReloadStart");
                break;

            case "GrabMag":
                if (_inventory.drawnWeaponItem.magGameObject != null)
                {
                    
                    _inventory.drawnWeaponItem.magGameObject.transform.parent = leftHandTransform;
                    _inventory.drawnWeaponItem.magGameObject.transform.position = leftHandTransform.position;
                }

                break;

            case "HideMag":

                if (_inventory.drawnWeaponItem.magGameObject != null)
                {
                    _inventory.drawnWeaponItem.magGameObject.SetActive(false);
                }

                break;

            case "ShowMag":
                if (_inventory.drawnWeaponItem.magGameObject != null)
                {
                    _inventory.drawnWeaponItem.magGameObject.SetActive(true);
                }

                break;

            case "AttachMag":

                if (_inventory.drawnWeaponItem.magGameObject != null)
                {
                    _inventory.drawnWeaponItem.magGameObject.transform.parent = _inventory.drawnWeaponItem.magHolder;
                    _inventory.drawnWeaponItem.magGameObject.transform.localPosition = new Vector3(0, 0, 0);
                    _inventory.drawnWeaponItem.magGameObject.transform.localEulerAngles = new Vector3(0, 0, 0);
                }

                break;

            case "ReloadEnd":
                _inventory.drawnWeaponItem.ReloadWeaponAnim("ReloadEnd");
                break;

            case "MeleeStart":
                _inventory.drawnWeaponItem.MeleeAttackAnim("Start");
                break;

            case "DoDamage":
                _healthManager.ConsumeStamina(_healthManager.meleeAttackPenalty);
                _inventory.drawnWeaponItem.MeleeAttackAnim("DoDamage");
                break;

            case "MeleeEnd":
                _inventory.drawnWeaponItem.MeleeAttackAnim("MeleeEnd");
                break;

            case "Throw":
                Throwable throwable = _inventory.drawnWeaponItem.itemModelGO.GetComponent<Throwable>();
                throwable.ThrowObject(crosshairTransform.position - _weaponHolder.transform.position,throwForce,_inventory);
                _animator.SetBool("ThrowableEquip", false);
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

                if (_stompDetector.agentController)
                {
                    AgentController agent = _stompDetector.agentController;
                    agent._healthManager.currentHealth -= stompDamage;
                    agent._animator.SetTrigger("Hit");
                    if (agent._healthManager.currentHealth <= 0)
                    {
                        _playerAudio.StompHeadSound();
                        Instantiate(bloodSplatterParticle,
                           agent._animator.GetBoneTransform(HumanBodyBones.Head).position,
                            this._animator.GetBoneTransform(HumanBodyBones.RightFoot).rotation);
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

    private void SetFacingDirection()
    {
        Vector3 direction = crosshairTransform.position - transform.position;
        direction.y = 0; // Para mantener la rotación solo en el eje Y

        // Calcular el ángulo de rotación
        float angle = Vector3.SignedAngle(Vector3.forward, direction, Vector3.up);

        // Limitar la rotación entre -90 y 90 grados
        angle = Mathf.Clamp(angle, -90f, 90f);

        // Aplicar la rotación al objeto
        transform.rotation = Quaternion.Euler(0, angle, 0);
    }
    private void SetCrosshairPosition()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;
        mouseAimMask = LayerMask.GetMask("MouseTarget");

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, mouseAimMask.value))
        {
            crosshairTransform.position = new Vector3(hit.point.x, hit.point.y, currentPlayLine);
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
        transform.position = ladderPosition;
        Quaternion ladderRot = Quaternion.LookRotation(-ladderTransform.right, Vector3.up);
        transform.rotation = ladderRot;
        _rigidbody.useGravity = false;
        //_animator.applyRootMotion = true;
    }

    /// <summary>
    /// Changes the object Z position to determined playline
    /// </summary>
    /// <param name="playLineTarget"></param>
    public void SwitchPlayLine(float newPlayLine)
    {
        //Set the new playline
        currentPlayLine = Mathf.RoundToInt(newPlayLine);

        //Center the player on the door transform
        if (doorScript != null)
        {
            transform.position = new Vector3(doorScript.doorPos.x, transform.position.y, transform.position.z);
        }

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
                isAscending = true;
                isDescending = false;
            }
            else
            {
                isAscending = false;
                isDescending = true;
            }
        }
        else
        {
            isDescending = false;
            isAscending = false;
        }

        lastPosition = currentPosition;
    }

    /// <summary>
    /// Used for calculate distance to certain object
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="direction"></param>
    /// <param name="targetTag"></param>
    private float CalculateDistance(Vector3 origin, Vector3 direction, string targetTag)
    {
        RaycastHit Hit = new RaycastHit();
        if (Physics.Raycast(origin, direction, out Hit))
        {
            if (Hit.collider.CompareTag(targetTag))
            {

                hitDistance = (origin - Hit.point).sqrMagnitude;
                Debug.DrawRay(origin, direction * hitDistance, Color.yellow);
            }
        }

        return hitDistance;
    }

    /// <summary>
    /// Shapes the collider depending the state of the player
    /// </summary>
    /// <param name="state"></param>
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
    public void PlayerRoll()
    {
        if (controllerType == ControllerType.DefaultController)
        {
            if (Input.GetKeyDown(keyAssignments.crouchKey.keyCode))
            {
                if (doubleTapTime > 0 && tapCount == 1 /*Number of Taps you want Minus One*/)
                {
                    //Has double tapped

                    if (_animator.GetFloat(AnimatorSpeed) > 0.1f
                        && _healthManager.currentStamina >= _healthManager.rollPenalty
                        && !_healthManager.isInjured)
                    {
                        _animator.SetBool("Crouch", false);
                        isRolling = true;
                        SetColliderShape(PlayerState.IsRolling);
                    }

                }
                else
                {
                    doubleTapTime = 0.5f;
                    tapCount += 1;
                }
            }

            if (isRolling)
            {
                _animator.applyRootMotion = true;
            }
        }

        if (isRolling)
        {
            rollTimer -= Time.deltaTime;
            if (rollTimer <= 0)
            {
                isRolling = false;
                rollTimer = rollTime;
            }
        }
    }
}