using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    EntityData entityData;

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

    private void WeaponController_onStateChange(Weapon.State obj) {
        if (animationController != null) {
            animationController.StateChange(obj);
        }
    }

    private void Movement_onVelocityChange(Vector3 obj) {
        velocity = obj;
    }

    private void Movement_onStateChange(Movement.State obj) {
        if (animationController != null) {
            animationController.StateChange(obj);
        }
    }
}
