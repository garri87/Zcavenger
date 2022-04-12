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
    public AudioClip explosionSound;
    public AudioClip magazineOutSound;
    public AudioClip magazineInSound;
    public AudioClip reloadEndSound;
    public AudioClip drawWeaponSound;


    private void OnEnable()
    {
        _playerController = GetComponent<PlayerController>();
        
        _audioSource = GetComponent<AudioSource>();
    }

    public void FireWeaponSound()
    {
        _audioSource.PlayOneShot(shotSound);
    }

    public void MeleeAttackSound()
    {
        _audioSource.PlayOneShot(meleeAttackSound);

    }
    public void ExplosiveSound()
    {
        _audioSource.PlayOneShot(explosionSound);
    }
    public void ReloadSound(string command)
    {
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
        _audioSource.PlayOneShot(drawWeaponSound);
    }
}
