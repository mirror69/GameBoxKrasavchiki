using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    [SerializeField] private float startHealth;
    private float health { get; set; }

    public void ReceiveDamage(float damageValue)
    {
        health -= damageValue;
        Debug.Log(health);
    }
    

    private void Start()
    {
        health = startHealth;
    }
}
