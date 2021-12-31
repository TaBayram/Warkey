using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedWeapon : Weapon
{
	[SerializeField] private Transform muzzle;
	[SerializeField] private GameObject projectile;
	[SerializeField] private AnimationClip attackAnimation;
	[SerializeField] private AnimationClip aimAnimation;
	[SerializeField] private float attackSpeed = 1;
	[SerializeField] private float attackDamage = 20;
	[SerializeField] private float launchSpeed = 35;
	[SerializeField] private float launchDelay = 0.2f;
	[SerializeField] private float attackCooldown = 0.5f;

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

	protected override void Start() {
		base.Start();
		aimTransform.parent = transform.root;
	}

	public override void Attack(Vector3 entityVelocity) {
		if (CurrentState == State.defending)
			StartAttacking(entityVelocity);			
	}

	private void StartAttacking(Vector3 entityVelocity) {
		if (Time.time > cooldown) {
			cooldown = Time.time + attackCooldown*attackSpeed;
			StartCoroutine(Shoot(entityVelocity));
			CurrentState = State.attacking;
			OnRequest("attackSpeed", 1 / attackSpeed);
		}
	}

	private IEnumerator Shoot(Vector3 entityVelocity) {
		yield return new WaitForSeconds(launchDelay);
		Vector3 aimDir = (mouseWorldPosition - muzzle.position).normalized;
		Projectile newProjectile = Instantiate(projectile, muzzle.position, Quaternion.LookRotation(aimDir, Vector3.up)).GetComponent<Projectile>();
		newProjectile.speed = (launchSpeed);
		newProjectile.damage = attackDamage;
		newProjectile.straighten = true;
		newProjectile.range = attackRange;
		newProjectile.layerMask = enemyLayer;
		newProjectile.knockback = knockback;
		newProjectile.straightenedRotation = Quaternion.LookRotation((mouseWorldPosition - centerPosition).normalized, Vector3.up);


		Defend(pressed);
	}

	bool pressed;

    public override void Defend(bool pressed) {
		this.pressed = pressed;
		if (pressed)
			CurrentState = State.defending;
		else
			CurrentState = State.idle;
	}

	public override WeaponAnimations GetAnimations() {
		WeaponAnimations weaponAnimations = new WeaponAnimations(true, aimAnimation, null, new AnimationClip[]{ attackAnimation });
		return weaponAnimations;
	}

    public override void Stop() {
		CurrentState = State.idle;
    }

}
