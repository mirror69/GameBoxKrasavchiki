using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Атакующий луч
/// </summary>
public abstract class AttackBeam : MonoBehaviour
{
    [SerializeField]
    protected float damagePerSecond = 20;
    [SerializeField]
    protected float damageIncreaseMultiplierPerSecond = 1f;
    [SerializeField]
    protected float noTargetHittingTime = 0.4f;
    [SerializeField]
    protected AttackBeamRenderer beamRenderer = null;
    [SerializeField]
    protected Transform visualAttackPoint = null;

    protected IDamageable target = null;
    protected float maxDistance = 0;
    protected Transform attackPoint = null;
    protected RaycastHit currentRaycastHit;
    protected bool isAttacking = false;

    protected Action<int, float> DamageCaused;

    public virtual void RegisterOnDamageCausedListener(Action<int, float> listener)
    {
        DamageCaused += listener;
    }

    public virtual void UnregisterOnDamageCausedListener(Action<int, float> listener)
    {
        DamageCaused -= listener;
    }

    public virtual void Initialize(Transform attackPoint, float maxDistance)
    {
        this.attackPoint = attackPoint;
        this.maxDistance = maxDistance;
    }

    public virtual void SetTarget(IDamageable target)
    {
        this.target = target;
    }

    public virtual IDamageable GetTarget()
    {
        return target;
    }

    public virtual bool IsAttacking()
    {
        return isAttacking;
    }

    public virtual void StartAttack()
    {
        beamRenderer.SetActive(true);
        isAttacking = true;
        if (target != null)
        {
            StartCoroutine(Hitting());            
        }
        else
        {
            StartCoroutine(NoTargetHitting());
        }
    }

    public virtual void StopAttack()
    {
        StopAllCoroutines();
        isAttacking = false;
        beamRenderer.SetActive(false);
        target = null;
    }

    protected virtual void Awake()
    {
        beamRenderer.SetActive(false);
    }
    protected virtual bool AreHitConditionsSatisfied()
    {
        return target != null && target.IsAlive();
    }

    protected virtual IEnumerator Hitting()
    {
        float accumulatedDamageValue = 0;
        float hitDuration = 0;
        Coroutine raycastCoroutine = StartCoroutine(RaycastRefreshing());

        while (AreHitConditionsSatisfied())
        {
            beamRenderer.SetPosition(visualAttackPoint.position, GetBeamEndPoint());

            int accumulatedDamageValueInt = (int)accumulatedDamageValue;
            if (accumulatedDamageValueInt > 0)
            {
                float currentDamageMultiplier = (int)hitDuration * damageIncreaseMultiplierPerSecond;
                target.ReceiveDamage(accumulatedDamageValueInt + accumulatedDamageValueInt * currentDamageMultiplier);
                DamageCaused?.Invoke(accumulatedDamageValueInt, currentDamageMultiplier);
                accumulatedDamageValue = 0;
            }
            
            yield return new WaitForFixedUpdate();
            hitDuration += Time.fixedDeltaTime;
            accumulatedDamageValue += damagePerSecond * Time.fixedDeltaTime;
        }
        StopCoroutine(raycastCoroutine);

        StopAttack();
    }

    protected virtual IEnumerator NoTargetHitting()
    {
        Coroutine raycastCoroutine = StartCoroutine(RaycastRefreshing());
        float endTime = Time.time + noTargetHittingTime;
        while (Time.time < endTime)
        {
            if (target != null)
            {
                break;
            }
            beamRenderer.SetPosition(visualAttackPoint.position, GetBeamEndPoint());
            yield return new WaitForFixedUpdate();
        }
        StopCoroutine(raycastCoroutine);

        if (target != null)
        {
            StartCoroutine(Hitting());
        }
        else
        {
            StopAttack();
        }
    }

    protected virtual void RefreshTargetRaycast()
    {
        currentRaycastHit = GetAttackRaycast();
        target = null;
        if (currentRaycastHit.collider != null)
        {
            target = currentRaycastHit.collider.GetComponent<IDamageable>();
        }
    }

    protected virtual IEnumerator RaycastRefreshing()
    {       
        while (true)
        {
            RefreshTargetRaycast();
            yield return new WaitForFixedUpdate();
        }
    }

    protected virtual Vector3 GetBeamEndPoint()
    {
        if (currentRaycastHit.collider == null)
        {
            return visualAttackPoint.position + attackPoint.forward * maxDistance;
        }
        else
        {
            return currentRaycastHit.point;
        }
    }

    protected virtual RaycastHit GetAttackRaycast()
    {
        Vector3 direction = attackPoint.forward;
        float distance = maxDistance;
        if (target != null)
        {
            direction = target.Transform.position - attackPoint.position;
            direction.y = 0;
            distance = direction.magnitude;
        }

        if (Physics.Raycast(attackPoint.position, direction, out RaycastHit hitInfo, distance,
            ~0, QueryTriggerInteraction.Ignore))
        {
            return hitInfo;
        }
        else
        {
            return new RaycastHit();
        }
    }
}
