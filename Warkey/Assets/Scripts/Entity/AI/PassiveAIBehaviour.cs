using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

public class PassiveAIBehaviour:AIBehaviour
{
    PassiveAISettings passiveAISettings;
    BehaviourData patrolData;
    BehaviourData wanderData;
    BehaviourData fleeData;

    public PassiveAIBehaviour(NavMeshAgent navMeshAgent, Transform transform, PassiveAISettings passiveAISettings) :base(navMeshAgent, transform) {
        this.passiveAISettings = passiveAISettings;
        patrolData = new BehaviourData();
        wanderData = new BehaviourData();
        fleeData = new BehaviourData();
    }


    public void Wander() {
        if (!wanderData.IsNextPointSet) {
            SetWanderPoint();
        }
        if (wanderData.IsNextPointSet) {
            navMeshAgent.SetDestination(wanderData.NextPoint);
        }
        if (!wanderData.HasArrived && (transform.position - wanderData.NextPoint).magnitude < 1f) {
            wanderData.Duration = Time.time + passiveAISettings.wanderStopTime;
            wanderData.HasArrived = true;
        }
        if (wanderData.HasArrived && Time.time > wanderData.Duration) {
            wanderData.IsNextPointSet = false;
        }
    }
    private void SetWanderPoint() {
        float x = Random.Range(-passiveAISettings.wanderRange, passiveAISettings.wanderRange);
        float z = Random.Range(-passiveAISettings.wanderRange, passiveAISettings.wanderRange);
        wanderData.NextPoint = new Vector3(x, 0, z) + transform.position;
        NavMesh.SamplePosition(wanderData.NextPoint, out NavMeshHit navMeshHit, 10f, NavMesh.AllAreas);

        if (navMeshHit.hit) {
            wanderData.NextPoint = navMeshHit.position;
            wanderData.IsNextPointSet = true;
            wanderData.HasArrived = false;
        }
    }

    public void Patrol(Vector3 origin) {
        if (!patrolData.IsNextPointSet) {
            SetPatrolPoint(origin);
        }
        if (patrolData.IsNextPointSet) {
            navMeshAgent.SetDestination(patrolData.NextPoint);
        }
        if (!patrolData.HasArrived && (transform.position - patrolData.NextPoint).magnitude < 1f) {
            patrolData.Duration = Time.time + passiveAISettings.patrolStopTime;
            patrolData.HasArrived = true;
        }
        if (patrolData.HasArrived && Time.time > patrolData.Duration) {
            patrolData.IsNextPointSet = false;
        }
    }

    private void SetPatrolPoint(Vector3 origin) {
        float x = Random.Range(-passiveAISettings.patrolRange, passiveAISettings.patrolRange);
        float z = Random.Range(-passiveAISettings.patrolRange, passiveAISettings.patrolRange);
        patrolData.NextPoint = new Vector3(x, 0, z) + origin;
        NavMesh.SamplePosition(patrolData.NextPoint, out NavMeshHit navMeshHit, 10f, NavMesh.AllAreas);

        if (navMeshHit.hit) {
            patrolData.NextPoint = navMeshHit.position;
            patrolData.IsNextPointSet = true;
            patrolData.HasArrived = false;
        }
    }

    public void Flee(Transform chaser) {
        if (!fleeData.IsNextPointSet) {
            SetFleePoint(chaser);
        }
        if (fleeData.IsNextPointSet) {
            navMeshAgent.SetDestination(fleeData.NextPoint);
        }
        if ((transform.position - fleeData.NextPoint).magnitude < 1f) {
            fleeData.IsNextPointSet = false;
        }
    }

    private void SetFleePoint(Transform chaser) {
        
        float distance = Random.Range(0, passiveAISettings.fleeRange);
        Vector3 direction = (transform.position - chaser.position)*distance;
        Vector3 jitter = new Vector3(direction.x*Random.Range(-0.75f,0.75f),0, direction.z * Random.Range(-0.75f, 0.75f));

        fleeData.NextPoint = new Vector3(direction.x * .50f + jitter.x * .50f, 0, direction.z * .50f + jitter.z * .50f) + transform.position;
        NavMesh.SamplePosition(fleeData.NextPoint, out NavMeshHit navMeshHit, 10f, NavMesh.AllAreas);

        if (navMeshHit.hit) {
            fleeData.NextPoint = navMeshHit.position;
            fleeData.IsNextPointSet = true;
        }
    }

    private struct BehaviourData
    {
        Vector3 nextPoint;
        bool isNextPointSet;
        bool hasArrived;
        float duration;

        public Vector3 NextPoint { get => nextPoint; set => nextPoint = value; }
        public bool IsNextPointSet { get => isNextPointSet; set => isNextPointSet = value; }
        public bool HasArrived { get => hasArrived; set => hasArrived = value; }
        public float Duration { get => duration; set => duration = value; }
    }
}


