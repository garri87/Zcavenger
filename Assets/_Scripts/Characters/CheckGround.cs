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

  private void Update()
  {
    playerAnimator.SetBool("IsGrounded",isGrounded);
  }

  private void SetGrounded(Collider collider, bool value)
  {
    if (collider.CompareTag("Ground") || collider.CompareTag("Crashable"))
    {
      isGrounded = value;
    }
  }

  private void OnTriggerEnter(Collider other)
  {
    SetGrounded(other,true);
  }

  private void OnTriggerStay(Collider other)
  {
    if (!isGrounded)
    {
      SetGrounded(other,true);
    }
    
  }

  private void OnTriggerExit(Collider other)
  {
    SetGrounded(other,false);
  }
}
