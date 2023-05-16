using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class IKHolder : MonoBehaviour
{
   
   public Transform leftHandTarget;
   public Transform rightHandTarget;
   public Transform leftElbowHint;
   public Transform rightElbowHint;
   
   private PlayerController _playerController;
   private Animator _animator;

   private float leftHandIKWeight;
   private float rightHandIKWeight;

   public Vector3 leftHandOffset;
   public Vector3 rightHandOffset;
   
   public Vector3 leftElbowOffset;
   public Vector3 rightElbowOffset;

   public Transform handguardTarget;
   public Transform handguarTargetHint;

   private void Start()
   {
      _playerController = GetComponent<PlayerController>();
      _animator = GetComponent<Animator>();
   }

   private void Update()
   {
      if (_playerController.weaponOnHands)
      {
         rightHandTarget = _playerController.drawnWeaponItem.gripTransform;
         
         leftHandTarget = _playerController.drawnWeaponItem.handguardTransform; 
         leftHandIKWeight = _animator.GetFloat("LeftHandIKWeight"); 
         rightHandIKWeight = _animator.GetFloat("RightHandIKWeight");
         
      }
   }

   private void OnAnimatorIK(int layerIndex)
   {
      if (_playerController.weaponOnHands)
      {
          handguardTarget.position = _playerController.drawnWeaponItem.handguardTransform.position;

          // _animator.SetIKPosition(AvatarIKGoal.RightHand,rightHandTarget.position);
         // _animator.SetIKRotation(AvatarIKGoal.RightHand,transform.rotation);
        // _animator.SetIKHintPosition(AvatarIKHint.RightElbow,rightElbowHint.position);
        // _animator.SetIKPositionWeight(AvatarIKGoal.RightHand,rightHandIKWeight);
         // _animator.SetIKRotationWeight(AvatarIKGoal.RightHand,rightHandIKWeight);
       //  _animator.SetIKHintPositionWeight(AvatarIKHint.RightElbow,rightHandIKWeight);
         if (!_playerController.isAiming)
         {
            
         }
         
         
       //  _animator.SetIKPosition(AvatarIKGoal.LeftHand,leftHandTarget.position);
        // _animator.SetIKRotation(AvatarIKGoal.LeftHand,Quaternion.LookRotation(leftHandTarget.TransformDirection(Vector3.right),leftHandTarget.TransformDirection(Vector3.down)) );
        // _animator.SetIKHintPosition(AvatarIKHint.LeftElbow,leftElbowHint.position);
        // _animator.SetIKPositionWeight(AvatarIKGoal.LeftHand,leftHandIKWeight);
         //_animator.SetIKRotationWeight(AvatarIKGoal.LeftHand,leftHandIKWeight);
        // _animator.SetIKHintPositionWeight(AvatarIKHint.LeftElbow,leftHandIKWeight);
         

         if (_playerController.isAiming)
         {
             
             
             
             
        //    _animator.SetIKRotation(AvatarIKGoal.RightHand,Quaternion.LookRotation(transform.TransformDirection(Vector3.forward), transform.TransformDirection(Vector3.up)));
         }
         
      }
   }
}
