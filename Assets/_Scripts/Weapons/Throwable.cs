using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throwable : MonoBehaviour
{
    public WeaponItem _weaponItem;
    public GameObject _explosionParticle;
    public GameObject ignitionParticle;
    public MeshRenderer objectRenderer;
    public Collider throwableCollider;
    public Rigidbody _rigidbody;
    public GameObject _impactFlameParticle;
    public float explosionRadius;
    public float explosionForce;
    private PlayerController _playerController;
    private WeaponSound _weaponSound;
    public bool explosiveArmed;
    private bool exploded;
    private float detonateTimer;
    public float detonationTime = 5;
    public float disableTime = 10;
    private float disableTimer;

    private void Awake()
    {
        _weaponItem = GetComponent<WeaponItem>();
        _rigidbody = GetComponent<Rigidbody>();
        ignitionParticle.SetActive(false);
    }

    private void OnEnable()
    {
        detonateTimer = detonationTime;
        disableTimer = disableTime;
        _playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        _weaponSound = GetComponent<WeaponSound>();
        exploded = false;

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
        if (explosiveArmed && !exploded)
        {
            detonateTimer -= Time.deltaTime;
            if (detonateTimer <= 0)
            {
                Explode();
            }
        }
        
        
        if (exploded)
        {
            explosiveArmed = false;
            disableTimer -= Time.deltaTime;
            if (disableTimer <= 0)
            {
                exploded = false;
                disableTimer = disableTime;
                detonateTimer = detonationTime;
                gameObject.SetActive(false);
                
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("Ground") || other.collider.CompareTag("Enemy") )
        {
            if (_weaponItem.ID == 6002)//molotov
            {
                if (explosiveArmed)
                {
                    ignitionParticle.SetActive(false);
               
                    _explosionParticle.transform.position = other.GetContact(0).point;
                    _explosionParticle.transform.parent = null;
                    Explode();
                    foreach (ContactPoint contactPoint in other.contacts)
                    {
                        GameObject damageFlame = ObjectPool.SharedInstance.GetPooledObject("DamageFlame");
                        damageFlame.transform.position = contactPoint.point;
                        damageFlame.SetActive(true);
                    }
                } 
            }
            
        }
    }

    public void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position,explosionRadius);

        foreach (var objectsInRange in colliders)
        {
            Rigidbody rigidbody = objectsInRange.GetComponent<Rigidbody>();

            if (rigidbody != null)
            {
                rigidbody.AddExplosionForce(explosionForce*100,transform.position,explosionRadius);
            }

            HealthManager healthManager = objectsInRange.GetComponent<HealthManager>();
            if (healthManager != null)
            {
                healthManager.currentHealth -= _weaponItem.damage;
            }            
        }
        ignitionParticle.SetActive(false);
        _weaponSound.ExplosiveSound();
        _rigidbody.freezeRotation = true;
        _rigidbody.velocity = Vector3.zero;
        _explosionParticle.SetActive(true);
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
            detonateTimer = detonationTime;
            transform.parent = null;
            Slot throwableSlot = _playerController.gameManager.uiManager.throwableEquipSlot.GetComponent<Slot>();
            throwableSlot.UpdateWeaponSlot(null);
            throwableCollider.enabled = true;
            _weaponItem.weaponEquipped = false;
            _weaponItem.weaponLocation = WeaponItem.WeaponLocation.Throwed;
            throwableCollider.isTrigger = false;
            ignitionParticle.SetActive(true);
            _rigidbody.useGravity = true;
            
            Vector3 throwDirection = _playerController.targetTransform.position - _playerController.transform.position;
            _rigidbody.AddForce(throwDirection * _playerController.throwForce, ForceMode.Impulse);
        }   
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position,explosionRadius);
    }
}
