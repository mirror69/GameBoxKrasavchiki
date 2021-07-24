using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Атака 
/// </summary>
public class AIDirectionAttackingStrategy : AIAttackingStrategy
{
    /// <summary>
    /// Периодичность проверки на возможность стрельбы
    /// </summary>
    const float CheckPeriod = 0.1f;

    protected override IEnumerator PerformAttacking()
    {
        weaponController.SetTarget(target);
        while (target.IsAlive())
        {
            if (weaponController.Status == WeaponStatus.Ready 
                && weaponController.IsTargetOnAttackLine(target) 
                && weaponController.CheckTargetAttackDistance(target) == AttackCheckResult.Ok)
            {
                weaponController.Attack();
            }

            yield return new WaitForSeconds(CheckPeriod);
        }
    }
}
