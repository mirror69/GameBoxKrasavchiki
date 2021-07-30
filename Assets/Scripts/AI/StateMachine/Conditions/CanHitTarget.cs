using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������� ����������� ����� ������ ������ (����� �� �� ������� ����� ������� �� ����)
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

        // ��������, ����� �� �� ������� ����� ������� �� ����
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
