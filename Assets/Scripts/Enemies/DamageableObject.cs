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

    public void SetHealth(int healthValue)
    {
        Health = healthValue;
        if (healthValue < 0)
        {
            Health = 0;
        }
    }

    public bool IsAlive()
    {
        return Health > 0;
    }

    public void ReceiveDamage(int damageValue)
    {
        SetHealth(Health - damageValue);
    }

    public void SetEnabledRegeneration(bool enabled)
    {
        regenerationEnabled = enabled;
        this.StopAndNullCoroutine(ref regenerationCoroutine);
        if (enabled)
        {
            regenerationCoroutine = StartCoroutine(PerformRegeneration());
        }
    }

    private void Start()
    {
        Health = startHealth;
        SetEnabledRegeneration(regenerationEnabled);
    }

    private IEnumerator PerformRegeneration()
    {
        float timeToNextRegeneration = Time.time + oneHitRegenerationTime;
        while (true)
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
