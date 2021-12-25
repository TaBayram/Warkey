using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class UnitData : ScriptableObject
{
    public string unitName;
    public float health;
    public float healthRegen;
    public float stamina;
    public float staminaRegen;
}