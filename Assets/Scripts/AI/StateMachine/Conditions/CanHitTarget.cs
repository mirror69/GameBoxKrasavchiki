using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Условие возможности атаки врагом игрока (можем ли из текущей точки попасть по цели)
/// </summary>
public class CanHitTarget : FsmCondPolled
{
    private IDamageable target;
    private ShootingWeaponController weaponController;

    protected override bool EvaluateCondition()
    {
        if (target == null || weaponController == null)
        {
            return false;
        }

        // Проверим, можем ли из текущей точки попасть по цели
        return weaponController.CheckTargetAttackDistance(target) == AttackCheckResult.Ok
            && !weaponController.ThereAreObstaclesOnAttackWay(target);
    }

    private void Awake()
    {
        weaponController = GetComponentInParent<ShootingWeaponController>();
    }

    private void Start()
    {
        target = GameManager.Instance.Player;
        StartEvals();
    }
}
