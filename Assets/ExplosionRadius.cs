using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionRadius : MonoBehaviour
{
    public enum ExplosiveType
    {
        Grenade,
        Molotov,
    }
    public ExplosiveType explosiveType;
    
    public SphereCollider sphereCollider;
    public ParticleSystem explosionParticle;
    private HealthManager _healthManager;
    public float explosionRange;
    public int explosionDamage;
    public float explosionSpeed = 20;

    private void OnEnable()
    {
        sphereCollider.radius = 0;
        explosionParticle.Play();
    }

    private void Update()
    {
        if (sphereCollider.radius < explosionRange)
        {
            sphereCollider.radius += Time.deltaTime * explosionSpeed;
        }
        else
        {
            sphereCollider.radius = explosionRange;
            sphereCollider.enabled = false;
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            _healthManager = other.GetComponent<HealthManager>();
            ExplosionDamage(explosionDamage);
            Debug.Log("Dealt " + explosionDamage);
        }
    }

    public void ExplosionDamage(int damage)
    {
        if (_healthManager != null)
        {
            _healthManager.currentHealth -= damage;
        }
    }
    
    
}
