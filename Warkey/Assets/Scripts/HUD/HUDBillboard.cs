using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDBillboard : MonoBehaviour
{
    public const float viewThreshold = 20;
    public Transform targetCamera;

    public void BindCamera(Transform transform) {
        targetCamera = transform;
    }

    void LateUpdate()
    {
        if(targetCamera != null && Mathf.Abs(targetCamera.position.sqrMagnitude - transform.position.sqrMagnitude) < viewThreshold) {
            transform.LookAt(transform.position + targetCamera.forward);
        }   
    }
}
