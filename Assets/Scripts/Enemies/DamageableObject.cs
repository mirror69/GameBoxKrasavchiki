using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageableObject : MonoBehaviour, IDamageable
{
    [SerializeField] 
    private float startHealth = 100;
    [SerializeField]
    private bool regenerationEnabled = false;
    [SerializeField] 
    private float oneHitRegenerationTime = 1;

    private Coroutine regenerationCoroutine = null;

    public float Health { get; private set; }
    public float MaxHealth => startHealth;
    public Transform Transform => transform;

    public void SetHealth(float healthValue)
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

    public void ReceiveDamage(float damageValue)
    {
        SetHealth(Health - damageValue);
        Debug.Log($"Hit on {name}. Current health: {Health}");
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
