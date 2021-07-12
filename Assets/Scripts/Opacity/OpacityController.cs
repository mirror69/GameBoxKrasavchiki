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
    public const float MinOpacityValue = 0;
    /// <summary>
    /// ������������ ������� ��������� ��������
    /// </summary>
    public const float MaxOpacityValue = 100;
    /// <summary>
    /// ��� ��������� ������������
    /// </summary>
    private const float OpacityChangeStep = 0.5f;
    /// <summary>
    /// ����������� �������� ��������� ��� ����, ����� ������ �������� ������������
    /// </summary>
    private const float MinPositionDifferenceToChangeOpacity = 0.1f;

    [Tooltip("������, ����������� ����������� ���������� ������ ���������")]
    [SerializeField]
    private Transform increaseVector = null;
    
    [Tooltip("������-��������� ��������� ������ ���������")]
    [SerializeField]
    private Transform changeInitiator = null;   
    
    [Tooltip("�������� ��������� ��������� �������� (� ��������� �� ���� ��������)")]
    [SerializeField]  
    private float changeSpeed = 45;
    
    [Tooltip("�������� ���������, ��� ������� ������ ���������� ���������")]
    [SerializeField]    
    [Range(MinOpacityValue, MaxOpacityValue)]
    private float fullFadeInValue = 99;
    
    [Tooltip("�������� ���������, ��� ������� ������ ��������� ��������")]
    [SerializeField]
    [Range(MinOpacityValue, MaxOpacityValue)]
    private float fullFadeOutValue = 1;

    [Tooltip("�������� ���������, ���� �������� ������ ��������� ��������� ������������")]
    [SerializeField]
    [Range(MinOpacityValue, MaxOpacityValue)]
    private float fullOpaqueValue = 99;

    [Tooltip("��������� ��������� ���������")]
    [SerializeField]
    private OpacityChangingStrategy opacityChangingStrategy = null;

    /// <summary>
    /// ������� ����, ��������� ������ ������� ����� ���������
    /// </summary>
    private AnimatedObstacle[] opacityChangingObjects = null;
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
    /// <summary>
    /// ����������� �������� ��������� �������
    /// </summary>
    private float accumulatedPositionDifference = 0;

    public float OpacityValue => lastAppliedOpacityValue;

    /// <summary>
    /// �������, ���������� ��� ��������� �������� ���������
    /// </summary>
    private event Action OpacityChanged;

    public void RegisterOpacityChangedListener(Action listener)
    {
        OpacityChanged += listener;
    }
    public void UnregisterOpacityChangedListener(Action listener)
    {
        OpacityChanged -= listener;
    }

    private void Awake()
    {
        opacityChangingObjects = FindObjectsOfType<AnimatedObstacle>();
        OpacityChangingParameters opacityChangingParameters = new OpacityChangingParameters(fullOpaqueValue);
        foreach (var item in opacityChangingObjects)
        {
            item.Initialize(opacityChangingParameters);
        }

        opacityIncreaseVector = increaseVector.forward;
        // ��������� ������, ����������� ������ ���������� ��������� ��������, �.�. � ���� �� �� �����
        increaseVector.gameObject.SetActive(false);

        currentPositionAlongIncreaseVector = GetCurrentPositionAlongIncreaseVector();
        currentOpacityValue = MaxOpacityValue;
        lastAppliedOpacityValue = currentOpacityValue;
    }

    private void Update()
    {
        float newPositionAlongIncreaseVector = GetCurrentPositionAlongIncreaseVector();
        float positionDifference = newPositionAlongIncreaseVector - currentPositionAlongIncreaseVector;
        currentPositionAlongIncreaseVector = newPositionAlongIncreaseVector;
        
        accumulatedPositionDifference += positionDifference;

        // ���� �������� ������������� �������� ��������� � ���������� ��������� � ����������� ���������� ���������
        // (� ��������), �� ���������� ����������� �������� ������� �������, ����� ������ ������
        // �������� ��������� ������ � ������ ������ �������� � ��������������� �����������
        if (accumulatedPositionDifference > 0 && lastAppliedOpacityValue >= MaxOpacityValue
            || accumulatedPositionDifference < 0 && lastAppliedOpacityValue <= MinOpacityValue)
        {
            accumulatedPositionDifference = 0;            
            return;
        }
    
        // ���� ������� �������� ��������� = ������������ ��� �������������,
        // ��������� �������� �������� �� �����, � ��� ������������ ����������� ���������� ��������
        if ((lastAppliedOpacityValue >= MaxOpacityValue
            || lastAppliedOpacityValue <= MinOpacityValue)
            && Mathf.Abs(accumulatedPositionDifference) < MinPositionDifferenceToChangeOpacity)
        {
            return;
        }

        accumulatedPositionDifference = 0;

        // ����������� �������� ��������� ��������� ���, ����� ��� �� �������� �� �������� opacityValueForFullFadeIn
        // � opacityValueForFullFadeOut
        // �������������� �����, ����� ����� ���� ������������ ��������� � PlayMode
        // TODO. ����� ����� ��������, ��������� � Awake
        opacityFullFadeChangeSpeed = changeSpeed *
            (fullFadeInValue - fullFadeOutValue) / (MaxOpacityValue - MinOpacityValue);

        // ��� ����������� ����������, ������������ �������������, � ��������.
        float newOpacityValue = currentOpacityValue + positionDifference * opacityFullFadeChangeSpeed;

        // ������� ��������� ����� �������� � ��������� [opacityValueForFullFadeIn, opacityValueForFullFadeOut]
        // ��� ���������� ������ ��������� ����� ������������� ������ ��������� ��� ������ ����������� 
        // ���� �������� �������� � ��������������� �����������, ����� �������� ������ ������������ � ���� ���������
        if (newOpacityValue > currentOpacityValue)
        {
            if (newOpacityValue < fullFadeOutValue)
            {
                newOpacityValue = fullFadeOutValue;
            }
            else if (newOpacityValue  > fullFadeInValue)
            {
                newOpacityValue = MaxOpacityValue;
            }
        }
        else if (newOpacityValue < currentOpacityValue)
        {
            if (newOpacityValue > fullFadeInValue)
            {
                newOpacityValue = fullFadeInValue;
            }
            else if (newOpacityValue < fullFadeOutValue)
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
            lastAppliedOpacityValue = currentOpacityValue;
            RefreshVisibilityValueForObjects();           
        }

        currentPositionAlongIncreaseVector = newPositionAlongIncreaseVector;
    }

    /// <summary>
    /// �������� ������� ��������� ������� ��������
    /// </summary>
    private void RefreshVisibilityValueForObjects()
    {
        OpacityChanged?.Invoke();
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
        return Vector3.Dot(changeInitiator.transform.position, opacityIncreaseVector);
    }
}
