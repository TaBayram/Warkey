using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : Projectile
{
    [SerializeField] private AudioSource audioSource;
    private void Start() {
    }

    public override void CheckCollisions(float moveDistance) {
        Ray ray = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, moveDistance, layerMask, QueryTriggerInteraction.Collide)) {
            audioSource?.Play();
            OnHitObject(hit);
        }
    }

    public override void Die() {
        Destroy();
    }

    public override void Destroy() {
        Destroy(this.gameObject);
    }

    public override void OnHitObject(RaycastHit raycastHit) {
        raycastHit.collider.gameObject.GetComponent<IWidget>().TakeDamage(damage);
        var ent = raycastHit.collider.gameObject.GetComponent<AIEntity>();
        if (ent) {
            ent.GetKnockedBack((raycastHit.collider.transform.position - transform.position).normalized * 10 * knockback);
        }

        Die();
    }

    private float travelled = 0;

    private void Update() {
        float moveDistance = speed * Time.deltaTime;
        travelled += moveDistance;

        CheckCollisions(moveDistance);
        transform.Translate(Vector3.forward * moveDistance);
        transform.Translate(Vector3.down * gravity/100);

        if(straighten && travelled > range) {
            straighten = false;
            transform.rotation = straightenedRotation;
        }

        lifetime -= Time.deltaTime;
        if(lifetime <= 0) {
            Die();
        }
    }


}
