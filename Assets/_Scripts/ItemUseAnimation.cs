using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemUseAnimation : StateMachineBehaviour
{
    public enum ItemType
    {
        Bandage,
        Water,
        Food,
        GrabItem,
    }

    public ItemType itemType;

    public PlayerController playerController;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        playerController = animator.GetComponent<PlayerController>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        switch (itemType)
        {
            case ItemType.Bandage:
                playerController.bandaging = false;
                break;
            
            case ItemType.Water:
                playerController.drinking = false;
                break;
            
            case ItemType.Food:
                playerController.eating = false;
                break;
            
            case ItemType.GrabItem:
                playerController.grabItem = false;
                break;
        }    
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
   // override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    
}
