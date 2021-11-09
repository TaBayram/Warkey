using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameState
{
    public static State state;

    public enum State{
        ingame,
        settings,
    }
}
