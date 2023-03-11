using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using Random = UnityEngine.Random;

public class Bullet : MonoBehaviour
{
    public Collider _collider;
    public Transform playerTransform;
    public AgentController agentController;
    public HealthManager enemyHealth;
    public Animator enemyAnimator;
    public PlayerController playerController;
    public CrashObject crashObject;
    public Transform objectPoolTransform;
    public WeaponItem weaponItem;
    private TrailRenderer _trailRenderer;

    #region Shooting

    RaycastHit hit;
    public LayerMask hitLayers;
    public float bulletSpeed;
    public float bulletLifeTime = 1f;
    public float timer;
    public int bulletDamage;
    #endregion

    private void Awake()
    {
        playerTransform = GameObject.Find("Player").GetComponent<Transform>();
        playerController = playerTransform.GetComponent<PlayerController>();
        objectPoolTransform = GameObject.Find("ObjectPool").GetComponent<Transform>();
        _trailRenderer = GetComponent<TrailRenderer>();
    }

    private void OnEnable()
    {
        if (playerController.weaponEquipped)
        {
            weaponItem = playerController.equippedWeaponItem;
            bulletDamage = weaponItem.damage;
            timer = bulletLifeTime;
            _collider.enabled = true;
           _trailRenderer.enabled = true;
            _trailRenderer.startColor = new Color(Random.Range(0, 255), Random.Range(0, 255), Random.Range(0, 255));
            Fire();
        }
       
    }

    
    void Start()
    {
       
    }


    void Update()
    {
        timer -= Time.deltaTime;
        if (transform.position.z != playerTransform.position.z)
        {
            transform.position = new Vector3(transform.position.x,transform.position.y,playerTransform.position.z);
        }
       if (Physics.Raycast(transform.position, transform.forward, out hit, bulletSpeed * Time.deltaTime,hitLayers))
        {
            
                transform.position = hit.point;
                Vector3 reflected = Vector3.Reflect(transform.forward, hit.normal);
                Vector3 direction = transform.forward;
                Vector3 vop = Vector3.ProjectOnPlane(reflected, Vector3.forward);
                transform.forward = vop;
                transform.rotation = Quaternion.LookRotation(vop, Vector3.forward);
                Hit(transform.position, direction,reflected, hit.collider);
        }
       else
       {
           transform.Translate(transform.TransformDirection(Vector3.up) * bulletSpeed * Time.deltaTime);
            
           if (timer <= 0)
           {
               gameObject.SetActive(false);
              // _trailRenderer.enabled = false;
               gameObject.transform.parent = objectPoolTransform.transform;

           }
       }
    }

    private void LateUpdate()
    { 
        transform.parent = ObjectPool.SharedInstance.ObjectPoolTransform;
    }

    

    public void Fire()
    {
        Transform gunMuzzleTransform = playerController.equippedWeaponItem.gunMuzzleTransform;
        transform.position = new Vector3(gunMuzzleTransform.position.x,gunMuzzleTransform.position.y,playerController.currentPlayLine);
       
        Vector3 vop = Vector3.ProjectOnPlane(transform.forward, Vector3.forward);
        transform.forward = vop;
        
        transform.rotation = Quaternion.LookRotation(vop,Vector3.forward);
    }
    
    
    private void Hit(Vector3 transformPosition, Vector3 direction, Vector3 reflected, Collider hitCollider)
    {
        
        if (hitCollider.CompareTag("Enemy"))
        {
            int damage = Mathf.RoundToInt(bulletDamage / weaponItem.bulletsPerShot);
            agentController = hitCollider.GetComponent<AgentController>();
            enemyHealth = hitCollider.GetComponentInParent<HealthManager>();
            enemyAnimator = hitCollider.GetComponentInParent<Animator>();
            agentController.hisHit = true;
            enemyHealth.currentHealth -= (bulletDamage / weaponItem.bulletsPerShot);
            Debug.Log("Enemy Took " + damage);
            enemyAnimator.SetTrigger("Hit");
            GameObject enemyImpactParticle = ObjectPool.SharedInstance.GetPooledObject("EnemyImpactParticle");
            if (enemyImpactParticle != null)
            {
                enemyImpactParticle.transform.position = hit.point;
                enemyImpactParticle.transform.rotation = Quaternion.LookRotation(-weaponItem.gunMuzzleTransform.forward,Vector3.up);
                enemyImpactParticle.SetActive(true);
            }
            gameObject.transform.parent = objectPoolTransform.transform;
            gameObject.SetActive(false);
        }
        else if (hitCollider.CompareTag("Crashable"))
        {
            crashObject = hitCollider.GetComponent<CrashObject>();
            crashObject.Crash();
            gameObject.transform.parent = objectPoolTransform.transform;
            gameObject.SetActive(false);
        }
        else
        {
            GameObject ImpactParticle = ObjectPool.SharedInstance.GetPooledObject("BulletImpactParticle");
            if (ImpactParticle != null)
            {
                ImpactParticle.transform.position = hit.point;
                ImpactParticle.transform.localEulerAngles = -transform.TransformDirection(playerTransform.position);
                ImpactParticle.SetActive(true);
            }

            gameObject.transform.parent = objectPoolTransform.transform;
            gameObject.SetActive(false);
        }
    }
    
}
