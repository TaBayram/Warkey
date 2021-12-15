using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactor : MonoBehaviour
{
    public Interactable focus;
    Camera cam;
    void setFocus(Interactable newFocus)
    {
        if (newFocus != focus)
        {
            // Defocus the old one
            if (focus != null)
                focus.deFocused();

            focus = newFocus;   // Set our new focus
        }

        newFocus.onFocused(transform);
    }
}
