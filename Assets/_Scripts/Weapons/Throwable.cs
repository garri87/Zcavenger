using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throwable : MonoBehaviour
{
    public WeaponItem _weaponItem;
    public GameObject _explosionParticle;
    public GameObject bottleFireParticle;
    public MeshRenderer objectRenderer;
    public CapsuleCollider throwableCollider;
    public Rigidbody _rigidbody;
    public GameObject _impactFlameParticle;
    public ExplosionRadius explosionRadius;
    private PlayerController _playerController;
    private WeaponSound _weaponSound;
    public bool explosiveArmed;
    private bool exploded;
    public float disableTime = 10;
    private float disableTimer;

    private void Awake()
    {
    }

    private void OnEnable()
    {
        disableTimer = 0;
        _playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        _weaponSound = GetComponent<WeaponSound>();
        switch (_weaponItem.weaponLocation)
        {
            case WeaponItem.WeaponLocation.World:
                _rigidbody.constraints = RigidbodyConstraints.FreezeAll;
                break;
            
            case WeaponItem.WeaponLocation.Player:
                objectRenderer.enabled = true;
                _rigidbody.constraints = RigidbodyConstraints.FreezePositionZ;
                

                break;
        }
    }

    private void Start()
    {
        
    }
    
    private void Update()
    {
        if (exploded)
        {
            explosiveArmed = false;
            disableTimer += Time.deltaTime;
            if (disableTimer >= disableTime)
            {
                exploded = false;
                disableTimer = 0;
                gameObject.SetActive(false);
                
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("Ground") || other.collider.CompareTag("Enemy") )
        {
            if (explosiveArmed)
            {
                bottleFireParticle.SetActive(false);
                Explode();
                _explosionParticle.transform.position = other.GetContact(0).point;
                _explosionParticle.transform.parent = null;
                foreach (ContactPoint contactPoint in other.contacts)
                {
                    GameObject damageFlame = ObjectPool.SharedInstance.GetPooledObject("DamageFlame");
                    damageFlame.transform.position = contactPoint.point;
                    damageFlame.SetActive(true);
                    
                }
            }
        }
    }

    public void Explode()
    {
        _weaponSound.FireWeaponSound("Molotov");
        explosionRadius.gameObject.SetActive(true);
        bottleFireParticle.SetActive(false);
        objectRenderer.enabled = false;
        exploded = true;
        Debug.Log("Item exploded!");
        
    }

    public void ThrowObject(bool enabled)
    {
        if (enabled)
        {
            _playerController._animator.SetBool("ThrowableEquip", false);
            _playerController.isAiming = false;
            explosiveArmed = true;
            transform.parent = null;
            throwableCollider.enabled = true;
            _weaponItem.weaponEquipped = false;
            _weaponItem.weaponLocation = WeaponItem.WeaponLocation.Throwed;
            
            bottleFireParticle.SetActive(true);
            _rigidbody.useGravity = true;
            Vector3 throwDirection = _playerController.targetTransform.position - _playerController.transform.position;
            _rigidbody.AddForce(throwDirection * _playerController.throwForce, ForceMode.Impulse);
        }   
        
    }
}
