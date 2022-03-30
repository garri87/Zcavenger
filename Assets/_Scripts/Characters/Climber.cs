using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Climber : MonoBehaviour
{
 private Transform targetLedgeTransform;
 public Climbable climbable;
 public LedgeDetector ledgeDetector;
 private PlayerController _playerController;
 private Animator _animator;
 public Climbable.Orientation orientation;
 public Climbable.LedgeType ledgeType;
 
 public bool climbingLedge;
 public bool attachedToLedge;
 public float transitionSpeed = 5;
 
 public float xOffset, yOffset;


 private void Start()
 {
  _playerController = GetComponent<PlayerController>();
  _animator = GetComponent<Animator>();
 }
 
 private void Update()
 {
  if (ledgeDetector.ledgeDetected)
  {
   targetLedgeTransform = ledgeDetector.ledgeTransform;
   climbable = ledgeDetector.climbable;
   ledgeType = climbable.ledgeType;
   orientation = climbable.orientation;

   if (Input.GetKey(_playerController.keyAssignments.jumpKey.keyCode) && _playerController.@descending)
   {
    if (climbable.orientation != Climbable.Orientation.Back)
    {
     if (ledgeDetector.ledge.rotation.y != transform.rotation.y)
     {
      AttachOnLedge();
     }
    }
    else
    {
     AttachOnLedge();
    }
    
    
    
   }
  }

  if (Input.GetKeyUp(_playerController.keyAssignments.jumpKey.keyCode))
  { 
   if (!climbingLedge) 
   {
    attachedToLedge = false;
   }
  }

  if (attachedToLedge)
  {
   if (!climbingLedge)
   {
    AttachOnLedge();
   }

   if (_playerController.verticalInput > 0.2f)
   {
    if (climbable.ledgeType == Climbable.LedgeType.HangerHangOnly ||
        climbable.ledgeType == Climbable.LedgeType.WallHangOnly)
    {
     climbingLedge = true;
    }
    else
    {
     if (climbable.ledgeType == Climbable.LedgeType.HangerRailing ||
         climbable.ledgeType == Climbable.LedgeType.WallRailing)
     {
      if (_playerController.onDoor)
      {
       climbingLedge = true;
      }
     }
    }
   }
  }
  else
  {
   _animator.SetBool("OnLedge", false);
   _animator.SetBool("OnWall", false);
  }
  _animator.SetBool("ClimbLedge", climbingLedge);
 }

 public void AttachOnLedge()
 {
  attachedToLedge = true;
  if (climbable.ledgeType == Climbable.LedgeType.HangerRailing ||
      climbable.ledgeType == Climbable.LedgeType.HangerHangOnly)
  {
   _animator.SetBool("OnLedge", true);
   xOffset = 0;
  }

  if (climbable.ledgeType == Climbable.LedgeType.WallRailing ||
      climbable.ledgeType == Climbable.LedgeType.WallHangOnly)
  {
   _animator.SetBool("OnWall", true);
   if (climbable.orientation == Climbable.Orientation.Left)
   {
    xOffset = -0.3f;

   }
   else
   {
    xOffset = 0.3f;

   }
  }
  
  if (climbable.orientation == Climbable.Orientation.Left || climbable.orientation == Climbable.Orientation.Right)
  {
   if (!climbingLedge)
   {
    transform.position = new Vector3(targetLedgeTransform.position.x + xOffset, targetLedgeTransform.position.y + yOffset, transform.position.z);
   }
  }
  else
  {
   if (!climbingLedge)
   {
    transform.position = new Vector3(transform.position.x, targetLedgeTransform.position.y + yOffset, transform.position.z);
   }
  }
  transform.rotation = quaternion.LookRotation(-targetLedgeTransform.forward, Vector3.up);
  
 }


 public void LedgeClimbStart()
 {
  AttachOnLedge();
  transform.position = new Vector3(targetLedgeTransform.position.x + xOffset, targetLedgeTransform.position.y + yOffset, transform.position.z);
  _playerController.SetColliderShape(PlayerController.PlayerState.IsClimbingLedge);
  if (climbable.orientation == Climbable.Orientation.Back)
  {
   if (_playerController.onDoor)
   {
    transform.position = new Vector3(_playerController.doorScript.doorTransform.position.x, transform.position.y, transform.position.z);
   }
  }
 }
 public void LedgeClimbEnd()
 {
  transform.position = new Vector3(transform.position.x, targetLedgeTransform.position.y, transform.position.z);
  transform.position = Vector3.MoveTowards(transform.position,targetLedgeTransform.position, Time.deltaTime*transitionSpeed);
  attachedToLedge = false;
  climbingLedge = false;

  if (_playerController.onDoor)
  {
   _playerController.SwitchPlayLine(_playerController.doorScript.insidePlayLine);
  }
  _playerController.SetColliderShape(PlayerController.PlayerState.Default);

 }
 
 private void OnDrawGizmos()
 {
  
 }
}
