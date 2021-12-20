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

	[SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();
	[SerializeField] private Transform aimTransform;

	private float cooldown;
	private int currentSubPool;
	private int currentMainPool;
	private Vector3 mouseWorldPosition;


	public override State CurrentState {
        get => state;
        set {
            state = value;
            OnStateChange();
        }
    }
	private void Awake() {

	}

	private void Start() {
		aimTransform.parent = transform.root;
	}
	private void Update() {
		if (CurrentState == State.defending) {

			Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
			Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
			if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderLayerMask)) {
				aimTransform.position = raycastHit.point;
				mouseWorldPosition = raycastHit.point;
			}

		}
	}

	public override void Attack(Vector3 entityVelocity) {
		StartAttacking(entityVelocity);
	}

	private void StartAttacking(Vector3 entityVelocity) {
		if (Time.time > cooldown) {
			cooldown = Time.time + attackSpeed;
			StartCoroutine(Shoot(entityVelocity));
			CurrentState = State.attacking;
		}
	}

	private IEnumerator Shoot(Vector3 entityVelocity) {
		yield return new WaitForSeconds(launchDelay);
		Vector3 aimDir = (mouseWorldPosition - muzzle.position).normalized;
		Projectile newProjectile = Instantiate(projectile, muzzle.position, Quaternion.LookRotation(aimDir, Vector3.up)) as Projectile;
		newProjectile.speed = (launchSpeed);
		newProjectile.damage = attackDamage;

		CurrentState = State.defending;
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

    public override void Stop() {
        throw new System.NotImplementedException();
    }
}
