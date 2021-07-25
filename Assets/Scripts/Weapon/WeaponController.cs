using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  онтроллер оружи€
/// </summary>
public class WeaponController : MonoBehaviour
{
    /// <summary>
    /// «начение отклонени€ угла, при котором можем стрел€ть по цели,
    /// т.е. насколько мы можем быть не довЄрнуты к цели, чтобы иметь возможность стрел€ть
    /// </summary>
    const float AngleDifferenceEpsilon = 10;

    [SerializeField]
    private Transform attackPoint;
    [SerializeField]
    private float attackDistance;
    [SerializeField]
    private float chargeDuration;
    [SerializeField]
    private float coolDownDuration;

    private Shooting shooting;

    private IDamageable target;

    public WeaponStatus Status { get; private set; }

    public Transform AttackPoint => attackPoint;

    public void SetTarget(IDamageable target)
    {
        this.target = target;
    }

    public void Attack()
    {
        StartCoroutine(PerformAttack());
    }

    public void InterruptAttack()
    {
        StopAllCoroutines();
        Status = WeaponStatus.Ready;
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

        RaycastHit[] hitInfo = Physics.SphereCastAll(attackPoint.position, bulletSize, 
            attackVector, distanceToTarget, GameManager.Instance.ObstacleLayers);

        foreach (var item in hitInfo)
        {
            if (!item.collider.isTrigger)
            {
                return true;
            }
        }
        return false;
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
        Status = WeaponStatus.Ready;
    }

    private void Hit()
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
    }

    private IEnumerator PerformAttack()
    {
        Status = WeaponStatus.Busy;

        if (chargeDuration > 0)
        {
            yield return new WaitForSeconds(chargeDuration);
        }

        Hit();

        if (coolDownDuration > 0)
        {
            yield return new WaitForSeconds(coolDownDuration);
        }

        Status = WeaponStatus.Ready;
    }
}
