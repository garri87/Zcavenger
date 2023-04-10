using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.TerrainTools;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;


[RequireComponent(typeof(WorldTextUI))]
[RequireComponent(typeof(UIDocument))]
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Outline))]
public class Item : MonoBehaviour
{
    public enum ItemClass
    {
        Item,
        Weapon,
        Equipment
    }

    public ItemClass itemClass;

    public ItemScriptableObject itemScriptableObject;
    [Header("Item ID")] public int ID;
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

    [FormerlySerializedAs("modelTransform")] [Header("Transform References")]
    public GameObject itemModel;

    private BoxCollider _boxCollider;
    public Outline outline;
    [HideInInspector] public Transform itemTransform;
    public float prefabRotationSpeed = 2f;

    [Header("UI")] public WorldTextUI textUI;

    [Header("Item Attributes")] public int healthRestore;
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

    [Header("Weapon Effects")] public GameObject bulletImpactPrefab;
    public GameObject enemyImpactPrefab;
    public GameObject muzzleFlashPrefab;

    [Header("Weapon Transforms")] public Transform flashLightTransform;
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
    private float lastfired;
    public bool aiming;
    public bool drawingWeapon;
    public float meleeAttackDistance;
    private int meleeAttackNumber;
    public LayerMask enemyLayer;

    public WeaponSound _weaponSound;
    private WeaponSound playerWeaponSound;

    [Header("Equipment Attributes")] 
    public int defense;
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
        textUI = GetComponent<WorldTextUI>();

        if (itemScriptableObject != null || weaponScriptableObject != null /*|| equipmentScriptableObject*/)
        {
            //OBTENEMOS LOS DATOS DEL ITEM SEGUN CLASE
            switch (itemClass)
            {
                case ItemClass.Item:
                    GetItemScriptableObject(itemScriptableObject);
                    break;

                case ItemClass.Weapon:
                    GetWeaponScriptableObject(weaponScriptableObject);
                    break;

                case ItemClass.Equipment:
                    break;
            }

            textUI.text = itemName;
            itemModel = InstantiateItem(itemPrefab);
            textUI.target = itemModel.transform;
            _boxCollider = GetComponent<BoxCollider>();
        }
        else
        {
            Debug.Log("No Scriptable Object attached on " + gameObject.name);
        }
    }

    private void Start()
    {
        textUI.uiDocument.enabled = false;
        outline.enabled = false;
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
                break;
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

    public void GetItemScriptableObject(ItemScriptableObject itemScriptableObject)
    {
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

    public void GetWeaponScriptableObject(WeaponScriptableObject weaponScriptableObject)
    {
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


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            switch (itemLocation)
            {
                case ItemLocation.World:
                    outline.enabled = true;
                    textUI.uiDocument.enabled = true;
                    break;
            }
        }

        if (weaponClass == WeaponScriptableObject.WeaponClass.Throwable)
        {
            if (other.CompareTag("Ground") && throwableActive)
            {
            }
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
                    textUI.uiDocument.enabled = false;
                    break;
            }
        }
    }
}