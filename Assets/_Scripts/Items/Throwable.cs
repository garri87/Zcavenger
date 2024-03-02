using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Throwable : MonoBehaviour
{
    public Item item;
    public GameObject _explosionParticle;
    public GameObject ignitionParticle;
    public MeshRenderer objectRenderer;
    public Collider throwableCollider;
    public Rigidbody _rigidbody;
    public GameObject _impactFlameParticle;
    public float explosionRadius;
    public float explosionForce;
    private WeaponSound _weaponSound;
    public bool explosiveArmed;
    private bool exploded;
    private float detonateTimer;
    public float detonationTime = 5;
    public float disableTime = 10;
    [SerializeField]private float disableTimer;
    
    
   
    
    private void Awake()
    {
        try
        {
            item = GetComponentInParent<Item>();
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
        
        _rigidbody = GetComponent<Rigidbody>();
        _weaponSound = GetComponent<WeaponSound>();
        ignitionParticle.SetActive(false);
    }

    private void OnEnable()
    {
        detonateTimer = detonationTime;
        disableTimer = disableTime;
        exploded = false;

        _rigidbody.isKinematic = true;

       switch (item.itemLocation)
        {
            case Item.ItemLocation.World:
                _rigidbody.constraints = RigidbodyConstraints.FreezeAll;
                break;
            
            case Item.ItemLocation.Player:
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
            throwableCollider.isTrigger = true; 
            if (disableTimer <= 0)
            {
                exploded = false;
                disableTimer = disableTime;
                detonateTimer = detonationTime;
                item.gameObject.SetActive(false);
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("Ground") || other.collider.CompareTag("Enemy") )
        {
            if (item.ID == 6002)//molotov
            {
                if (explosiveArmed)
                {
                    ignitionParticle.SetActive(false);
               
                    _explosionParticle.transform.position = other.GetContact(0).point;
                    
                    Explode();
                    foreach (ContactPoint contactPoint in other.contacts)
                    {
                        if (ObjectPool.SharedInstance) {
                            GameObject damageFlame = ObjectPool.SharedInstance.GetPooledObject("DamageFlame");
                            damageFlame.transform.position = contactPoint.point;
                            damageFlame.SetActive(true);
                        }
                      
                    }
                } 
            }
            item._weaponSound.DropSound();
        }
    }

    public void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position,explosionRadius);
        Transform originPos = this.transform;
        foreach (var objectsInRange in colliders)
        {
            Rigidbody rigidbody = objectsInRange.GetComponent<Rigidbody>();

            if (objectsInRange.CompareTag("Crashable"))
            {
                CrashObject crashObject = objectsInRange.GetComponent<CrashObject>();
                crashObject.Crash();
            }
            
            if (rigidbody)
            {
                rigidbody.AddExplosionForce(explosionForce*100,transform.position,explosionRadius);
            }

            try
            {
                HealthManager healthManager = objectsInRange.GetComponent<HealthManager>();
                healthManager.currentHealth -= item.damage;
            }
            catch
            {
                
            }
        }

        transform.position = originPos.position;
        ignitionParticle.SetActive(false);
        _weaponSound.ExplosiveSound();
        _rigidbody.freezeRotation = true;
        _rigidbody.velocity = Vector3.zero;
        _explosionParticle.SetActive(true);
        objectRenderer.enabled = false;
        exploded = true;
        //Debug.Log("Item exploded!");
        
    }

    public void ThrowObject(Vector2 throwDirection, float throwForce, Inventory inventory)
    {
            
            explosiveArmed = true;
            throwableCollider.enabled = true;
            throwableCollider.isTrigger = false;
            ignitionParticle.SetActive(true);
            _rigidbody.isKinematic = false;
            _rigidbody.useGravity = true;
            
            Vector2 velocidadInicial = throwDirection * throwForce;

            _rigidbody.velocity = velocidadInicial;
            
            if (item)
            {
                inventory.DropItem(item.gameObject,Item.ItemLocation.Throwed);
                inventory.drawnWeaponItem = null;
            }
    }
    
    
    

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position,explosionRadius);
    }
}
