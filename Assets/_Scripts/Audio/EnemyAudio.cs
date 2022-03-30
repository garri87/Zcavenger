using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyAudio : MonoBehaviour
{
    private AgentController _agentController;
    
    public AudioClip[] walkerIdleSounds;
    public AudioClip[] runnerIdleSounds;
    public AudioClip[] runStepsClips;
    public AudioClip[] walkStepsClips;
    public AudioClip[] bruteStepsClips;
    public AudioClip[] proneMovementClips;
    public AudioClip[] walkerGrowlSounds;
    public AudioClip[] runnerGrowlSounds;
    public AudioClip[] attackSounds;
    public AudioClip[] hitSounds;
    public AudioClip[] deathSounds;
    
    public AudioClip jumpStartSound;
    public AudioClip jumpEndSound;
    public AudioClip damagedSound;
    public AudioClip deathEndSound;
    public AudioClip biteSound;
    
    public AudioClip pistolFireSound;
    public AudioClip rifleFireSound;
    public AudioClip shotgunFireSound;

    
    
    private AudioSource enemyAudioSource;
    
    private int selectedClip;
    public int chance;
    
    private void Start()
    {
        enemyAudioSource = GetComponent<AudioSource>();
        _agentController = GetComponent<AgentController>();
    }

    private void Update()
    {
         
    }

    public void IdleSound()
    {
        chance = Random.Range(0,2);
        if (chance >= 1)
        {
            switch (_agentController.enemyType)
            {
                case Enemy.EnemyType.Walker:
                    selectedClip = Random.Range(0, walkerIdleSounds.Length);
                    enemyAudioSource.PlayOneShot(walkerIdleSounds[selectedClip]);
                    break;
            
                case Enemy.EnemyType.Runner:
                    selectedClip = Random.Range(0, runnerIdleSounds.Length);
                    enemyAudioSource.PlayOneShot(runnerIdleSounds[selectedClip]);
                    break;
            
                case Enemy.EnemyType.Brute:
                
                    break;
            
                case Enemy.EnemyType.Crippled:
                    selectedClip = Random.Range(0, walkerIdleSounds.Length);
                    enemyAudioSource.PlayOneShot(walkerIdleSounds[selectedClip]);
                    break;
            
                case Enemy.EnemyType.Crawler:
                    selectedClip = Random.Range(0, runnerIdleSounds.Length);
                    enemyAudioSource.PlayOneShot(runnerIdleSounds[selectedClip]);
                    break;
            }
        }
    }
    
    public void WalkSound()
    {
        selectedClip = Random.Range(0, walkStepsClips.Length);
        enemyAudioSource.clip = walkStepsClips[selectedClip];
        enemyAudioSource.PlayOneShot(enemyAudioSource.clip);    
    }

    public void RunSound()
    {
        if (_agentController.enemyType == Enemy.EnemyType.Brute)
        {
            selectedClip = Random.Range(0, bruteStepsClips.Length);
            enemyAudioSource.PlayOneShot(bruteStepsClips[selectedClip]);
        }
        else
        {
            selectedClip = Random.Range(0, runStepsClips.Length);
            enemyAudioSource.PlayOneShot(runStepsClips[selectedClip]);
        }

    }

    public void ProneSound()
    {
        selectedClip = Random.Range(0, proneMovementClips.Length);
        enemyAudioSource.PlayOneShot(proneMovementClips[selectedClip]);
    }
    
    public void JumpSound(string command)
    {
        switch (command)
        {
            case "JumpStart":
                enemyAudioSource.PlayOneShot(jumpStartSound);
                break;
            
            case "JumpEnd":
                enemyAudioSource.PlayOneShot(jumpEndSound);
                break;
            
        }
        
    }

    public void HitSound()
    {
        selectedClip = Random.Range(0, hitSounds.Length);
        enemyAudioSource.PlayOneShot(hitSounds[selectedClip]);
    }

    public void DeathSound(string command)
    {
        switch (command)
        {
            case "Start":
                selectedClip = Random.Range(0, deathSounds.Length);
                enemyAudioSource.PlayOneShot(deathSounds[selectedClip]);
                break;
            
            case "End":
                enemyAudioSource.PlayOneShot(deathEndSound);
                break;
        }
        
        
    }

    public void BiteSound()
    {
        enemyAudioSource.PlayOneShot(biteSound);
    }
    
    public void AttackSound()
    {
        selectedClip = Random.Range(0, attackSounds.Length);
        enemyAudioSource.PlayOneShot(attackSounds[selectedClip]);
    }

    public void GrowlSound()
    {
        switch (_agentController.enemyType)
        {
            case Enemy.EnemyType.Walker:
                selectedClip = Random.Range(0, walkerGrowlSounds.Length);
                enemyAudioSource.PlayOneShot(walkerGrowlSounds[selectedClip]);
                break;
            
            case Enemy.EnemyType.Runner:
                selectedClip = Random.Range(0, runnerGrowlSounds.Length);
                enemyAudioSource.PlayOneShot(runnerGrowlSounds[selectedClip]);
                break;
            
            case Enemy.EnemyType.Brute:
                
                break;
            
            case Enemy.EnemyType.Crippled:
                selectedClip = Random.Range(0, walkerGrowlSounds.Length);
                enemyAudioSource.PlayOneShot(walkerGrowlSounds[selectedClip]);
                break;
            
            case Enemy.EnemyType.Crawler:
                selectedClip = Random.Range(0, runnerGrowlSounds.Length);
                enemyAudioSource.PlayOneShot(runnerGrowlSounds[selectedClip]);
                break; 
        }
    }
    
    public void FireWeaponSound(string weaponType)
    {
        switch (weaponType)
        {
            case "Pistol":
                enemyAudioSource.PlayOneShot(pistolFireSound);

                break;
            
            case "Shotgun":
                enemyAudioSource.PlayOneShot(shotgunFireSound);

                break;
            
            case "Rifle":
                enemyAudioSource.PlayOneShot(rifleFireSound);

                break;
        }
    }
    
}

