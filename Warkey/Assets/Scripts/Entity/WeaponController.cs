using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public Transform weaponHold;
    public Weapon startingWeapon;
	private Weapon equippedWeapon;

	void Start() {
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
	}

	public void Attack() {
		if (equippedWeapon != null) {
			equippedWeapon.Attack();
		}
	}
}
