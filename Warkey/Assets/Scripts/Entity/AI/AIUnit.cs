using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

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

        photonView = GetComponent<PhotonView>();
    }

    public new void Destroy() {
        Destroy(gameObject,5);
    }

    public new void Die() { 
        SetLayerRecursively(gameObject,LayerMask.NameToLayer("Dead"));
        widgetAudio?.PlayAudio(WidgetAudio.Name.death);
        State = IWidget.State.dead;
        ai.IsDead = true;
        Invoke("Destroy", 5);
        disabler?.DisableComponents(0);
        disabler?.RemoveComponents(0);
        ragdollManager?.CreateRagdoll();
    }

    public override void TakeDamage(float damage) {

        photonView.RPC("RPC_TakeDamage", RpcTarget.All, damage);
    }

    [PunRPC]
    void RPC_TakeDamage(float damage)
    {
        if (State == IWidget.State.dead) return;
        animator.SetTrigger("hit");
        onDamageTaken?.Invoke(damage);
        healthRegenCooldown = health.Cooldown;
        health.Current -= damage;
        if (health.Current <= 0)
        {
            Die();
        }
        else {
            widgetAudio?.PlayAudio(WidgetAudio.Name.gotHit);
        }
    }

    void SetLayerRecursively(GameObject obj, int newLayer  ) {
        obj.layer = newLayer;
        foreach(Transform child  in obj.transform) {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }

}


