using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerMovementThird : Movement
{
    public const float turnSmoothTime = 0.1f;
    [SerializeField] private PlayerCamera prefabCamera;
    
    private PlayerCamera playerCamera;
    private float turnSmoothVelocity;

    public PlayerCamera Camera {
        get => playerCamera;
    }

    private void Awake() {
        characterController = GetComponent<CharacterController>();

        if (GetComponent<PhotonView>().IsMine || !PhotonNetwork.IsConnected) {
            playerCamera = Instantiate<PlayerCamera>(prefabCamera);
            if (this.transform.parent != null) playerCamera.transform.parent = transform.parent.transform;
            playerCamera.BindPlayer(transform);
        }
    }

    protected override void Start() {
        base.Start();
        
    }
    protected override void Update(){
        base.Update();
        if (!GetComponent<PhotonView>().IsMine  && PhotonNetwork.IsConnected) return;
        movementMultiplier = entity.MovementScaler();
        if (movementMultiplier == 0) {
            if(state != State.idle) {
                Move(Vector3.zero, false);
            }
            return;
        }


        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        if( x != 0 || z != 0) {
            Vector3 direction = new Vector3(x, 0f, z).normalized;
            Vector3 moveDirection = Vector3.zero;
            if (direction.magnitude >= 0.1f) {
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + ((playerCamera != null) ? playerCamera.CameraTransform.eulerAngles.y : 0);
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                if (!isRotating) {
                    transform.rotation = Quaternion.Euler(0f, angle, 0f);
                }
                moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            }
            bool wantToSprint = Input.GetKey(KeyCode.LeftShift);
            bool sprint = wantToSprint && entity.UseStamina(movementData.sprintStaminaCost);

            Move(moveDirection.normalized, sprint);
        }
        else {
            Stop();
        }

        VerticalMovement(Input.GetButtonDown("Jump"));
    }

    public void RotateToTarget(float duration = 0) {
        if (playerCamera == null) return;
        float rotationalTarget = playerCamera.CameraTransform.eulerAngles.y;
        StartCoroutine(Rotate(rotationalTarget, duration == 0 ? false : true, duration));
    }


    private IEnumerator Rotate(float rotationalTarget, bool hasDuration, float duration) {
        isRotating = true;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, rotationalTarget, ref turnSmoothVelocity, turnSmoothTime);
        transform.rotation = Quaternion.Euler(0f, angle, 0f);
        yield return new WaitForSeconds(0.005f);
        duration -= 0.005f;
        if (Mathf.Abs(rotationalTarget - angle) > 0.1f && (!hasDuration || duration > 0) ) {
            StartCoroutine(Rotate(rotationalTarget, hasDuration, duration));
        }
        else {
            isRotating = false;
        }
    }

    private void OnDestroy() {
        Destroy(playerCamera);
    }
}
