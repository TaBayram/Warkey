using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [SerializeField] private AnimationClip[] replaceableAttackAnimations;


    public Animator animator;
    public Movement.State movementState;
    public Weapon.State weaponState;

    protected AnimatorOverrideController animatorOverride;
    AnimationState animationState;
    private void Start() {
        animator = GetComponentInChildren<Animator>();
        animatorOverride = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = animatorOverride;
    }

    public void StateChange(Movement.State state) {
        if(state != movementState) {
            
            switch (state) {
                case Movement.State.idle:
                    animator.SetInteger(Variables.moveState, (int)state);
                    break;
                case Movement.State.walking:
                    animator.SetInteger(Variables.moveState, (int)state);
                    break;
                case Movement.State.sprinting:
                    animator.SetInteger(Variables.moveState, (int)state);
                    break;
            }
            movementState = state;
        }
    }

    public void isJumping(bool isJumping) {
        animator.SetBool(Variables.isJumping, isJumping);
    }

    public void StateChange(Weapon.State state) {
        if (state != weaponState) {
            switch (state) {
                case Weapon.State.idle:
                    animator.SetInteger(Variables.attackState, (int)state);
                    break;
                case Weapon.State.attacking:
                    animator.SetInteger(Variables.attackState, (int)state);
                    break;
            }
            weaponState = state;
        }
    }

    public void SetValue(string name, object value) {
        if (value == null) {
            animator.SetTrigger(name);
            return;
        }
        System.Type type = value.GetType();
        if (type == typeof(int)) {
            animator.SetInteger(name, (int)value);
        }
        else if (type == typeof(bool)) {
            animator.SetBool(name, (bool)value);
        }
        else if (type == typeof(float)) {
            animator.SetFloat(name, (float)value);
        }
        
    }

    public void OnWeaponChanged(AnimationClip[] animationClips) {
        for (int i = 0; i < Mathf.Min(animationClips.Length, 3); i++)
            animatorOverride[replaceableAttackAnimations[i].name] = animationClips[i];
    }
    

    public static class Variables
    {
        public static string moveState = "moveState";
        public static string attackState = "attackState";
        public static string isJumping = "isJumping";
        public static string attackOne = "AttackOne";
        public static string attackTwo = "AttackTwo";
    }
}

