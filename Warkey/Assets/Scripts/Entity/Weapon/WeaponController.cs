using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Entity))]
public class WeaponController : MonoBehaviour
{
	protected Entity entity;

	[SerializeField] private WidgetAudio widgetAudio;
	[SerializeField] private LayerMask enemyLayer;
    public Transform rightHand;
	public Transform leftHand;
    public Weapon startingWeapon;
	public Weapon.State state;

	public event System.Action<Weapon.State> onStateChange;
	public event System.Action<Weapon> onWeaponChange;

	private Weapon equippedWeapon;
	private bool isDefending;

	public float AttackRange { get { if (equippedWeapon != null) return equippedWeapon.AttackRange; else return 2; } }

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
        equippedWeapon.onRotateRequest += EquippedWeapon_onRotateRequest;
        equippedWeapon.onAttack += EquippedWeapon_onAttack;
		equippedWeapon.enemyLayer = enemyLayer;
		equippedWeapon.parent = transform;

		entity.OnWeaponChanged(equippedWeapon.GetAnimations());

		onWeaponChange?.Invoke(weapon);
	}

    private void EquippedWeapon_onAttack() {
		widgetAudio?.PlayAudio(WidgetAudio.Name.attacks);
    }

    private void EquippedWeapon_onRotateRequest(float obj,Transform transform) {
		entity.RotateToCameraTarget(obj, transform);
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

	public void Stop() {
		if (equippedWeapon != null) {
			equippedWeapon.Stop();
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
