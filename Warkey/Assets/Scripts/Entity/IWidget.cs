using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWidget
{
    public void TakeDamage(float damage);
    public void Die();
    public void Destroy();
}
