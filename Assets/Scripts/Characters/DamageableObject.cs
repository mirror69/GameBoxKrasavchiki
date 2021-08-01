using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageableObject : MonoBehaviour, IDamageable
{
    [SerializeField] 
    private int startHealth = 100;
    [SerializeField]
    private bool regenerationEnabled = false;
    [SerializeField] 
    private float oneHitRegenerationTime = 1;

    private Coroutine regenerationCoroutine = null;

    public int Health { get; private set; }
    public int MaxHealth => startHealth;
    public Transform Transform => transform;

    private event Action Killed;

    public void SetHealth(int healthValue)
    {
        Health = healthValue;
        if (healthValue <= 0)
        {
            Health = 0;
            Killed?.Invoke();
        }
    }

    public bool IsAlive()
    {
        return Health > 0;
    }

    public void ReceiveDamage(int damageValue)
    {
        if (Health <= 0)
        {
            return;
        }

        SetHealth(Health - damageValue);
        
        if (Health <= 0)
        {
            Killed?.Invoke();
        }
    }

    public void SetEnabledRegeneration(bool enabled)
    {
        if (enabled && !IsAlive())
        {
            return;
        }
        regenerationEnabled = enabled;
        this.StopAndNullCoroutine(ref regenerationCoroutine);
        if (enabled)
        {
            regenerationCoroutine = StartCoroutine(PerformRegeneration());
        }
    }

    public void RegisterKilledListener(Action listener)
    {
        Killed += listener;
    }
    public void UnregisterKilledListener(Action listener)
    {
        Killed -= listener;
    }

    private void Start()
    {
        Health = startHealth;
        SetEnabledRegeneration(regenerationEnabled);
    }

    private IEnumerator PerformRegeneration()
    {
        float timeToNextRegeneration = Time.time + oneHitRegenerationTime;
        while (IsAlive())
        {
            if (Time.time > timeToNextRegeneration)
            {
                if (Health < MaxHealth)
                {
                    SetHealth(Health + 1);
                }
                timeToNextRegeneration = Time.time + oneHitRegenerationTime;
            }

            yield return null;
        }
    }
}
