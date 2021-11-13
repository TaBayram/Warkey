using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class EntityData : ScriptableObject
{
    [Header("Unit")]
    public string unitName;
    public float health;
    public float healthRegen;
    public float stamina;
    public float staminaRegen;

    [Header("Movement")]
    public bool canMove;
    public float weight;
    public float movementSpeed;
    public float sprintSpeed;
    public float jumpHeight;


    [Header("Combat")]
    public bool canAttack;
}
