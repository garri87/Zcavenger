using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;


[RequireComponent(typeof(WorldTextUI))]
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(WorldTextUI))]
[RequireComponent(typeof(UIDocument))]
public class Item : MonoBehaviour
{
    public enum ItemClass
    {
        Item,
        Weapon,
        Outfit
    }

    public ItemClass itemClass;
    public ScriptableObject scriptableObject;

    [Header("Item ID")]
    public int ID;
    public string itemName;
    public string description;
    public Sprite itemIcon;
    public GameObject itemPrefab;
    public int quantity = 1;
    public int minLootQuantity = 1;
    public int maxLootQuantity = 1;
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

    public GameObject itemModelGO;
    public SkinnedMeshRenderer modelSkinnedRenderer;

    private bool modelInstantiated = false;


    private BoxCollider _boxCollider;
    public Outline outline;
    [HideInInspector] public Transform itemTransform;
    public float prefabRotationSpeed = 2f;

    [Header("UI")]
    public WorldTextUI worldTextUI;
    public VisualTreeAsset worldTextVTAsset;

    [Header("Item Attributes")]
    public ItemScriptableObject itemScriptableObject;
    public int healthRestore;
    public int foodRestore;
    public int waterRestore;
    public bool usable;
    public bool consumable;
    public bool isStackable;
    public int maxStack;

    [Header("Weapon Attributes")] 
    public WeaponScriptableObject weaponScriptableObject;

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

    public bool weaponDrawn;
    public int totalBullets;
    public bool attacking;
    public int attackNumber;
    public float meleeAttackTimer;
    public bool firing;
    public bool aiming;
    public bool drawingWeapon;
    public float meleeAttackDistance = 1;
    private int meleeAttackNumber;
    public LayerMask enemyLayer;

    public WeaponSound _weaponSound;
    private WeaponSound playerWeaponSound;

    [Header("Equipment Attributes")]
    public OutfitScriptableObject outfitScriptableObject;
    public enum EquipmentSlot { 
        Head, 
        Torso, 
        Vest, 
        Legs, 
        Feet,
        Backpack }
    public EquipmentSlot equipmentSlot;

    public int defense;
    public int backpackCapacity;
    public GameObject equipmentPrefab;
    public SkinnedMeshRenderer targetSkinMesh;
    private MeshFilter modelMeshFilter;
    private MeshRenderer modelMeshRenderer;
    
    private Transform playerTransform;
    private PlayerController playerController;
    private IKManager playerIKManager;
    private Inventory playerInventory;
    private Animator playerAnimator;

    

    private void OnValidate()
    {
        if (scriptableObject != null)
        {
            GetScriptableObject(scriptableObject);
        }
    }

    private void Awake()
    {
       
      //  InitItem();
        
        
    }


    private void OnEnable()
    {
        InitItem();
    }

    private void Update()
    {
        if (scriptableObject)
        {
            if(itemLocation == ItemLocation.Container || 
            itemLocation == ItemLocation.Inventory ||
            itemLocation == ItemLocation.Player || 
            itemLocation == ItemLocation.Throwed){
                    _boxCollider.enabled = false;
            }

            switch (itemLocation)
            {
                case ItemLocation.World:
                    transform.parent = null;
                    transform.Rotate(Vector3.up * (Time.deltaTime * prefabRotationSpeed));
                    _boxCollider.enabled = true;
                    itemModelGO.SetActive(true);
                    if (itemClass == ItemClass.Outfit)
                    {
                        modelMeshRenderer.enabled = true;
                    }
                    itemPickedUp = false;
                    itemEquipped = false;
                    break;

                case ItemLocation.Container:
                    _boxCollider.enabled = false;
                    itemModelGO.SetActive(false);
                    if (itemClass == ItemClass.Outfit)
                    {
                        modelMeshRenderer.enabled = false;
                    }                    
                    itemPickedUp = false;
                    itemEquipped = false;
                    break;

                case ItemLocation.Inventory:
                    _boxCollider.enabled = false;
                    transform.parent = playerInventory.inventoryGo.transform;
                    itemModelGO.SetActive(false);
                    if (itemClass == ItemClass.Outfit)
                    {
                        modelMeshRenderer.enabled = false;
                    }
                    itemPickedUp = true;
                    break;

                case ItemLocation.Player:
                    _boxCollider.enabled = false;
                    itemPickedUp = true;
                    meleeAttackTimer -= Time.deltaTime;
                    switch (itemClass)
                    {
                        case ItemClass.Weapon:
                            if (playerInventory.drawnWeaponItem == this)
                            {
                                weaponDrawn = true;
                            }
                            itemModelGO.SetActive(weaponDrawn);
                            transform.position = playerController._weaponHolder.position;
                            transform.rotation = Quaternion.LookRotation(playerController._weaponHolder.forward);
                            transform.parent = playerController._weaponHolder;
                            break;

                        case ItemClass.Outfit:
                            itemModelGO.SetActive(true);
                            modelMeshRenderer.enabled = false;
                            break;
                    }
                    
                    break;

                case ItemLocation.Throwed:
                    _boxCollider.enabled = false;
                    itemModelGO.SetActive(true);
                    itemPickedUp = false;
                    if (itemClass == ItemClass.Outfit)
                    {
                        modelMeshRenderer.enabled = true;
                    }
                    transform.parent = null;
                    itemEquipped = false;
                    worldTextUI.enabled = false;
                    break;
            }
            if (itemLocation != ItemLocation.World && itemLocation != ItemLocation.Throwed)
            {
                worldTextUI.uIEnabled = false;
                outline.enabled = false;
            }
        }
        
        if (quantity <= 0)
        {
            playerInventory.itemsList.Remove(this.gameObject);
            Destroy(gameObject);
        }
    }


    /// <summary>
    /// Updates item data from scriptable object. <br></br>
    /// If item is weapon, gets weapon transform from model and gets weapon sounds from scriptable object.
    /// </summary>
    public void InitItem()
    {

        _boxCollider = GetComponent<BoxCollider>();
        if (!playerTransform)
        {
            playerTransform = GameObject.Find("Player").transform;
        }
        
        if (playerTransform)
        {
            playerController = playerTransform.GetComponent<PlayerController>();
            playerIKManager = playerTransform.GetComponent<IKManager>();
            playerInventory = playerTransform.GetComponent<Inventory>();
            playerAnimator = playerTransform.GetComponent<Animator>();
        }

        if (!modelMeshFilter) modelMeshFilter = GetComponent<MeshFilter>();
        if (!modelMeshRenderer) modelMeshRenderer = GetComponent<MeshRenderer>();
        if (!worldTextUI)
        {
            worldTextUI = GetComponent<WorldTextUI>();
            worldTextUI.uiDocument.visualTreeAsset = worldTextVTAsset;
        }
            
        modelInstantiated = false;


        if (scriptableObject)
        {
            GetScriptableObject(scriptableObject);

            switch (itemLocation)
            {
                case ItemLocation.World:

                    if (!modelInstantiated)
                    {
                        itemModelGO = InstantiateItem(itemPrefab);
                    }
                    else
                    {
                        itemModelGO.SetActive(true);
                    }

                    outline = itemModelGO.GetComponent<Outline>();
                    outline.enabled = false;

                    worldTextUI.targetTransform = itemModelGO.transform;
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

            if (itemClass == ItemClass.Outfit)
            {
                modelMeshFilter.mesh = modelSkinnedRenderer.sharedMesh;
                modelMeshRenderer.materials = modelSkinnedRenderer.materials;
                transform.localScale = itemModelGO.transform.localScale;
            }

            //OBTENEMOS LOS DATOS DEL ITEM SEGUN CLASE
            switch (itemClass)
            {
                case ItemClass.Item:
                    
                    break;

                case ItemClass.Weapon:
                    GetWeaponTransforms(itemModelGO);
                    if (!itemModelGO.TryGetComponent(out WeaponSound weaponSound))
                    {
                        _weaponSound = gameObject.AddComponent<WeaponSound>();
                    }
                    else
                    {
                        _weaponSound = weaponSound;
                    }
                    _weaponSound.GetSounds(weaponScriptableObject);
                    meleeAttackTimer = fireRate;
                    break;

                case ItemClass.Outfit:
                    break;
            }

            worldTextUI = GetComponent<WorldTextUI>();
            worldTextUI.text = itemName + " (" + quantity + ")";
            
        }
        else
        {
            Debug.Log("No Scriptable Object attached on " + gameObject.name);
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
        modelSkinnedRenderer = instantiatedItem.GetComponent<SkinnedMeshRenderer>();
        modelInstantiated = true;
        
        return instantiatedItem;
    }


    /// <summary>
    /// Gets data from an ScriptableObject. If no parameter is given, the current ScriptableObject is used. 
    /// </summary>
    /// <param name="scriptableObj"></param>
    public void GetScriptableObject(ScriptableObject scriptableObj = null)
    {
        if (scriptableObj)
        {
            this.scriptableObject = scriptableObj;
        }

        if (scriptableObject)
        {
            if (scriptableObj is ItemScriptableObject)
            {
                itemClass = ItemClass.Item;
                ItemScriptableObject itemScriptable = scriptableObj as ItemScriptableObject;
                itemScriptableObject = itemScriptable;
                ID = itemScriptable.ID;
                itemIcon = itemScriptable.itemIcon;
                itemName = itemScriptable.itemName;
                description = itemScriptable.description;
                itemPrefab = itemScriptable.itemPrefab;

                usable = itemScriptable.usable;
                consumable = itemScriptable.consumable;
                isStackable = itemScriptable.isStackable;
                maxStack = itemScriptable.maxStack;

                healthRestore = itemScriptable.healthRestore;
                foodRestore = itemScriptable.foodRestore;
                waterRestore = itemScriptable.waterRestore;
            }

            else if (scriptableObj is WeaponScriptableObject)
            {
                itemClass = ItemClass.Weapon;
                quantity = 1;
                WeaponScriptableObject weaponScriptable = scriptableObj as WeaponScriptableObject;
                weaponScriptableObject = weaponScriptable;
                weaponClass = weaponScriptable.weaponClass;
                ID = weaponScriptable.ID;
                itemIcon = weaponScriptable.weaponIcon;
                itemName = weaponScriptable.weaponName;
                itemPrefab = weaponScriptable.weaponPrefab;
                
                description = weaponScriptable.description;

                bulletID = weaponScriptable.bulletID;

                damage = weaponScriptable.damage;
                bulletsPerShot = weaponScriptable.bulletsPerShot;
                fireRate = weaponScriptable.fireRate;
                magazineCap = weaponScriptable.magazineCap;
                blockAttacks = weaponScriptable.blockAttacks;

                recoilDuration = weaponScriptable.recoilDuration;
                recoilMaxRotation = weaponScriptable.recoilMaxRotation;

                bulletImpactPrefab = weaponScriptable.bulletImpactPrefab;
                enemyImpactPrefab = weaponScriptable.enemyImpactPrefab;
                muzzleFlashPrefab = weaponScriptable.muzzleFlashPrefab;

                isStackable = false;
                maxStack = 1;
            }

            else if (scriptableObject is OutfitScriptableObject)
            {
                itemClass = ItemClass.Outfit;
                quantity = 1;
                OutfitScriptableObject outfitScriptable = scriptableObj as OutfitScriptableObject;
                outfitScriptableObject = outfitScriptable;
                ID = outfitScriptable.ID;
                equipmentSlot = outfitScriptable.equipmentSlot;
                itemIcon = outfitScriptable.itemIcon;
                itemName = outfitScriptable.itemName;
                description = outfitScriptable.description;
                itemPrefab = outfitScriptable.outfitPrefab;
                defense = outfitScriptable.defense;
                backpackCapacity = outfitScriptable.backpackCapacity;
                
                isStackable = false;
                maxStack = 1;
            }
        }

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
            playerInventory.UpdateBulletCounterUI(this);
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
        totalBullets = playerInventory.CheckItemsLeft(bulletID); //Check existence of compatible ammo

        List<Item> itemList = new List<Item>();

        foreach (GameObject item in playerInventory.itemsList)
        {
            itemList.Add(item.GetComponent<Item>());
        }

        for (int i = 0; i < itemList.Count; i++)
        {
            if (itemList[i].ID == bulletID)
            {
                while (bulletsInMag < magazineCap)
                {
                    itemList[i].quantity -= 1;
                    bulletsInMag += 1;

                    if (itemList[i].quantity < 1)
                    {
                        playerInventory.UpdateBulletCounterUI(this);
                        totalBullets = playerInventory.CheckItemsLeft(bulletID);
                        break; // break the loop if the slot has no more bullets
                    }

                    if (bulletsInMag == magazineCap)
                    {
                        totalBullets = playerInventory.CheckItemsLeft(bulletID);
                        playerInventory.UpdateBulletCounterUI(this);
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

                break;

            case "ReloadEnd":
                if (magGameObject != null)
                {
                    magGameObject.transform.parent = magHolder;
                    magGameObject.transform.localPosition = new Vector3(0, 0, 0);
                    magGameObject.transform.localEulerAngles = new Vector3(0, 0, 0);
                }

                ReloadMagazine();

                playerController.isReloadingWeapon = false;
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
                playerAnimator.SetBool("AxeAttack", attacking);
                playerAnimator.SetBool("BatAttack", attacking);
                playerAnimator.SetBool("KnifeAttack", attacking);
                // playerController.controllerType = PlayerController.ControllerType.DefaultController;

                break;
        }
    }

    #endregion


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (scriptableObject)
            {
                switch (itemLocation)
                {
                    case ItemLocation.World:
                        outline.enabled = true;
                        worldTextUI.uIEnabled = true;
                        break;
                }
            }
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (scriptableObject)
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(transform.position,new Vector3(.5f,.5f,.5f));
    }
}