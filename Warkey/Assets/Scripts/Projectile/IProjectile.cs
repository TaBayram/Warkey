using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface IProjectile
{
    internal void CheckCollisions(float moveDistance);
    internal void OnHitObject(RaycastHit raycastHit);
    internal void Die();
    internal void Destroy();
}

