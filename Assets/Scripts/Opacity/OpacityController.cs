using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// ���������� ��� ���������� ������� ��������� ���� �������� �� �����, ��������� ������ ���� ������������
/// </summary>
public class OpacityController : MonoBehaviour
{
    /// <summary>
    /// ����������� ������� ��������� ��������
    /// </summary>    
    private const float MinOpacityValue = 0;
    /// <summary>
    /// ������������ ������� ��������� ��������
    /// </summary>
    private const float MaxOpacityValue = 100;
    /// <summary>
    /// ��� ��������� ������������
    /// </summary>
    private const float OpacityChangeStep = 0.5f;
    /// <summary>
    /// ����������� �������� ��������� ��� ����, ����� ������ �������� ������������
    /// </summary>
    private const float MinPositionDifferenceToChangeOpacity = 1f;

    [Tooltip("������, ����������� ����������� ���������� ������ ���������")]
    [SerializeField]
    private Transform opacityIncreaseDirectionTransform = null;
    
    [Tooltip("������-��������� ��������� ������ ���������")]
    [SerializeField]
    private Transform opacityChangeInitiator = null;   
    
    [Tooltip("�������� ��������� ��������� �������� (� ��������� �� ���� ��������)")]
    [SerializeField]  
    private float opacityChangeSpeed = 20;
    
    [Tooltip("�������� ���������, ��� ������� ������ ���������� ���������")]
    [SerializeField]    
    [Range(MinOpacityValue, MaxOpacityValue)]
    private float opacityValueForFullFadeIn = 70;
    
    [Tooltip("�������� ���������, ��� ������� ������ ��������� ��������")]
    [SerializeField]
    [Range(MinOpacityValue, MaxOpacityValue)]
    private float opacityValueForFullFadeOut = 20;

    /// <summary>
    /// ������� ����, ��������� ������ ������� ����� ���������
    /// </summary>
    private OpacityChangingObject[] opacityChangingObjects = null;
    /// <summary>
    /// ������� ������� ���������
    /// </summary>
    private float currentOpacityValue = 0;

    /// <summary>
    /// ������ ���������� ������ ���������
    /// </summary>
    private Vector3 opacityIncreaseVector = Vector3.zero;
    /// <summary>
    /// ������� ������� �������-���������� ����� ������� ���������� ������ ������������
    /// </summary>
    private float currentPositionAlongIncreaseVector = 0;
    /// <summary>
    /// ��������� ����������� �������� ��������� ��� ��������
    /// </summary>
    private float lastAppliedOpacityValue = 0;
    /// <summary>
    /// �������� ��������� ������������, �� ��������� �� �������� opacityValueForFullFadeIn � opacityValueForFullFadeOut
    /// </summary>
    private float opacityFullFadeChangeSpeed = 0;
    private float accumulatedPositionDifference = 0;
    private void Awake()
    {
        opacityChangingObjects = FindObjectsOfType<OpacityChangingObject>();
        opacityIncreaseVector = opacityIncreaseDirectionTransform.forward;
        // ��������� ������, ����������� ������ ���������� ��������� ��������, �.�. � ���� �� �� �����
        opacityIncreaseDirectionTransform.gameObject.SetActive(false);

        currentPositionAlongIncreaseVector = GetCurrentPositionAlongIncreaseVector();
        currentOpacityValue = MaxOpacityValue;
        lastAppliedOpacityValue = currentOpacityValue;
        accumulatedPositionDifference = 0;
    }

    private void Update()
    {
        float newPositionAlongIncreaseVector = GetCurrentPositionAlongIncreaseVector();
        float positionDifference = newPositionAlongIncreaseVector - currentPositionAlongIncreaseVector;

        // ������ ������������ ������ � ��� ������, ���� �������� ���������� ��������� �� ������ �����
        accumulatedPositionDifference += positionDifference;
        if (Mathf.Abs(accumulatedPositionDifference) < MinPositionDifferenceToChangeOpacity)
        {
            return;
        }
        accumulatedPositionDifference = 0;

        // ����������� �������� ��������� ��������� ���, ����� ��� �� �������� �� �������� opacityValueForFullFadeIn
        // � opacityValueForFullFadeOut
        // �������������� �����, ����� ����� ���� ������������ ��������� � PlayMode
        // TODO. ����� ����� ��������, ��������� � Awake
        opacityFullFadeChangeSpeed = opacityChangeSpeed *
            (opacityValueForFullFadeIn - opacityValueForFullFadeOut) / (MaxOpacityValue - MinOpacityValue);

        // ��� ����������� ����������, ������������ �������������, � ��������.
        float newOpacityValue = currentOpacityValue + positionDifference * opacityFullFadeChangeSpeed;

        // ������� ��������� ����� �������� � ��������� [opacityValueForFullFadeIn, opacityValueForFullFadeOut]
        // ��� ���������� ������ ��������� ����� ������������� ������ ��������� ��� ������ ����������� 
        // ���� �������� �������� � ��������������� �����������, ����� �������� ������ ������������ � ���� ���������
        if (newOpacityValue > currentOpacityValue)
        {
            if (newOpacityValue < opacityValueForFullFadeOut)
            {
                newOpacityValue = opacityValueForFullFadeOut;
            }
            else if (newOpacityValue  > opacityValueForFullFadeIn)
            {
                newOpacityValue = MaxOpacityValue;
            }
        }
        else if (newOpacityValue < currentOpacityValue)
        {
            if (newOpacityValue > opacityValueForFullFadeIn)
            {
                newOpacityValue = opacityValueForFullFadeIn;
            }
            else if (newOpacityValue < opacityValueForFullFadeOut)
            {
                newOpacityValue = MinOpacityValue;
            }
        }

        currentOpacityValue = newOpacityValue;
        // ��� ��������� ������������ ������� ����� ������� � ����� ������� ������������ 
        // ������ ���� ������ �������������� ����
        if (Mathf.Abs(lastAppliedOpacityValue - currentOpacityValue) >= OpacityChangeStep)
        {
            currentOpacityValue = Mathf.Round(currentOpacityValue / OpacityChangeStep) * OpacityChangeStep;
            RefreshVisibilityValueForObjects();
            lastAppliedOpacityValue = currentOpacityValue;
        }
        
        currentPositionAlongIncreaseVector = newPositionAlongIncreaseVector;
    }

    /// <summary>
    /// �������� ������� ��������� ������� ��������
    /// </summary>
    private void RefreshVisibilityValueForObjects()
    {
        foreach (var item in opacityChangingObjects)
        {
            item.SetOpacityValue(currentOpacityValue);
        }
    }

    /// <summary>
    /// �������� ������� ������� �������-���������� ����� ������� ���������� ������ ���������
    /// </summary>
    /// <returns></returns>
    private float GetCurrentPositionAlongIncreaseVector()
    {
        return Vector3.Dot(opacityChangeInitiator.transform.position, opacityIncreaseVector);
    }
}
