using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;

public class IKAimer : MonoBehaviour
{

  public RigBuilder rigBuilder;
   public Animator _animator;
   private PlayerController _playerController;
   private Inventory _inventory;
      
   public TwoBoneIKConstraint leftHandBoneConstraint,rightHandBoneConstraint;

   private List<TwoBoneIKConstraint> boneConstraints;

   public Transform rightHandBone;
   public Transform headBone;
   public Transform spine3Bone;
   public Transform hipsBone;
   
   public MultiAimConstraint 
      spineAimConstraint, 
      chestAimConstraint, 
      headAimConstraint, 
      rightHandAimConstraint;
   private List<MultiAimConstraint> aimConstraints;


   //public Transform targetTransform;
   public float[] aimWeights;

   [SerializeField] private float targetDistance;
   public float minDistance = 1f;
   public float aimTime = 1.2f;

   public int[] oneHandedWeapons;
   
   
   private void Awake()
   {
     // _animator = GetComponentInParent<Animator>();
      _playerController = GetComponent<PlayerController>();
      _inventory = GetComponent<Inventory>();
   //   rigBuilder = GetComponentInParent<RigBuilder>();  

      //spineAimConstraint.data.constrainedObject = _animator.GetBoneTransform(HumanBodyBones.Spine);
      //chestAimConstraint.data.constrainedObject = _animator.GetBoneTransform(HumanBodyBones.Chest);
      //headAimConstraint.data.constrainedObject = _animator.GetBoneTransform(HumanBodyBones.Head);
      //rightHandAimConstraint.data.constrainedObject = _animator.GetBoneTransform(HumanBodyBones.RightHand);


      aimConstraints = new List<MultiAimConstraint>()
      {
          spineAimConstraint,
          chestAimConstraint,
          headAimConstraint,
          rightHandAimConstraint
      };

        boneConstraints = new List<TwoBoneIKConstraint>()
        {
            leftHandBoneConstraint,
            rightHandBoneConstraint
        };

        oneHandedWeapons = new int[]
        {
           2004,
           2005,
           1001,
           6004,
           6002
           
        };
   }

    private void Start()
   {
      aimWeights = new float[aimConstraints.Count]; 

        for (int i = 0; i < aimConstraints.Count; i++) //Save the weights parameters of the editor
        {
            aimWeights[i] = aimConstraints[i].weight;
        }

        foreach (MultiAimConstraint aimConstraint in aimConstraints)
        {
            aimConstraint.weight = 0;
        }

        foreach (var bone in boneConstraints)
        {
           bone.weight = 0;
        }

    }

   private void Update()
   {
      if (_inventory.drawnWeaponItem && !_playerController.isReloadingWeapon)
      {
         AimAtTarget();
         if (!_playerController._healthManager.isBleeding || 
             _playerController.isAiming)
         {
               ConstraintLeftHand(_inventory.drawnWeaponItem);
         }
         if (oneHandedWeapons.Contains(_inventory.drawnWeaponItem.ID) && !_playerController.isAiming)
         {
             
            foreach (var bone in boneConstraints)
            {
               DecreaseConstraintWeight(bone);
            }
         }
         else
         {
            if (_inventory.drawnWeaponItem.weaponClass != WeaponScriptableObject.WeaponClass.Throwable)
            {
               foreach (var bone in boneConstraints)
            {
               IncreaseConstraintWeight(bone);
            }
               
            }
         }
        
         
      }
      else
      {
         foreach (var bone in boneConstraints)
         {
            DecreaseConstraintWeight(bone);
         }

         foreach (var aimConstraint in aimConstraints)
         {
            aimConstraint.weight -= Time.deltaTime * aimTime;
         }
      }
    }
   
   /// <summary>
   /// Controls aim constraints to face a target
   /// </summary>
   private void AimAtTarget()
   {
      targetDistance = Vector3.Distance(_playerController._weaponHolder.position,
         _playerController.crosshairTransform.position);
    
      if (_playerController.isAiming 
          && targetDistance > minDistance)
      {
         if (_inventory.drawnWeaponItem.weaponClass == WeaponScriptableObject.WeaponClass.Throwable)
         {
            chestAimConstraint.data.aimAxis = MultiAimConstraintData.Axis.X_NEG;
         }
         else
         {
            chestAimConstraint.data.aimAxis = MultiAimConstraintData.Axis.Z;
         }
         if (_playerController.isProne)
         {
            rightHandAimConstraint.weight += Time.deltaTime * aimTime;
            headAimConstraint.weight += Time.deltaTime * aimTime;
         }
         else
         {

            for (int i = 0; i < aimConstraints.Count; i++)
            {
               if (aimConstraints[i].weight < aimWeights[i])
               {
                  aimConstraints[i].weight += Time.deltaTime * aimTime;
               }
            }
         }
      }
      if (targetDistance < minDistance)
      {
         foreach (MultiAimConstraint aimConstraint in aimConstraints) // Gradually decreases aim constraints weights if target is lower than the minimum aim distance
         {
            aimConstraint.weight -= Time.deltaTime / aimTime;
         }
      }

      if (!_playerController.isAiming || !_inventory.drawnWeaponItem) 
            //If not aiming or not weapon on hands, decrease the aim constraint weights
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

   
   public void ConstraintLeftHand(Item weapon)
   {
            try
            {
               leftHandBoneConstraint.data.target.position = weapon.handguardTransform.position;
               leftHandBoneConstraint.data.target.rotation = weapon.handguardTransform.rotation;

               //rightHandBoneConstraint.data.target.position = weapon.gripTransform.position;
            }
            catch (Exception e)
            {
               Debug.Log(e);
            }
   }

   /// <summary>
   /// Gradually decrease a TwoBoneIKConstraint to 0
   /// </summary>
   /// <param name="boneIKConstraint"></param>
   private void DecreaseConstraintWeight(TwoBoneIKConstraint boneIKConstraint)
   {
      if (boneIKConstraint.weight > 0)
      {
         boneIKConstraint.weight -= Time.deltaTime * aimTime;
      }
      else
      {
         boneIKConstraint.weight = 0;
      }
   }

   /// <summary>
   /// Gradually increase a TwoBoneIKConstraint to 1
   /// </summary>
   /// <param name="boneIKConstraint"></param>
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

