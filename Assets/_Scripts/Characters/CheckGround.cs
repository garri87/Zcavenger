using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckGround : MonoBehaviour
{

  public bool isGrounded;
  private Animator _animator;
  public float distance = 0.2f;
  
  private void Start()
  {
    try
    {
      _animator = GetComponentInParent<Animator>();
    }
    catch 
    {
      Debug.LogWarning("No animator component found" );
    }
  }

  private void FixedUpdate()
  {

    isGrounded = IsGrounded();

    _animator.SetBool("IsGrounded",isGrounded);
        
  }

/// <summary>
/// Determines if object is grounded
/// </summary>
/// <returns></returns>
  private bool IsGrounded()
  {
    Ray ray = new Ray(transform.position + new Vector3(0,0.1f,0), Vector3.down * distance);
    return Physics.Raycast(ray, distance);
  }

  private void OnDrawGizmosSelected()
  {
    Gizmos.color = Color.yellow;
    Gizmos.DrawRay(transform.position + new Vector3(0,0.1f,0), Vector3.down * distance);
  }  
}
