using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class WeaponItem : MonoBehaviour
{
    [Header("Weapon ID")]
   [HideInInspector]public WeaponScriptableObject.WeaponClass weaponItemClass;

    public enum WeaponLocation
    {
        World,
        Container,
        Player,
        Inventory,
        Throwed,
    }

    public WeaponLocation weaponLocation;

    private GameManager _gameManager;
    
    [HideInInspector]public Transform holderTarget;
    
    public WeaponScriptableObject weaponScriptableObject;

    [HideInInspector]public int ID;
    [HideInInspector]public string weaponName;
    [HideInInspector]public string description;
    [HideInInspector]public int bulletID;
    
    [Header("Weapon Attributes")]  
    public int damage;
    public float fireRate;
    public int magazineCap;
    public int bulletsInMag;
    public float recoilDuration;
    public float recoilMaxRotation;
    public float shotgunMaxFireAngle;
    public float shotgunMinFireAngle;
    public int bulletsPerShot;

    [Header("Weapon Transforms")] 
    public Transform flashLightTransform;
    public Transform gunMuzzleTransform;
    public Transform handguardTransform;
    public Transform gripTransform;
    public Collider muzzleCollider;
    public Light flashLight;
    public Transform magHolder;
    [HideInInspector]public GameObject magGameObject;

    [Header("Weapon Render")] 
    public Sprite weaponIcon;
    public MeshRenderer meshRenderer;
    public Outline modelOutline;

    [Header("Weapon Effects")] 
    public GameObject bulletImpactPrefab;
    public GameObject enemyImpactPrefab;
    public GameObject muzzleFlashPrefab;

   

    public bool weaponPicked;
    public bool weaponEquipped;
    public int totalBullets;
    public bool reloadingWeapon;
    public bool attacking;
    public int attackNumber;
    public float meleeAttackTimer;
    public bool firing;
    private float lastfired;
    public bool aiming;
    public bool drawingWeapon;
    public float meleeAttackDistance;
    private int meleeAttackNumber;
    public LayerMask enemyLayer;

    #region Components

    public WeaponSound _weaponSound;
    private WeaponSound playerWeaponSound;

    private Transform playerTransform;
    private Animator playerAnimator;
    private HealthManager playerHealthManager;
    private PlayerController playerController;
    private Inventory playerInventory;
    private IKManager playerIKManager;
    public Throwable _throwable;

    #endregion


    public int quantity = 1;

    public GameObject titleTextGameObject;
    [SerializeField] private float textFontSize = 3;
    [HideInInspector] public BoxCollider pickupCollider;

    public float rotationSpeed = 2f;
    public TextMeshPro titleTextMesh;

    private Vector3 originalScale;
    private Quaternion originalRot;
    public bool throwableActive;

    private Vector3 playerHandHolderPos;
    private Quaternion playerHandHolderRot;

    public float xOffset, yOffset, zOffset;
    
    
    private void OnValidate()
    {
        if (weaponScriptableObject != null)
        {
            titleTextMesh.text = weaponScriptableObject.weaponName;
        }
        else
        {
            titleTextMesh.text = "No Scriptable Object";
        }

        titleTextMesh.fontSize = textFontSize;
    }

    private void Awake()
    {
        originalScale = this.transform.localScale;
        originalRot = Quaternion.identity;

        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        playerTransform = GameObject.Find("Player").GetComponent<Transform>();
        playerAnimator = playerTransform.GetComponent<Animator>();
        playerHealthManager = playerTransform.GetComponent<HealthManager>();
        playerInventory = playerTransform.GetComponent<Inventory>();
        playerController = playerTransform.GetComponent<PlayerController>();
        pickupCollider = GetComponent<BoxCollider>();

        titleTextMesh = titleTextGameObject.GetComponent<TextMeshPro>();
        GetWeaponScriptableObject(weaponScriptableObject);
        _weaponSound = GetComponent<WeaponSound>();
        titleTextMesh.text = weaponName;
        titleTextMesh.enabled = false;
        modelOutline.enabled = false;

        if (magHolder != null)
        {
            if (magHolder.childCount >0)
            {
                magGameObject = magHolder.GetChild(0).gameObject;
            }
        }
        
        switch (weaponItemClass)
        {
            case WeaponScriptableObject.WeaponClass.Primary:
                holderTarget = _gameManager.uiManager.primaryEquipSlot.Find("WeaponHolder");
                break;
            case WeaponScriptableObject.WeaponClass.Secondary:
                holderTarget = _gameManager.uiManager.secondaryEquipSlot.Find("WeaponHolder");
                break;
            case WeaponScriptableObject.WeaponClass.Melee:
                holderTarget = _gameManager.uiManager.meleeEquipSlot.Find("WeaponHolder");
                break;
            case WeaponScriptableObject.WeaponClass.Throwable:
                holderTarget = _gameManager.uiManager.throwableEquipSlot.Find("WeaponHolder");
                _throwable = GetComponent<Throwable>();
                break;
        }

        switch (weaponLocation)
        {
            case WeaponLocation.World:
                gameObject.SetActive(true);
                break;
        }
    }

    private void OnEnable()
    {
        switch (weaponLocation)
        {
            case WeaponLocation.World:

                weaponEquipped = false;
                pickupCollider.enabled = true;

                if (weaponItemClass != WeaponScriptableObject.WeaponClass.Melee && weaponItemClass != WeaponScriptableObject.WeaponClass.Throwable)
                {
                    muzzleCollider.enabled = false;
                }
                meshRenderer.enabled = true;
                transform.localScale = originalScale;
                gripTransform.position = this.transform.position;
                
                gripTransform.localPosition = Vector3.zero;
                gripTransform.localRotation = Quaternion.Euler(0,transform.rotation.y,0);
                if (weaponItemClass == WeaponScriptableObject.WeaponClass.Throwable)
                {
                    _throwable.throwableCollider.isTrigger = true;
                }
                break;

            case WeaponLocation.Container:
                weaponEquipped = false;
                pickupCollider.enabled = false;
                muzzleCollider.enabled = false;

                gameObject.SetActive(false);
                break;

            case WeaponLocation.Inventory:
                weaponEquipped = false;
                pickupCollider.enabled = false;
                muzzleCollider.enabled = false;

                gameObject.SetActive(false);
                break;

            case WeaponLocation.Player:
                transform.localScale = originalScale;
                playerTransform = GameObject.Find("Player").GetComponent<Transform>();
                playerWeaponSound = playerTransform.GetComponent<WeaponSound>();
                UpdatePlayerWeaponSounds();
                playerController = playerTransform.GetComponent<PlayerController>();
                playerInventory = playerTransform.GetComponent<Inventory>();
                playerAnimator = playerTransform.GetComponent<Animator>();
                playerHealthManager = playerTransform.GetComponent<HealthManager>();
                playerIKManager = playerTransform.GetComponent<IKManager>();
                playerInventory.currentWeaponImage.sprite = weaponIcon;
                pickupCollider.enabled = false;
                muzzleCollider.enabled = true;
                meshRenderer.enabled = true;
                modelOutline.enabled = false;
                
                if (flashLight !=null)
                {
                    flashLight.enabled = false;
                }
                
                switch (weaponItemClass)
                {
                    case WeaponScriptableObject.WeaponClass.Primary:
                        playerAnimator.SetBool("PistolEquip", false);
                        playerAnimator.SetBool("RifleEquip", true);
                        playerAnimator.SetBool("BatEquip", false);
                        playerAnimator.SetBool("KnifeEquip", false);
                        playerAnimator.SetBool("MeleeEquip", false);
                        break;

                    case WeaponScriptableObject.WeaponClass.Secondary:
                        playerAnimator.SetBool("PistolEquip", true);
                        playerAnimator.SetBool("RifleEquip", false);
                        playerAnimator.SetBool("BatEquip", false);
                        playerAnimator.SetBool("KnifeEquip", false);
                        playerAnimator.SetBool("MeleeEquip", false);

                        break;

                    case WeaponScriptableObject.WeaponClass.Melee:
                        playerAnimator.SetBool("PistolEquip", false);
                        playerAnimator.SetBool("RifleEquip", false);
                        playerAnimator.SetBool("MeleeEquip", true);
                        break;
                    
                    case WeaponScriptableObject.WeaponClass.Throwable:
                        _throwable = GetComponent<Throwable>();
                        _throwable.explosionRadius = weaponScriptableObject.explosionRange;
                        muzzleCollider.isTrigger = true;
                        playerAnimator.SetBool("PistolEquip", false);
                        playerAnimator.SetBool("RifleEquip", false);
                        playerAnimator.SetBool("BatEquip", false);
                        playerAnimator.SetBool("KnifeEquip", false);
                        playerAnimator.SetBool("MeleeEquip", false);
                        playerAnimator.SetBool("ThrowableEquip", true);
                        break;
                }

                switch (ID)
                {
                    case 1001: // Knife
                        playerAnimator.SetBool("KnifeEquip", true);
                        playerAnimator.SetBool("BatEquip", false);
                        break;
                    case 1002: // Baseball Bat
                        playerAnimator.SetBool("BatEquip", true);
                        playerAnimator.SetBool("KnifeEquip", false);
                        break;
                    case 1006: // Fire Axe
                        playerAnimator.SetBool("BatEquip", false);
                        playerAnimator.SetBool("KnifeEquip", false);
                        break;
                }

                break;
        }
    }

    private void Start()
    {
        meleeAttackTimer = 2;
    }

    private void FixedUpdate()
    {
        playerController.reloadingWeapon = reloadingWeapon;

        if (weaponLocation == WeaponLocation.Player)
        {
            playerHandHolderPos = playerInventory.playerWeaponHolderTransform.position;
           // playerHandHolderRot = Quaternion.LookRotation(playerInventory.playerWeaponHolderTransform.up);
            playerHandHolderRot = Quaternion.LookRotation(playerAnimator.GetBoneTransform(HumanBodyBones.RightHand).up);
            transform.position = playerHandHolderPos;
            transform.rotation = playerHandHolderRot;
            //transform.position = new Vector3(playerHandHolderPos.x + xOffset,playerHandHolderPos.y + yOffset,playerHandHolderPos.z + zOffset);
            //  transform.rotation = playerHandHolderRot;
            //Vector3(0.00249999994,0.00889999978,0.000310000003) pos
            //Vector3(332.349915,357.041718,5.11632919) rot
        }
        
        if (weaponLocation == WeaponLocation.World)
        {
            
           
        }
    }

    private void Update()
    {

        switch (weaponLocation)
        {
            case WeaponLocation.World:

                if (weaponPicked)
                {

                }
                else
                {
                   gripTransform.Rotate(Vector3.up * Time.deltaTime * rotationSpeed);
                }

                break;

            case WeaponLocation.Container:

                
                titleTextMesh.enabled = false;
                if (!weaponPicked)
                {
                    meshRenderer.enabled = false;
                    pickupCollider.enabled = false;
                    
                }

                if (weaponPicked)
                {
                    quantity = 0;
                }
                gameObject.SetActive(false);
                break;

            case WeaponLocation.Inventory:
                
                meshRenderer.enabled = false;
                pickupCollider.enabled = false;
                titleTextMesh.enabled = false;
                gameObject.SetActive(false);
                break;
            
            case WeaponLocation.Player:

                weaponEquipped = true;
                
                titleTextMesh.enabled = false;
                
                
                if (!playerHealthManager.IsDead)
                { 
                    if (weaponEquipped)
                    {
                        totalBullets = playerInventory.CheckItemsLeft(bulletID, totalBullets);
                        playerInventory.UpdateBulletCounter(this);
                        
                        if (playerController.isAiming)
                        {
                            muzzleCollider.enabled = true;
                            if (Input.GetKeyDown(KeyAssignments.SharedInstance.attackKey.keyCode)
                                && !attacking
                                && !playerInventory.showInventory
                                && !reloadingWeapon
                                && !playerController.canStomp
                                && weaponItemClass == WeaponScriptableObject.WeaponClass.Throwable)
                            {
                                playerAnimator.SetTrigger("Throw");
                                weaponEquipped = false;
                            }
                        }
                        else
                        {
                            if (weaponItemClass != WeaponScriptableObject.WeaponClass.Throwable)
                            {
                                muzzleCollider.enabled = false;
                            }
                        }
                        if (!playerController.climbingLadder)
                        {
                            if (Input.GetKey(KeyAssignments.SharedInstance.attackKey.keyCode) 
                                && !attacking 
                                && !playerInventory.showInventory 
                                && !reloadingWeapon 
                                && !playerController.canStomp)
                            {
                                if (weaponItemClass == WeaponScriptableObject.WeaponClass.Melee 
                                    && !playerController.prone 
                                    && playerController._checkGround.isGrounded 
                                    && playerHealthManager.currentStamina >= playerHealthManager.meleeAttackPenalty)
                                {
                                    attacking = true;
                                    switch (ID)
                                    {
                                        case 1001: // knife
                                            attackNumber = 1;
                                            break;
                                        case 1002: // baseball bat
                                            attackNumber = 2;
                                            break;
                                        case 1006: //fire axe
                                            attackNumber = 3;
                                            break;
                                    }
                                    playerAnimator.SetBool("MeleeAttack" + attackNumber, attacking);
                                    
                                }
                               
                                else if (playerController.isAiming && bulletsInMag > 0)
                                {
                                    firing = true;
                                        if (Time.time - lastfired > 1 / fireRate)
                                        { 
                                            lastfired = Time.time;
                                            FireWeapon(); 
                                        }
                                }
                                

                                if (Input.GetKeyUp(KeyAssignments.SharedInstance.attackKey.keyCode) || Input.GetKeyUp(KeyAssignments.SharedInstance.aimBlockKey.keyCode) ||
                                    !playerController.isAiming)
                                {
                                    firing = false;
                                }
                            }

                           /* if (!attacking || weaponItemClass != WeaponScriptableObject.WeaponClass.Throwable)
                            {
                                playerAnimator.SetBool("MeleeAttack1", false);
                                playerAnimator.SetBool("MeleeAttack2", false);
                                playerAnimator.SetBool("MeleeAttack3", false);
                            }*/
                            
                            if (Input.GetKeyDown(KeyAssignments.SharedInstance.reloadKey.keyCode) && bulletsInMag < magazineCap) 
                            {
                                CancelInvoke("FireWeapon");
                                
                                if (!reloadingWeapon)
                                {
                                    totalBullets = playerInventory.CheckItemsLeft(bulletID, totalBullets);
                                    playerInventory.UpdateBulletCounter(this);
                                    if (totalBullets > 0)
                                    {
                                        reloadingWeapon = true;
                                        playerAnimator.SetBool("Reloading", true);
                                    }
                                    else
                                    {
                                        Debug.Log("No more bullets for " + weaponName);
                                    }
                                }
                            }

                            if (attacking && weaponItemClass != WeaponScriptableObject.WeaponClass.Throwable)
                            {
                                meleeAttackTimer -= Time.deltaTime;
                                if (meleeAttackTimer<=0)
                                {
                                    attacking = false;

                                    for (int i = 1; i == 3; i++)
                                    {
                                        playerAnimator.SetBool("MeleeAttack" + i, attacking);
                                    }
                                    meleeAttackTimer = 2;
                                }
                            }
                        }
                    }
                }
                break;
            
            case WeaponLocation.Throwed:
                pickupCollider.enabled = false;
                muzzleCollider.isTrigger = false;
                break;
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (weaponItemClass == WeaponScriptableObject.WeaponClass.Throwable)
        {
            if (other.CompareTag("Ground") && throwableActive)
            {
                
            }
        }
        
        
        if (other.CompareTag("Player") && !weaponPicked)
        {
            titleTextMesh.enabled = true;
            modelOutline.enabled = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            titleTextMesh.enabled = false;
            modelOutline.enabled = false;

        }
    }

    public void GetBulletFromPool()
    {
        if (ID == 5004)
        {
            float shootAngle = shotgunMinFireAngle;
            float nextAngle =  (shotgunMinFireAngle*-1 + shotgunMaxFireAngle)/bulletsPerShot;
            
            for (int i = 0; i < bulletsPerShot; i++) 
            { 
                GameObject shotgunBullet = ObjectPool.SharedInstance.GetPooledObject("Bullet"); 
                if (shotgunBullet != null)
                {
                    shotgunBullet.transform.parent = gunMuzzleTransform;
                    shotgunBullet.transform.position = gunMuzzleTransform.position; 
                    shotgunBullet.transform.localEulerAngles = new Vector3(shootAngle,gunMuzzleTransform.rotation.y,gunMuzzleTransform.rotation.z); 
                    shotgunBullet.SetActive(true);
                    shootAngle += nextAngle;
                } 
            } 
        }
        else
        { 
            GameObject bullet = ObjectPool.SharedInstance.GetPooledObject("Bullet");
            
            if (bullet != null) 
            { 
                bullet.transform.position = gunMuzzleTransform.position; 
                bullet.transform.rotation = gunMuzzleTransform.rotation; 
                bullet.transform.parent = gunMuzzleTransform; 
                bullet.SetActive(true); 
            }
        }
    }
    public void FireWeapon()
    {
        if (bulletsInMag>0)
        {
            playerIKManager.recoilTimer = Time.time;
            
            GetBulletFromPool();

            GameObject muzzleFlash = ObjectPool.SharedInstance.GetPooledObject("MuzzleFlashParticle");

            if (muzzleFlash != null)
            {
                muzzleFlash.transform.position = gunMuzzleTransform.position;
                muzzleFlash.transform.rotation = Quaternion.LookRotation(-gunMuzzleTransform.forward,Vector3.up);
                muzzleFlash.transform.parent = gunMuzzleTransform;
                muzzleFlash.SetActive(true);
            }
            bulletsInMag -=1;
            _weaponSound.FireWeaponSound();

        }
        else
        {
            Debug.Log("Magazine is empty! Reload!");
        }
    }
    
    public void DealDamage()
    {
        RaycastHit meleeHit;
        if (Physics.Raycast( playerTransform.position + Vector3.up,
            playerTransform.TransformDirection(Vector3.forward) * meleeAttackDistance, out meleeHit, meleeAttackDistance,
            enemyLayer.value))
        {
            Debug.DrawRay(playerTransform.position + Vector3.up, playerTransform.TransformDirection(Vector3.forward) * meleeAttackDistance,Color.blue,Time.deltaTime);
            if (meleeHit.collider.CompareTag("Enemy"))
            {
                AgentController agentController = meleeHit.collider.GetComponentInParent<AgentController>();
                Animator enemyAnimator = meleeHit.collider.GetComponentInParent<Animator>();
                HealthManager enemyHealth = meleeHit.collider.GetComponentInParent<HealthManager>();
                AudioSource enemyAudioSource = meleeHit.collider.GetComponentInParent<AudioSource>();
                EnemyAudio enemyAudio = meleeHit.collider.GetComponentInParent<EnemyAudio>();
                agentController.hisHit = true;
                enemyAnimator.SetTrigger("Hit");
                int critical = damage * 2;
                int damageGiven = damage;
                enemyHealth.currentHealth -= damageGiven;
                if (damageGiven > damage)
                {
                    Debug.Log("Critical Hit!!");
                }
                Debug.Log("Enemy Took " + damageGiven + " damage");
                enemyAudioSource.PlayOneShot(enemyAudio.damagedSound);
                
                
            }

            if (meleeHit.collider.CompareTag("Crashable"))
            {
                CrashObject crashObject = meleeHit.collider.GetComponent<CrashObject>();
                crashObject.Crash();
            }
        }
    }
    
    public void ReloadMagazine()
    {
        totalBullets= playerInventory.CheckItemsLeft(bulletID, totalBullets);
        
        for (int i = 0; i < playerInventory.totalInventorySlots; i++)
        {
            Slot slotIndex = playerInventory.slotArray[i].GetComponent<Slot>();
            if (!slotIndex.empty && slotIndex.itemScriptableObject != null)
            {
                if (slotIndex.itemID == bulletID)
                {
                    while (bulletsInMag < magazineCap)
                    {
                        slotIndex.quantity -= 1;
                        bulletsInMag += 1;

                        if (slotIndex.quantity < 1)
                        {
                            slotIndex.empty = true;
                            break; // break the loop if the slot has no more bullets
                        }

                        if (bulletsInMag == magazineCap)
                        {
                            totalBullets = playerInventory.CheckItemsLeft(bulletID, totalBullets);
                            playerInventory.UpdateBulletCounter(this);
                            return; //stop the method if we fill the magazine
                        }
                    }
                }
            }
        }
    }   
    public void ReloadWeaponAnim(string command) //called by animator event
    {
        switch (command)
        {
            case "ReloadStart":
                reloadingWeapon = true;

                break;
            
            case "ReloadEnd":
                if (magGameObject != null)
                {
                    magGameObject.transform.parent = magHolder;
                    magGameObject.transform.localPosition = new Vector3(0,0,0);
                    magGameObject.transform.localEulerAngles = new Vector3(0,0,0);
                }
                ReloadMagazine();
                
                reloadingWeapon = false;
                playerAnimator.SetBool("Reloading", false);
                break;
            
            
        }
       
    }
    
    public void MeleeAttackAnim(string command)
    {
        switch (command)
        {
            case "MeleeStart":
                playerController.controllerType = PlayerController.ControllerType.StandByController;
                break;
            
            case "DoDamage":
                DealDamage();
                break;
            
            case "MeleeEnd":
                attacking = false;
                playerAnimator.SetBool("MeleeAttack1", attacking);
                playerAnimator.SetBool("MeleeAttack2", attacking);
                playerAnimator.SetBool("MeleeAttack3", attacking);
                playerController.controllerType = PlayerController.ControllerType.DefaultController;
                
                break;
        }
    }

    public void GetWeaponScriptableObject(WeaponScriptableObject weaponScriptableObject)
    { 
        weaponItemClass = weaponScriptableObject.weaponClass; 
        ID = weaponScriptableObject.ID; 
        weaponIcon = weaponScriptableObject.weaponIcon; 
        weaponName = weaponScriptableObject.weaponName; 
        description = weaponScriptableObject.description; 
        bulletID = weaponScriptableObject.bulletID;
        
        damage = weaponScriptableObject.damage;
        bulletsPerShot = weaponScriptableObject.bulletsPerShot;
        fireRate = weaponScriptableObject.fireRate; 
        magazineCap = weaponScriptableObject.magazineCap;
        
        recoilDuration = weaponScriptableObject.recoilDuration; 
        recoilMaxRotation = weaponScriptableObject.recoilMaxRotation;
        
        bulletImpactPrefab = weaponScriptableObject.bulletImpactPrefab; 
        enemyImpactPrefab = weaponScriptableObject.enemyImpactPrefab; 
        muzzleFlashPrefab = weaponScriptableObject.muzzleFlashPrefab;
    }

    public void UpdatePlayerWeaponSounds()
    {
        if(_weaponSound.shotSound != null) { playerWeaponSound.shotSound = _weaponSound.shotSound;}
        if(_weaponSound.drawWeaponSound != null) { playerWeaponSound.drawWeaponSound = _weaponSound.drawWeaponSound;}
        if(_weaponSound.explosionSound != null){playerWeaponSound.explosionSound = _weaponSound.explosionSound;}
        if(_weaponSound.magazineInSound != null){playerWeaponSound.magazineInSound = _weaponSound.magazineInSound;}
        if(_weaponSound.magazineOutSound != null){playerWeaponSound.magazineOutSound = _weaponSound.magazineOutSound;}
        if(_weaponSound.meleeAttackSound != null){playerWeaponSound.meleeAttackSound = _weaponSound.meleeAttackSound;}
        if(_weaponSound.reloadEndSound != null){playerWeaponSound.reloadEndSound = _weaponSound.reloadEndSound;}
        if(_weaponSound.dropSound != null){playerWeaponSound.dropSound = _weaponSound.dropSound;}
    }
}