using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class CrashObject : MonoBehaviour
{
    [Header("Crate Components")]
    public MeshRenderer defaultCrate;
    private Rigidbody defaultCrateRB;
    public BoxCollider crateCollider;
    public GameObject fracturedCrate;
    
    public AudioClip[] crashSounds;
    private AudioSource _audioSource;
    
    private NavMeshObstacle _navMeshObstacle;

    [Header("Crate Loot")]
    public ItemScriptableObject insideItem;
    public int itemQuantity;
    
    public GameObject insideWeapon;
    
    public Transform weaponHolder;
    public Transform itemHolder;
    private Item _item;
    
    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        defaultCrateRB = GetComponent<Rigidbody>();
        _navMeshObstacle = GetComponent<NavMeshObstacle>();
        if (insideItem !=null)
        {
            _item = itemHolder.GetComponent<Item>();
            _item.itemScriptableObject = insideItem;
            _item.GetItemScriptableObject(insideItem);
            _item.quantity = itemQuantity;
            _item.itemLocation = Item.ItemLocation.World;
        }

        if (insideWeapon != null)
        {
            GameObject instantiatedWeapon = Instantiate(insideWeapon, weaponHolder.position + (Vector3.up/2), weaponHolder.rotation);
            instantiatedWeapon.transform.parent = weaponHolder.transform;
            Item weaponItem = instantiatedWeapon.GetComponent<Item>();
            weaponItem.itemLocation = Item.ItemLocation.World;
            instantiatedWeapon.transform.rotation = Quaternion.Euler(0,90,0);
        }
    }

    private void OnEnable()
    {
        itemHolder.gameObject.SetActive(false);
        weaponHolder.gameObject.SetActive(false);
        _navMeshObstacle.enabled = true;
        defaultCrateRB.useGravity = true;
        defaultCrateRB.isKinematic = false;
        defaultCrate.enabled = true;
        crateCollider.enabled = true;
    }


    public void Crash()
    {
        defaultCrateRB.useGravity = false;
        defaultCrateRB.isKinematic = true;
        defaultCrate.enabled = false;
        crateCollider.enabled = false;
        fracturedCrate.SetActive(true);
        
        if (insideItem != null)
        {
            //_item.InstantiateItem(_item.itemPrefab);
           // _item.prefabHolder.gameObject.SetActive(true);
            itemHolder.gameObject.SetActive(true);
        }

        if (insideWeapon != null)
        {
            weaponHolder.gameObject.SetActive(true);
        }
        _audioSource.PlayOneShot(crashSounds[Random.Range(0,crashSounds.Length)]);

        _navMeshObstacle.enabled = false;
        StartCoroutine("DisableTimer");
    }

    IEnumerator DisableTimer()
    {
        yield return new WaitForSeconds(10);
        fracturedCrate.SetActive(false);
    }
}
