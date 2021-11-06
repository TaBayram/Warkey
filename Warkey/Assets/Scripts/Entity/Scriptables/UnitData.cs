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

    public bool canMove;
    public float weight;
    public float movementSpeed;
    public float sprintSpeed;
    public float jumpHeight;

    public bool canAttack;
}