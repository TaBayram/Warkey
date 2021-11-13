using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponController : WeaponController
{
	void Update() {
		// Weapon input
		if (Input.GetMouseButton(0) && GameState.CurrentState == GameState.State.ingame) {
			Attack();
		}
	}
}
