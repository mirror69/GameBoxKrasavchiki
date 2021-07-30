using System.Collections;
using UnityEngine;

public abstract class WeaponController : MonoBehaviour
{
    [SerializeField]
    protected Transform attackPoint = null;
    [SerializeField]
    protected float attackDistance = 10;
    [SerializeField]
    protected float chargeDuration = 1;
    [SerializeField]
    protected float coolDownDuration = 1;

    protected bool isInterruptionRequested = false;
    protected bool isHitPerforming = false;
    protected Coroutine attackCoroutine = null;

    public Transform AttackPoint => attackPoint;

    public virtual void SetTarget(IDamageable target)
    {
    }

    public virtual IDamageable GetTarget()
    {
        return null;
    }

    public virtual void Attack()
    {
        if (attackCoroutine == null)
        {
            attackCoroutine = StartCoroutine(PerformAttack());
        }
    }

    public virtual void InterruptAttack()
    {
        isInterruptionRequested = true;
    }

    public virtual bool IsWeaponReady()
    {
        return attackCoroutine == null;
    }

    private IEnumerator PerformAttack()
    {
        isInterruptionRequested = false;

        float hitTime = Time.time + chargeDuration;
        while (Time.time < hitTime)
        {
            yield return null;

            if (isInterruptionRequested)
            {
                attackCoroutine = null;
                yield break;
            }
        }

        isHitPerforming = true;
        Hit();
        while (isHitPerforming)
        {
            yield return null;
        }

        if (coolDownDuration > 0)
        {
            yield return new WaitForSeconds(coolDownDuration);
        }

        attackCoroutine = null;
    }

    protected virtual void Hit()
    {
        isHitPerforming = false;
    }
}
