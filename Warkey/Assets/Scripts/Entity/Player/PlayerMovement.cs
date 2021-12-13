using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

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

    public GameObject prefab;
    public Transform spawner;

    PhotonView view;

    // Update is called once per frame
    private void Start()
    {
        view = GetComponent<PhotonView>();
    }
    void Update()
    {

        if (!view.IsMine){ return; }
   
            if (Input.GetKeyDown(KeyCode.H))
            {
                GameObject prefa = Instantiate(prefab, spawner.position, Quaternion.identity, this.transform.parent);
                prefa.GetComponent<ArtificialIntelligence>().player = this.transform;
            }
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");


            Vector3 move = transform.right * x + transform.forward * z;
            Vector3 movement;
            if (Input.GetKey(KeyCode.LeftShift))
            {
                movement = move * speed * 2 * Time.deltaTime;
            }
            else
            {
                movement = move * speed * Time.deltaTime;

            }
            characterController.Move(movement);

            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask.value);

            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                velocity.y += Mathf.Sqrt(jumpHeight * -2f * gravity);
            }


            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
            }
            else
            {
                velocity.y += gravity * 2 * Time.deltaTime;
            }
            characterController.Move(velocity * Time.deltaTime);
        

    }
}
