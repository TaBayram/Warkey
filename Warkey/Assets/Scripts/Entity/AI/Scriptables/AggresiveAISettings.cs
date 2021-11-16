using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class AggresiveAISettings:ScriptableObject
{
    public float attackDamage;
    public float attackRange;
    public float attackSpeed;

    public bool chaseWhenDamaged;
    public float chaseWhenDamagedTime;
    public float chaseWhenDamagedRange;
}
