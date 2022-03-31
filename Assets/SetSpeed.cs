using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetSpeed : StateMachineBehaviour
{
    private PlayerController playerController;
    public enum ActionType
    {
        Walk,
        Run,
        Prone,
        Crouch,
        Injured,
        
    }

    public ActionType actionType;
    
    
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        playerController = animator.GetComponent<PlayerController>();
    }

    //OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        switch (actionType)
        {
            case ActionType.Walk:
                playerController.currentSpeed = playerController.crouchWalkSpeed;
                break;
            
            case ActionType.Run:
                
                break;
            
            case ActionType.Crouch:
                playerController.currentSpeed = playerController.crouchWalkSpeed;
                break;
            
            case ActionType.Prone:
                playerController.currentSpeed = playerController.crouchWalkSpeed /2;

                break;
            
            case ActionType.Injured:
                playerController.currentSpeed = playerController.injuredSpeed;

                break;
            
        
        }
        
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

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
}
