using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class AIBehaviour
{
    protected AIEntity aiEntity;
    protected NavMeshAgent navMeshAgent;
    protected Transform transform;

    public AIBehaviour(AIEntity aiEntity) {
        this.aiEntity = aiEntity;
        this.navMeshAgent = aiEntity.navMeshAgent;
        this.transform = aiEntity.gameObject.transform;
    }
}
