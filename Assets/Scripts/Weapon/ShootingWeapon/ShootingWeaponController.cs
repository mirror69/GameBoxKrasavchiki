using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  онтроллер оружи€
/// </summary>
public class ShootingWeaponController : WeaponController
{
    /// <summary>
    /// «начение отклонени€ угла, при котором можем стрел€ть по цели,
    /// т.е. насколько мы можем быть не довЄрнуты к цели, чтобы иметь возможность стрел€ть
    /// </summary>
    const float AngleDifferenceEpsilon = 10;

    private Shooting shooting = null;
    protected IDamageable target = null;

    public override void SetTarget(IDamageable target)
    {
        this.target = target;
    }

    public bool IsTargetOnAttackLine(IDamageable target)
    {
        Vector3 vectorToTarget = (target.Transform.position - AttackPoint.position);
        vectorToTarget.y = 0;

        return Vector3.Angle(AttackPoint.forward, vectorToTarget) <= AngleDifferenceEpsilon;
    }

    public bool ThereAreObstaclesOnAttackWay(IDamageable target)
    {
        Vector3 attackVector = (target.Transform.position - attackPoint.position);
        attackVector.y = 0;
        
        float bulletSize = shooting.BulletBounds.extents.x;
        
        // »спользуем SphereCast, т.к. нужно учитывать размер пули, определ€€ преп€тстви€ на пути.
        // «а целью может оказатьс€ стена. ≈сли будем провер€ть преп€тстви€ просто на
        // рассто€нии от точки атаки до цели, то при большом размере пули метод SphereCast может найти
        // стену за целью. ѕоэтому вычитаем радиус пули из рассто€ни€ до цели.
        float distanceToTarget = attackVector.magnitude - bulletSize;

        if (distanceToTarget < 0)
        {
            return false;
        }

        return Physics.SphereCast(attackPoint.position, bulletSize, attackVector, out RaycastHit hitInfo, 
            distanceToTarget, GameManager.Instance.ObstacleLayers, QueryTriggerInteraction.Ignore);
    }

    public AttackCheckResult CheckTargetAttackDistance(IDamageable target)
    {
        Vector3 targetPosition = target.Transform.position;
        targetPosition.y = attackPoint.position.y;

        float distanceToTarget = Vector3.Distance(attackPoint.position, targetPosition);
        if (distanceToTarget > attackDistance)
        {
            return AttackCheckResult.TooLongDistance;
        }

        return AttackCheckResult.Ok;
    }

    private void Awake()
    {
        shooting = GetComponent<Shooting>();
    }

    protected override void Hit()
    {
        Vector3 attackDirection;
        if (target != null)
        {
            attackDirection = (target.Transform.position - attackPoint.position).normalized;
            attackDirection.y = 0;
        }
        else
        {
            attackDirection = attackPoint.forward;
        }

        Vector3 endAttackPoint = attackPoint.position + attackDirection * attackDistance;
        Quaternion rotation = Quaternion.LookRotation(attackDirection);

        shooting.Shoot(attackPoint.position,
                        rotation,
                        Vector3.zero,
                        endAttackPoint,
                        1);

        isHitPerforming = false;
    }

}
