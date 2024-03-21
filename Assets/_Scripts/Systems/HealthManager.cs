using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
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
    public int maxHealth = 100;
    public int currentHealth;
    public int maxHunger;
    public int currentHunger;
    public int maxThirst;
    public int currentThirst;
    public int healthWarningValue = 15;
    
    [FormerlySerializedAs("updateRate")] [Header("Regen Rate")]
    
    public float defaultUpdateRate = 15;//Value in seconds that updates regeneration
    public float lowNutritionUpdateRate = 20;
    public float bleedingUpdateRate = 5;
    public int maxNutritionDecreaseSize = 5;//max points of nutrition to decrease in every cycle
    public int bleedingDamagePerRate;
    public int lowNutritionDamagePerRate = 1; //Low nutrition damage to player per cycle
    
    [HideInInspector] public float currentUpdateRate;
    public int regenRate; // Amount of regeneration points when timer reaches updateRate
    public int targetRegen; //Set the limit of regeneration depending the consumed item
    public int currentRegen; //the current regenerated points
    private float _updateTimer; 
    private float _regenTimer; 
    private float _bleedingTimer;
    private float _sickTimer;

    
    [Header("Stamina")]
    public float currentStamina;
    public float maxStamina = 100;
    public float staminaRegenRate = 20;
    private float defaultStaminaRegenRate;

    [Header("Stamina Penalties")] 
    public float jumpPenalty = 30;
    public float meleeAttackPenalty = 40;
    public float rollPenalty = 20;
    public float blockHitPenalty = 20;

    [Header("Status")] 
    public bool isBleeding;
    public bool isInjured;
    public bool isSick;
    
    
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
                maxHunger = 100;
                maxThirst = 100;
                maxStamina = 100;
                currentHealth = maxHealth;
                currentHunger = maxHunger;
                currentThirst = maxThirst;
                currentStamina = maxStamina;

                _bleedingTimer = 0;
                _sickTimer = 0;

                defaultStaminaRegenRate = staminaRegenRate;
                break;
        }

        isDead = false;
    }
    private void FixedUpdate()
    {
        if (playerType == PlayerType.mainPlayer)
        {
            if (currentHunger <= 25 || currentThirst <=25 )
            {
                currentUpdateRate = lowNutritionUpdateRate;
            }
            else
            {
                currentUpdateRate = defaultUpdateRate;
            }

            if (currentHealth <= healthWarningValue)
            {
                isInjured = true;
            }
            else
            {
                isInjured = false;
            }
            
            if (!_playerController.jump)
            {
                    currentStamina += Time.deltaTime * staminaRegenRate;
            }
            

            _updateTimer += Time.fixedDeltaTime;

            if (_updateTimer >= currentUpdateRate)
            {
                _updateTimer = 0;
                currentHunger -= Random.Range(1,maxNutritionDecreaseSize);
                currentThirst -= Random.Range(1,maxNutritionDecreaseSize);
                currentHealth += regenRate;
                currentRegen += regenRate;
                
                if (currentRegen >= targetRegen || currentHealth == maxHealth)
                    //if regeneration reach the target value or health is at max, stop regeneration
                {
                    regenRate = 0;
                    currentRegen = 0;
                }

                if (currentHealth >= maxHealth)
                {
                    currentHealth = maxHealth;
                }
                
                if (currentHunger <= 0)
                {
                    currentHunger = 0;
                    currentHealth -= lowNutritionDamagePerRate;
                }

                if (currentThirst <= 0)
                {
                    currentThirst = 0;
                    currentHealth -= lowNutritionDamagePerRate;
                }
            }
            
            BleedingStatus(isBleeding);
            InjuredStatus(isInjured);
            SickStatus(isSick);
        }
        
        
        
    }
    void Update()
    {
        if (playerType == PlayerType.mainPlayer)
        {
            UpdateUIStatus();
        }
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            isDead = true;
        }

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        if (currentHunger >= maxHunger)
        {
            currentHunger = maxHunger;
        }

        if (currentThirst >= maxThirst)
        {
            currentThirst = maxThirst;
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
    
    public void BleedingStatus(bool enabled)
    {
        if (enabled == true)
        {
            _bleedingTimer += Time.fixedDeltaTime;
            if (_bleedingTimer >= bleedingUpdateRate)
            {
                currentHealth -= bleedingDamagePerRate;
                _bleedingTimer = 0;
                return;
            }
        }
        _playerController._animator.SetBool("Bleeding", enabled);
    }

    public void InjuredStatus(bool enabled)
    {
        if (enabled)
        {
            _playerController.currentSpeed = _playerController.injuredSpeed;
        }
        _playerController._animator.SetBool("Injured", enabled);

    }

    public void SickStatus(bool enabled)
    {
        if (enabled == true)
        {
            _sickTimer += Time.fixedDeltaTime;
            if (_sickTimer >= bleedingUpdateRate)
            {
                currentHealth -= bleedingDamagePerRate;
                _sickTimer = 0;
                return;
            }

            staminaRegenRate = defaultStaminaRegenRate / 4;

        }
        else
        {
            staminaRegenRate = defaultStaminaRegenRate;
        }
    }

    private void UpdateUIStatus()
    {
        InGameOverlayUI ui = GameManager.Instance.uiManager.inGameOverlayUI;
        ui.healthLabel.text = currentHealth.ToString() + " %";
        ui.hungerLabel.text = currentHunger.ToString() + " %";
        ui.thirstLabel.text = currentThirst.ToString() + " %";
        ui.staminaBar.style.width = Length.Percent(currentStamina);

        if (currentStamina < jumpPenalty)
        {
            ui.staminaBar.style.backgroundColor = new StyleColor(Color.red);
        }
        else
        {
            ui.staminaBar.style.backgroundColor = new StyleColor(Color.green);
        }
        
        
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DeathZone"))
        {
            currentHealth = 0;
        }
    }
}
