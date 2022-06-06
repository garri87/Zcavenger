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
    _animator = GetComponentInParent<Animator>();
  }

  private void FixedUpdate()
  {
    _animator.SetBool("IsGrounded",isGrounded);
    
    Ray ray = new Ray(transform.position + new Vector3(0,0.1f,0), Vector3.down * distance);
    RaycastHit hit;
    if (Physics.Raycast(ray,out hit, distance))
    {
      isGrounded = true;
    }
    else
    {
      isGrounded = false;
    }
    
  }

  

  private void OnDrawGizmosSelected()
  {
    Gizmos.color = Color.yellow;
    Gizmos.DrawRay(transform.position + new Vector3(0,0.1f,0), Vector3.down * distance);
  }

  /*private void OnTriggerStay(Collider other)
  {
    if (!isGrounded)
    {
      if (other.CompareTag("Ground") || other.CompareTag("Crashable"))
      {
        isGrounded = true;
        playerAnimator.SetBool("IsGrounded",isGrounded);
      }
    }
    
  }*/
  
}
