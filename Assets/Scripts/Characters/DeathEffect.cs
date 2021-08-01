using System.Collections;
using UnityEngine;

public abstract class DeathEffect : MonoBehaviour
{
    [SerializeField]
    protected DamageableObject damageableObject;
    [SerializeField]
    protected float deathDuration;
    [SerializeField]
    protected float smoothYShift;

    protected virtual void ProcessDeath()
    {
        if (smoothYShift != 0)
        {
            StartCoroutine(PerformSmoothShift());
        }
        
        Destroy(damageableObject.Transform.gameObject, deathDuration);
    }

    protected virtual void Awake()
    {
        damageableObject.RegisterKilledListener(ProcessDeath);
    }
    protected virtual void OnDestroy()
    {
        damageableObject.UnregisterKilledListener(ProcessDeath);
    }

    private IEnumerator PerformSmoothShift()
    {
        float coordChangeSpeed = smoothYShift / deathDuration;
        float endTime = Time.time + deathDuration;
        while (Time.time < endTime)
        {
            damageableObject.Transform.Translate(new Vector3(0, coordChangeSpeed * Time.deltaTime, 0));
            yield return null;
        }
    }
}
