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
        SetLayerRecursively(gameObject,LayerMask.NameToLayer("Ground"));
        state = IWidget.State.dead;
        ai.IsDead = true;
        Invoke("Destroy", 5);
        animator.enabled = false;
        GetComponentInChildren<AudioSource>().Play();
    }

    public override void TakeDamage(float damage) {
        if (state == IWidget.State.dead) return;
        animator.SetTrigger("hit");
        health.Current -= damage;
        if (health.Current <= 0) {
            Die();
        }
    }

    void SetLayerRecursively(GameObject obj, int newLayer  ) {
        obj.layer = newLayer;
        foreach(Transform child  in obj.transform) {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }

}


