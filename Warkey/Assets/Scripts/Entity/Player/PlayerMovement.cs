using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PlayerMovement : Movement
{
    public GameObject prefab;
    public Transform spawner;

    private void Start() {
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.H)) {
            GameObject prefa = Instantiate(prefab, spawner.position, Quaternion.identity, this.transform.parent);
            prefa.GetComponent<ArtificialIntelligence>().player = this.transform;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Move(new Vector2(x, z), Input.GetKey(KeyCode.LeftShift));
        VerticalMovement(Input.GetButtonDown("Jump"));
    }
}
