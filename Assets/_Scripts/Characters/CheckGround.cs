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

<<<<<<< HEAD
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

=======
>>>>>>> parent of ec30881 (Update V0.4 Alpha)
  /*private void OnTriggerStay(Collider other)
  {
    if (!isGrounded)
    {
      SetGrounded(other,true);
    }
    
  }*/

  private void OnTriggerExit(Collider other)
  {
    SetGrounded(other,false);
  }
}
