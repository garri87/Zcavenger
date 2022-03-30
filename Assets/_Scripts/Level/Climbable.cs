
using System;
using UnityEngine;

public class Climbable : MonoBehaviour
{
   public enum LedgeType
   {
       HangerHangOnly,
       WallHangOnly,
       HangerRailing,
       WallRailing,
       WallClimb,
       ObstacleVault,
   }

   public LedgeType ledgeType;
   
   public enum Orientation
   {
       Back,
       Right,
       Left,
   }
   public Orientation orientation;
   public Transform ledgeTransform;
   public Transform finalPositionTransform;


   private void OnValidate()
   {
       switch (orientation)
       {
           case Orientation.Back:
               transform.eulerAngles = new Vector3(0,180,0); 
               break;
           
           case Orientation.Right:
               transform.eulerAngles = new Vector3(0,90,0); 
               break;
           
           case Orientation.Left: 
               transform.eulerAngles = new Vector3(0,-90,0); 
               break;
           
       }
   }

   private void Start()
   {
   }
   /*TODO: Implementar escalamiento vertical entre ledges con el boton de salto
    1.raycastear desde este transform hacia arriba y hacia abajo devolver un bool si detecta un ledge
    2.en caso de detectar un ledge superior, activar la animación de salto para alcanzar el siguiente ledge
    3.en caso de detectar un ledge inferior, dejar caer al player y permitir que detecte el ledge si el rigidbody.velocity.y <0
    
    
    
        */
   
   
   
}
