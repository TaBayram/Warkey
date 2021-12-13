using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Entity))]
public class WeaponController : MonoBehaviour
{
	private Entity entity;

    public Transform weaponHold;
    public Weapon startingWeapon;
	private Weapon equippedWeapon;

	public State state;

	public event System.Action<Weapon.State> onStateChange;
	
	public enum State
	{
		idle = 0,
		attacking = 1,
	}

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
		equippedWeapon = Instantiate(weapon, weaponHold.position, weaponHold.rotation) as Weapon;
		equippedWeapon.transform.parent = weaponHold;
        equippedWeapon.onStateChange += EquippedWeapon_onStateChange;
        equippedWeapon.onAnimationChangeRequest += EquippedWeapon_onAnimationChangeRequest; ;
		entity.animationController.OnWeaponChanged(equippedWeapon.animations);
	}

    private void EquippedWeapon_onAnimationChangeRequest(string arg1, object arg2) {
		entity.animationController.SetValue(arg1, arg2);
    }

    private void EquippedWeapon_onStateChange(Weapon.State obj) {
		state = (State)(int)obj;
		onStateChange(obj);
	}

    public void Attack() {
		if (equippedWeapon != null) {
			equippedWeapon.Attack(GetComponent<Entity>().velocity);
		}
	}

	public void SetState(WeaponController.State state) {
		this.state = state;
		equippedWeapon.CurrentState = (Weapon.State)state;
    }
}
