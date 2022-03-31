using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WeaponItem : MonoBehaviour
{
    [Header("Weapon ID")]
    public WeaponScriptableObject.WeaponClass weaponItemClass;

    [HideInInspector]public Transform holderTarget;
    
    public WeaponScriptableObject weaponScriptableObject;

    public int ID;
    public string weaponName;
    public string description;
    public int bulletID;
    
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
    public bool flashLightPower;
    public Transform magHolder;
    public GameObject magGameObject;

    [Header("Weapon Render")] 
    public Sprite weaponIcon;
    public Mesh weaponMesh;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    public Outline modelOutline;

    [Header("Weapon Effects")] 
    public GameObject bulletImpactPrefab;
    public GameObject enemyImpactPrefab;
    public GameObject muzzleFlashPrefab;

    public enum WeaponLocation
    {
        World,
        Container,
        Player,
        Inventory,
    }

    public WeaponLocation weaponLocation;

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

    private Transform playerTransform;
    private Animator playerAnimator;
    private HealthManager playerHealthManager;
    private PlayerController playerController;
    private Inventory inventory;
    private IKManager playerIKManager;

    #endregion


    public int quantity = 1;

    public GameObject titleTextGameObject; 
    [HideInInspector] public BoxCollider pickupCollider;

    public float rotationSpeed = 2f;
    public TextMeshPro titleTextMesh;

    private Vector3 originalScale;
    private Quaternion originalRot;

    
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
    }

    private void Awake()
    {
        originalScale = this.transform.localScale;
        originalRot = Quaternion.identity;
        playerTransform = GameObject.Find("Player").GetComponent<Transform>();
        playerAnimator = playerTransform.GetComponent<Animator>();
        playerHealthManager = playerTransform.GetComponent<HealthManager>();
        inventory = playerTransform.GetComponent<Inventory>();
        playerController = playerTransform.GetComponent<PlayerController>();

        titleTextMesh = titleTextGameObject.GetComponent<TextMeshPro>();
        GetWeaponScriptableObject(weaponScriptableObject);
        titleTextMesh.text = weaponName;
        pickupCollider = GetComponent<BoxCollider>();
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
                holderTarget = inventory.uIManager.primaryEquipSlot.Find("WeaponHolder");
                break;
            case WeaponScriptableObject.WeaponClass.Secondary:
                holderTarget = inventory.uIManager.secondaryEquipSlot.Find("WeaponHolder");
                break;
            case WeaponScriptableObject.WeaponClass.Melee:
                holderTarget = inventory.uIManager.meleeEquipSlot.Find("WeaponHolder");
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
                muzzleCollider.enabled = false;
                meshFilter.mesh = weaponMesh;
                meshRenderer.enabled = true;
                transform.localScale = originalScale;
                gripTransform.position = this.transform.position;
                
                gripTransform.localPosition = Vector3.zero;
                gripTransform.localRotation = Quaternion.Euler(0,transform.rotation.y,0);

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
                _weaponSound = GetComponent<WeaponSound>();
                playerController = playerTransform.GetComponent<PlayerController>();
                inventory = playerTransform.GetComponent<Inventory>();
                playerAnimator = playerTransform.GetComponent<Animator>();
                playerHealthManager = playerTransform.GetComponent<HealthManager>();
                playerIKManager = playerTransform.GetComponent<IKManager>();
                inventory.currentWeaponImage.sprite = weaponIcon;
                pickupCollider.enabled = false;
                muzzleCollider.enabled = true;
                meshFilter.mesh = weaponMesh;
                meshRenderer.enabled = true;
                modelOutline.enabled = false;
               

                
                if (flashLight !=null)
                {
                    flashLight.enabled = flashLightPower;
                    if (Input.GetKeyDown(KeyAssignments.SharedInstance.flashLightKey.keyCode))
                    {
                        flashLightPower = !flashLightPower;
                    }
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
            
                Transform rightHandTransform = playerAnimator.GetBoneTransform(HumanBodyBones.RightMiddleProximal);
                
                gripTransform.position = inventory.playerHandHolderTransform.position;
               //gripTransform.rotation = Quaternion.LookRotation(rightHandTransform.up);
                gripTransform.rotation = Quaternion.LookRotation(playerController._weaponHolderTransform.up);
                //Vector3(0.00249999994,0.00889999978,0.000310000003) pos
                //Vector3(332.349915,357.041718,5.11632919) rot
                playerIKManager.HoldWeapon(gripTransform,handguardTransform);
                
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
                        totalBullets = inventory.CheckItemsLeft(bulletID, totalBullets);
                        inventory.UpdateBulletCounter(this);
                        
                        if (playerController.isAiming)
                        {
                            muzzleCollider.enabled = true;
                        }
                        else
                        {
                            muzzleCollider.enabled = false;
                        }
                        if (!playerController.climbingLadder)
                        {
                            if (Input.GetKey(KeyAssignments.SharedInstance.attackKey.keyCode) && !attacking && !inventory.showInventory && !reloadingWeapon && !playerController.canStomp)
                            {
                                if (weaponItemClass == WeaponScriptableObject.WeaponClass.Melee && !playerController.prone &&
                                    playerController._checkGround.isGrounded && playerHealthManager.currentStamina >= playerHealthManager.meleeAttackPenalty)
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

                            if (!attacking)
                            {
                                playerAnimator.SetBool("MeleeAttack1", false);
                                playerAnimator.SetBool("MeleeAttack2", false);
                                playerAnimator.SetBool("MeleeAttack3", false);
                            }
                            
                            if (Input.GetKeyDown(KeyAssignments.SharedInstance.reloadKey.keyCode) && bulletsInMag < magazineCap) 
                            {
                                CancelInvoke("FireWeapon");
                                
                                if (!reloadingWeapon)
                                {
                                    totalBullets = inventory.CheckItemsLeft(bulletID, totalBullets);
                                    inventory.UpdateBulletCounter(this);
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

                            if (attacking)
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
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
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
            GetWeaponSound();

            
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
                agentController.headBodyWeight = 1;
                agentController.upperBodyWeight = 1;
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
        totalBullets= inventory.CheckItemsLeft(bulletID, totalBullets);
        
        for (int i = 0; i < inventory.totalSlots; i++)
        {
            Slot slotIndex = inventory.slotCount[i].GetComponent<Slot>();
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
                            totalBullets = inventory.CheckItemsLeft(bulletID, totalBullets);
                            inventory.UpdateBulletCounter(this);
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

    public void GetWeaponSound()
    {
        switch (ID)
        {
            case 3003: // m4a1
                _weaponSound.FireWeaponSound("Rifle");
                break;
                
            case 3004:// ak74
                _weaponSound.FireWeaponSound("Rifle");
                break;
                
            case 2004:
                _weaponSound.FireWeaponSound("Pistol");
                break;
                
            case 2005://M1911
                _weaponSound.FireWeaponSound("Pistol");
                break;
            case 5004:
                _weaponSound.FireWeaponSound("Shotgun");
                
                playerAnimator.SetTrigger("FireShotgun");
                break;
        }
    }
}