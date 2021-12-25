using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class AggresiveAISettings:ScriptableObject
{
    public float attackRange;
    public bool chaseWhenDamaged;
    public float chaseWhenDamagedTime;
    public float chaseWhenDamagedRange;
}
