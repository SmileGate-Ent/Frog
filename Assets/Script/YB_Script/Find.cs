using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Find : MonoBehaviour
{
    [SerializeField] private Transform moveTarget;

    private NavMeshAgent _agent;
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;    
    }

    // Update is called once per frame
    void Update()
    {
        _agent.SetDestination(moveTarget.position);
    }
}
