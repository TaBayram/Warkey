using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public Animator animator;
    public event System.Action<State> onStateChange;
    public State state;

    public enum State
    {
        idle = 0,
        attacking = 1,
    }

    public abstract void Attack(Vector3 initialVelocity);

    public void OnStateChange() {
        if(onStateChange != null) {
            onStateChange(state);
        }
    }
}
