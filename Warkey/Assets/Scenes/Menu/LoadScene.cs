using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LoadScene
{

    public static Scenes SceneIndex;

    public enum Scenes
    {
        Lobby = 0,
        Camp = 1,
        MainMenu = 2,
        World = 3,
        Winter = 4,
        Persistent = 5,
    }
}
