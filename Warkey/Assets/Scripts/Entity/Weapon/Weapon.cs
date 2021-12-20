using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Weapon : MonoBehaviour
{
    public event System.Action<State> onStateChange;
    public event System.Action<string, object> onAnimationChangeRequest;
    public AnimationClip[] animations;
    public Sprite weaponIcon;

    protected State state;

    public abstract State CurrentState {
        get;
        set;
    }

    public enum State
    {
        idle = 0,
        attacking = 1,
    }

    public abstract void Attack(Vector3 initialVelocity);

    public void OnStateChange() {
        onStateChange?.Invoke(state);
    }

    public void OnRequest(string name, object value) {
        onAnimationChangeRequest?.Invoke(name, value);
    }
}


[System.Serializable]
public struct WeaponAnimations
{

}
