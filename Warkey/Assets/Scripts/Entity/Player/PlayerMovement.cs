using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController characterController;
    public Transform groundCheck;
    public float groundDistance = 0.8f;
    public LayerMask groundMask;

    public float speed = 12f;
    public float gravity = -9.81f;
    public float jumpHeight = 8;
    private Vector3 velocity;
    private bool isGrounded;


    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        characterController.Move(move * speed * Time.deltaTime);

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask.value);

        if(Input.GetButtonDown("Jump") && isGrounded) {
            velocity.y += Mathf.Sqrt(jumpHeight * -2f * gravity);
        }


        if (isGrounded && velocity.y < 0) {
            velocity.y = -2f;
        }
        else {
            velocity.y += gravity * Time.deltaTime;
        }
        characterController.Move(velocity * Time.deltaTime);
    }
}
