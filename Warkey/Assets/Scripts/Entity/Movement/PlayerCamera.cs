using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private CinemachineFreeLook cinemachine;
    [SerializeField] private new Camera camera;

    bool hasBinded;
    Transform targetTransform;

    public Transform CameraTransform {
        get => camera.transform;
    }
    public void BindPlayer(Transform transform) {
        cinemachine.LookAt = transform;
        cinemachine.Follow = transform;
        this.targetTransform = transform;
        hasBinded = true;
    }

    private void Update() {
        if (hasBinded && ( targetTransform == null || targetTransform.gameObject == null)) {
            Destroy(this.gameObject);
        }
    }
}
