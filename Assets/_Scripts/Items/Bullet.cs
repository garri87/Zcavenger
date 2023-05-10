using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using Random = UnityEngine.Random;

public class Bullet : MonoBehaviour
{
    private SphereCollider _collider;
    private Transform playerTransform;
    private PlayerController playerController;

    private AgentController agentController;
    private HealthManager enemyHealth;
    private Animator enemyAnimator;

    private CrashObject crashObject;
    
    private Transform objectPoolTransform;
    
    private Item weaponItem;
    
    private TrailRenderer _trailRenderer;

    #region Shooting

    RaycastHit hit;
    public LayerMask hitLayers;
    public float bulletSpeed;
    public float bulletLifeTime = 1f;
    [SerializeField]private float timer;
    private int bulletDamage;


    #endregion

    private void Awake()
    {
        playerTransform = GameObject.Find("Player").GetComponent<Transform>();
        playerController = playerTransform.GetComponent<PlayerController>();
        objectPoolTransform = GameObject.Find("ObjectPool").GetComponent<Transform>();
        _trailRenderer = GetComponent<TrailRenderer>();
        _collider = GetComponent<SphereCollider>();
    }

    private void OnEnable()
    {
        if (playerController.weaponDrawn)
        {
            weaponItem = playerController.drawnWeaponItem;
            timer = bulletLifeTime;
            _collider.enabled = true;
           _trailRenderer.enabled = true;
            _trailRenderer.startColor = new Color(Random.Range(0, 255), Random.Range(0, 255), Random.Range(0, 255));
            Fire(weaponItem);
        }
       
    }

    
    void Start()
    {
       
    }


    void Update()
    {
        timer -= Time.deltaTime;
       
       if (Physics.Raycast(transform.position, transform.forward, out hit, bulletSpeed * Time.deltaTime, hitLayers)) //Look for bullet hits
        {
            
                transform.position = hit.point;
                Vector3 reflected = Vector3.Reflect(transform.forward, hit.normal);
                Vector3 direction = transform.forward;
                Vector3 vop = Vector3.ProjectOnPlane(reflected, Vector3.forward);
                transform.forward = vop;
                transform.rotation = Quaternion.LookRotation(vop, Vector3.forward);
                CheckHit(hit.collider);
        }
       else // Bullet trayectory
       {
           transform.Translate(transform.TransformDirection(Vector3.up) * bulletSpeed * Time.deltaTime);
            
           if (timer <= 0) // Disable the bullet GameObject if time expires
           {
               gameObject.SetActive(false);
               gameObject.transform.parent = objectPoolTransform.transform;//Place back to pool
           }
       }
    }

    private void LateUpdate()
    { 
        transform.parent = ObjectPool.SharedInstance.ObjectPoolTransform;
    }

    

    public void Fire(Item item)
    {
        bulletDamage = Mathf.RoundToInt( item.damage / item.bulletsPerShot ); //Get weapon item damage and divide in equal values. for example: shotgun shells.
        Transform gunMuzzleTransform = item.gunMuzzleTransform;//Get transform of the weapon muzzle
        
        transform.position = new Vector3(gunMuzzleTransform.position.x,gunMuzzleTransform.position.y,playerController.currentPlayLine);//Place the bullet on the muzzle
       
        Vector3 vop = Vector3.ProjectOnPlane(transform.forward, Vector3.forward);
        transform.forward = vop;
        
        transform.rotation = Quaternion.LookRotation(vop,Vector3.forward);

    }
    
  
    
    /// <summary>
    /// Manages the hit status of the bullet
    /// </summary>
    /// <param name="transformPosition"></param>
    /// <param name="direction"></param>
    /// <param name="reflected"></param>
    /// <param name="hitCollider"></param>
    private void CheckHit(Collider hitCollider)
    {
        
        if (hitCollider.CompareTag("Enemy"))
        {
          
            agentController = hitCollider.GetComponent<AgentController>();
            enemyHealth = agentController._healthManager;
            enemyAnimator = agentController._animator;
            
            agentController.hisHit = true;
            enemyHealth.currentHealth -= (bulletDamage);
            Debug.Log("Enemy Took " + bulletDamage);
            enemyAnimator.SetTrigger("Hit");

            GenerateHitParticle("EnemyImpactParticle");
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
            GenerateHitParticle("BulletImpactParticle");
           
            gameObject.transform.parent = objectPoolTransform.transform;
            gameObject.SetActive(false);
        }
    }

    public void GenerateHitParticle(string particleName)
    {
        GameObject particle = ObjectPool.SharedInstance.GetPooledObject(particleName);
        if (particle != null)
        {
            particle.transform.position = hit.point;

            Vector3 vop = Vector3.ProjectOnPlane(particle.transform.forward, Vector3.forward);

            particle.transform.rotation = Quaternion.LookRotation(vop, Vector3.forward);

            particle.SetActive(true);
        }
    }
}
