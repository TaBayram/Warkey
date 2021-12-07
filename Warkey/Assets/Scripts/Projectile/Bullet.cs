using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : Projectile
{
    internal override void CheckCollisions(float moveDistance) {
        Ray ray = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, moveDistance, layerMask, QueryTriggerInteraction.Collide)) {
            OnHitObject(hit);
        }
    }

    internal override void Die() {
        throw new System.NotImplementedException();
    }

    internal override void Destroy() {
        throw new System.NotImplementedException();
    }

    internal override void OnHitObject(RaycastHit raycastHit) {
        raycastHit.collider.gameObject.GetComponent<IWidget>().TakeDamage(damage);
        GameObject.Destroy(gameObject);
    }

    void Update() {
        float moveDistance = speed * Time.deltaTime;
        CheckCollisions(moveDistance);
        transform.Translate(Vector3.forward * moveDistance + initialVelocity);
    }


}
