using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Serialization;
using UnityEngine.UIElements;


[RequireComponent(typeof(WorldTextUI))]
[RequireComponent(typeof(BoxCollider))]
public class Item : MonoBehaviour
{
    public enum ItemClass
    {
        Item,
        Weapon,
        Outfit
    }

    public ItemClass itemClass;

    [Header("Item ID")] 
    public int ID;
    public string itemName;
    public string description;
    public Sprite itemIcon;
    public GameObject itemPrefab;
    public int quantity = 1;
    public bool itemPickedUp;
    public bool itemEquipped;


    public enum ItemLocation
    {
        World,
        Container,
        Player,
        Inventory,
        Throwed,
    }

    public ItemLocation itemLocation;


    /// <summary>
    /// Instantiated model from prefab
    /// </summary>    
   [Header("Transform References")]

    public GameObject itemModel;

    private BoxCollider _boxCollider;
    public Outline outline;
    [HideInInspector] public Transform itemTransform;
    public float prefabRotationSpeed = 2f;

    [Header("UI")] 
    public WorldTextUI worldTextUI;

    [Header("Item Attributes")]
    public ItemScriptableObject itemScriptableObject;
    public int healthRestore;
    public int foodRestore;
    public int waterRestore; 
    public bool usable;
    public bool consumable;
    public bool isStackable;
    public int maxStack;

    [Header("Weapon Attributes")] public WeaponScriptableObject weaponScriptableObject;
    
    public WeaponScriptableObject.WeaponClass weaponClass;

    [HideInInspector] public int bulletID;
    public int damage;
    public float fireRate;
    public int magazineCap;
    public int bulletsInMag;
    public float recoilDuration;
    public float recoilMaxRotation;
    public float maxFireAngle;
    public float minFireAngle;
    public int bulletsPerShot;
    public bool blockAttacks;

    [Header("Weapon Effects")] 
    public GameObject bulletImpactPrefab;
    public GameObject enemyImpactPrefab;
    public GameObject muzzleFlashPrefab;

    [Header("Weapon Transforms")] 
    public WeaponTransforms weaponTransforms;
    public Transform flashLightTransform;
    public Transform gunMuzzleTransform;
    public Transform handguardTransform;
    public Transform gripTransform;
    public Collider muzzleCollider;
    public Light flashLight;
    public Transform magHolder;
    [HideInInspector] public GameObject magGameObject;

    public bool weaponEquipped;
    public int totalBullets;
    public bool reloadingWeapon;
    public bool attacking;
    public int attackNumber;
    public float meleeAttackTimer;
    public bool firing;
    public bool aiming;
    public bool drawingWeapon;
    public float meleeAttackDistance;
    private int meleeAttackNumber;
    public LayerMask enemyLayer;

    public WeaponSound _weaponSound;
    private WeaponSound playerWeaponSound;

    [Header("Equipment Attributes")]
    public OutfitScriptableObject outfitScriptableObject;
    public OutfitScriptableObject.OutfitBodyPart outfitBodyPart;
    public int defense;
    public int backpackCapacity;
    public GameObject equipmentPrefab;
    public HumanBodyBones targetBone;
   
    private Transform playerTransform;
    private IKManager playerIKManager;
    private Inventory playerInventory;
    private Animator playerAnimator;


    private void OnValidate()
    {
    }

    private void Awake()
    {
        _boxCollider = GetComponent<BoxCollider>();


    }

    public void InitItem()
    {
        if (itemScriptableObject || weaponScriptableObject || outfitScriptableObject)
        {
            //OBTENEMOS LOS DATOS DEL ITEM SEGUN CLASE
            switch (itemClass)
            {
                case ItemClass.Item:
                    GetItemScriptableObject(itemScriptableObject);
                    break;

                case ItemClass.Weapon:
                    GetWeaponScriptableObject(weaponScriptableObject);
                    GetWeaponTransforms(itemModel);
                    _weaponSound = gameObject.AddComponent<WeaponSound>();
                    _weaponSound.GetSounds(weaponScriptableObject);
                    break;

                case ItemClass.Outfit:
                    GetOutfitScriptableObject(outfitScriptableObject);
                    break;
            }

            worldTextUI = GetComponent<WorldTextUI>();
            worldTextUI.text = itemName;

            switch (itemLocation)
            {   
                case ItemLocation.World:
                    itemModel = InstantiateItem(itemPrefab);
                    outline.enabled = false;

                    worldTextUI.targetTransform = itemModel.transform;
                    worldTextUI.uIEnabled = false;
                    break;
                case ItemLocation.Container:
                    break;
                case ItemLocation.Player:
                    break;
                case ItemLocation.Inventory:
                    break;
                case ItemLocation.Throwed:
                    break;
                default:
                    break;
            }

            
        }
        else
        {
            Debug.Log("No Scriptable Object attached on " + gameObject.name);
        }
    }

    private void OnEnable()
    {
        InitItem();
    }

    private void Update()
    {
        switch (itemLocation)
        {
            case ItemLocation.World:
                itemModel.transform.Rotate(Vector3.up * (Time.deltaTime * prefabRotationSpeed));
                _boxCollider.enabled = true;
                itemModel.SetActive(true);
                itemPickedUp = false;
                break;

            case ItemLocation.Container:
                _boxCollider.enabled = false;
                itemModel.SetActive(false);
                itemPickedUp = false;
                break;

            case ItemLocation.Inventory:
                _boxCollider.enabled = false;
                itemModel.SetActive(false);
                itemPickedUp = true;
                break;

            case ItemLocation.Player:
                _boxCollider.enabled = false;
                itemModel.SetActive(true);
                itemPickedUp = true;
                break;

            case ItemLocation.Throwed:
                itemModel.SetActive(true);
                break;
        }
        if(itemLocation != ItemLocation.World && itemLocation != ItemLocation.Throwed)
        {
            worldTextUI.uIEnabled = false;
        }
    }

    /// <summary>
    /// Instantiates a prefab as child of item GameObject
    /// </summary>
    /// <param name="prefab"></param>
    /// <returns>Gameobject</returns>
    public GameObject InstantiateItem(GameObject prefab)
    {
        GameObject instantiatedItem = Instantiate(prefab, this.transform.position, this.transform.rotation, transform);
        instantiatedItem.SetActive(true);
        instantiatedItem.AddComponent<Outline>();
        outline = instantiatedItem.GetComponent<Outline>();
        outline.OutlineWidth = 1;
        outline.OutlineMode = Outline.Mode.OutlineAll;
        outline.OutlineColor = Color.white;
        outline.enabled = false;

        return instantiatedItem;
    }

    public void GetItemScriptableObject(ItemScriptableObject itemScriptableObject = null)
    {
        if (itemScriptableObject)
        {
            this.itemScriptableObject = itemScriptableObject;

        }
        else
        {
            itemScriptableObject = this.itemScriptableObject;
        }
        ID = itemScriptableObject.ID;
        itemName = itemScriptableObject.itemName;
        description = itemScriptableObject.description;
        itemIcon = itemScriptableObject.itemIcon;
        itemPrefab = itemScriptableObject.itemPrefab;
        usable = itemScriptableObject.usable;
        consumable = itemScriptableObject.consumable;
        isStackable = itemScriptableObject.isStackable;
        maxStack = itemScriptableObject.maxStack;
        healthRestore = itemScriptableObject.healthRestore;
        foodRestore = itemScriptableObject.foodRestore;
        waterRestore = itemScriptableObject.waterRestore;
    }

    public void GetWeaponScriptableObject(WeaponScriptableObject weaponScriptableObject = null)
    {
        if (weaponScriptableObject)
        {
            this.weaponScriptableObject = weaponScriptableObject;
        }
        else
        {
            weaponScriptableObject = this.weaponScriptableObject;
        }
        
        weaponClass = weaponScriptableObject.weaponClass;
        ID = weaponScriptableObject.ID;
        itemIcon = weaponScriptableObject.weaponIcon;
        itemName = weaponScriptableObject.weaponName;
        description = weaponScriptableObject.description;
        
        bulletID = weaponScriptableObject.bulletID;

        damage = weaponScriptableObject.damage;
        bulletsPerShot = weaponScriptableObject.bulletsPerShot;
        fireRate = weaponScriptableObject.fireRate;
        magazineCap = weaponScriptableObject.magazineCap;
        blockAttacks = weaponScriptableObject.blockAttacks;

        recoilDuration = weaponScriptableObject.recoilDuration;
        recoilMaxRotation = weaponScriptableObject.recoilMaxRotation;

        bulletImpactPrefab = weaponScriptableObject.bulletImpactPrefab;
        enemyImpactPrefab = weaponScriptableObject.enemyImpactPrefab;
        muzzleFlashPrefab = weaponScriptableObject.muzzleFlashPrefab;
    }

    public void GetOutfitScriptableObject(OutfitScriptableObject outfitScriptableObject = null)
    {
        if (weaponScriptableObject)
        {
            this.outfitScriptableObject = outfitScriptableObject;
        }
        else
        {
            outfitScriptableObject = this.outfitScriptableObject;
        }

        ID = outfitScriptableObject.ID;
        name = outfitScriptableObject.itemName;
        description = outfitScriptableObject.description;   
        itemIcon = outfitScriptableObject.itemIcon;

        itemPrefab = outfitScriptableObject.outfitPrefab;

        outfitBodyPart = outfitScriptableObject.outfitBodyPart;

        defense = outfitScriptableObject.defense;  

        backpackCapacity = outfitScriptableObject.backpackCapacity; 
    }

    #region Weapon Functions

    public void GetWeaponTransforms(GameObject model)
    {
        weaponTransforms = model.GetComponent<WeaponTransforms>();
        flashLightTransform = weaponTransforms.flashLightTransform;
        gunMuzzleTransform = weaponTransforms.gunMuzzleTransform;
        handguardTransform = weaponTransforms.handguardTransform;
        gripTransform = weaponTransforms.gripTransform;
        muzzleCollider = weaponTransforms.muzzleCollider;
        flashLight = weaponTransforms.flashLight;
        magHolder = weaponTransforms.magHolder;
        magGameObject = weaponTransforms.magGameObject;

    }

    public void FireWeapon()
    {
        if (bulletsInMag > 0)
        {
            playerIKManager.recoilTimer = Time.time;

            GetBulletFromPool();

            GameObject muzzleFlash = ObjectPool.SharedInstance.GetPooledObject("MuzzleFlashParticle");

            if (muzzleFlash != null)
            {
                muzzleFlash.transform.position = gunMuzzleTransform.position;
                muzzleFlash.transform.rotation = Quaternion.LookRotation(-gunMuzzleTransform.forward, Vector3.up);
                muzzleFlash.transform.parent = gunMuzzleTransform;
                muzzleFlash.SetActive(true);
            }

            bulletsInMag -= 1;
            _weaponSound.FireWeaponSound();
        }
        else
        {
            Debug.Log("Magazine is empty! Reload!");
        }
    }

    public void GetBulletFromPool()
    {
        float shootAngle = minFireAngle;
        float nextAngle = (minFireAngle * -1 + maxFireAngle) / bulletsPerShot;

        for (int i = 0; i < bulletsPerShot; i++)
        {
            GameObject bullet = ObjectPool.SharedInstance.GetPooledObject("Bullet");
            if (bullet != null)
            {
                bullet.transform.position = gunMuzzleTransform.position;
                if (i > 0)
                {
                    bullet.transform.localEulerAngles = new Vector3(shootAngle, gunMuzzleTransform.rotation.y,
                        gunMuzzleTransform.rotation.z);
                }
                else
                {
                    bullet.transform.rotation = gunMuzzleTransform.rotation;
                }

                bullet.transform.parent = gunMuzzleTransform;

                bullet.SetActive(true);
                shootAngle += nextAngle;
            }
        }
    }

    public void DealDamage()
    {
        RaycastHit meleeHit;
        if (Physics.Raycast(playerTransform.position + Vector3.up,
                playerTransform.TransformDirection(Vector3.forward) * meleeAttackDistance, out meleeHit,
                meleeAttackDistance,
                enemyLayer.value))
        {
            Debug.DrawRay(playerTransform.position + Vector3.up,
                playerTransform.TransformDirection(Vector3.forward) * meleeAttackDistance, Color.blue, Time.deltaTime);
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
        totalBullets = playerInventory.CheckItemsLeft(bulletID, totalBullets);

        for (int i = 0; i < playerInventory.itemsList.Count; i++)
        {
            if (playerInventory.itemsList[i].ID == bulletID)
            {
                while (bulletsInMag < magazineCap)
                {
                    playerInventory.itemsList[i].quantity -= 1;
                    bulletsInMag += 1;

                    if (playerInventory.itemsList[i].quantity < 1)
                    {
                        playerInventory.itemsList.RemoveAt(i);
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

    public void ReloadWeaponAnim(string command) //Animator events called by WeaponAnimEvent() in PlayerController
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
                    magGameObject.transform.localPosition = new Vector3(0, 0, 0);
                    magGameObject.transform.localEulerAngles = new Vector3(0, 0, 0);
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
                // playerController.controllerType = PlayerController.ControllerType.StandByController;
                break;

            case "DoDamage":
                DealDamage();
                break;

            case "MeleeEnd":
                attacking = false;
                playerAnimator.SetBool("MeleeAttack1", attacking);
                playerAnimator.SetBool("MeleeAttack2", attacking);
                playerAnimator.SetBool("MeleeAttack3", attacking);
                // playerController.controllerType = PlayerController.ControllerType.DefaultController;

                break;
        }
    }

    #endregion


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            switch (itemLocation)
            {
                case ItemLocation.World:
                    outline.enabled = true;
                    worldTextUI.uIEnabled = true;
                    break;
            }
        }

        if (weaponClass == WeaponScriptableObject.WeaponClass.Throwable)
        {
            /*if (other.CompareTag("Ground") && throwableActive)
            {
            }*/
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            switch (itemLocation)
            {
                case ItemLocation.World:
                    outline.enabled = false;
                    worldTextUI.uIEnabled = false;
                    break;
            }
        }
    }
}