using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public Unit unit;
    public Movement movement;
    public WeaponController weaponController;
    public AnimationController animationController;

    public Vector3 velocity;

    private void Start() {
        if(movement != null) {
            movement.onStateChange += Movement_onStateChange;
            movement.onVelocityChange += Movement_onVelocityChange;
        }
        if(weaponController != null) {
            weaponController.onStateChange += WeaponController_onStateChange;
        }
    }

    private void Update() {
    }

    protected void WeaponController_onStateChange(Weapon.State obj) {
        animationController?.StateChange(obj);
    }

    internal void OnWeaponChanged(WeaponAnimations weaponAnimations) {
        animationController?.OnWeaponChanged(weaponAnimations);
    }

    protected void Movement_onVelocityChange(Vector3 obj) {
        velocity = obj;
    }

    protected void Movement_onStateChange(Movement.State obj) {
        animationController?.StateChange(obj);   
    }

    public void RotateToCameraTarget(float duration) {
        if (movement != null && movement.GetType() == typeof(PlayerMovementThird))
            ((PlayerMovementThird)movement).RotateToTarget(duration);
    }

    public bool CanMove() {
        return weaponController == null ? true : weaponController.state != Weapon.State.attacking;
    }

    public bool CanAttack() {
        return movement == null ? true : movement.state != Movement.State.sprinting;
    }

}
