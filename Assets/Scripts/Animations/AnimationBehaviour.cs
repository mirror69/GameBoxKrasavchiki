using System.Collections;
using UnityEngine;

/// <summary>
/// ������� ����������� ����� ��� ��������� ��������
/// </summary>
public abstract class AnimationBehaviour : MonoBehaviour
{
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
    protected float MaxVelocityForMovementBlendTree = 20f;

    protected virtual void Start()
    {
        // ������������ ��������� �������� ��������, � ����������� �� �������� ���������
        if (IsParamaterExists(AnimatorConstants.Parameters.VelocityMultiplier))
        {
            animator.SetFloat(AnimatorConstants.Parameters.VelocityMultiplier, 
                player.PlayerSpeed / MaxVelocityForMovementBlendTree);
        }
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

    /// <summary>
    /// ����������� �������� (������������ � �����������, ������� ���������� ��� ��������)
    /// </summary>
    protected virtual void AnimateMove()
    {
    }

    private void Update()
    {
        AnimateMove();
    }
}


