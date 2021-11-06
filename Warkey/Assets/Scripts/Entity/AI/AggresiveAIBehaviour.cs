using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;
public class AggresiveAIBehaviour : AIBehaviour
{
    AggresiveAISettings aggresiveAISettings;
    float attackCooldown;

    public AggresiveAIBehaviour(NavMeshAgent navMeshAgent, Transform transform, AggresiveAISettings aggresiveAISettings) : base(navMeshAgent, transform) {
        this.aggresiveAISettings = aggresiveAISettings;

    }

    public void Attack(Transform target) {
        navMeshAgent.ResetPath();
        if (Time.time > attackCooldown) {
            transform.LookAt(target);
            attackCooldown = Time.time + 1 / aggresiveAISettings.attackSpeed;
            target.GetComponent<IWidget>().TakeDamage(aggresiveAISettings.attackDamage);
        }

    }

    public void Chase(Transform chased) {
        navMeshAgent.SetDestination(chased.position);
    }
}
