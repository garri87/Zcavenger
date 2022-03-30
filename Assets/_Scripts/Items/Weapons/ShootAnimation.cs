using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootAnimation : MonoBehaviour
{
    public WeaponItem weaponItem;
    public Animator _animator;
    public KeyAssignments keyAssignments;
    private void Start()
    {
        keyAssignments = KeyAssignments.SharedInstance;
        _animator.GetComponent<Animator>();
    }

    private void Update()
    {
        if (weaponItem.bulletsInMag > 0 && Input.GetKeyDown(keyAssignments.attackKey.keyCode))
        {
            _animator.SetBool("Shoot", true);
        }
        else
        {
            _animator.SetBool("Shoot", false);

        }
    }
}
