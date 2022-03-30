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
    public AudioClip pistolFireSound;
    public AudioClip shotgunFireSound;
    public AudioClip rifleFireSound;
    public AudioClip axeSlashSound;
    public AudioClip batSwingSound;
    public AudioClip knifeSlashSound;
    
    public AudioClip magazineOutSound;
    public AudioClip magazineInSound;
    public AudioClip shotgunReloadEndSound;
    public AudioClip rifleReloadEndSound;
    
    public AudioClip drawWeaponSound;


    private void OnEnable()
    {
        _playerController = GetComponent<PlayerController>();
        
        _audioSource = GetComponent<AudioSource>();
    }

    public void FireWeaponSound(string weaponType)
    {
        switch (weaponType)
        {
            case "Pistol":
                _audioSource.PlayOneShot(pistolFireSound);

                break;
            
            case "Shotgun":
                _audioSource.PlayOneShot(shotgunFireSound);

                break;
            
            case "Rifle":
                _audioSource.PlayOneShot(rifleFireSound);

                break;
            
            case "Axe":
                _audioSource.PlayOneShot(axeSlashSound);
                break;
            
            case "Bat":
                _audioSource.PlayOneShot(batSwingSound);
                break;
            
            case "Knife":
                _audioSource.PlayOneShot(knifeSlashSound);
                break;
        }
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
                _weaponItem = _playerController.equippedWeaponItem;
                switch (_weaponItem.ID)
                {
                    case 5004: //Shotgun
                        _audioSource.PlayOneShot(shotgunReloadEndSound);
                        break;
                    
                    case 3003: //Rifle
                        _audioSource.PlayOneShot(rifleReloadEndSound);
                        break;
                }
                break;
        }
    }
    
    public void DrawWeaponSound()
    {
        _audioSource.PlayOneShot(drawWeaponSound);
    }
}
