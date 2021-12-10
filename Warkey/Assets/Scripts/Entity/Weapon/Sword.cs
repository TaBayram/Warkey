using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : Weapon
{
    public float attackSpeed = 1f;
	public float missileSpeed = 35;
	public float attackDamage = 20;
    public int attackCombo = 0;
    public int maxCombo = 3;
    public bool hasPressedForCombo = false;
    public bool takeInputForCombo = false;

	public override void Attack(Vector3 vector) {
		if (state == State.idle) {
            state = State.attacking;
            OnStateChange();
            StartCoroutine(Cooldown());
        }
        else if(state == State.attacking && takeInputForCombo) {
            hasPressedForCombo = true;
        }
        
    }

    IEnumerator Cooldown() {
        alreadyHit.Clear();
        float totalWaitTime = animations[attackCombo].length * attackSpeed;
        float comboWaitTime = 0.50f;
        yield return new WaitForSeconds(totalWaitTime - comboWaitTime);
        takeInputForCombo = true;
        yield return new WaitForSeconds(comboWaitTime-0.01f);
        takeInputForCombo = false;
        if (hasPressedForCombo) {
            attackCombo++;
            if(attackCombo == 1) {
                OnRequest(AnimationController.Variables.attackOne, null);
                StartCoroutine(Cooldown());
            }
            else if(attackCombo == 2) {
                OnRequest(AnimationController.Variables.attackTwo, null);
                StartCoroutine(Cooldown());
            }
            else {
                state = State.idle;
                attackCombo = 0;
                OnStateChange();
            }
        }
        else {
            state = State.idle;
            attackCombo = 0;
            OnStateChange();
        }
        hasPressedForCombo = false;
    }


    



    private void OnCollisionEnter(Collision other) {

    }
    List<Collider> alreadyHit = new List<Collider>();
    private void OnTriggerEnter(Collider other) {
        if (state == State.attacking && !alreadyHit.Contains(other) && other.gameObject.layer == LayerMask.NameToLayer("Enemy")) {
            other.gameObject.GetComponent<IWidget>().TakeDamage(attackDamage);
            alreadyHit.Add(other);
        }
    }
}


