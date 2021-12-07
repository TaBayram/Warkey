using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class ArtificialIntelligence : MonoBehaviour
{
    public Animator animator;
    private State state  = State.Wander;
    private State originalPassiveState;

    public PassiveAISettings passiveAISettings;
    public AggresiveAISettings aggresiveAISettings;
    public PassiveAIBehaviour passiveAIBehaviour;
    public AggresiveAIBehaviour aggresiveAIBehaviour;

    public NavMeshAgent navMeshAgent;
    public Vector3 origin;

    public Transform player;
    public LayerMask groundLayer, playerLayer;

    private void Awake() {
        navMeshAgent = GetComponent<NavMeshAgent>();
        origin = transform.position;

        passiveAIBehaviour = new PassiveAIBehaviour(navMeshAgent,transform, passiveAISettings);
        aggresiveAIBehaviour = new AggresiveAIBehaviour(navMeshAgent,transform, aggresiveAISettings);
        
    }

    private void Update() {
        if (this.transform == null) return;
        if (!IsOnNavMesh()) {
            //Integrate with Character Controller instead of this
            navMeshAgent.Warp(transform.position + Vector3.down * Time.deltaTime);
            return;
        }

        bool playerSighted = Physics.CheckSphere(transform.position, passiveAISettings.sightRange, playerLayer);
        bool playerLostSight = Physics.CheckSphere(transform.position, passiveAISettings.loseSightRange, playerLayer);
        bool playerIsAttackable = Physics.CheckSphere(transform.position, aggresiveAISettings.attackRange, playerLayer);

        switch (state) {
            case State.Stop:
                originalPassiveState = state;
                if (playerSighted)
                    state = State.Chase;
                break;
            case State.Wander:
                originalPassiveState = state;
                if (playerSighted)
                    state = State.Chase;
                else
                    passiveAIBehaviour.Wander();
                break;
            case State.Patrol:
                originalPassiveState = state;
                if (playerSighted)
                    state = State.Chase;
                else
                    passiveAIBehaviour.Patrol(origin);
                break;
            case State.Chase:
                if (playerIsAttackable)
                    state = State.Attack;
                else if (playerSighted || playerLostSight)
                    aggresiveAIBehaviour.Chase(player);
                else
                    state = State.Return;
                break;
            case State.Attack:
                if (playerIsAttackable)
                    aggresiveAIBehaviour.Attack(player);
                else
                    state = State.Chase;
                break;
            case State.Return:
                if (playerSighted)
                    state = State.Chase;
                else
                    Return();
                break;
            case State.Flee:
                if (playerSighted)
                    passiveAIBehaviour.Flee(player);
                else if (playerLostSight)
                    Return();
                break;
        }

        if((int)state == 0) {
            animator.SetInteger("moveState", 0);
        }
        else if((int)state != 4) {
            animator.SetInteger("moveState", 1);
        }
        else {
            animator.SetInteger("moveState", 0);
        }
        if((int)state == 4) {
            animator.SetInteger("attackState", 1);
        }
        else {
            animator.SetInteger("attackState", 0);
        }

    }

    public bool IsOnNavMesh() {
        NavMesh.SamplePosition(transform.position, out NavMeshHit navMeshHit, 2f, NavMesh.AllAreas);
        return navMeshHit.hit;
    }

    
    private void Return() {
        switch (originalPassiveState) {
            case State.Stop:
                state = originalPassiveState;
                navMeshAgent.SetDestination(origin);
                break;
            case State.Wander:
                state = originalPassiveState;
                break;
            case State.Patrol:
                state = originalPassiveState;
                break;
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
        Flee = 6,
    }
}
