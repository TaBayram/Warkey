using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private CinemachineFreeLook cinemachine;
    [SerializeField] private new Camera camera;

    [SerializeField] private CinemachineOrbits[] orbits;
    private int currentIndex = 0;

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
        else {
            if (Input.GetKeyDown(KeyCode.Mouse2)) {
                currentIndex = (currentIndex + 1) % orbits.Length;
                SetLook();
            }

        }
    }

    private void SetLook() {
        var orbit = orbits[currentIndex];
        cinemachine.m_Orbits[0].m_Height = orbit.top.height;
        cinemachine.m_Orbits[0].m_Radius = orbit.top.radius;
        cinemachine.m_Orbits[1].m_Height = orbit.mid.height;
        cinemachine.m_Orbits[1].m_Radius = orbit.mid.radius;
        cinemachine.m_Orbits[2].m_Height = orbit.bot.height;
        cinemachine.m_Orbits[2].m_Radius = orbit.bot.radius;
    }
}
[System.Serializable]
public struct CinemachineOrbits
{
    public Orbit top;
    public Orbit mid;
    public Orbit bot;
}

[System.Serializable]
public struct Orbit
{
    public float height;
    public float radius;
}