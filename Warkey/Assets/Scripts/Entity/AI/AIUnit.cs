using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIUnit : Unit
{
    ArtificialIntelligence ai;
    public Animator animator;

    private new void Start() {
        base.Start();

        ai = GetComponent<ArtificialIntelligence>();
        animator = GetComponentInChildren<Animator>();
    }

    public new void Destroy() {
        Destroy(gameObject);
    }

    public new void Die() {
        ai.IsDead = true;
        animator.SetTrigger("die");
        Invoke("Destroy", 5);
    }

    public override void TakeDamage(float damage) {
        animator.SetTrigger("hit");
        health.Current -= damage;
        if (health.Current <= 0) {
            Die();
        }
    }

}
