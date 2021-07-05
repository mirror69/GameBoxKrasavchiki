using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������, ��������� ������ ���� ������������
/// </summary>
public class OpacityChangingObject : MonoBehaviour
{
    /// <summary>
    /// ���������� ��� ���������/���������� � ����������� �� ������ ���������
    /// </summary>
    private Collider[] colliders;
    /// <summary>
    /// ��������� ��� ��������� ������ ���������
    /// </summary>
    private List<OpacityChangingMaterial> opacityChangingMaterials;
    /// <summary>
    /// �������� ��� ��������, ��������� �� �������� ������ ������� ��� ���
    /// </summary>
    private StayInsideTrigger[] stayInsideTriggers;

    /// <summary>
    /// ���������� ������� ��������� �������
    /// </summary>
    /// <param name="valueInPercents">������� ��������� � ���������</param>
    public void SetOpacityValue(float opacityValueInPercents)
    {
        foreach (var item in opacityChangingMaterials)
        {
            item.SetOpacityValue(opacityValueInPercents);
        }
    }

    private void Awake()
    {
        colliders = gameObject.GetComponentsInChildren<Collider>();
        
        // ��������� � ������ ��� ��������� ������� � �������� �������� ��� ������������ ���������
        // �� ������������
        Renderer[] meshRenderers = gameObject.GetComponentsInChildren<Renderer>();
        opacityChangingMaterials = new List<OpacityChangingMaterial>();
        foreach (var meshRenderer in meshRenderers)
        {
            foreach (var material in meshRenderer.materials)
            {
                OpacityChangingMaterial opacityChangingMaterial = new OpacityChangingMaterial(material);
                opacityChangingMaterials.Add(opacityChangingMaterial);
                // ������ �������� ��������� ����� �������, ����� �� ��� ������ ������������
                opacityChangingMaterial.SetFadeMode();
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
}
