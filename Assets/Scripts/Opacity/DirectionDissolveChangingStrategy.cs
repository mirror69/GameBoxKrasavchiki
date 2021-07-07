using UnityEngine;

/// <summary>
/// Стратегия изменения видимости путем применения направленного эффекта Dissolve
/// </summary>
public class DirectionDissolveChangingStrategy: OpacityChangingStrategy
{
    [Tooltip("Минимальное значение смещения эффекта растворения")]
    [SerializeField]
    private float minDirectionDissolveOffset;
    [Tooltip("Максимальное значение смещения эффекта растворения")]
    [SerializeField]
    private float maxDirectionDissolveOffset;

    public float MinDirectionDissolveOffset => minDirectionDissolveOffset;
    public float MaxDirectionDissolveOffset => maxDirectionDissolveOffset;

    public override void SetOpacity(Material material, float value)
    {
        material.SetOpacity(value);
        material.SetDirectionDissolve(value, MinDirectionDissolveOffset, MaxDirectionDissolveOffset);
    }
}
