using System.Collections;
using UnityEngine;

/// <summary>
/// ������� ����������� ����� ��� ��������� ��������
/// </summary>
public abstract class AnimationBehaviour : MonoBehaviour
{
    protected const float TransitionTimeBetweenLayers = 0.2f;
    /// <summary>
    /// ����� ����������� �������� ��������� ��� ������� ���������
    /// </summary>
    protected const float ParameterChangeValueDampTime = 0.5f;
    /// <summary>
    /// ������� ����� ������� � ������� ��������� ���������, ������ ������� �������, ��� �������� �����
    /// </summary>    
    protected const float ParameterChangeValueAccuracy = 0.01f;

    [SerializeField]
    protected Animator animator;
    [SerializeField]
    protected PlayerController player;
    [SerializeField]
    protected WeaponController weapon;
    [SerializeField]
    protected float MaxVelocityForMovementBlendTree = 20f;

    protected virtual void Start()
    {
        // ������������ ��������� �������� ��������, � ����������� �� �������� ���������
        if (IsParamaterExists(AnimatorConstants.Parameters.VelocityMultiplier))
        {
            animator.SetFloat(AnimatorConstants.Parameters.VelocityMultiplier, 
                player.PlayerSpeed / MaxVelocityForMovementBlendTree);
        }
        weapon.RegisterAttackStartedListener(AnimateAttack);
        weapon.RegisterAttackEndedListener(AnimateStopAttack);
    }

    protected virtual void OnDestroy()
    {
        if (weapon == null)
        {
            return;
        }
        weapon.UnregisterAttackStartedListener(AnimateAttack);
        weapon.UnregisterAttackEndedListener(AnimateStopAttack);
    }

    /// <summary>
    /// ������ �������� ��������� float (������� ���������)
    /// </summary>
    /// <param name="paramName">��� ���������</param>
    /// <param name="paramValue">������� �������� ���������</param>
    protected void SetParameterValueSmooth(string paramName, float paramValue)
    {
        if (animator == null || !IsParamaterExists(paramName))
        {
            return;
        }

        float currentValue = animator.GetFloat(paramName);
        int nearestIntValue = Mathf.RoundToInt(currentValue);
        if (Mathf.Abs(currentValue - nearestIntValue) < ParameterChangeValueAccuracy
             && paramValue == nearestIntValue)
        {
            animator.SetFloat(paramName, nearestIntValue);
        }
        else
        {
            animator.SetFloat(paramName, paramValue, ParameterChangeValueDampTime, Time.deltaTime);
        }
    }

    /// <summary>
    /// ������ �������� ��������� float
    /// </summary>
    /// <param name="paramName">��� ���������</param>
    /// <param name="paramValue">�������� ���������</param>
    protected void SetParameterValue(string paramName, float paramValue)
    {
        if (animator == null || !IsParamaterExists(paramName))
        {
            return;
        }

        animator.SetFloat(paramName, paramValue);
    }

    /// <summary>
    /// ������ �������� ��������� bool
    /// </summary>
    /// <param name="paramName">��� ���������</param>
    /// <param name="paramValue">�������� ���������</param>
    protected void SetParameterValue(string paramName, bool paramValue)
    {
        if (animator == null || !IsParamaterExists(paramName))
        {
            return;
        }

        animator.SetBool(paramName, paramValue);
    }

    protected void SetEnabledLayer(string layerName, bool enabled)
    {
        StartCoroutine(PerformSetEnabledLayer(layerName, enabled));
    }

    protected IEnumerator PerformSetEnabledLayer(string layerName, bool enabled)
    {
        if (animator == null || !IsLayerExists(layerName))
        {
            yield break;
        }

        float targetValue = enabled ? 1 : 0;

        int layerIndex = animator.GetLayerIndex(layerName);

        float currentTransitionTime = 0;
        while (currentTransitionTime < TransitionTimeBetweenLayers)
        {
            float currentValue = currentTransitionTime / TransitionTimeBetweenLayers;
            if (!enabled)
            {
                currentValue = 1 - currentValue;
            }
            animator.SetLayerWeight(layerIndex, currentValue);
            yield return null;
            currentTransitionTime += Time.deltaTime;
        }
        animator.SetLayerWeight(layerIndex, targetValue);
    }

    /// <summary>
    /// ���������, ���������� �� �������� � ���������
    /// </summary>
    /// <param name="paramName">��� ���������</param>
    /// <returns></returns>
    protected bool IsParamaterExists(string paramName)
    {
        for (int i = 0; i < animator.parameterCount; i++)
        {
            if (animator.parameters[i].name == paramName)
            {
                return true;
            }
        }
        return false;
    }

    protected bool IsLayerExists(string paramName)
    {
        for (int i = 0; i < animator.layerCount; i++)
        {
            if (animator.GetLayerName(i) == paramName)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// ����������� �������� (������������ � �����������, ������� ���������� ��� ��������)
    /// </summary>
    protected virtual void AnimateMove()
    {
    }
    /// <summary>
    /// ����������� ����� (������������ � �����������, ������� ���������� ��� ��������)
    /// </summary>
    protected virtual void AnimateAttack()
    {
    }
    /// <summary>
    /// ����������� ��������� ����� (������������ � �����������, ������� ���������� ��� ��������)
    /// </summary>
    protected virtual void AnimateStopAttack()
    {
    }

    private void Update()
    {
        AnimateMove();
    }
}


