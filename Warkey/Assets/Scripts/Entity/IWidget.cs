using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWidget
{
    float Health { get; set; }

    public void TakeDamage(float damage);
    public void Death();
    public void Destroy();
}
