using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseCenterLock : MonoBehaviour
{
    void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        GameState.onStateChange += MouseLook_onStateChange;
    }
    private void MouseLook_onStateChange(GameState.State state) {
        if (state == GameState.State.ingame) {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else {
            Cursor.lockState = CursorLockMode.Confined;
        }
    }
}
