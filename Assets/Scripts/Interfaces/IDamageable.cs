using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable 
{
    Transform Transform { get; }
    void ReceiveDamage(float damageValue);
    bool IsAlive();
}
