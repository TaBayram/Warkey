using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class PassiveAISettings:ScriptableObject
{
    public float wanderRange;
    [Tooltip("In Seconds")]
    public float wanderStopTime;
    public float patrolRange;
    [Tooltip("In Seconds")]
    public float patrolStopTime;
    public float sightRange;
    public float loseSightRange;
}
