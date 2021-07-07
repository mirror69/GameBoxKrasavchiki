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
    public OpacityChangingStrategy OpacityChangingParameters { get; private set; }

    public OpacityChangingMaterial(Material material, 
        OpacityChangingStrategy opacityChangingParameters)
    {
        Material = material;
        OpacityChangingParameters = opacityChangingParameters;
    }

    /// <summary>
    /// Задать уровень видимости материала
    /// </summary>
    /// <param name="opacityValueInPercents">Уровень видимости в процентах</param>
    public void SetOpacityValue(float opacityValueInPercents)
    {
        OpacityChangingParameters.SetOpacity(Material, opacityValueInPercents * 0.01f);
        //Material.SetOpacity(opacityValueInPercents * 0.01f);
    }

    /// <summary>
    /// Установить режим прозрачного материала
    /// </summary>
    public void SetFadeMode()
    {
        OpacityChangingParameters.SetFadeMode(Material);
    }

    /// <summary>
    /// Установить режим непрозрачного материала
    /// </summary>
    public void SetOpaqueMode()
    {
        OpacityChangingParameters.SetOpaqueMode(Material);
    }
}
