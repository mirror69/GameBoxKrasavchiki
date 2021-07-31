using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    protected Coroutine hittingCoroutine = null;

    protected IDamageable target = null;
    protected Transform attackPoint = null;

    public virtual void Initialize(Transform attackPoint)
    {
        this.attackPoint = attackPoint;
    }

    public virtual IDamageable GetTarget()
    {
        return target;
    }

    public virtual bool IsAttacking()
    {
        return false;
    }

    public virtual void StartAttack()
    {

    }
    public virtual void StopAttack()
    {

    }
}
