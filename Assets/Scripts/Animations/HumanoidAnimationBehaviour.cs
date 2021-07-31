using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Класс для обработки анимаций гуманойдного персонажа
/// </summary>
public class HumanoidAnimationBehaviour : AnimationBehaviour
{
    /// <summary>
    /// Анимировать движение 
    /// </summary>
    protected override void AnimateMove()
    {
        if (animator == null)
        {
            return;
        }

        Vector3 velocityVector = player.CurrentVelocity;
        Vector3 movementDirection = velocityVector.normalized;

        int turnDirection = player.GetTurnDirection();
        SetParameterValueSmooth(AnimatorConstants.Parameters.TurnValue, turnDirection);
        
        float velocityZ = 0;
        float velocityX = 0;
        if (movementDirection != Vector3.zero)
        {
            velocityZ = Vector3.Dot(velocityVector / player.PlayerSpeed, transform.forward);
            velocityX = Vector3.Dot(velocityVector / player.PlayerSpeed, transform.right);
        }

        SetParameterValue(AnimatorConstants.Parameters.ForwardVelocity, velocityZ);
        SetParameterValue(AnimatorConstants.Parameters.SideVelocity, velocityX);

        if (!IsParamaterExists(AnimatorConstants.Parameters.Moving))
        {
            return;
        }

        bool moving = animator.GetBool(AnimatorConstants.Parameters.Moving);
        if (moving && movementDirection == Vector3.zero
            || !moving && movementDirection != Vector3.zero)
        {
            animator.SetBool(AnimatorConstants.Parameters.Moving, movementDirection != Vector3.zero);
        }
    }

    /// <summary>
    /// Анимировать атаку
    /// </summary>
    protected override void AnimateAttack()
    {
        SetAttackModeEnabled(true);
    }
    /// <summary>
    /// Анимировать остановку атаки
    /// </summary>
    protected override void AnimateStopAttack()
    {
        SetAttackModeEnabled(false);
    }

    private void SetAttackModeEnabled(bool enabled)
    {
        SetParameterValue(AnimatorConstants.Parameters.Attacking, enabled);
        SetEnabledLayer(AnimatorConstants.Layers.UpperBody, enabled);
    }
}

