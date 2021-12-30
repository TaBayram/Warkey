using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDBillboard : MonoBehaviour
{
    [SerializeField] private float viewThreshold = 15;
    [SerializeField] private bool handleVisibility = true;
    public Transform targetCamera;
    public GameObject targetObject;
    public void BindCamera(Transform transform) {
        targetCamera = transform;
    }

    private void Awake() {
        if(Camera.main)
            targetCamera = Camera.main.transform;
    }

    void LateUpdate()
    {
        if(targetCamera == null && Camera.main) {
            targetCamera = Camera.main.transform;
        }
        if (targetCamera == null) return;
        else targetObject.transform.LookAt(targetObject.transform.position + targetCamera.forward);

        if (handleVisibility) {
            if ((targetCamera.position - targetObject.transform.position).sqrMagnitude > viewThreshold * viewThreshold) {
                targetObject.SetActive(false);
            }
            else {
                targetObject.SetActive(true);
            }
        }
          
    }
}
