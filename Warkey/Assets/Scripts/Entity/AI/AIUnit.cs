using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIUnit : Unit
{
    AIEntity ai;
    public Animator animator;
    public RagdollManager ragdollManager;

    public new event System.Action<float> onDamageTaken;

    private new void Start() {
        base.Start();

        ai = GetComponent<AIEntity>();
        animator = GetComponentInChildren<Animator>();
    }

    public new void Destroy() {
        Destroy(gameObject,5);
    }

    public new void Die() { 
        SetLayerRecursively(gameObject,LayerMask.NameToLayer("Dead"));
        widgetAudio?.PlayAudio(WidgetAudio.Name.death);
        state = IWidget.State.dead;
        ai.IsDead = true;
        Invoke("Destroy", 5);
        disabler?.DisableComponents(0);
        disabler?.RemoveComponents(0);
        ragdollManager?.CreateRagdoll();
    }

    public override void TakeDamage(float damage) {
        if (state == IWidget.State.dead) return;
        animator.SetTrigger("hit");
        onDamageTaken?.Invoke(damage);
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


