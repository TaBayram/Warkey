using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Gravity 
{
    public static readonly float GRAVITY = -9.81f;

    public static float GRAVITYSCALED {
        get => GRAVITY * 4;
    }


}
