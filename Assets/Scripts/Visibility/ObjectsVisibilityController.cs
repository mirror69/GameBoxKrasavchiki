using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// ���������� ��� ���������� ������� ��������� ���� �������� �� �����, ��������� ������ ���� ������������
/// </summary>
public class ObjectsVisibilityController : MonoBehaviour
{
    /// <summary>
    /// ����������� ������� ��������� ��������
    /// </summary>    
    private const float MinVisibilityValue = 0;
    /// <summary>
    /// ������������ ������� ��������� ��������
    /// </summary>
    private const float MaxVisibilityValue = 100;
    /// <summary>
    /// ��� ��������� ������������
    /// </summary>
    private const float VisibilityChangeStep = 0.5f;

    [Tooltip("������, ����������� ����������� ���������� ������ ���������")]
    [SerializeField]
    private Transform visibilityIncreaseDirectionTransform = null;
    [Tooltip("������-��������� ��������� ������ ���������")]
    [SerializeField]
    private Transform visibilityChangeInitiator = null;   
    [Tooltip("�������� ��������� ��������� �������� (� ��������� �� ���� ��������)")]
    [SerializeField]
    private float visibilityChangeSpeed = 10;

    /// <summary>
    /// ������� ����, ��������� ������ ������� ����� ���������
    /// </summary>
    private VisibilityChangingObject[] transparencyChangingObjects = null;
    /// <summary>
    /// ������� ������� ���������
    /// </summary>
    private float currentVisibilityValue = MaxVisibilityValue;

    /// <summary>
    /// ������ ���������� ������ ���������
    /// </summary>
    private Vector3 visibilityIncreaseVector = Vector3.zero;
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
        transparencyChangingObjects = FindObjectsOfType<VisibilityChangingObject>();
        visibilityIncreaseVector = visibilityIncreaseDirectionTransform.forward;
        // ��������� ������, ����������� ������ ���������� ��������� ��������, �.�. � ���� �� �� �����
        visibilityIncreaseDirectionTransform.gameObject.SetActive(false);
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
        float newOpacityValue = currentVisibilityValue + accumulatedPositionDifference * visibilityChangeSpeed;

        // ������������, ���� ����� �� ���������� ������� ������������.
        // ����� � ���� ������ ���������� ����������� ������� ���������, ����� ��� �������� � ������ �����������
        // ����� ������ ������ ������������
        if (newOpacityValue > MaxVisibilityValue)
        {
            newOpacityValue = MaxVisibilityValue;
            accumulatedPositionDifference = 0;
        }
        else if (newOpacityValue < MinVisibilityValue)
        {
            newOpacityValue = MinVisibilityValue;
            accumulatedPositionDifference = 0;
        }

        if (Mathf.Abs(currentVisibilityValue - newOpacityValue) >= VisibilityChangeStep)
        {
            currentVisibilityValue = Mathf.Round(newOpacityValue / VisibilityChangeStep) * VisibilityChangeStep;
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
        foreach (var item in transparencyChangingObjects)
        {
            item.SetVisibilityValue(currentVisibilityValue);
        }
    }

    /// <summary>
    /// �������� ������� ������� �������-���������� ����� ������� ���������� ������ ���������
    /// </summary>
    /// <returns></returns>
    private float GetCurrentPositionAlongIncreaseVector()
    {
        return Vector3.Dot(visibilityChangeInitiator.transform.position, visibilityIncreaseVector);
    }
}
