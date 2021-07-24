using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Состояние атаки
/// </summary>
[RequireComponent(typeof(AIAimingStrategy))]
[RequireComponent(typeof(AIAttackingStrategy))]
public class AttackingState : FsmState
{
    private AIMovingObject movingObject;
    private WeaponController weaponController;
    private AIAimingStrategy aimingStrategy;
    private AIAttackingStrategy attackingStrategy;

    public override void OnStateEnter()
    {
        Debug.Log($"{movingObject.name} starting attacking");

        IDamageable target = GameManager.Instance.Player;
        attackingStrategy.Initialize(weaponController, target);
        aimingStrategy.Initialize(movingObject, target);

        aimingStrategy.StartMoving();
        attackingStrategy.StartAttacking();
    }

    public override void OnStateLeave()
    {
        aimingStrategy.StopMoving();
        attackingStrategy.StopAttacking();
    }

    public override bool IsPerforming()
    {
        return !aimingStrategy.IsStopped();
    }

    private void Awake()
    {
        movingObject = GetComponentInParent<AIMovingObject>();
        weaponController = GetComponentInParent<WeaponController>();
        attackingStrategy = GetComponent<AIAttackingStrategy>();
        aimingStrategy = GetComponent<AIAimingStrategy>();
    }
}
