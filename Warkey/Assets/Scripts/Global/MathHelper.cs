using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathHelper
{
    public static Vector2 Project(Vector2 vector, float distance, float angle) {
        Vector2 direction = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
        Vector2 candidate = vector + direction * distance;
        return candidate;
    }

}
