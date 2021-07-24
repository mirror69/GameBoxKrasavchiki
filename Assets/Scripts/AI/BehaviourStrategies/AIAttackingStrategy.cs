using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Абстрактный класс стратегии атаки AI
/// </summary>
public abstract class AIAttackingStrategy : MonoBehaviour
{
    protected WeaponController weaponController;
    protected IDamageable target;
    protected Coroutine attackingCoroutine = null;

    public void SetTarget(IDamageable target)
    {
        this.target = target;
    }

    public virtual void Initialize(WeaponController weaponController, IDamageable target)
    {
        this.weaponController = weaponController;
        SetTarget(target);
    }

    public virtual void StartAttacking()
    {
        if (attackingCoroutine == null)
        {
            attackingCoroutine = StartCoroutine(PerformAttacking());
        }
    }

    public virtual void StopAttacking()
    {
        if (attackingCoroutine != null)
        {
            StopAllCoroutines();
            attackingCoroutine = null;

            weaponController.InterruptAttack();
        }
    }

    protected abstract IEnumerator PerformAttacking();
}
