using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Контроллер для управления уровнем видимости всех объектов на сцене, способных менять свою прозрачность
/// </summary>
public class OpacityController : MonoBehaviour
{
    /// <summary>
    /// Минимальный уровень видимости объектов
    /// </summary>    
    public const float MinOpacityValue = 0;
    /// <summary>
    /// Максимальный уровень видимости объектов
    /// </summary>
    public const float MaxOpacityValue = 100;
    /// <summary>
    /// Шаг изменения прозрачности
    /// </summary>
    private const float OpacityChangeStep = 0.5f;
    /// <summary>
    /// Минимальное смешение персонажа для того, чтобы начала меняться прозрачность
    /// </summary>
    private const float MinPositionDifferenceToChangeOpacity = 0.1f;

    [Tooltip("Объект, указывающий направление увеличения уровня видимости")]
    [SerializeField]
    private Transform opacityIncreaseDirectionTransform = null;
    
    [Tooltip("Объект-инициатор изменения уровня видимости")]
    [SerializeField]
    private Transform opacityChangeInitiator = null;   
    
    [Tooltip("Скорость изменения видимости объектов (в процентах на метр движения)")]
    [SerializeField]  
    private float opacityChangeSpeed = 20;
    
    [Tooltip("Значение видимости, при которой объект появляется полностью")]
    [SerializeField]    
    [Range(MinOpacityValue, MaxOpacityValue)]
    private float opacityValueForFullFadeIn = 70;
    
    [Tooltip("Значение видимости, при которой объект полностью исчезает")]
    [SerializeField]
    [Range(MinOpacityValue, MaxOpacityValue)]
    private float opacityValueForFullFadeOut = 20;

    [Tooltip("Стратегия изменения видимости")]
    [SerializeField]
    private OpacityChangingStrategy opacityChangingStrategy = null;

    /// <summary>
    /// Объекты игры, способные менять уровень своей видимости
    /// </summary>
    private AnimatedObstacle[] opacityChangingObjects = null;
    /// <summary>
    /// Текущий уровень видимости
    /// </summary>
    private float currentOpacityValue = 0;

    /// <summary>
    /// Вектор увеличения уровня видимости
    /// </summary>
    private Vector3 opacityIncreaseVector = Vector3.zero;
    /// <summary>
    /// Текущая позиция объекта-инициатора вдоль вектора увеличения уровня прозрачности
    /// </summary>
    private float currentPositionAlongIncreaseVector = 0;
    /// <summary>
    /// Последнее применнённое значение видимости для объектов
    /// </summary>
    private float lastAppliedOpacityValue = 0;
    /// <summary>
    /// Скорость изменения прозрачности, не зависящая от значений opacityValueForFullFadeIn и opacityValueForFullFadeOut
    /// </summary>
    private float opacityFullFadeChangeSpeed = 0;
    /// <summary>
    /// Накопленное значение изменения позиции
    /// </summary>
    private float accumulatedPositionDifference = 0;

    public float OpacityValue => lastAppliedOpacityValue;

    /// <summary>
    /// Событие, вызываемое при изменении значения видимости
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

        opacityIncreaseVector = opacityIncreaseDirectionTransform.forward;
        // Выключаем объект, указывающий вектор увеличения видимости объектов, т.к. в игре он не нужен
        opacityIncreaseDirectionTransform.gameObject.SetActive(false);

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

        // Если достигли максимального значения видимости и продолжаем двигаться в направлении увеличения видимости
        // (и наоборот), то сбрасываем накопленное значение разницы позиций, чтобы начать менять
        // значение видимости именно в момент начала движения в противоположном направлении
        if (accumulatedPositionDifference > 0 && lastAppliedOpacityValue >= MaxOpacityValue
            || accumulatedPositionDifference < 0 && lastAppliedOpacityValue <= MinOpacityValue)
        {
            accumulatedPositionDifference = 0;            
            return;
        }
    
        // Если текущее значение видимости = минимальному или максимальному,
        // видимость начинает меняться не сразу, а при определенном накопленном количестве движения
        if ((lastAppliedOpacityValue >= MaxOpacityValue
            || lastAppliedOpacityValue <= MinOpacityValue)
            && Mathf.Abs(accumulatedPositionDifference) < MinPositionDifferenceToChangeOpacity)
        {
            return;
        }

        accumulatedPositionDifference = 0;

        // Преобразуем скорость изменения видимости так, чтобы она не зависела от значений opacityValueForFullFadeIn
        // и opacityValueForFullFadeOut
        // Рассчитывается здесь, чтобы можно было регулировать параметры в PlayMode
        // TODO. Когда будет отлажено, перенести в Awake
        opacityFullFadeChangeSpeed = opacityChangeSpeed *
            (opacityValueForFullFadeIn - opacityValueForFullFadeOut) / (MaxOpacityValue - MinOpacityValue);

        // При возрастании координаты, прозрачность увеличивается, и наоборот.
        float newOpacityValue = currentOpacityValue + positionDifference * opacityFullFadeChangeSpeed;

        // Уровень видимости будет меняться в интервале [opacityValueForFullFadeIn, opacityValueForFullFadeOut]
        // При достижении границ интервала будем устанавливать полную видимость или полную невидимость 
        // Если начинаем движение в противоположном направлении, сразу начинаем менять прозрачность в этом интервале
        if (newOpacityValue > currentOpacityValue)
        {
            if (newOpacityValue < opacityValueForFullFadeOut)
            {
                newOpacityValue = opacityValueForFullFadeOut;
            }
            else if (newOpacityValue  > opacityValueForFullFadeIn)
            {
                newOpacityValue = MaxOpacityValue;
            }
        }
        else if (newOpacityValue < currentOpacityValue)
        {
            if (newOpacityValue > opacityValueForFullFadeIn)
            {
                newOpacityValue = opacityValueForFullFadeIn;
            }
            else if (newOpacityValue < opacityValueForFullFadeOut)
            {
                newOpacityValue = MinOpacityValue;
            }
        }

        currentOpacityValue = newOpacityValue;
        // Для изменения прозрачности разница между текущим и новым уровнем прозрачности 
        // должна быть больше установленного шага
        if (Mathf.Abs(lastAppliedOpacityValue - currentOpacityValue) >= OpacityChangeStep)
        {
            currentOpacityValue = Mathf.Round(currentOpacityValue / OpacityChangeStep) * OpacityChangeStep;
            lastAppliedOpacityValue = currentOpacityValue;
            RefreshVisibilityValueForObjects();           
        }

        currentPositionAlongIncreaseVector = newPositionAlongIncreaseVector;
    }

    /// <summary>
    /// Обновить уровень видимости игровых объектов
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
    /// Получить текущую позицию объекта-инициатора вдоль вектора увеличения уровня видимости
    /// </summary>
    /// <returns></returns>
    private float GetCurrentPositionAlongIncreaseVector()
    {
        return Vector3.Dot(opacityChangeInitiator.transform.position, opacityIncreaseVector);
    }
}
