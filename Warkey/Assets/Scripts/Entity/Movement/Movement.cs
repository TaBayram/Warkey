using System;
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
    public bool isRotating = false;

    public event System.Action<State> onStateChange;
    public event System.Action<Vector3> onVelocityChange;

    protected float movementMultiplier = 1f;
    protected float movementSpeedBonus = 0f;
    
    public Vector3 Velocity { get => characterController.velocity; }

    protected virtual void Start() {
        entity = GetComponent<Entity>();
    }
    public void IncreaseSpeed(float amount, float v) {
        StartCoroutine(IncreaseSpeedBuff(amount, v));
    }

    public IEnumerator IncreaseSpeedBuff(float amount, float v) {
        movementSpeedBonus += amount;
        yield return new WaitForSeconds(v);
        movementSpeedBonus -= amount;
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

    public void RotateToTarget(Transform target) {
        //transform.LookAt(target);
        //Quaternion.Look(transform.rotation, )
        float rotationalTarget = Quaternion.LookRotation(target.position - transform.position).eulerAngles.y;
        transform.rotation = Quaternion.Euler(0f, rotationalTarget, 0f);
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
        if (state == State.idle)
            return direction;

        Vector3 movement = Vector3.zero;
        if (!isJumping) {
            movement = direction * Time.deltaTime * movementMultiplier * (((sprint) ? movementData.sprintSpeed : movementData.walkSpeed) + movementSpeedBonus );
            MoveCharacter(movement);
        }
        return movement;
    }

    protected void Stop() {
        State oldState = state;
        state = State.idle;
        if (oldState != state && onStateChange != null) {
            onStateChange(state);
        }
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
        if(transform.position.y < -1000) {
            if (Physics.Raycast(transform.position, Vector3.up, out RaycastHit raycastHit, 5000f, 1 << LayerMask.NameToLayer("Ground"))) {
                characterController.enabled = false;
                transform.position = raycastHit.point + Vector3.up*10f;
                characterController.enabled = true;
            }
        }

        MoveCharacter(velocity * Time.deltaTime);
    }

    private void MoveCharacter(Vector3 velocity) {
        characterController.Move(velocity);
    }
}
