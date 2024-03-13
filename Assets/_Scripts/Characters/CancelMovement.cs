using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CancelMovement : StateMachineBehaviour
{
    public enum ObjectType
    {
     AI,
     Player,
    }

    public ObjectType objectType;
    private AgentController _agentController;
    private PlayerController _playerController;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        switch (objectType)
        {
            case ObjectType.Player:
                _playerController = animator.GetComponent<PlayerController>();
                _playerController.onTransition = true;
                break;
            
            case ObjectType.AI:
                _agentController = animator.GetComponent<AgentController>();
                _agentController.agentState = AgentController.AgentState.Ontransition;
                break;
        }
        
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
        switch (objectType)
        {
            case ObjectType.Player:
                _playerController.onTransition = true;
                break;
            
            case ObjectType.AI:
                _agentController.agentState = AgentController.AgentState.Ontransition;
                break;
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        switch (objectType)
        {
            case ObjectType.Player:
                _playerController.onTransition = false;
                _playerController.beingBitten = false;
                break;
            
            case ObjectType.AI:
                if (!_agentController.playerCatch)
                {
                    _agentController.agentState = AgentController.AgentState.Active;
                    _agentController.attacking = false;
                }    
                break;
        }
        
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
