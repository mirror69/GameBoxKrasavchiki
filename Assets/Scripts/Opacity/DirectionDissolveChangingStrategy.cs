using UnityEngine;

/// <summary>
/// Стратегия изменения видимости путем применения направленного эффекта Dissolve
/// </summary>
public class DirectionDissolveChangingStrategy: OpacityChangingStrategy
{
    private const float HighestDissolveOffset = float.MaxValue;

    [Tooltip("Минимальное значение смещения эффекта растворения")]
    [SerializeField]
    private float minDirectionDissolveOffset;
    
    [Tooltip("Максимальное значение смещения эффекта растворения")]
    [SerializeField]
    private float maxDirectionDissolveOffset;

    [Tooltip("Цвет границы эффекта растворения по умолчанию")]
    [SerializeField]
    [ColorUsageAttribute(true, true)]
    private Color defaultDissolveEdgeColor;

    [Tooltip("Цвет границы эффекта растворения для обозначения полностью невидимого объекта")]
    [SerializeField]
    [ColorUsageAttribute(true, true)]
    private Color fullDissolvedEdgeColor;

    private float currentDissolveOffset;

    public float MinDirectionDissolveOffset => minDirectionDissolveOffset;
    public float MaxDirectionDissolveOffset => maxDirectionDissolveOffset;

    public override void SetOpacity(Material material, float value)
    {
        material.SetOpacity(value);

        currentDissolveOffset = value >= 1 ? HighestDissolveOffset :
            minDirectionDissolveOffset + (maxDirectionDissolveOffset - minDirectionDissolveOffset) * value;

        Color edgeColor = currentDissolveOffset <= minDirectionDissolveOffset ?
            fullDissolvedEdgeColor : defaultDissolveEdgeColor;

        material.SetDirectionDissolve(currentDissolveOffset, edgeColor);
    }
}
