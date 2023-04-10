using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

[CustomEditor(typeof(Item))]
public class ItemGUI : Editor
{
    Item item;
    private SerializedObject _object;
    private SerializedProperty _itemClass;
    private SerializedProperty _ID;
    private SerializedProperty _itemName;
    private SerializedProperty _description;
    private SerializedProperty _itemIcon;
    private SerializedProperty _itemPrefab;
    private SerializedProperty _quantity;
    private SerializedProperty _itemPickedUp;
    private SerializedProperty _itemEquipped;
    private SerializedProperty _itemLocation;
    private SerializedProperty _modelTransform;
    private SerializedProperty _boxCollider;
    private SerializedProperty _outline;
    private SerializedProperty _itemTransform;
    private SerializedProperty _prefabRotationSpeed;
    private SerializedProperty _uiDocument;
    private SerializedProperty _itemText;

    // Item variables
    private SerializedProperty _itemScriptableObject;
    private SerializedProperty _healthRestore;
    private SerializedProperty _foodRestore;
    private SerializedProperty _waterRestore;
    private SerializedProperty _usable;
    private SerializedProperty _consumable;
    private SerializedProperty _isStackable;
    private SerializedProperty _maxStack;

    // Weapon variables
    private SerializedProperty _weaponScriptableObject;
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

    // Equipment variables
    private SerializedProperty _defense;
    private SerializedProperty _targetBone;

    private List<SerializedProperty> _properties;

    private void OnEnable()
    {
        _object = new SerializedObject(target);

        _ID = _object.FindProperty("ID");
        _itemClass = _object.FindProperty("itemClass");
        _itemName = _object.FindProperty("itemName");
        _description = _object.FindProperty("description");
        _itemIcon = _object.FindProperty("itemIcon");
        _itemPrefab = _object.FindProperty("itemPrefab");
        _quantity = _object.FindProperty("quantity");
        _itemPickedUp = _object.FindProperty("itemPickedUp");
        _itemEquipped = _object.FindProperty("itemEquipped");
        _itemLocation = _object.FindProperty("itemLocation");
        _modelTransform = _object.FindProperty("modelTransform");
        _boxCollider = _object.FindProperty("_boxCollider");
        _outline = _object.FindProperty("outline");
        _itemTransform = _object.FindProperty("itemTransform");
        _prefabRotationSpeed = _object.FindProperty("prefabRotationSpeed");
        _uiDocument = _object.FindProperty("_uiDocument");
        _itemText = _object.FindProperty("itemText");

        // Item variables
        _itemScriptableObject = _object.FindProperty("itemScriptableObject");
        _healthRestore = _object.FindProperty("healthRestore");
        _foodRestore = _object.FindProperty("foodRestore");
        _waterRestore = _object.FindProperty("waterRestore");
        _usable = _object.FindProperty("usable");
        _consumable = _object.FindProperty("consumable");
        _isStackable = _object.FindProperty("isStackable");
        _maxStack = _object.FindProperty("maxStack");

        // Weapon variables
        _weaponScriptableObject = _object.FindProperty("weaponScriptableObject");
        _weaponClass = _object.FindProperty("weaponClass");
        _bulletID = _object.FindProperty("bulletID");
        _damage = _object.FindProperty("damage");
        _fireRate = _object.FindProperty("fireRate");
        _magazineCap = _object.FindProperty("magazineCap");
        _bulletsInMag = _object.FindProperty("bulletsInMag");
        _recoilDuration = _object.FindProperty("recoilDuration");
        _recoilMaxRotation = _object.FindProperty("recoilMaxRotation");
        _maxFireAngle = _object.FindProperty("maxFireAngle");
        _minFireAngle = _object.FindProperty("minFireAngle");
        _bulletsPerShot = _object.FindProperty("bulletsPerShot");
        _blockAttacks = _object.FindProperty("blockAttacks");
        _defense = _object.FindProperty("defense");
        _targetBone = _object.FindProperty("targetBone");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        GUI.enabled = false;
        SerializedProperty prop = serializedObject.FindProperty("m_Script");
        EditorGUILayout.PropertyField(prop, true, new GUILayoutOption[0]);
        GUI.enabled = true;
        EditorGUILayout.PropertyField(_itemClass);
        switch (_itemClass.enumValueIndex)
        {
            case (int)Item.ItemClass.Item:
                EditorGUILayout.PropertyField(_itemScriptableObject);
                break;

            case (int)Item.ItemClass.Weapon:
                EditorGUILayout.PropertyField(_weaponScriptableObject);
                break;

            case (int)Item.ItemClass.Equipment:
                break;
        }
        EditorGUILayout.PropertyField(_ID);
        EditorGUILayout.PropertyField(_itemName);
        EditorGUILayout.PropertyField(_description);
        EditorGUILayout.PropertyField(_itemIcon);
        EditorGUILayout.PropertyField(_itemPrefab);
        EditorGUILayout.PropertyField(_quantity);
        EditorGUILayout.PropertyField(_itemPickedUp);
        EditorGUILayout.PropertyField(_itemEquipped);
        EditorGUILayout.PropertyField(_itemLocation);
        EditorGUILayout.PropertyField(_modelTransform);
//        EditorGUILayout.PropertyField(_boxCollider);
        EditorGUILayout.PropertyField(_outline);
        EditorGUILayout.PropertyField(_itemTransform);
        EditorGUILayout.PropertyField(_prefabRotationSpeed);
        EditorGUILayout.PropertyField(_uiDocument);
//        EditorGUILayout.PropertyField(_itemText);

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
                break;

            case (int)Item.ItemClass.Equipment:
                EditorGUILayout.PropertyField(_defense);
                EditorGUILayout.PropertyField(_targetBone);
                break;
        }

        serializedObject.ApplyModifiedProperties();
    }
}