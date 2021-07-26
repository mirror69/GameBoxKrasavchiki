using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���������� ������
/// </summary>
public class WeaponController : MonoBehaviour
{
    /// <summary>
    /// �������� ���������� ����, ��� ������� ����� �������� �� ����,
    /// �.�. ��������� �� ����� ���� �� �������� � ����, ����� ����� ����������� ��������
    /// </summary>
    const float AngleDifferenceEpsilon = 10;

    [SerializeField]
    private Transform attackPoint = null;
    [SerializeField]
    private float attackDistance = 10;
    [SerializeField]
    private float chargeDuration = 1;
    [SerializeField]
    private float coolDownDuration = 1;

    private Shooting shooting = null;

    private IDamageable target = null;

    private bool isInterruptionRequested = false;

    Coroutine attackCoroutine = null;

    public Transform AttackPoint => attackPoint;

    public void SetTarget(IDamageable target)
    {
        this.target = target;
    }

    public void Attack()
    {
        if (attackCoroutine == null)
        {
            attackCoroutine = StartCoroutine(PerformAttack());
        }        
    }

    public void InterruptAttack()
    {
        isInterruptionRequested = true;
    }

    public bool IsWeaponReady()
    {
        return attackCoroutine == null;
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
        
        // ���������� SphereCast, �.�. ����� ��������� ������ ����, ��������� ����������� �� ����.
        // �� ����� ����� ��������� �����. ���� ����� ��������� ����������� ������ ��
        // ���������� �� ����� ����� �� ����, �� ��� ������� ������� ���� ����� SphereCast ����� �����
        // ����� �� �����. ������� �������� ������ ���� �� ���������� �� ����.
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

        Hit();

        if (coolDownDuration > 0)
        {
            yield return new WaitForSeconds(coolDownDuration);
        }

        attackCoroutine = null;
    }
}
