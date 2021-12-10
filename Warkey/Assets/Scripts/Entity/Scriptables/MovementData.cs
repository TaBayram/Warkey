using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class MovementData : ScriptableObject
{
    public bool canMove;
    public float weight;
    public float walkSpeed;
    public float sprintSpeed;
    public float jumpHeight;
    public float jumpForwardScale = 1f;
}
