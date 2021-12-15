using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedWeapon : Weapon
{
	[SerializeField] private Transform muzzle;
	[SerializeField] private Projectile projectile;
	[SerializeField] private AnimationClip attackAnimation;
	[SerializeField] private AnimationClip aimAnimation;
	[SerializeField] private float attackSpeed = 1;
	[SerializeField] private float attackDamage = 20;
	[SerializeField] private float launchSpeed = 35;
	[SerializeField] private float launchDelay = 0.2f;

	[SerializeField] private bool IsUnlimited;
	[SerializeField] private int mainPool = 20;
	[SerializeField] private int subPool = 20;
	[SerializeField] private float reloadTime;


	private float cooldown;
	private int currentSubPool;
	private int currentMainPool;


	public override State CurrentState {
        get => state;
        set {
            state = value;
            OnStateChange();
        }
    }
	private void Awake() {

	}

	public override void Attack(Vector3 entityVelocity) {
		StartAttacking(entityVelocity);
	}

	private void StartAttacking(Vector3 entityVelocity) {
		if (Time.time > cooldown) {
			cooldown = Time.time + attackSpeed;
			StartCoroutine(Shoot(entityVelocity));
		}
	}

	private IEnumerator Shoot(Vector3 entityVelocity) {
		yield return new WaitForSeconds(launchDelay);
		Projectile newProjectile = Instantiate(projectile, muzzle.position, muzzle.rotation) as Projectile;
		newProjectile.speed = (launchSpeed);
		newProjectile.damage = attackDamage;
		newProjectile.initialVelocity = ((entityVelocity != null) ? entityVelocity : Vector3.zero);
	}

    public override void Defend(bool pressed) {
		if (pressed)
			CurrentState = State.defending;
		else
			CurrentState = State.idle;
	}

	public override WeaponAnimations GetAnimations() {
		WeaponAnimations weaponAnimations = new WeaponAnimations(true, aimAnimation, null, new AnimationClip[]{ attackAnimation });
		return weaponAnimations;
	}
}
