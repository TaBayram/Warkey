using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Entity))]
public class WeaponController : MonoBehaviour
{
	protected Entity entity;

    public Transform rightHand;
	public Transform leftHand;
    public Weapon startingWeapon;
	public Weapon.State state;

	public event System.Action<Weapon.State> onStateChange;

	private Weapon equippedWeapon;
	private bool isDefending;

	void Start() {
		entity = GetComponent<Entity>();

		if (startingWeapon != null) {
			EquipWeapon(startingWeapon);
		}
	}

	public void EquipWeapon(Weapon weapon) {
		if (equippedWeapon != null) {
			Destroy(equippedWeapon.gameObject);
		}
        if (weapon.isMainHandRight) {
			equippedWeapon = Instantiate(weapon, rightHand.position, rightHand.rotation);
			equippedWeapon.transform.parent = rightHand;
		}
        else {
			equippedWeapon = Instantiate(weapon, leftHand.position, leftHand.rotation);
			equippedWeapon.transform.parent = leftHand;
		}

		
        equippedWeapon.onStateChange += EquippedWeapon_onStateChange;
        equippedWeapon.onAnimationChangeRequest += EquippedWeapon_onAnimationChangeRequest;

		entity.animationController.OnWeaponChanged(equippedWeapon.GetAnimations());
	}

    private void EquippedWeapon_onAnimationChangeRequest(string arg1, object arg2) {
		entity.animationController.SetValue(arg1, arg2);
    }

    private void EquippedWeapon_onStateChange(Weapon.State obj) {
		state = obj;
		onStateChange(obj);
	}

    public void Attack() {
		if (equippedWeapon != null) {
			equippedWeapon.Attack(GetComponent<Entity>().velocity);
		}
	}

	public void Defend(bool pressed) {
		if (equippedWeapon != null && isDefending != pressed) {
			isDefending = pressed;
			equippedWeapon.Defend(pressed);
		}
	}

	public void SetState(Weapon.State state) {
		this.state = state;
		equippedWeapon.CurrentState = state;
    }
}
