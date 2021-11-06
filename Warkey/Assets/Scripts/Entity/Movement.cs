using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public abstract class Movement : MonoBehaviour
{
    public CharacterController characterController;

    public float weight = 1;
    public Transform groundCheck;
    public float groundDistance = 0.8f;
    public LayerMask groundMask;

    public float speed = 8f;
    public float jumpHeight = 12;

    protected Vector3 velocity;
    protected bool isGrounded;

    public Vector3 Velocity { get => characterController.velocity; }

    protected Vector3 Move(Vector2 direction, bool sprint) {
        Vector3 move = transform.right * direction.x + transform.forward * direction.y;
        Vector3 movement = move * Time.deltaTime * ((sprint)?speed*2:speed);
        characterController.Move(movement);
        return movement;
    }


    protected void VerticalMovement(bool jump) {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask.value);

        if (isGrounded) {
            if (jump) {
                velocity.y = Mathf.Sqrt(jumpHeight * Gravity.GRAVITY * -2f);
            }
            else if(velocity.y < 0){
                velocity.y = -5f;
            }
        }
        else {
            velocity.y += Gravity.GRAVITYSCALED * weight * Time.deltaTime;
        }

        characterController.Move(velocity * Time.deltaTime);
    }

}
