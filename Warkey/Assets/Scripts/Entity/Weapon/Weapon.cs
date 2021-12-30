using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [HideInInspector] public Transform parent;
    public Sprite icon;

    public bool isMainHandRight;
    public event System.Action<State> onStateChange;
    public event System.Action<string, object> onAnimationChangeRequest;
    public event System.Action<float,Transform> onRotateRequest;
    [HideInInspector] public AnimationClip[] animations;
    [HideInInspector] public LayerMask enemyLayer;

    protected State state;
    [SerializeField] protected float attackRange = 1f;
    [SerializeField] protected float knockback = 1f;

    [SerializeField] protected LayerMask aimColliderLayerMask = new LayerMask();
    protected Transform aimTransform;
    protected Vector3 mouseWorldPosition;
    protected Vector3 centerPosition;

    protected float buffedAttackDamage;

    public abstract State CurrentState {
        get;
        set;
    }

    public enum State
    {
        idle = 0,
        attacking = 1,
        defending = 2,
    }
    public float AttackRange { get => attackRange; }

    protected virtual void Start() {
        aimTransform = new GameObject("Aim").transform;
        aimTransform.parent = this.parent;
    }


    protected virtual void Update() {
        if (CurrentState == State.defending) {
            Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
            Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
            if (Physics.Raycast(ray, out RaycastHit raycastHit1, 99f, enemyLayer)) {
                aimTransform.position = raycastHit1.point;
                mouseWorldPosition = raycastHit1.point;
                centerPosition = ray.origin;
            }
            else if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderLayerMask)) {
                aimTransform.position = raycastHit.point;
                mouseWorldPosition = raycastHit.point;
                centerPosition = ray.origin;
            }

        }
    }

    protected virtual void LateUpdate() {
        if (CurrentState == State.defending) {
            OnRotateRequest(0.015f * Time.deltaTime, aimTransform);
        }
    }


    public abstract void Attack(Vector3 initialVelocity);
    public abstract void Defend(bool pressed);
    public abstract void Stop();
    public abstract WeaponAnimations GetAnimations();

    public void OnStateChange() {
        onStateChange?.Invoke(state);
    }

    public void OnRotateRequest(float duration = 0,Transform transform = null) {
        onRotateRequest?.Invoke(duration, transform);
    }

    public void OnRequest(string name, object value) {
        onAnimationChangeRequest?.Invoke(name, value);
    }

    private void OnDestroy() {
        Destroy(aimTransform.gameObject);
    }
}


[System.Serializable]
public struct WeaponAnimations
{
    public bool isRanged;
    public AnimationClip defendAnimation;
    public AnimationClip idleAnimation;
    public AnimationClip[] attackAnimations;

    public WeaponAnimations(bool isRanged, AnimationClip defendAnimation, AnimationClip idleAnimation, AnimationClip[] attackAnimations) {
        this.isRanged = isRanged;
        this.defendAnimation = defendAnimation;
        this.idleAnimation = idleAnimation;
        this.attackAnimations = attackAnimations;
    }
}
