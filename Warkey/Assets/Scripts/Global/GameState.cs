using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameState
{
    private static State currentState = State.ingame;

    public enum State{
        ingame,
        settings,
    }

    public static System.Action<State> onStateChange;

    public static State CurrentState { get => currentState; set { currentState = value; onStateChange?.Invoke(value); } }
}
