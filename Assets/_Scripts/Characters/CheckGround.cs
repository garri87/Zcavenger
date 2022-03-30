using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckGround : MonoBehaviour
{

  public bool isGrounded;
  private Animator playerAnimator;
  
  private void Start()
  {
    playerAnimator = GetComponentInParent<Animator>();
  }

  private void OnTriggerStay(Collider other)
  {
    if (other.CompareTag("Ground") || other.CompareTag("Crashable"))
    {
      isGrounded = true;
      playerAnimator.SetBool("IsGrounded",isGrounded);
    }
  }

  private void OnTriggerExit(Collider other)
  {
    if (other.CompareTag("Ground") || other.CompareTag("Crashable"))
    {
      isGrounded = false;
      playerAnimator.SetBool("IsGrounded",isGrounded);
    }
  }
}
