using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyObject : MonoBehaviour
{
   private float timer;
   public float destroyTime;
   public bool onlyDeactivate;
   private Transform objectPoolTransform;

   private void Awake()
   {
      objectPoolTransform = GameObject.Find("ObjectPool").GetComponent<Transform>();
   }

   private void OnEnable()
   {
      timer = destroyTime;
   }

   private void Update()
   {
      if (onlyDeactivate)
      {
         timer -= Time.deltaTime;
         if (timer <= 0)
         {
            gameObject.transform.parent = objectPoolTransform.transform;
            gameObject.SetActive(false);
         }
      }
     else if (!onlyDeactivate)
      {
        Destroy(gameObject,destroyTime);
      }
   }
}
