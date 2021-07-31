using System.Collections;
using UnityEngine;

/// <summary>
/// Ѕазовый абстрактный класс дл€ обработки анимаций
/// </summary>
public abstract class AnimationBehaviour : MonoBehaviour
{
    protected const float TransitionTimeBetweenLayers = 0.2f;
    /// <summary>
    /// ¬рем€ сглаживани€ значени€ параметра при плавном изменении
    /// </summary>
    protected const float ParameterChangeValueDampTime = 0.5f;
    /// <summary>
    /// –азница между текущим и целевым значением параметра, меньше которой считаем, что значени€ равны
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
        // –ассчитываем множитель скорости анимации, в зависимости от скорости персонажа
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
    /// «адать значение параметра float (плавное изменение)
    /// </summary>
    /// <param name="paramName">»м€ параметра</param>
    /// <param name="paramValue">÷елевое значение параметра</param>
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
    /// «адать значение параметра float
    /// </summary>
    /// <param name="paramName">»м€ параметра</param>
    /// <param name="paramValue">«начение параметра</param>
    protected void SetParameterValue(string paramName, float paramValue)
    {
        if (animator == null || !IsParamaterExists(paramName))
        {
            return;
        }

        animator.SetFloat(paramName, paramValue);
    }

    /// <summary>
    /// «адать значение параметра bool
    /// </summary>
    /// <param name="paramName">»м€ параметра</param>
    /// <param name="paramValue">«начение параметра</param>
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
    /// ѕроверить, существует ли параметр в аниматоре
    /// </summary>
    /// <param name="paramName">»м€ параметра</param>
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
    /// јнимировать движение (определ€етс€ в наследниках, которые используют это действие)
    /// </summary>
    protected virtual void AnimateMove()
    {
    }
    /// <summary>
    /// јнимировать атаку (определ€етс€ в наследниках, которые используют это действие)
    /// </summary>
    protected virtual void AnimateAttack()
    {
    }
    /// <summary>
    /// јнимировать остановку атаки (определ€етс€ в наследниках, которые используют это действие)
    /// </summary>
    protected virtual void AnimateStopAttack()
    {
    }

    private void Update()
    {
        AnimateMove();
    }
}


