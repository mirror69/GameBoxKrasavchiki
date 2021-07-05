using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Объект, способный менять свою прозрачность
/// </summary>
public class OpacityChangingObject : MonoBehaviour
{
    /// <summary>
    /// Коллайдеры для включения/отключения в зависимости от уровня видимости
    /// </summary>
    private Collider[] colliders;
    /// <summary>
    /// Материалы для изменения уровня видимости
    /// </summary>
    private List<OpacityChangingMaterial> opacityChangingMaterials;
    /// <summary>
    /// Триггеры для проверки, находится ли персонаж внутри объекта или нет
    /// </summary>
    private StayInsideTrigger[] stayInsideTriggers;

    /// <summary>
    /// Установить уровень видимости объекта
    /// </summary>
    /// <param name="valueInPercents">Уровень видимости в процентах</param>
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
        
        // Добавляем в список все материалы объекта и дочерних объектов для последующего изменения
        // их прозрачности
        Renderer[] meshRenderers = gameObject.GetComponentsInChildren<Renderer>();
        opacityChangingMaterials = new List<OpacityChangingMaterial>();
        foreach (var meshRenderer in meshRenderers)
        {
            foreach (var material in meshRenderer.materials)
            {
                OpacityChangingMaterial opacityChangingMaterial = new OpacityChangingMaterial(material);
                opacityChangingMaterials.Add(opacityChangingMaterial);
                // Меняем рендерер материала таким образом, чтобы он мог менять прозрачность
                opacityChangingMaterial.SetFadeMode();
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
}
