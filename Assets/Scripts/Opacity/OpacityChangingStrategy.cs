using UnityEngine;

/// <summary>
/// Стратегия изменения видимости. Отвечает на то, как материал будет обрабатывать различные значения видимости
/// </summary>
public abstract class OpacityChangingStrategy: MonoBehaviour
{
    /// <summary>
    /// Установить значение видимости для материала
    /// </summary>
    /// <param name="material"></param>
    /// <param name="value"></param>
    public abstract void SetOpacity(Material material, float value);
    
    /// <summary>
    /// Установить режим прозрачности для материала
    /// </summary>
    /// <param name="material"></param>
    public virtual void SetFadeMode(Material material)
    {
    }
    /// <summary>
    /// Установить режим непрозрачности для материала
    /// </summary>
    /// <param name="material"></param>
    public virtual void SetOpaqueMode(Material material)
    {
    }
}
