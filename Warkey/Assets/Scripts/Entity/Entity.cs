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

    public event System.Action<Entity,IWidget.State> onUnitStateChange;

    protected virtual void Start() {
        if(movement != null) {
            movement.onStateChange += Movement_onStateChange;
            movement.onVelocityChange += Movement_onVelocityChange;
        }
        if(weaponController != null) {
            weaponController.onStateChange += WeaponController_onStateChange;
        }
        if(unit != null) {
            unit.onStateChange += Unit_onStateChange;
        }
    }

    protected virtual void Update() {
    }


    private void Unit_onStateChange(IWidget.State obj) {
        onUnitStateChange?.Invoke(this, obj);
        if (unit.IsHero) {

        }
        else {

        }
    }


    protected void WeaponController_onStateChange(Weapon.State obj) {
        animationController?.StateChange(obj);

        if (movement == null) return;
        if(obj == Weapon.State.defending) {
            movement.isRotating = true;
            unit.IsBlocking = true;
        }
        else {
            movement.isRotating = false;
            unit.IsBlocking = false;
        }
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

    public void RotateToCameraTarget(float duration,Transform transform = null) {
        if (transform == null) {
            if (movement != null && movement.GetType() == typeof(PlayerMovementThird))
                ((PlayerMovementThird)movement).RotateToTarget(duration);
        }
        else {
            if (movement != null)
                (movement).RotateToTarget(transform);
        }

    }

    public float MovementScaler() {
        if (weaponController == null) return 1;

        switch (weaponController.state) {
            case Weapon.State.attacking:
                return .25f;
            case Weapon.State.defending:
                return .5f;
            default: 
                return 1;
        }
    }

    public bool CanMove() {
        return true;
    }

    public bool CanAttack() {
        return movement == null ? true : movement.state != Movement.State.sprinting;
    }

    internal bool UseStamina(float v, float minV = 0) {
        if(unit != null) {
            return unit.UseStamina(v,minV);
        }
        else {
            return true;
        }
    }
}
