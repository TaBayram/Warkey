using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : Movement
{
    private void Start() {
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Move(new Vector2(x, z), Input.GetKey(KeyCode.LeftShift));
        VerticalMovement(Input.GetButtonDown("Jump"));
    }
}
