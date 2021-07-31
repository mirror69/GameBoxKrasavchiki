using UnityEngine;

/// <summary>
///  Атакующий луч, повреждающий владельца
/// </summary>
public class SelfDamageAttackBeam : AttackBeam
{
    [SerializeField]
    private DamageableObject healthObject;

    public override void StartAttack()
    {
        base.StartAttack();
        healthObject.SetEnabledRegeneration(false);
    }

    public override void StopAttack()
    {
        base.StopAttack();
        healthObject.SetEnabledRegeneration(true);
    }

    protected override void Awake()
    {
        base.Awake();
        RegisterOnDamageCausedListener(OnTargetDamageCaused);
    }

    protected override bool AreHitConditionsSatisfied()
    {
        return base.AreHitConditionsSatisfied() && healthObject.Health > 1;
    }

    private void OnDestroy()
    {
        UnregisterOnDamageCausedListener(OnTargetDamageCaused);
    }

    private void OnTargetDamageCaused(int damage, float multiplier)
    {
        healthObject.ReceiveDamage(damage);
    }
}
