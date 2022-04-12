using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyAudio : MonoBehaviour
{
    private AgentController _agentController;
    
    public AudioClip[] idleSounds;
    public AudioClip[] stepsClips;
    public AudioClip[] growlSounds;
    public AudioClip[] attackSounds;
    public AudioClip[] hitSounds;
    public AudioClip[] deathSounds;
    
    public AudioClip jumpStartSound;
    public AudioClip jumpEndSound;
    public AudioClip damagedSound;
    public AudioClip deathEndSound;
    public AudioClip biteSound;
    
    private AudioSource enemyAudioSource;
    
    private int selectedClip;
    public int chance;
    
    private void Start()
    {
        enemyAudioSource = GetComponent<AudioSource>();
        _agentController = GetComponent<AgentController>();
    }
    
    public void IdleSound()
    {
        chance = Random.Range(0,3);
        if (chance == 0)
        {
            selectedClip = Random.Range(0, idleSounds.Length);
            enemyAudioSource.PlayOneShot(idleSounds[selectedClip]);
        }
    }
    
    public void StepSound()
    {
        selectedClip = Random.Range(0, stepsClips.Length);
        enemyAudioSource.clip = stepsClips[selectedClip];
        enemyAudioSource.PlayOneShot(enemyAudioSource.clip);    
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
        selectedClip = Random.Range(0, growlSounds.Length);
        enemyAudioSource.PlayOneShot(growlSounds[selectedClip]);
    }
}

