using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

[RequireComponent(typeof(NavMeshAgent))]
public class AIEntity : Entity
{
    private State state  = State.Wander;
    private State originalPassiveState;
    private Movement.State movementState;

    public PassiveAISettings passiveAISettings;
    public AggresiveAISettings aggresiveAISettings;
    public PassiveAIBehaviour passiveAIBehaviour;
    public AggresiveAIBehaviour aggresiveAIBehaviour;

    public NavMeshAgent navMeshAgent;
    public LayerMask groundLayer, playerLayer;

    public event System.Action<State> onStateChange;
    
    private bool isDead = false;
    private List<GameObject> players = new List<GameObject>();
    private GameObject targetPlayer;

    private Vector3 origin;
    private bool playerSighted;
    private bool playerLostSight;
    private bool playerIsAttackable;

    PhotonView PV;


    private float AttackRange { get { if (weaponController != null) return weaponController.AttackRange; else return aggresiveAISettings.attackRange; } }
    public bool IsDead { get => isDead; set { isDead = value; } }

    public State CurrentState { get => state; set { state = value; onStateChange?.Invoke(state); SetMovementState(); } }

    private void SetMovementState() {
        if(CurrentState == State.Attack || CurrentState == State.Stop) {
            movementState = Movement.State.idle;
        }
        else {
            movementState = Movement.State.walking;
        }
        Movement_onStateChange(movementState);
    }

    private void Awake() {
        PV = GetComponent<PhotonView>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        origin = transform.position;

        passiveAIBehaviour = new PassiveAIBehaviour(this, passiveAISettings);
        aggresiveAIBehaviour = new AggresiveAIBehaviour(this, aggresiveAISettings);


        if (!IsOnNavMesh()) {
            NavMesh.SamplePosition(transform.position, out NavMeshHit navMeshHit, 50f, NavMesh.AllAreas);
            if(navMeshHit.hit)
                navMeshAgent.Warp(navMeshHit.position);
        } 
    }

    protected override void Start() {
        CurrentState = State.Patrol;
        base.Start();
    }
    protected override void Update() {
        if (!PV.IsMine) return;
        if (IsDead || this.transform == null) return;
        if (!IsOnNavMesh()) {
            NavMesh.SamplePosition(transform.position, out NavMeshHit navMeshHit, 100f, NavMesh.AllAreas);
            if (navMeshHit.hit)
                navMeshAgent.Warp(navMeshHit.position);
            return;
        }

        UpdatePlayers();

        playerSighted = targetPlayer != null ? true: false;
        playerLostSight = targetPlayer != null ? (transform.position - targetPlayer.transform.position).magnitude > passiveAISettings.loseSightRange : true;
        playerIsAttackable = targetPlayer != null ?  (transform.position - targetPlayer.transform.position).magnitude < AttackRange : false;
        SetBehaviour();
        
        if (playerLostSight) targetPlayer = null;
    }

    private void SetBehaviour() {
        if(weaponController.state == Weapon.State.attacking || state == State.Knocked) {
            return; 
        }

        switch (CurrentState) {
            case State.Stop:
                originalPassiveState = CurrentState;
                if (playerSighted)
                    CurrentState = State.Chase;
                break;
            case State.Wander:
                originalPassiveState = CurrentState;
                if (playerSighted)
                    CurrentState = State.Chase;
                else
                    passiveAIBehaviour.Wander();
                break;
            case State.Patrol:
                originalPassiveState = CurrentState;
                if (playerSighted)
                    CurrentState = State.Chase;
                else
                    passiveAIBehaviour.Patrol(origin);
                break;
            case State.Chase:
                if (playerIsAttackable)
                    CurrentState = State.Attack;
                else if (playerSighted)
                    aggresiveAIBehaviour.Chase(targetPlayer.transform);
                else if (playerLostSight)
                    CurrentState = State.Return;
                break;
            case State.Attack:
                if (playerIsAttackable)
                    aggresiveAIBehaviour.Attack(targetPlayer.transform);
                else
                    CurrentState = State.Chase;
                break;
            case State.Return:
                if (playerSighted)
                    CurrentState = State.Chase;
                else
                    Return();
                break;
            case State.Flee:
                if (playerSighted)
                    passiveAIBehaviour.Flee(targetPlayer.transform);
                else if (playerLostSight)
                    Return();
                break;
        }
    }

    private void UpdatePlayers() {
        Collider[] colliders = Physics.OverlapSphere(transform.position, passiveAISettings.sightRange, playerLayer);
        players.Clear();
        foreach (Collider collider in colliders) {
            players.Add(collider.gameObject);
        }

        if(targetPlayer == null && players.Count != 0) {
            GameObject closestPlayer = players[0];
            for (int i = 1; i < players.Count; i++) {
                if ((transform.position - closestPlayer.transform.position).sqrMagnitude > (transform.position - players[i].transform.position).sqrMagnitude) {
                    closestPlayer = players[i];
                }
            }
            targetPlayer = closestPlayer;
        }
    }

    public void FillTargetPlayer(GameObject newTarget,bool replace = false) {
        if (targetPlayer != null && replace) {
            targetPlayer = newTarget;
        }
        if(targetPlayer == null) {
            targetPlayer = newTarget;
        }
    }

    public bool IsOnNavMesh() {
        NavMesh.SamplePosition(transform.position, out NavMeshHit navMeshHit, 2f, NavMesh.AllAreas);
        return navMeshAgent.isOnNavMesh;
    }

    private void Return() {
        switch (originalPassiveState) {
            case State.Stop:
                CurrentState = originalPassiveState;
                navMeshAgent.SetDestination(origin);
                break;
            case State.Wander:
                CurrentState = originalPassiveState;
                break;
            case State.Patrol:
                CurrentState = originalPassiveState;
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
        Knocked = 7
    }

    public void GetKnockedBack(Vector3 force) {
        navMeshAgent.enabled = false;
        var rigid = GetComponent<Rigidbody>();
        rigid.isKinematic = false;

        rigid.AddForce(force, ForceMode.Impulse);
        state = State.Knocked;
        knockTime = 0.5f;
        StartCoroutine(StopKnock());
    }
    float knockTime;
    const float knockbackDeceleration = 0.10f;
    public IEnumerator StopKnock() {
        yield return new WaitForSeconds(0.01f);
        knockTime -= 0.01f;
        var rigid = GetComponent<Rigidbody>();
        if (knockTime > 0) {
            rigid.velocity *= (1f - knockbackDeceleration);
            StartCoroutine(StopKnock());
        }
        else {
            rigid.velocity = Vector3.zero;
            rigid.isKinematic = true;
            navMeshAgent.enabled = true;
            state = State.Stop;
        }

    }
}
