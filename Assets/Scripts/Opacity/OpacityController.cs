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

    [Tooltip("������, ����������� ����������� ���������� ������ ���������")]
    [SerializeField]
    private Transform opacityIncreaseDirectionTransform = null;
    [Tooltip("������-��������� ��������� ������ ���������")]
    [SerializeField]
    private Transform opacityChangeInitiator = null;   
    [Tooltip("�������� ��������� ��������� �������� (� ��������� �� ���� ��������)")]
    [SerializeField]
    private float opacityChangeSpeed = 10;

    /// <summary>
    /// ������� ����, ��������� ������ ������� ����� ���������
    /// </summary>
    private OpacityChangingObject[] opacityChangingObjects = null;
    /// <summary>
    /// ������� ������� ���������
    /// </summary>
    private float currentOpacityValue = MaxOpacityValue;

    /// <summary>
    /// ������ ���������� ������ ���������
    /// </summary>
    private Vector3 opacityIncreaseVector = Vector3.zero;
    /// <summary>
    /// ������� ������� �������-���������� ����� ������� ���������� ������ ������������
    /// </summary>
    private float currentPositionAlongIncreaseVector = 0;
    /// <summary>
    /// ����������� �������� ������� ������� ������� �������-���������� � ������� �������, ��� ������� 
    /// ������� ��������� ��� ������� � ��������� ���
    /// </summary>
    private float accumulatedPositionDifference = 0;

    private void Awake()
    {
        opacityChangingObjects = FindObjectsOfType<OpacityChangingObject>();
        opacityIncreaseVector = opacityIncreaseDirectionTransform.forward;
        // ��������� ������, ����������� ������ ���������� ��������� ��������, �.�. � ���� �� �� �����
        opacityIncreaseDirectionTransform.gameObject.SetActive(false);
    }

    private void Start()
    {
        currentPositionAlongIncreaseVector = GetCurrentPositionAlongIncreaseVector();
    }

    private void Update()
    {
        float newPositionAlongIncreaseVector = GetCurrentPositionAlongIncreaseVector();
        float positionDifference = newPositionAlongIncreaseVector - currentPositionAlongIncreaseVector;
        accumulatedPositionDifference += positionDifference;

        // ��� ����������� ����������, ������������ �������������, � ��������.
        float newOpacityValue = currentOpacityValue + accumulatedPositionDifference * opacityChangeSpeed;

        // ������������, ���� ����� �� ���������� ������� ������������.
        // ����� � ���� ������ ���������� ����������� ������� ���������, ����� ��� �������� � ������ �����������
        // ����� ������ ������ ������������
        if (newOpacityValue > MaxOpacityValue)
        {
            newOpacityValue = MaxOpacityValue;
            accumulatedPositionDifference = 0;
        }
        else if (newOpacityValue < MinOpacityValue)
        {
            newOpacityValue = MinOpacityValue;
            accumulatedPositionDifference = 0;
        }

        if (Mathf.Abs(currentOpacityValue - newOpacityValue) >= OpacityChangeStep)
        {
            currentOpacityValue = Mathf.Round(newOpacityValue / OpacityChangeStep) * OpacityChangeStep;
            RefreshVisibilityValueForObjects();
            // ���������� ����������� ������� ���������, ����� ������ ������ ��-����� �� ���������� ���� ���������
            accumulatedPositionDifference = 0;
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
