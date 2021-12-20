using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disabler : MonoBehaviour
{
    [SerializeField] private Behaviours[] disables;
    [SerializeField] private Behaviours[] removes;


    public void DisableComponents(int index) {
        foreach(Behaviours component in disables) {
            if(component.index == index) {
                for (int i = 0; i < component.behaviours.Length; i++) {
                    if(component.behaviours[i] != null)
                        component.behaviours[i].enabled = false;
                }
            }
        }
    }

    public void RemoveComponents(int index) {
        foreach (Behaviours component in removes) {
            if (component.index == index) {
                for (int i = 0; i < component.behaviours.Length; i++) {
                    if (component.behaviours[i] != null)
                        Destroy(component.behaviours[i]);
                }
                for (int i = 0; i < component.components.Length; i++) {
                    if (component.components[i] != null)
                        Destroy(component.components[i]);
                }
            }
        }
    }


    [System.Serializable]
    private struct Behaviours{
        public int index;
        public Behaviour[] behaviours;
        public Component[] components;
    }
}


