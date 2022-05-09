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
    
    public float lowSoundSensorScale = 0.2f;
    public float normalSoundSensorScale = 2f;
    public float highSoundSensorScale = 5f;

    public SoundSensor _soundSensor;

    private void Start()
    {
        playerAudioSource = GetComponent<AudioSource>();
    }

    public void WalkSound()
    {
        _soundSensor.sensorScale = lowSoundSensorScale;
        selectedClip = Random.Range(0, walkStepsClips.Length);
        playerAudioSource.PlayOneShot(walkStepsClips[selectedClip]);    
    }

    public void RunSound()
    {
        _soundSensor.sensorScale = normalSoundSensorScale;
        selectedClip = Random.Range(0, runStepsClips.Length);
        playerAudioSource.PlayOneShot(runStepsClips[selectedClip]);    
    }

    public void ProneSound()
    {
        _soundSensor.sensorScale = lowSoundSensorScale;
        selectedClip = Random.Range(0, proneMovementClips.Length);
        playerAudioSource.PlayOneShot(proneMovementClips[selectedClip]);
    }

    public void RollSound()
    {
        _soundSensor.sensorScale = normalSoundSensorScale;
        playerAudioSource.PlayOneShot(rollSound);

    }
    
    public void JumpSound(string command)
    {
        switch (command)
        {
            case "JumpStart":
                _soundSensor.sensorScale = lowSoundSensorScale;
                playerAudioSource.PlayOneShot(jumpStartSound);
                break;
            
            case "JumpEnd":
                _soundSensor.sensorScale = normalSoundSensorScale;
                playerAudioSource.PlayOneShot(jumpEndSound);
                break;
            
        }
        
    }

    public void ActionSound(string command)
    {
        switch (command)
        {
            case "LadderClimb":
                _soundSensor.sensorScale = lowSoundSensorScale;
                playerAudioSource.PlayOneShot(ladderClimbSound);
                break;
        }
    }
    public void HitSound()
    {
        _soundSensor.sensorScale = 0;
        selectedClip = Random.Range(0, hitSounds.Length);
        playerAudioSource.PlayOneShot(hitSounds[selectedClip]); 
    }

    public void DeathSound()
    {
        _soundSensor.sensorScale = highSoundSensorScale;
        playerAudioSource.PlayOneShot(deathSound);
    }

    public void StompHeadSound()
    {
        _soundSensor.sensorScale = normalSoundSensorScale;
        playerAudioSource.PlayOneShot(stompHeadSound);
    }

    public void StompHitSound()
    {
        _soundSensor.sensorScale = normalSoundSensorScale;
        playerAudioSource.PlayOneShot(stompHitSound);
    }
}
