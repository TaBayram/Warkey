using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    public Animator animator;
    public Movement.State movementState;
    public Weapon.State weaponState;

    private void Start() {
        animator = GetComponentInChildren<Animator>();
    }

    public void StateChange(Movement.State state) {
        if(state != movementState) {
            
            switch (state) {
                case Movement.State.idle:
                    animator.SetInteger("moveState", (int)state);
                    break;
                case Movement.State.walking:
                    animator.SetInteger("moveState", (int)state);
                    break;
                case Movement.State.sprinting:
                    animator.SetInteger("moveState", (int)state);
                    break;
            }

            movementState = state;
        }
    }
    public void StateChange(Weapon.State state) {
        if (state != weaponState) {
            switch (state) {
                case Weapon.State.idle:
                    animator.SetInteger("attackState", (int)state);
                    break;
                case Weapon.State.attacking:
                    animator.SetInteger("attackState", (int)state);
                    break;
            }


            weaponState = state;
        }
    }
}
