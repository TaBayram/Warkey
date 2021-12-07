using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : Weapon
{
	public Transform muzzle;
	public Projectile projectile;
	public float attackSpeed = 1;
	public float missileSpeed = 35;
	public float attackDamage = 20;

	float cooldown;

	public override void Attack(Vector3 entityVelocity) {

		if (Time.time > cooldown) {
			cooldown = Time.time + 1 / attackSpeed;
			Projectile newProjectile = Instantiate(projectile, muzzle.position, muzzle.rotation) as Projectile;
			newProjectile.speed = (missileSpeed);
			newProjectile.damage = attackDamage;
			newProjectile.initialVelocity = ((entityVelocity != null) ? entityVelocity : Vector3.zero);
		}
	}

}
