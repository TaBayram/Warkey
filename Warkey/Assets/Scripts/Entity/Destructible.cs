using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : IWidget
{
    public float Health { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    public void Death() {
        throw new System.NotImplementedException();
    }

    public void Destroy() {
        throw new System.NotImplementedException();
    }

    public void TakeDamage(float damage) {
        throw new System.NotImplementedException();
    }
}
