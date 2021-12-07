using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : Weapon
{
    
    public float attackSpeed = 0.25f;
	public float missileSpeed = 35;
	public float attackDamage = 20;

	public override void Attack(Vector3 vector) {
		if (state == State.idle) {
            state = State.attacking;
            OnStateChange();
            StartCoroutine(Cooldown());
        }
        
    }

    IEnumerator Cooldown() {
        yield return new WaitForSeconds(1/attackSpeed);
        state = State.idle;
        OnStateChange();
    }



    private void OnCollisionEnter(Collision other) {
        
    }

    private void OnTriggerEnter(Collider other) {
        if (state == State.attacking && other.gameObject.layer == LayerMask.NameToLayer("Enemy")) {
            other.gameObject.GetComponent<IWidget>().TakeDamage(attackDamage);
        }
    }



}
