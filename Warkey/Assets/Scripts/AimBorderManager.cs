using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimBorderManager : MonoBehaviour
{
    [SerializeField] ColliderInfo[] colliders;

    private void Awake() {
        /*WeaponController weaponController = GetComponentInParent<WeaponController>();
        if (weaponController) {
            weaponController.onWeaponChange += WeaponController_onWeaponChange;
        }
        */
    }

    private void WeaponController_onWeaponChange(Weapon weapon) {
        float range = weapon.AttackRange;
        foreach(ColliderInfo colliderInfo in colliders) {
            colliderInfo.boxCollider.transform.localPosition = new Vector3(colliderInfo.coordinate.x * range, 0f, colliderInfo.coordinate.y * range);
            colliderInfo.boxCollider.size = new Vector3(colliderInfo.coordinate.y * range, range * 2f, colliderInfo.coordinate.x * range);
        }
    }

    [System.Serializable]
    private struct ColliderInfo
    {
        public Vector2 coordinate;
        public BoxCollider boxCollider;
    }
}
