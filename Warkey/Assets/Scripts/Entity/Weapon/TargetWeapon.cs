using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetWeapon : Weapon
{
    private const int maxCombo = 3;
    private const float nextAttackInputWaitTime = 0.50f;

    [SerializeField] protected float attackSpeed = 1f;
    [SerializeField] protected float baseDamage = 20;
    [SerializeField] protected int maxTargetPerAttack = 0;
    [SerializeField] TargetWeaponAttack[] attacks;
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
        Invoke(nameof(TryDamage), attacks[currentAttackIndex].damageDelay * currentAttackSpeed);
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

    private void TryDamage() {
        if (maxTargetPerAttack != 0 && alreadyHit.Count > maxTargetPerAttack) return;
        if (state == State.attacking) {
            Collider[] colliders = Physics.OverlapSphere(parent.position, attackRange + .5f);

            foreach(Collider collider in colliders) {
                if(!alreadyHit.Contains(collider) && 1 << collider.gameObject.layer == enemyLayer.value) {
                    float angle = Vector3.Angle(parent.forward, collider.transform.position - parent.position);
                    Debug.Log(angle);
                    if (Mathf.Abs(angle) <= attacks[currentAttackIndex].maxAngleDifference) {
                        collider.gameObject.GetComponent<IWidget>()?.TakeDamage(currentAttackDamage);
                        alreadyHit.Add(collider);
                        audioSource.Play();

                        var ent = collider.gameObject.GetComponent<AIEntity>();
                        if (ent) {
                            ent.GetKnockedBack((collider.transform.position - this.parent.position).normalized * 10 * knockback);
                        }
                    }  
                }
            }

            
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
public class TargetWeaponAttack
{
    public AnimationClip animationClip;
    public float damageDelay;
    [Range(0,180)]
    public float maxAngleDifference;
    public float damageMultiplier = 1f;
    public float knockbackMultiplier = 1f;
    public Vector3 movement;
}