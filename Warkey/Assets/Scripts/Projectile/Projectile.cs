using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    public float lifetime;
    public float gravity;
    public bool straighten;

    [HideInInspector] public Quaternion straightenedRotation;
    [HideInInspector] public Vector3 initialVelocity = Vector3.zero;
    [HideInInspector] public LayerMask layerMask;
    [HideInInspector] public float speed;
    [HideInInspector] public float damage;
    [HideInInspector] public float range;
    [HideInInspector] public float knockback;

    public abstract void CheckCollisions(float moveDistance);
    public abstract void OnHitObject(RaycastHit raycastHit);
    public abstract void Die();
    public abstract void Destroy();

}
