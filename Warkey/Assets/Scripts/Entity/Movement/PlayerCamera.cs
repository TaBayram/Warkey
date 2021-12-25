using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private CinemachineFreeLook cinemachine;
    [SerializeField] private new Camera camera;

    public Transform CameraTransform {
        get => camera.transform;
    }
    public void BindPlayer(Transform transform) {
        cinemachine.LookAt = transform;
        cinemachine.Follow = transform;
    }
}
