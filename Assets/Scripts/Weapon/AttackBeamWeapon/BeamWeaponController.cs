using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Контроллер оружия игрока
/// </summary>
public class BeamWeaponController : WeaponController
{
    [SerializeField]
    private AttackBeam attackBeam = null;

    public override void SetTarget(IDamageable target)
    {
        attackBeam.SetTarget(target);
    }

    public override IDamageable GetTarget()
    {
        return attackBeam.GetTarget();
    }

    protected override void Hit()
    {
        isHitPerforming = true;
        StartCoroutine(PerformHit());
    }

    private void Awake()
    {
        attackBeam.Initialize(attackPoint, attackDistance);   
    }  

    private IEnumerator PerformHit()
    {
        attackBeam.StartAttack();
        while (true)
        {
            yield return null;
            if (isInterruptionRequested || !attackBeam.IsAttacking())
            {
                attackBeam.StopAttack();
                break;
            }
        }

        isHitPerforming = false;
    }
}
