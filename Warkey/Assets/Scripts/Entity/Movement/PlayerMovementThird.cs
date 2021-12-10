using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementThird : Movement
{
    public PlayerCamera prefabCamera;

    public float turnSmoothTime = 0.1f;
    public PlayerCamera playerCamera;
    private float turnSmoothVelocity;

    protected override void Start() {
        base.Start();
         
        characterController = GetComponent<CharacterController>();
        playerCamera = Instantiate<PlayerCamera>(prefabCamera, transform.parent.transform);
        playerCamera.BindPlayer(transform);


    }
    void Update(){
        base.Update();

        //float x = Input.GetAxis("Horizontal");
        //float z = Input.GetAxis("Vertical");
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(x, 0f, z).normalized;

        Vector3 moveDirection = Vector3.zero;
        if (direction.magnitude >= 0.1f) {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + ((playerCamera != null)? playerCamera.CameraTransform.eulerAngles.y : 0);
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        }
        Move(moveDirection.normalized, Input.GetKey(KeyCode.LeftShift));
        VerticalMovement(Input.GetButtonDown("Jump"));
    }

    public void RotateToTarget() {
        float targetAngle = playerCamera.CameraTransform.eulerAngles.y;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
        transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);
    }
}
