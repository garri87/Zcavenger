using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HoldPlayer : StateMachineBehaviour
{
    public AgentController agentController;
    public PlayerController playerController;
    private NavMeshAgent _navMeshAgent;
    private Vector3 catchVector;
    public float transitionSpeed = 8;
    private Transform headBoneTransform;
    private Transform playerTransform;
    private bool playerAlreadyCatched = false;
    public float catchTimer;
    public float catchTime = 10;
    public float catchForce = 4;
    public float struggleForce; 


    
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
   override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
   {
       //Debug.Log("HoldPlayer OnStateEnter");
       agentController = animator.GetComponent<AgentController>();
       playerController = agentController.player.GetComponent<PlayerController>();
       _navMeshAgent = agentController._navMeshAgent;
       headBoneTransform = animator.GetBoneTransform(HumanBodyBones.Head);
       playerTransform = agentController.player.transform;
       struggleForce = playerController.struggleForce;
   }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        playerController.alreadyCatched = playerAlreadyCatched;
        if (agentController.playerCatch)
        {
            catchVector = new Vector3(headBoneTransform.position.x,animator.transform.position.y, playerTransform.position.z);

            if (!playerAlreadyCatched)
            {
                if (playerTransform.position != catchVector) 
                { 
                    playerTransform.position = Vector3.Lerp(playerTransform.position, catchVector, Time.deltaTime * transitionSpeed);
                }
                else 
                { 
                    playerTransform.position = new Vector3(playerTransform.position.x, catchVector.y,catchVector.z); 
                }   
                playerAlreadyCatched = true;

            }
            agentController.agentState = AgentController.AgentState.Ontransition;
            _navMeshAgent.isStopped = true;
            switch (agentController.enemyType)
            {
                case Enemy.EnemyType.Crippled:
                    playerController.isTrapped = true;

                    break;
                
                case Enemy.EnemyType.Walker:
                    playerController.beingBitten = true;
                    break;
                
            }
            StruggleTimer();
        }
        if (catchTimer <=0)
        {
            ReleasePlayer();
        }
    }

    
    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("HoldPlayer OnStateExit");
        
      //  ReleasePlayer();

    }
    
    public void StruggleTimer()
    {
        if (catchTimer < catchTime)
        {
            catchTimer += Time.deltaTime * catchForce;
        }

        if (catchTimer > catchTime)
        {
            catchTimer = catchTime;
        }
        if (Input.GetKeyDown(GameManager.Instance._keyAssignments.jumpKey.keyCode))
        {
            catchTimer -= struggleForce;
            if (catchTimer <=0)
            {
                agentController.attacking = false;
                agentController.playerCatch = false;
                _navMeshAgent.isStopped = false;
                playerController.isTrapped = false;
                playerController.beingBitten = false;
                catchTimer = catchTime;
                playerController.alreadyCatched = false;
                playerController.controllerType = PlayerController.ControllerType.DefaultController;
                playerController.onTransition = false;
            }
        }
    }
    
    public void ReleasePlayer()
    {
        agentController.playerCatch = false;
        playerController.isTrapped = false;
        playerController.beingBitten = false;
        playerAlreadyCatched = false;
        playerController.onTransition = false;

        playerController.controllerType = PlayerController.ControllerType.DefaultController;
        agentController.attacking = false;
        if (!agentController._healthManager.IsDead)
        {
            agentController.agentState = AgentController.AgentState.Active;
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
