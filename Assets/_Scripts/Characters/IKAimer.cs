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
      rigBuilder = GetComponentInParent<RigBuilder>();  

      spineAimConstraint.data.constrainedObject = _animator.GetBoneTransform(HumanBodyBones.Spine);
      chestAimConstraint.data.constrainedObject = _animator.GetBoneTransform(HumanBodyBones.Chest);
      headAimConstraint.data.constrainedObject = _animator.GetBoneTransform(HumanBodyBones.Head);
      rightHandAimConstraint.data.constrainedObject = _animator.GetBoneTransform(HumanBodyBones.RightHand);


      aimConstraints = new List<MultiAimConstraint>()
      {
          spineAimConstraint,
          chestAimConstraint,
          headAimConstraint
      };

        boneConstraints = new List<TwoBoneIKConstraint>()
        {
            leftHandBoneConstraint,
            rightHandBoneConstraint
        };

        WeightedTransform targetSource = new WeightedTransform
        {
            transform = _playerController.crosshairTransform,
            weight = 1f
        };
        foreach (MultiAimConstraint constraint in aimConstraints) //Add crosshair to aim constraints as target
        {
            var data = constraint.data.sourceObjects;
            data.Clear();
            data.Add(targetSource);
            constraint.data.sourceObjects = data;

        }
        rigBuilder.Build();


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

    }

   private void Update()
   {
     // if (_playerController.drawnWeaponItem != null) _weaponItem = _playerController.drawnWeaponItem;
            
      AimAtTarget();       
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
                leftHandBoneConstraint.data.target = weapon.handguardTransform;
                rightHandBoneConstraint.data.target = weapon.gripTransform;

                rigBuilder.Build();
            }
            catch
            {

                
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

