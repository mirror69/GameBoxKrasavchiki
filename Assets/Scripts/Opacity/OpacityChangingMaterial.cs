using UnityEngine;
/// <summary>
/// Материал, изменяющий уровень своей видимости
/// </summary>
public class OpacityChangingMaterial
{
    /// <summary>
    /// Материал, у которого может изменяться видимость
    /// </summary>
    public Material Material { get; private set; }
    /// <summary>
    /// Максимальное значение альфа-канала, установленное для данного материала
    /// </summary>
    public float MaxAlphaValue { get; private set; }

    public OpacityChangingMaterial(Material material)
    {
        Material = material;
        MaxAlphaValue = material.color.a;
    }

    /// <summary>
    /// Задать уровень видимости материала
    /// </summary>
    /// <param name="opacityValueInPercents">Уровень видимости в процентах</param>
    public void SetOpacityValue(float opacityValueInPercents)
    {
        Color newColor = Material.color;
        newColor.a = opacityValueInPercents * 0.01f * MaxAlphaValue;
        Material.color = newColor;
    }

    /// <summary>
    /// Установить режим прозрачного материала
    /// </summary>
    public void SetFadeMode()
    {
        Material.ToFadeMode();
    }

    /// <summary>
    /// Установить режим непрозрачного материала
    /// </summary>
    public void SetOpaqueMode()
    {
        Material.ToOpaqueMode();
    }
}
