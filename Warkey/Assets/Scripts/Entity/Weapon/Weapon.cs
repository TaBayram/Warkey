using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [HideInInspector] public Transform parent;
    public bool isMainHandRight;
    public event System.Action<State> onStateChange;
    public event System.Action<string, object> onAnimationChangeRequest;
    public event System.Action<float> onRotateRequest;
    [HideInInspector] public AnimationClip[] animations;
    [HideInInspector] public LayerMask enemyLayer;

    protected State state;
    [SerializeField] protected float attackRange = 1f;
    [SerializeField] protected float knockback = 1f;

    public abstract State CurrentState {
        get;
        set;
    }

    public enum State
    {
        idle = 0,
        attacking = 1,
        defending = 2,
    }
    public float AttackRange { get => attackRange; }
    public abstract void Attack(Vector3 initialVelocity);
    public abstract void Defend(bool pressed);
    public abstract void Stop();
    public abstract WeaponAnimations GetAnimations();

    public void OnStateChange() {
        onStateChange?.Invoke(state);
    }

    public void OnRotateRequest(float duration = 0) {
        onRotateRequest?.Invoke(duration);
    }

    public void OnRequest(string name, object value) {
        onAnimationChangeRequest?.Invoke(name, value);
    }
}


[System.Serializable]
public struct WeaponAnimations
{
    public bool isRanged;
    public AnimationClip defendAnimation;
    public AnimationClip idleAnimation;
    public AnimationClip[] attackAnimations;

    public WeaponAnimations(bool isRanged, AnimationClip defendAnimation, AnimationClip idleAnimation, AnimationClip[] attackAnimations) {
        this.isRanged = isRanged;
        this.defendAnimation = defendAnimation;
        this.idleAnimation = idleAnimation;
        this.attackAnimations = attackAnimations;
    }
}
