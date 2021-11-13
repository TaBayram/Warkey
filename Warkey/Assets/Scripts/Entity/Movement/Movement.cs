using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Entity))]
public abstract class Movement : MonoBehaviour
{
    public CharacterController characterController;
    public MovementData movementData;

    public float groundDistance = 0.8f;
    public Transform groundCheck;
    public LayerMask groundMask;

    protected Vector3 velocity;
    protected bool isGrounded;

    public Movement.State state = State.standing;
    
    public Vector3 Velocity { get => characterController.velocity; }

    public enum State
    {
        standing = 0,
        walking = 1,
        sprinting = 2,
    }

    protected Vector3 Move(Vector2 direction, bool sprint) {
        Vector3 move = transform.right * direction.x + transform.forward * direction.y;
        Vector3 movement = move * Time.deltaTime * ((sprint)?movementData.sprintSpeed: movementData.walkSpeed);
        characterController.Move(movement);
        return movement;
    }


    protected void VerticalMovement(bool jump) {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask.value);

        if (isGrounded) {
            if (jump) {
                velocity.y = Mathf.Sqrt(movementData.jumpHeight * Gravity.GRAVITY * -2f);
            }
            else if(velocity.y < 0){
                velocity.y = -5f;
            }
        }
        else {
            velocity.y += Gravity.GRAVITYSCALED * movementData.weight * Time.deltaTime;
        }

        characterController.Move(velocity * Time.deltaTime);
    }

}
