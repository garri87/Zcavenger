using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCotroller : StateMachineBehaviour
{
    public enum Controller
    {
        Default,
        StandBy,
        OnLadder,
        OnLedge,
        OnUI,
    }

    public Controller controller;

    private PlayerController _playerController;
    

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _playerController = animator.GetComponent<PlayerController>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        switch (controller)
        {
               case Controller.Default:
                   _playerController.controllerType = PlayerController.ControllerType.DefaultController;    
               break;
               case Controller.StandBy:
                   _playerController.controllerType = PlayerController.ControllerType.StandByController;    

               break;
               case Controller.OnLadder:
                   _playerController.controllerType = PlayerController.ControllerType.OnLadderController;    

               break;
               case Controller.OnLedge:
                   _playerController.controllerType = PlayerController.ControllerType.OnLedgeController;    

               break;
               case Controller.OnUI:
                   _playerController.controllerType = PlayerController.ControllerType.OnUIController;
                   break;
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _playerController.controllerType = PlayerController.ControllerType.DefaultController;
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
}
