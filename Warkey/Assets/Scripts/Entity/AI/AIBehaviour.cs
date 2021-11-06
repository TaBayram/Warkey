using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class AIBehaviour
{
    protected Transform transform;
    protected NavMeshAgent navMeshAgent;

    public AIBehaviour(NavMeshAgent navMeshAgent, Transform transform) {
        this.navMeshAgent = navMeshAgent;
        this.transform = transform;
    }
}
