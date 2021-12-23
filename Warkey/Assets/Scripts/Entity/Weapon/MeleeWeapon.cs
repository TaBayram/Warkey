using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : Weapon
{
    private const int maxCombo = 3;
    private const float nextAttackInputWaitTime = 0.50f;

    [SerializeField] protected float attackSpeed = 1f;
    [SerializeField] protected float baseDamage = 20;
    [SerializeField] protected int maxTargetPerAttack = 0;
    [SerializeField] MeleeWeaponAttack[] attacks;
    [SerializeField] AnimationClip defendClip;
    [SerializeField] AudioSource audioSource;
    

    private bool hasPressedForCombo = false;
    private bool takeInputForCombo = false;
    private int currentAttackIndex = 0;
    private float attackStartTime = 0;
    private float currentAttackSpeed;
    private float currentAttackDamage;
    private Coroutine coroutine;



    public override State CurrentState {
        get => state;
        set {
            
            if(state == State.attacking && value == State.idle && coroutine != null) {
                StopCoroutine(coroutine);
                currentAttackIndex = 0;
            }
            state = value;
            OnStateChange();
        }
    }

    private void LateUpdate() {
        if(CurrentState == State.defending) {
            OnRotateRequest(0.015f*Time.deltaTime);
        }
    }

    public override void Attack(Vector3 vector) {
        if (state == State.idle) {
            CurrentState = State.attacking;
            OnRotateRequest();
            StartAttack();
        }
        else if (state == State.attacking && takeInputForCombo) {
            hasPressedForCombo = true;
        }

    }

    private void StartAttack() {
        alreadyHit.Clear();
        coroutine = StartCoroutine(Cooldown());
        attackStartTime = Time.time;
        currentAttackSpeed = attackSpeed;
        currentAttackDamage = baseDamage * attacks[currentAttackIndex].damageMultiplier;
        OnRequest("attackSpeed", 1/attackSpeed);
    }

    IEnumerator Cooldown() {
        float totalWaitTime = attacks[currentAttackIndex].animationClip.length * currentAttackSpeed;
        float comboWaitTime = nextAttackInputWaitTime;
        if (totalWaitTime - comboWaitTime <= 0) {
            comboWaitTime = totalWaitTime;
            totalWaitTime = 0;
        }
        else {
            totalWaitTime -= comboWaitTime;
        }

        yield return new WaitForSeconds(totalWaitTime);
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
                OnRequest(AnimatorVariables.attackOne, null);
            }
            else if (currentAttackIndex == 2) {
                OnRequest(AnimatorVariables.attackTwo, null);
            }
            StartAttack();
            OnRotateRequest(0.050f);
        }
        else {
            CurrentState = State.idle;
            currentAttackIndex = 0;
        }
    }

    List<Collider> alreadyHit = new List<Collider>();
    private void OnTriggerEnter(Collider other) {
        TryDamage(other);
    }

    private void OnTriggerStay(Collider other) {
        TryDamage(other);
    }

    private void TryDamage(Collider other) {
        if (!IsInDamageTime()) return;
        if (maxTargetPerAttack != 0 && alreadyHit.Count > maxTargetPerAttack) return;
        if (state == State.attacking && !alreadyHit.Contains(other) && 1 << other.gameObject.layer == enemyLayer.value) {
            other.gameObject.GetComponent<IWidget>()?.TakeDamage(currentAttackDamage);
            alreadyHit.Add(other);
            audioSource.Play();

            var ent = other.gameObject.GetComponent<AIEntity>();
            if (ent) {
                ent.GetKnockedBack((other.transform.position - this.parent.position).normalized*10*knockback);
            }
        }
    }

    private bool IsInDamageTime() {
        float start = attackStartTime + attacks[currentAttackIndex].damageActivationDelay * currentAttackSpeed;
        float end = attackStartTime + attacks[currentAttackIndex].damageEndTime * currentAttackSpeed;
        return start <= Time.time && end >= Time.time;
    }

    private void OnTriggerExit(Collider other) {
        if (1 << other.gameObject.layer == enemyLayer.value) {
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

    public override void Stop() {
        CurrentState = State.idle;
    }
}


[System.Serializable]
public class MeleeWeaponAttack
{
    public AnimationClip animationClip;
    public float damageActivationDelay;
    public float damageEndTime;
    public float damageMultiplier = 1f;
    public float knockbackMultiplier = 1f;
    public Vector3 movement;
}