using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;

public class IKAimer : MonoBehaviour
{

  public RigBuilder rigBuilder;
   public Animator _animator;
   private PlayerController _playerController;
      
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
   
   private void Awake()
   {
     // _animator = GetComponentInParent<Animator>();
      _playerController = GetComponentInParent<PlayerController>();
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
      AimAtTarget();

      if (_playerController.drawnWeaponItem)
      {
         ConstraintHands(_playerController.drawnWeaponItem);
         foreach (var bone in boneConstraints)
         {
            IncreaseConstraintWeight(bone);
         }
      }
      else
      {
         foreach (var bone in boneConstraints)
         {
            DecreaseConstraintWeight(bone);
         }
      }
    }
   
   private void AimAtTarget()
   {
      
      targetDistance = Vector3.Distance(_playerController._WeaponHolder.position,
         _playerController.crosshairTransform.position);
    
      if (_playerController.isAiming 
          && targetDistance > minDistance)
      {
         if (_playerController.drawnWeaponItem.weaponClass != WeaponScriptableObject.WeaponClass.Melee)
         {
            

            for (int i = 0; i < aimConstraints.Count; i++) 
            { 
               if (aimConstraints[i].weight < aimWeights[i]) 
               {
                  aimConstraints[i].weight += Time.deltaTime * aimTime;
               }
               
            } 

         

            

         }
         else
         {
            
           
         }
      }
      if (targetDistance < minDistance)
      {
         foreach (MultiAimConstraint aimConstraint in aimConstraints) // Gradually decreases aim constraints weights if target is lower than the minimum aim distance
         {
            aimConstraint.weight -= Time.deltaTime / aimTime;
         }
      }

      if (!_playerController.isAiming || !_playerController.weaponOnHands) 
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

   
   public void ConstraintHands(Item weapon)
   {
            try
            {
               leftHandBoneConstraint.data.target.position = weapon.handguardTransform.position;
               //rightHandBoneConstraint.data.target.position = weapon.gripTransform.position;
            }
            catch (Exception e)
            {
               Debug.Log(e);
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

