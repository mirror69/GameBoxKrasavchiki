using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable 
{
    Transform Transform { get; }
    float Health { get; }
    float MaxHealth { get; }
    void SetHealth(float healthValue);
    void ReceiveDamage(float damageValue);
    bool IsAlive();
}
