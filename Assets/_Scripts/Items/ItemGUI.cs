using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(Item))]
public class ItemGUI : Editor
{
    Item item;

    //private SerializedObject _object;
    private SerializedProperty _itemClass;
    private SerializedProperty _scriptableObject;
    private SerializedProperty _ID;
    private SerializedProperty _itemName;
    private SerializedProperty _description;
    private SerializedProperty _itemIcon;
    private SerializedProperty _itemPrefab;
    private SerializedProperty _quantity;
    private SerializedProperty _itemPickedUp;
    private SerializedProperty _itemEquipped;
    private SerializedProperty _itemLocation;
    private SerializedProperty _itemModelGO;
    //private SerializedProperty _boxCollider;
    private SerializedProperty _outline;
    private SerializedProperty _itemTransform;
    private SerializedProperty _prefabRotationSpeed;
    private SerializedProperty _worldTextUI;

    // Item variables
    private SerializedProperty _healthRestore;
    private SerializedProperty _foodRestore;
    private SerializedProperty _waterRestore;
    private SerializedProperty _usable;
    private SerializedProperty _consumable;
    private SerializedProperty _isStackable;
    private SerializedProperty _maxStack;

    // Weapon variables
    private SerializedProperty _weaponClass;
    private SerializedProperty _bulletID;
    private SerializedProperty _damage;
    private SerializedProperty _fireRate;
    private SerializedProperty _magazineCap;
    private SerializedProperty _bulletsInMag;
    private SerializedProperty _recoilDuration;
    private SerializedProperty _recoilMaxRotation;
    private SerializedProperty _maxFireAngle;
    private SerializedProperty _minFireAngle;
    private SerializedProperty _bulletsPerShot;
    private SerializedProperty _blockAttacks;

    // Weapon Effects Variables
    private SerializedProperty _bulletImpactPrefab;
    private SerializedProperty _enemyImpactPrefab;
    private SerializedProperty _muzzleFlashPrefab;
    private SerializedProperty _flashLightTransform;
    private SerializedProperty _gunMuzzleTransform;
    private SerializedProperty _handguardTransform;
    private SerializedProperty _gripTransform;
    private SerializedProperty _muzzleCollider;
    private SerializedProperty _flashLight;
    private SerializedProperty _magHolder;
    private SerializedProperty _magGameObject;

    private SerializedProperty _totalBullets;
    private SerializedProperty _attacking;
    private SerializedProperty _attackNumber;
    private SerializedProperty _meleeAttackTimer;
    private SerializedProperty _firing;
    private SerializedProperty _aiming;
    private SerializedProperty _drawingWeapon;
    private SerializedProperty _meleeAttackDistance;
    //private SerializedProperty _meleeAttackNumber;
    private SerializedProperty _enemyLayer;

    private SerializedProperty _weaponSound;
    //private SerializedProperty _playerWeaponSound;

    // Equipment variables
    private SerializedProperty _defense;
    private SerializedProperty _targetBone;
   
    private SerializedProperty _backpackCapacity;
    private SerializedProperty _equipmentPrefab;

    //Player Components
   // private SerializedProperty _playerTransform;
   // private SerializedProperty _playerIKManager;
   // private SerializedProperty _playerInventory;
   // private SerializedProperty _playerAnimator;



    private List<SerializedProperty> _properties;

    private void OnEnable()
    {
        item = (Item)target;

        _itemClass = serializedObject.FindProperty("itemClass");
        _scriptableObject = serializedObject.FindProperty("scriptableObject");
        _ID = serializedObject.FindProperty("ID");
        _itemName = serializedObject.FindProperty("itemName");
        _description = serializedObject.FindProperty("description");
        _itemIcon = serializedObject.FindProperty("itemIcon");
        _itemPrefab = serializedObject.FindProperty("itemPrefab");
        _quantity = serializedObject.FindProperty("quantity");
        _itemPickedUp = serializedObject.FindProperty("itemPickedUp");
        _itemEquipped = serializedObject.FindProperty("itemEquipped");
        _itemLocation = serializedObject.FindProperty("itemLocation");
        _itemModelGO = serializedObject.FindProperty("itemModelGO");
        //_boxCollider = _object.FindProperty("_boxCollider");
        _outline = serializedObject.FindProperty("outline");
        _itemTransform = serializedObject.FindProperty("itemTransform");
        _prefabRotationSpeed = serializedObject.FindProperty("prefabRotationSpeed");
        _worldTextUI = serializedObject.FindProperty("worldTextUI");

        // Item variables
        _healthRestore = serializedObject.FindProperty("healthRestore");
        _foodRestore = serializedObject.FindProperty("foodRestore");
        _waterRestore = serializedObject.FindProperty("waterRestore");
        _usable = serializedObject.FindProperty("usable");
        _consumable = serializedObject.FindProperty("consumable");
        _isStackable = serializedObject.FindProperty("isStackable");
        _maxStack = serializedObject.FindProperty("maxStack");

        // Weapon variables
        _weaponClass = serializedObject.FindProperty("weaponClass");
        _bulletID = serializedObject.FindProperty("bulletID");
        _damage = serializedObject.FindProperty("damage");
        _fireRate = serializedObject.FindProperty("fireRate");
        _magazineCap = serializedObject.FindProperty("magazineCap");
        _bulletsInMag = serializedObject.FindProperty("bulletsInMag");
        _recoilDuration = serializedObject.FindProperty("recoilDuration");
        _recoilMaxRotation = serializedObject.FindProperty("recoilMaxRotation");
        _maxFireAngle = serializedObject.FindProperty("maxFireAngle");
        _minFireAngle = serializedObject.FindProperty("minFireAngle");
        _bulletsPerShot = serializedObject.FindProperty("bulletsPerShot");
        _blockAttacks = serializedObject.FindProperty("blockAttacks");

        _bulletImpactPrefab = serializedObject.FindProperty("bulletImpactPrefab");
        _enemyImpactPrefab = serializedObject.FindProperty("enemyImpactPrefab");
        _muzzleFlashPrefab = serializedObject.FindProperty("muzzleFlashPrefab");
        _flashLightTransform = serializedObject.FindProperty("flashLightTransform");
        _gunMuzzleTransform = serializedObject.FindProperty("gunMuzzleTransform");
        _handguardTransform = serializedObject.FindProperty("handguardTransform");
        _gripTransform = serializedObject.FindProperty("gripTransform");
        _muzzleCollider = serializedObject.FindProperty("muzzleCollider");
        _flashLight = serializedObject.FindProperty("flashLight");
        _magHolder = serializedObject.FindProperty("magHolder");
        _magGameObject = serializedObject.FindProperty("magGameObject");
        _totalBullets = serializedObject.FindProperty("totalBullets");
        _attacking = serializedObject.FindProperty("attacking");
        _attackNumber = serializedObject.FindProperty("attackNumber");
        _meleeAttackTimer = serializedObject.FindProperty("meleeAttackTimer");
        _firing = serializedObject.FindProperty("firing");
        _aiming = serializedObject.FindProperty("aiming");
        _drawingWeapon = serializedObject.FindProperty("drawingWeapon");
        _meleeAttackDistance = serializedObject.FindProperty("meleeAttackDistance");
        //_meleeAttackNumber = _object.FindProperty("meleeAttackNumber");
        _enemyLayer = serializedObject.FindProperty("enemyLayer");
        _weaponSound = serializedObject.FindProperty("_weaponSound");
        //_playerWeaponSound = _object.FindProperty("playerWeaponSound");

        //Outfit variables
       
        _defense = serializedObject.FindProperty("defense");
        _backpackCapacity = serializedObject.FindProperty("backpackCapacity");
        _equipmentPrefab = serializedObject.FindProperty("equipmentPrefab");

        //PlayerComponents
        //_playerTransform = _object.FindProperty("playerTransform");
        //_playerIKManager = _object.FindProperty("playerIKManager");
        //_playerInventory = _object.FindProperty("playerInventory");
        //_playerAnimator = _object.FindProperty("playerAnimator");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        GUI.enabled = false;
        SerializedProperty prop = serializedObject.FindProperty("m_Script");
        EditorGUILayout.PropertyField(prop, true, new GUILayoutOption[0]);
        GUI.enabled = true;

        EditorGUILayout.PropertyField(_itemClass);
        EditorGUILayout.PropertyField(_scriptableObject);
        EditorGUILayout.PropertyField(_ID);
        EditorGUILayout.PropertyField(_itemName);
        EditorGUILayout.PropertyField(_description);
        EditorGUILayout.PropertyField(_itemIcon);
        EditorGUILayout.PropertyField(_itemPrefab);
        EditorGUILayout.PropertyField(_quantity);
        EditorGUILayout.PropertyField(_itemPickedUp);
        EditorGUILayout.PropertyField(_itemEquipped);
        EditorGUILayout.PropertyField(_itemLocation);
        
        EditorGUILayout.PropertyField(_itemModelGO);
        //EditorGUILayout.PropertyField(_boxCollider);
        EditorGUILayout.PropertyField(_outline);
        EditorGUILayout.PropertyField(_itemTransform);
        EditorGUILayout.PropertyField(_prefabRotationSpeed);
        EditorGUILayout.PropertyField(_worldTextUI);

        

        switch (_itemClass.enumValueIndex)
        {
            case (int)Item.ItemClass.Item:
                EditorGUILayout.PropertyField(_healthRestore);
                EditorGUILayout.PropertyField(_foodRestore);
                EditorGUILayout.PropertyField(_waterRestore);
                EditorGUILayout.PropertyField(_usable);
                EditorGUILayout.PropertyField(_consumable);
                EditorGUILayout.PropertyField(_isStackable);
                EditorGUILayout.PropertyField(_maxStack);
                break;

            case (int)Item.ItemClass.Weapon:
                EditorGUILayout.PropertyField(_weaponClass);
                EditorGUILayout.PropertyField(_bulletID);
                EditorGUILayout.PropertyField(_damage);
                EditorGUILayout.PropertyField(_fireRate);
                EditorGUILayout.PropertyField(_magazineCap);
                EditorGUILayout.PropertyField(_bulletsInMag);
                EditorGUILayout.PropertyField(_recoilDuration);
                EditorGUILayout.PropertyField(_recoilMaxRotation);
                EditorGUILayout.PropertyField(_maxFireAngle);
                EditorGUILayout.PropertyField(_minFireAngle);
                EditorGUILayout.PropertyField(_bulletsPerShot);
                EditorGUILayout.PropertyField(_blockAttacks);
                //weapon effects
                EditorGUILayout.PropertyField(_bulletImpactPrefab);
                EditorGUILayout.PropertyField(_enemyImpactPrefab);
                EditorGUILayout.PropertyField(_muzzleFlashPrefab);
                //weapon transforms
                EditorGUILayout.PropertyField(_flashLightTransform);
                EditorGUILayout.PropertyField(_gunMuzzleTransform);
                EditorGUILayout.PropertyField(_handguardTransform);
                EditorGUILayout.PropertyField(_gripTransform);
                EditorGUILayout.PropertyField(_muzzleCollider);
                EditorGUILayout.PropertyField(_flashLight);
                EditorGUILayout.PropertyField(_magHolder);
                EditorGUILayout.PropertyField(_magGameObject);
                EditorGUILayout.PropertyField(_totalBullets);
                EditorGUILayout.PropertyField(_attacking);
                EditorGUILayout.PropertyField(_attackNumber);
                EditorGUILayout.PropertyField(_meleeAttackTimer);
                EditorGUILayout.PropertyField(_firing);
                EditorGUILayout.PropertyField(_aiming);
                EditorGUILayout.PropertyField(_drawingWeapon);
                EditorGUILayout.PropertyField(_meleeAttackDistance);
                //EditorGUILayout.PropertyField(_meleeAttackNumber);
                EditorGUILayout.PropertyField(_enemyLayer);
                EditorGUILayout.PropertyField(_weaponSound);
                //EditorGUILayout.PropertyField(_playerWeaponSound);
                break;

            case (int)Item.ItemClass.Outfit:
                
                EditorGUILayout.PropertyField(_defense);
                EditorGUILayout.PropertyField(_backpackCapacity);

                EditorGUILayout.PropertyField(_equipmentPrefab);

               


                break;
        }

        //EditorGUILayout.PropertyField(_playerTransform);
        //EditorGUILayout.PropertyField(_playerIKManager);
        //EditorGUILayout.PropertyField(_playerInventory);
        //EditorGUILayout.PropertyField(_playerAnimator);

        EditorUtility.SetDirty(target);
        serializedObject.ApplyModifiedProperties();
    }
}