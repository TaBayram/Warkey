using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IProjectile
{
    public LayerMask LayerMask { get; set; }
    public float Speed { get; set; }

    internal void CheckCollisions(float moveDistance);
    internal void OnHitObject(RaycastHit raycastHit);
    internal void Death();
    internal void Destroy();


}
