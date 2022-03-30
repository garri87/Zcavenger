using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;
using Random = UnityEngine.Random;

public class HealthManager : MonoBehaviour
{
    private PlayerController _playerController;
    private AgentController _agentController;
    
    
    public enum PlayerType
    {
        mainPlayer,
        enemyAI,
    }

    public PlayerType playerType;
    
    [Header("Health and nutrition")]
    public int maxHealth;
    public int currentHealth;
    public int maxFood;
    public int currentFood;
    public int maxWater;
    public int currentWater;
    
    [Header("Regen Rate")]
    
    public float updateRate = 15;//Value in seconds that update the regeneration
    public float lowNutritionUpdateRate = 20;
    [HideInInspector] public float currentUpdateRate;
    public int regenRate; // Amount of regeneration points when timer reaches updateRate
    public int targetRegen; //Set the limit of regeneration depending the consumed item
    public int currentRegen; //the current regenerated points
    private float timer; 

    
    [Header("Stamina")]
    public float currentStamina;
    public float maxStamina = 100;
    public float staminaRegenRate = 20;

    [Header("Stamina Penalties")] 
    public float jumpPenalty = 30;
    public float meleeAttackPenalty = 40;
    public float rollPenalty = 20;
    public float blockHitPenalty = 20;
    
    private bool isDead = false;

    public bool IsDead => isDead;

    private void OnEnable()
    {
        _playerController = GetComponent<PlayerController>();
    }

    void Start()
    {
        switch (playerType)
        {
            case PlayerType.enemyAI:
                _agentController = GetComponent<AgentController>();
                maxHealth = _agentController.enemyScriptableObject.maxHealth;
                currentHealth = maxHealth;
                break;
            
            case PlayerType.mainPlayer:
                
                maxHealth = 100;
                maxFood = 100;
                maxWater = 100;
                maxStamina = 100;
                currentHealth = maxHealth;
                currentFood = maxFood;
                currentWater = maxWater;
                currentStamina = maxStamina;
                break;
        }

        isDead = false;
    }
    private void FixedUpdate()
    {
        if (playerType == PlayerType.mainPlayer)
        {
            if (currentFood <= 25 || currentWater <=25 )
            {
                currentUpdateRate = lowNutritionUpdateRate;
            }
            else
            {
                currentUpdateRate = updateRate;
            }
            
            
            if (!_playerController.jump)
            { 
                currentStamina += Time.deltaTime * staminaRegenRate;
            }


            timer += Time.fixedDeltaTime;

            if (timer >= currentUpdateRate)
            {
                timer = 0;
                currentFood -= Random.Range(1,5);
                currentWater -= Random.Range(1,5);
                currentHealth += regenRate;
                currentRegen += regenRate;
                if (currentRegen >= targetRegen || currentHealth == maxHealth)
                {
                    regenRate = 0;
                    currentRegen = 0;
                }

                if (currentHealth >= maxHealth)
                {
                    currentHealth = maxHealth;
                }

                
                
                
                if (currentFood <= 0)
                {
                    currentFood = 0;
                    currentHealth -= 1;
                }

                if (currentWater <= 0)
                {
                    currentWater = 0;
                    currentHealth -= 1;
                }

                
            }
        }
        
        
        
    }
    void Update()
    {
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            isDead = true;
        }

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        if (currentFood >= maxFood)
        {
            currentFood = maxFood;
        }

        if (currentWater >= maxWater)
        {
            currentWater = maxWater;
        }

        if (currentStamina >= maxStamina)
        {
            currentStamina = maxStamina;
        }

        if (currentStamina < 0)
        {
            currentStamina = 0;
        }
        
    }

   

    public void UpdateHealth()
    {
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    public void ConsumeStamina(float staminaCost)
    {
        currentStamina -= staminaCost;
    }
    
    public void BleedingStatus()
    {
        //TODO: Implementar efecto de sangrado: Perdida gradual de vida
    }

    public void InjuredStatus()
    {
        //TODO: Implementar efecto de jugador lastimado: Velocidad de movimiento reducida
    }

    public void SickStatus()
    {
        //TODO: Implementar efecto de jugador enfermo: Perdida gradual de vida y consumo de hambre y sed mas rapido.

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DeathZone"))
        {
            currentHealth = 0;
        }
    }
}
