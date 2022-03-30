using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;


public class PlayerAudio : MonoBehaviour
{
    private AudioSource playerAudioSource;

    public AudioClip[] runStepsClips;
    public AudioClip[] walkStepsClips;
    public AudioClip[] proneMovementClips;
    public AudioClip[] hitSounds;
    public AudioClip rollSound;
    public AudioClip stompHitSound;
    public AudioClip stompHeadSound;
    public AudioClip jumpStartSound;
    public AudioClip jumpEndSound;
    public AudioClip ladderClimbSound;
    public AudioClip deathSound;
    
    private int selectedClip;
    
    

    private void Start()
    {
        playerAudioSource = GetComponent<AudioSource>();
    }

    public void WalkSound()
    {
        selectedClip = Random.Range(0, walkStepsClips.Length);
        playerAudioSource.PlayOneShot(walkStepsClips[selectedClip]);    
    }

    public void RunSound()
    {
        selectedClip = Random.Range(0, runStepsClips.Length);
        playerAudioSource.PlayOneShot(runStepsClips[selectedClip]);    
    }

    public void ProneSound()
    {
        selectedClip = Random.Range(0, proneMovementClips.Length);
        playerAudioSource.PlayOneShot(proneMovementClips[selectedClip]);
    }

    public void RollSound()
    {
        playerAudioSource.PlayOneShot(rollSound);

    }
    
    public void JumpSound(string command)
    {
        switch (command)
        {
            case "JumpStart":
                playerAudioSource.PlayOneShot(jumpStartSound);
                break;
            
            case "JumpEnd":
                playerAudioSource.PlayOneShot(jumpEndSound);
                break;
            
        }
        
    }

    public void ActionSound(string command)
    {
        switch (command)
        {
            case "LadderClimb":
                playerAudioSource.PlayOneShot(ladderClimbSound);
                break;
        }
    }
    public void HitSound()
    {
        selectedClip = Random.Range(0, hitSounds.Length);
        playerAudioSource.PlayOneShot(hitSounds[selectedClip]); 
    }

    public void DeathSound()
    {
        playerAudioSource.PlayOneShot(deathSound);
    }

    public void StompHeadSound()
    {
        playerAudioSource.PlayOneShot(stompHeadSound);
    }

    public void StompHitSound()
    {
        playerAudioSource.PlayOneShot(stompHitSound);

    }





}
