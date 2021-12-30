using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponController : WeaponController
{
	void Update() {
		if (!entity.CanAttack()) return;
		if (!GetComponent<PhotonView>().IsMine && PhotonNetwork.IsConnected) return;
		// Weapon input
		if (Input.GetMouseButton(0) && GameState.CurrentState == GameState.State.ingame) {
			Attack();
		}
		if (GameState.CurrentState == GameState.State.ingame) {
			Defend(Input.GetMouseButton(1));
		}
	}
}
