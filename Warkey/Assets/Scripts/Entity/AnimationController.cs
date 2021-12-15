using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [SerializeField] private AnimationClip[] replaceableMeleeAttackAnimations;
    [SerializeField] private AnimationClip[] replaceableRangedAttackAnimations;


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
            animator.SetInteger(Variables.moveState, (int)state);
            movementState = state;
        }
    }

    public void isJumping(bool isJumping) {
        animator.SetBool(Variables.isJumping, isJumping);
    }

    public void StateChange(Weapon.State state) {
        if (state != weaponState) {
            animator.SetInteger(Variables.attackState, (int)state);
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

    public void OnWeaponChanged(WeaponAnimations weaponAnimations) {
        if (weaponAnimations.isRanged) {
            animator.SetLayerWeight(1, 0);
            animator.SetLayerWeight(2, 1);
            animatorOverride[replaceableRangedAttackAnimations[0].name] =  weaponAnimations.attackAnimations[0];
            animatorOverride[replaceableRangedAttackAnimations[1].name] = weaponAnimations.attackAnimations[0];
        }
        else {
            animator.SetLayerWeight(1, 1);
            animator.SetLayerWeight(2, 0);
            for (int i = 0; i < 3 && i < weaponAnimations.attackAnimations.Length; i++)
                animatorOverride[replaceableMeleeAttackAnimations[i].name] = weaponAnimations.attackAnimations[i];
            animatorOverride[replaceableMeleeAttackAnimations[3].name] = weaponAnimations.defendAnimation;
        }
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

