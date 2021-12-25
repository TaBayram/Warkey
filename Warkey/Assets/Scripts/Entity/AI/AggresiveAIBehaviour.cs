using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;
public class AggresiveAIBehaviour : AIBehaviour
{
    AggresiveAISettings aggresiveAISettings;
    WeaponController weaponController;

    private bool lookAtTarget;

    public AggresiveAIBehaviour(AIEntity aIEntity, AggresiveAISettings aggresiveAISettings) : base(aIEntity) {
        this.aggresiveAISettings = aggresiveAISettings;
        this.weaponController = aiEntity.weaponController;

        if(this.aiEntity.unit != null)
            this.aiEntity.unit.onDamageTaken += Unit_onDamageTaken;
        this.weaponController.onStateChange += WeaponController_onStateChange;
    }

    private void WeaponController_onStateChange(Weapon.State obj) {
        if(obj == Weapon.State.attacking) {
            lookAtTarget = true;
        }
    }

    private void Unit_onDamageTaken(float damage) {
        if (aggresiveAISettings.chaseWhenDamaged) {
            //aiEntity.FillTargetPlayer()
        }
    }

    public void Attack(Transform target) {
        if (lookAtTarget) {
            navMeshAgent.ResetPath();
            lookAtTarget = false;
            transform.LookAt(target);
        }
            
        weaponController.Attack();
    }

    public void Chase(Transform chased) {
        navMeshAgent.SetDestination(chased.position);
    }

}
