using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSound : MonoBehaviour
{
    public AudioSource _audioSource;
    private WeaponItem _weaponItem;
    private PlayerController _playerController;

    [Header("Audio Clips")]
    public AudioClip shotSound;
    public AudioClip meleeAttackSound;
    public AudioClip dropSound;
    public AudioClip explosionSound;
    public AudioClip magazineOutSound;
    public AudioClip magazineInSound;
    public AudioClip reloadEndSound;
    public AudioClip drawWeaponSound;

    public SoundSensor _soundSensor;
    
    public float lowSoundSensorScale = 20f;
    public float normalSoundSensorScale = 200f;
    public float highSoundSensorScale = 400f;

    private void OnEnable()
    {
        _playerController = GetComponent<PlayerController>();
        _audioSource = GetComponent<AudioSource>();
    }

    public void FireWeaponSound()
    {
        _soundSensor.sensorScale = normalSoundSensorScale;
        _audioSource.PlayOneShot(shotSound);
    }

    public void MeleeAttackSound()
    {
        _soundSensor.sensorScale = lowSoundSensorScale;
        _audioSource.PlayOneShot(meleeAttackSound);
    }
    public void ExplosiveSound()
    {
        _soundSensor.sensorScale = highSoundSensorScale;
        _audioSource.PlayOneShot(explosionSound);
    }
    public void ReloadSound(string command)
    {
        _soundSensor.sensorScale = lowSoundSensorScale;

        switch (command)
        {
            case "MagOut":
                _audioSource.PlayOneShot(magazineOutSound);

                break;
            
            case "MagIn":
                _audioSource.PlayOneShot(magazineInSound);
                
                break;
            
            case "End":
                _audioSource.PlayOneShot(reloadEndSound);
                break;
        }
    }
    
    public void DrawWeaponSound()
    {
        _soundSensor.sensorScale = 0;
        _audioSource.PlayOneShot(drawWeaponSound);
    }

    public void DropSound()
    {
        _soundSensor.sensorScale = normalSoundSensorScale;
        _audioSource.PlayOneShot(dropSound);
    }
}
