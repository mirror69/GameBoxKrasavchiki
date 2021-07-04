using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������, ��������� ������ ���� ������������
/// </summary>
public class VisibilityChangingObject : MonoBehaviour
{
    /// <summary>
    /// ���������� ��� ���������/���������� � ����������� �� ������ ���������
    /// </summary>
    private Collider[] colliders;
    /// <summary>
    /// ��������� ��� ��������� ������ ���������
    /// </summary>
    private Renderer[] meshRenderers;
    /// <summary>
    /// �������� ��� ��������, ��������� �� �������� ������ ������� ��� ���
    /// </summary>
    private StayInsideTrigger[] stayInsideTriggers;

    /// <summary>
    /// ���������� ������� ��������� �������
    /// </summary>
    /// <param name="valueInPercents">������� ��������� � ���������</param>
    public void SetVisibilityValue(float visibilityValueInPercents)
    {
        float alphaValue = visibilityValueInPercents / 100;
        foreach (var meshRenderer in meshRenderers)
        {
            foreach (var material in meshRenderer.materials)
            {
                Color newColor = material.color;
                newColor.a = alphaValue;
                material.color = newColor;
            }
        }
    }

    /// <summary>
    /// �������� ��������� ����� �������, ����� ��� ����� ������ ������������
    /// </summary>
    public void ChangeMaterialsToFade()
    {
        foreach (var meshRenderer in meshRenderers)
        {
            foreach (var material in meshRenderer.materials)
            {
                material.ToFadeMode();
            }
        }
    }

    /// <summary>
    /// ��������/��������� ����������
    /// </summary>
    /// <param name="enable">true - �������� ����������, false - ��������� ����������</param>
    public void SetEnableColliders(bool enable)
    {
        foreach (var collider in colliders)
        {
            collider.enabled = enable;
        }
    }

    private void Awake()
    {
        colliders = gameObject.GetComponentsInChildren<Collider>();
        meshRenderers = gameObject.GetComponentsInChildren<Renderer>();
        ChangeMaterialsToFade();
    }
}
