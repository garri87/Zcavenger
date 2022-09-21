using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    private Rigidbody[] _rigidbodies;
    private Animator _animator;
    private CapsuleCollider[] _capsuleColliders;
    private BoxCollider[] _boxColliders;
    
    private void Start()
    {
        _animator = GetComponentInParent<Animator>();
        _rigidbodies = transform.GetComponentsInChildren<Rigidbody>();
        _boxColliders = transform.GetComponentsInChildren<BoxCollider>();
        _capsuleColliders= transform.GetComponentsInChildren<CapsuleCollider>();
    }

    private void Update()
    {
        if (_animator.enabled)
        {
            foreach (Rigidbody rb in _rigidbodies)
            {
                rb.isKinematic = true;
            }
            foreach (BoxCollider box in _boxColliders)
            {
                box.enabled = false;
            } 
            foreach (CapsuleCollider capsule in _capsuleColliders)
            {
                capsule.enabled = false;
            }
        }
        else
        {
            foreach (Rigidbody rb in _rigidbodies)
            {
                rb.isKinematic = false;
            }
            foreach (BoxCollider box in _boxColliders)
            {
                box.enabled = true;
            } foreach (CapsuleCollider capsule in _capsuleColliders)
            {
                capsule.enabled = true;
            }

        }
    }
}
