using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable 
{
    Transform Transform { get; }
    int Health { get; }
    int MaxHealth { get; }
    void SetHealth(int healthValue);
    void ReceiveDamage(int damageValue);
    bool IsAlive();
    void SetEnabledRegeneration(bool enabled);
}
