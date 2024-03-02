using System.Collections;
using System.Collections.Generic;
using System.Threading;
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

    private float timer;
    private float animationTime = 5;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        playerController = animator.GetComponent<PlayerController>();
        timer = 0;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer += Time.deltaTime;
        if (timer >= animationTime)
        {
            EndAnimation();
            timer = 0;
        }
    }

    
    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        EndAnimation();
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
    
    
    public void EndAnimation()
    {
        switch (itemType)
        {
            case ItemType.Bandage:
                playerController.isBandaging = false;
                break;
            
            case ItemType.Water:
                playerController.isDrinking = false;
                break;
            
            case ItemType.Food:
                playerController.isEating = false;
                break;
            
            case ItemType.GrabItem:
                playerController.grabbingItem = false;
                break;
        }    
    }
}
