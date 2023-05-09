using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New_Weapon", menuName = "Items/New Weapon")]
public class WeaponScriptableObject : ScriptableObject
{



    [Header("Item ID")]
    public int ID;
    public string weaponName;
    public string description;
    public Sprite weaponIcon;

    [Header("Prefabs")]
    public GameObject weaponPrefab;

    public enum WeaponClass
    {
        None,
        Primary,
        Secondary,
        Melee,
        Throwable,
    }
    public WeaponClass weaponClass;

    public int bulletID;

    [Header("Properties")]
    public int damage;
    public float fireRate;
    public int bulletsPerShot;
    public int magazineCap;
    public float minFireAngle;
    public float maxFireAngle;
    public float recoilDuration;
    public float recoilMaxRotation;
    public bool blockAttacks = false;

    [Header("Throwable Properties")]
    public float explosionRange;

    [Header("Weapon Effects")]
    public GameObject bulletImpactPrefab;
    public GameObject enemyImpactPrefab;
    public GameObject muzzleFlashPrefab;

    [Header("Weapon Sounds")]
    public AudioClip shotSound;
    public AudioClip meleeAttackSound;
    public AudioClip dropSound;
    public AudioClip explosionSound;
    public AudioClip magazineOutSound;
    public AudioClip magazineInSound;
    public AudioClip reloadEndSound;
    public AudioClip drawWeaponSound;


}
