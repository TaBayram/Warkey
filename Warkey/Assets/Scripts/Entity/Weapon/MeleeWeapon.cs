using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : Weapon
{
    private const int maxCombo = 3;
    private const float nextAttackInputWaitTime = 0.50f;

    [SerializeField] protected float attackSpeed = 1f;
    [SerializeField] protected float baseDamage = 20;
    [SerializeField] MeleeWeaponAttack[] attacks;
    [SerializeField] AnimationClip defendClip;
    [SerializeField] AudioSource audioSource;
    

    private bool hasPressedForCombo = false;
    private bool takeInputForCombo = false;
    private int currentAttackIndex = 0;
    private float attackStartTime = 0;
    private float currentAttackSpeed;
    private Coroutine coroutine;

    public override State CurrentState {
        get => state;
        set {
            state = value;
            if(value == State.idle && coroutine != null) {
                StopCoroutine(coroutine);
            }
            OnStateChange();
        }
    }

    public override void Attack(Vector3 vector) {
        if (state == State.idle) {
            CurrentState = State.attacking;
            StartAttack();
        }
        else if (state == State.attacking && takeInputForCombo) {
            hasPressedForCombo = true;
        }

    }

    private void StartAttack() {
        coroutine = StartCoroutine(Cooldown());
        attackStartTime = Time.time;
        currentAttackSpeed = attackSpeed;
        alreadyHit.Clear();
    }

    IEnumerator Cooldown() {
        float totalWaitTime = attacks[currentAttackIndex].animationClip.length * currentAttackSpeed;
        float comboWaitTime = nextAttackInputWaitTime;
        yield return new WaitForSeconds(totalWaitTime - comboWaitTime);
        takeInputForCombo = true;
        yield return new WaitForSeconds(comboWaitTime - 0.01f);
        takeInputForCombo = false;
        OnAttackEnd();
        hasPressedForCombo = false;
    }

    void OnAttackEnd() {
        if (state != State.attacking) return;
        if (hasPressedForCombo && (currentAttackIndex + 1) < maxCombo && (currentAttackIndex + 1) < attacks.Length) {
            currentAttackIndex++;
            if (currentAttackIndex == 1) {
                OnRequest(AnimationController.Variables.attackOne, null);
                StartAttack();
            }
            else if (currentAttackIndex == 2) {
                OnRequest(AnimationController.Variables.attackTwo, null);
                StartAttack();
            }
        }
        else {
            CurrentState = State.idle;
            currentAttackIndex = 0;
        }
    }
    private void OnCollisionEnter(Collision other) {

    }
    List<Collider> alreadyHit = new List<Collider>();
    private void OnTriggerEnter(Collider other) {
        if (attackStartTime + attacks[currentAttackIndex].damageActivationDelay*currentAttackSpeed > Time.time || attackStartTime + attacks[currentAttackIndex].damageEndTime * currentAttackSpeed < Time.time) return;
        if (state == State.attacking && !alreadyHit.Contains(other) && other.gameObject.layer == LayerMask.NameToLayer("Enemy")) {
            other.gameObject.GetComponent<IWidget>().TakeDamage(baseDamage);
            alreadyHit.Add(other);
            audioSource.Play();
        }
    }

    public override void Defend(bool pressed) {
        if (pressed)
            CurrentState = State.defending;
        else
            CurrentState = State.idle;
    }

    public override WeaponAnimations GetAnimations() {
        WeaponAnimations weaponAnimations = new WeaponAnimations(false,defendClip,null,new AnimationClip[this.attacks.Length]);
        for(int i = 0; i < this.attacks.Length; i++) {
            weaponAnimations.attackAnimations[i] = this.attacks[i].animationClip;
        }

        return weaponAnimations;
    }
}


[System.Serializable]
public struct MeleeWeaponAttack
{
    public AnimationClip animationClip;
    public float damageActivationDelay;
    public float damageEndTime;
    public Vector3 movement;
}