using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class SoundSensor : MonoBehaviour
{
    public AudioSource _audioSource;
    public float sensorScale;
    public LayerMask layerMask;

    private void Awake()
    {
        sensorScale = 0;
    }

    private void Update()
    {
        if (_audioSource.isPlaying)
        {
            GetListenersInRange(transform.root.position,sensorScale);
        }
        else
        {
            sensorScale = 0;
        }
    }

    public void GetListenersInRange(Vector3 origin, float soundRange)
    {
        Collider[] listeners = Physics.OverlapSphere(origin, soundRange, layerMask.value);

        foreach (Collider listener in listeners)
        {
            if (listener.CompareTag("Enemy"))
            {

                Debug.Log(listener.name + " is in range of noise ");
                Debug.Log(listener.name + " found the noise source");
                    AgentController listenerAgentController = listener.GetComponent<AgentController>();
                    if (!listenerAgentController.enemyFov
                        .playerInSight) //if the agent had not seen the player before, look for the noise source
                    {
                        listenerAgentController.enemyFov.playerLastLocation = origin;
                        if (listenerAgentController._navMeshAgent.enabled)
                        {
                            if (listenerAgentController._navMeshAgent.CalculatePath(origin,
                                listenerAgentController._navMeshAgent.path))
                            {
                                listenerAgentController._navMeshAgent.SetDestination(origin);
                                listenerAgentController.alertTimer = listenerAgentController.agentAlertTime;
                                listenerAgentController._animator.SetBool("IsMoving", true);
                            }
                        }
                    }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow; 
        Gizmos.DrawWireSphere(transform.position+Vector3.up,sensorScale);
    }
}
