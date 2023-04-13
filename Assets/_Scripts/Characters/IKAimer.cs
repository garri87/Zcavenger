using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class IKAimer : MonoBehaviour
{
   private Animator _animator;
   private PlayerController _playerController;
   private Item _weaponItem;
   
   private IKManager _ikManager;
   
   public TwoBoneIKConstraint leftHandConstraint;

   public MultiAimConstraint 
      spineAimConstraint, 
      chestAimConstraint, 
      headAimConstraint, 
      rightHandAimConstraint;
   
   private List<MultiAimConstraint> aimConstraints;
   public float[] aimWeights;

  [SerializeField] private float targetDistance;
   public float minDistance;
   public float aimTime = 2;
   
   private void Awake()
   {
      _animator = GetComponent<Animator>();
      _playerController = GetComponent<PlayerController>();
      _ikManager = GetComponent<IKManager>();
      aimConstraints = new List<MultiAimConstraint>();
      aimConstraints.Add(spineAimConstraint);
      aimConstraints.Add(chestAimConstraint);
      aimConstraints.Add(headAimConstraint);
      aimConstraints.Add(rightHandAimConstraint);
      
      
   }

   private void Start()
   {
      aimWeights = new float[aimConstraints.Count];
      
      for (int i = 0; i < aimConstraints.Count; i++)
      {
         aimWeights[i] = aimConstraints[i].weight;
      }
      
   }

   private void Update()
   {
      if (_playerController.equippedWeaponItem != null) _weaponItem = _playerController.equippedWeaponItem;
      
      ConstraintLeftHand();
      
      AimAtTarget();
      
   }
   
   private void AimAtTarget()
   {
      targetDistance = Vector3.Distance(_playerController._WeaponHolder.position,
         _playerController.crosshairTransform.position);
    
      if (_playerController.isAiming 
          && targetDistance > minDistance)
      {
         if (_weaponItem.weaponClass != WeaponScriptableObject.WeaponClass.Throwable)
         {
            for (int i = 0; i < aimConstraints.Count; i++) 
            { 
               if (aimConstraints[i].weight < aimWeights[i]) 
               {
                  aimConstraints[i].weight += Time.deltaTime * aimTime;
               }
               //aimConstraints[i].weight = aimWeights[i];
            } 
         }
         else
         {
            
           
         }
      }
      if (targetDistance < minDistance)
      {
         foreach (MultiAimConstraint aimConstraint in aimConstraints)
         {
            aimConstraint.weight -= Time.deltaTime / aimTime;
         }
      }

      if (!_playerController.isAiming || !_playerController.weaponEquipped)
      {
         foreach (MultiAimConstraint aimConstraint in aimConstraints)
         {
            if (aimConstraint.weight > 0)
            {
               aimConstraint.weight -= Time.deltaTime * aimTime;
            }
            else
            {
               aimConstraint.weight = 0;
            }
         }
      }
   }

   private void ConstraintLeftHand()
   {
      if (_playerController.weaponEquipped && !_playerController._healthManager.isBleeding || _playerController.isAiming)
      {
         if (_weaponItem.weaponClass == WeaponScriptableObject.WeaponClass.Primary && !_playerController.reloadingWeapon)
         {
            IncreaseConstraintWeight(leftHandConstraint);
         }
         else
         {
            DecreaseConstraintWeight(leftHandConstraint);
         }
      }
      else
      {
         DecreaseConstraintWeight(leftHandConstraint);
      }
   }

   private void DecreaseConstraintWeight(TwoBoneIKConstraint boneIKConstraint)
   {
      if (boneIKConstraint.weight > 0)
      {
         boneIKConstraint.weight -= Time.deltaTime / aimTime;
      }
      else
      {
         boneIKConstraint.weight = 0;
      }
   }

   private void IncreaseConstraintWeight(TwoBoneIKConstraint boneIKConstraint)
   {
      if (boneIKConstraint.weight < 1)
      {
         boneIKConstraint.weight += Time.deltaTime * aimTime;
      }
      else
      {
         boneIKConstraint.weight = 1;
      }
   }
   
}

