using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ArtificialIntelligence : MonoBehaviour
{
    private State state  = State.Patrol;
    public AISettings aiSettings;
    public NavMeshAgent navMeshAgent;
    public Vector3 origin;

    public Transform player;
    public LayerMask groundLayer, playerLayer;

    private bool isPatrolPointSet;
    private Vector3 nextPatrolPoint;

    private void Awake() {
        navMeshAgent = GetComponent<NavMeshAgent>();
        origin = transform.position;
    }

    private void Update() {
        bool playerSighted = Physics.CheckSphere(transform.position, aiSettings.sightRange, playerLayer);
        bool playerIsAttackable = Physics.CheckSphere(transform.position, aiSettings.attackRange, playerLayer);

        switch (state) {
            case State.Stop:
                if (playerSighted)
                    state = State.Chase;
                break;
            case State.Wander:
                if (playerSighted)
                    state = State.Chase;
                break;
            case State.Patrol:
                if (playerSighted)
                    state = State.Chase;
                else
                    Patrol();
                break;
            case State.Chase:
                if (playerIsAttackable)
                    state = State.Attack;
                else if (playerSighted)
                    Chase();
                else
                    state = State.Patrol;
                break;
            case State.Attack:
                if (playerIsAttackable)
                    Attack();
                else
                    Chase();
                break;
        }

    }

    private void Patrol() {
        if (!isPatrolPointSet) {
            SetPatrolPoint();
        }
        if (isPatrolPointSet) {
            navMeshAgent.SetDestination(nextPatrolPoint);
        }
        if((transform.position - nextPatrolPoint).magnitude < 1f) {
            isPatrolPointSet = false;
        }
    }

    private void Chase() {
        navMeshAgent.SetDestination(player.transform.position);
    }

    private void Attack() {

    }

    private void SetPatrolPoint() {
        float x = Random.Range(-aiSettings.patrolRange, aiSettings.patrolRange);
        float z = Random.Range(-aiSettings.patrolRange, aiSettings.patrolRange);
        nextPatrolPoint = new Vector3(x, 0, z) + origin;

        if (Physics.Raycast(nextPatrolPoint, -transform.up, 2f, groundLayer.value)) {
            isPatrolPointSet = true;
        }

    }



    public enum State
    {
        Stop = 0,
        Wander = 1,
        Patrol = 2,
        Chase = 3,
        Attack = 4,
        Return = 5,
    }
}


[System.Serializable]
public class AISettings
{
    public float patrolRange;
    public float sightRange;
    public float attackRange;
    public float attackSpeed;
}
