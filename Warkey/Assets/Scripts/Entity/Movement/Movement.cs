using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Entity))]
public abstract class Movement : MonoBehaviour
{
    protected Entity entity;

    public CharacterController characterController;
    public MovementData movementData;

    public float groundDistance = 0.8f;
    public Transform groundCheck;
    public LayerMask groundMask;

    protected Vector3 velocity;
    protected bool isGrounded;

    public Movement.State state = State.idle;
    public bool isJumping = false;

    public event System.Action<State> onStateChange;
    public event System.Action<Vector3> onVelocityChange;
    
    public Vector3 Velocity { get => characterController.velocity; }

    protected virtual void Start() {
        entity = GetComponent<Entity>();
    }

    protected virtual void Update() {
        if(onVelocityChange != null)
            onVelocityChange(velocity);
    }

    public enum State
    {
        idle = 0,
        walking = 1,
        sprinting = 2,
    }

    protected Vector3 Move(Vector3 direction, bool sprint) {
        State oldState = state;
        if(direction != Vector3.zero) {
            if (sprint)
                state = State.sprinting;
            else
                state = State.walking;
        }
        else {
            state = State.idle;
        }

        if (oldState != state && onStateChange != null) {
            onStateChange(state);
        }
        Vector3 movement = Vector3.zero;
        if (!isJumping) {
            movement = direction * Time.deltaTime * ((sprint) ? movementData.sprintSpeed : movementData.walkSpeed);
            MoveCharacter(movement);
        }
        return movement;
    }


    protected void VerticalMovement(bool jump) {
        bool wasGrounded = isGrounded;
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask.value);

        if (isGrounded) {
            if(wasGrounded != isGrounded) {
                isJumping = false;
                velocity.x = 0;
                velocity.z = 0;
                entity?.animationController?.isJumping(isJumping);
            }
            if (jump) {
                velocity.y = Mathf.Sqrt(movementData.jumpHeight * Gravity.GRAVITY * -2f);
                velocity.x = characterController.velocity.x*movementData.jumpForwardScale;
                velocity.z = characterController.velocity.z*movementData.jumpForwardScale;
                isJumping = true;

               /* float time = -velocity.y / Gravity.GRAVITYSCALED * movementData.weight;
                Debug.Log(time);*/
                entity?.animationController?.isJumping(isJumping);
            }
            else if(velocity.y < 0){
                velocity.y = -5f;
            }
        }
        else {
            velocity.y += Gravity.GRAVITYSCALED * movementData.weight * Time.deltaTime;
        }
        MoveCharacter(velocity * Time.deltaTime);
    }

    private void MoveCharacter(Vector3 velocity) {
        characterController.Move(velocity);
    }
}
