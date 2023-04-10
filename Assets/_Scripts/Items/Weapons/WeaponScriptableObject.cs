using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New_Weapon", menuName = "Items/New Weapon")]
public class WeaponScriptableObject : ScriptableObject
{
   
   public enum WeaponClass
   {
      None,
      Primary,
      Secondary,
      Melee,
      Throwable,
   }
   public  WeaponClass weaponClass;
   public int ID;
   public Sprite weaponIcon;
   public string weaponName;
   public string description;
   public int bulletID;

   [Header("Weapon Effects")] 
   public GameObject bulletImpactPrefab;
   public GameObject enemyImpactPrefab;
   public GameObject muzzleFlashPrefab;

   [Header("Weapon Attributes")] 
   public int damage;
   public float fireRate;
   public int bulletsPerShot;
   public int magazineCap;
   public float minFireAngle;
   public float maxFireAngle;
   public float recoilDuration;
   public float recoilMaxRotation;
   public bool blockAttacks = false;

   [Header("Throwable Attributes")]
   public float explosionRange;
   
}
