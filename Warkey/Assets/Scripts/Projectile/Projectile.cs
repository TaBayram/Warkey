using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    public LayerMask layerMask;
    public float speed;
    public float damage;
    public Vector3 initialVelocity = Vector3.zero;


    internal abstract void CheckCollisions(float moveDistance);
    internal abstract void OnHitObject(RaycastHit raycastHit);
    internal abstract void Die();
    internal abstract void Destroy();


}
