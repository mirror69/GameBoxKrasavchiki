using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Объект, способный менять свою прозрачность
/// </summary>
public class VisibilityChangingObject : MonoBehaviour
{
    /// <summary>
    /// Коллайдеры для включения/отключения в зависимости от уровня видимости
    /// </summary>
    private Collider[] colliders;
    /// <summary>
    /// Рендереры для изменения уровня видимости
    /// </summary>
    private Renderer[] meshRenderers;
    /// <summary>
    /// Триггеры для проверки, находится ли персонаж внутри объекта или нет
    /// </summary>
    private StayInsideTrigger[] stayInsideTriggers;

    /// <summary>
    /// Установить уровень видимости объекта
    /// </summary>
    /// <param name="valueInPercents">Уровень видимости в процентах</param>
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
    /// Изменить материалы таким образом, чтобы они могли менять прозрачность
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
    /// Включить/выключить коллайдеры
    /// </summary>
    /// <param name="enable">true - включить коллайдеры, false - выключить коллайдеры</param>
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
