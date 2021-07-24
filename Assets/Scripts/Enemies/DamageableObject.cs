using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageableObject : MonoBehaviour, IDamageable
{
    [SerializeField] private float startHealth;
    private float health { get; set; }

    public Transform Transform => transform;

    public bool IsAlive()
    {
        return health > 0;
    }

    public void ReceiveDamage(float damageValue)
    {
        health -= damageValue;
        Debug.Log($"Hit on {name}. Current health: {health}");
    }
    

    private void Start()
    {
        health = startHealth;
    }
}
