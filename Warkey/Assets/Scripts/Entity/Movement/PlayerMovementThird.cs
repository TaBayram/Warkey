using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementThird : Movement
{
    public float turnSmoothTime = 0.1f;
    public Transform camera;
    private float turnSmoothVelocity;

    private void Start() {
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {

        bool moveAnim = Mathf.Abs(Input.GetAxisRaw("Horizontal")) == 1|| Mathf.Abs(Input.GetAxisRaw("Vertical")) == 1;
        //float x = Input.GetAxis("Horizontal");
        //float z = Input.GetAxis("Vertical");
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(x, 0f, z).normalized;

        Vector3 moveDirection = Vector3.zero;
        if (direction.magnitude >= 0.1f) {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + camera.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        }
        Move(moveDirection.normalized, Input.GetKey(KeyCode.LeftShift), moveAnim);
        VerticalMovement(Input.GetButtonDown("Jump"));
    }
}
