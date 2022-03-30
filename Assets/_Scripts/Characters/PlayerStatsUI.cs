using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatsUI : MonoBehaviour
{
    public TextMeshProUGUI healthCount;
    public TextMeshProUGUI waterCount;
    public TextMeshProUGUI foodCount;
    public Slider staminaCount;
    private HealthManager playerHealthManager;
    
    
    void Start()
    {
        playerHealthManager = GameObject.Find("Player").GetComponent<HealthManager>();
    }

    void Update()
    {
        healthCount.SetText("" + playerHealthManager.currentHealth);
        foodCount.SetText("" + playerHealthManager.currentFood + " %");
        waterCount.SetText("" + playerHealthManager.currentWater + " %");
        staminaCount.maxValue = playerHealthManager.maxStamina;
        staminaCount.value = playerHealthManager.currentStamina;
        
        if (playerHealthManager.currentHealth <=15)
        {
            healthCount.color = Color.red;
        }
        else
        {
            healthCount.color = Color.white;
        }

        if (playerHealthManager.currentStamina < playerHealthManager.jumpPenalty)
        {
            staminaCount.targetGraphic.color = Color.red;
        }
        else
        {
            if (playerHealthManager.currentStamina >= playerHealthManager.jumpPenalty + playerHealthManager.meleeAttackPenalty)
            {
                staminaCount.targetGraphic.color = Color.green;
            }
            else
            {
                staminaCount.targetGraphic.color = Color.yellow;
            }
        }
    }
}
