using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDBillboard : MonoBehaviour
{
    public const float viewThreshold = 15;
    public Transform targetCamera;
    public GameObject healthBar;
    public void BindCamera(Transform transform) {
        targetCamera = transform;
    }

    private void Awake() {
        targetCamera = Camera.main.transform;
    }

    void LateUpdate()
    {
        if (targetCamera == null || ((targetCamera.position - transform.position).sqrMagnitude) > viewThreshold*viewThreshold) {
            healthBar.SetActive(false);   
        }
        else{
            if(!healthBar.activeSelf)
                healthBar.SetActive(true);
            transform.LookAt(transform.position + targetCamera.forward);
        }   
    }
}
