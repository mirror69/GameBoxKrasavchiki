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
    private const float MinOpacityValue = 0;
    /// <summary>
    /// Максимальный уровень видимости объектов
    /// </summary>
    private const float MaxOpacityValue = 100;
    /// <summary>
    /// Шаг изменения прозрачности
    /// </summary>
    private const float OpacityChangeStep = 0.5f;
    /// <summary>
    /// Минимальное смешение персонажа для того, чтобы начала меняться прозрачность
    /// </summary>
    private const float MinPositionDifferenceToChangeOpacity = 1f;

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

    /// <summary>
    /// Объекты игры, способные менять уровень своей видимости
    /// </summary>
    private OpacityChangingObject[] opacityChangingObjects = null;
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
    private float accumulatedPositionDifference = 0;
    private void Awake()
    {
        opacityChangingObjects = FindObjectsOfType<OpacityChangingObject>();
        opacityIncreaseVector = opacityIncreaseDirectionTransform.forward;
        // Выключаем объект, указывающий вектор увеличения видимости объектов, т.к. в игре он не нужен
        opacityIncreaseDirectionTransform.gameObject.SetActive(false);

        currentPositionAlongIncreaseVector = GetCurrentPositionAlongIncreaseVector();
        currentOpacityValue = MaxOpacityValue;
        lastAppliedOpacityValue = currentOpacityValue;
        accumulatedPositionDifference = 0;
    }

    private void Update()
    {
        float newPositionAlongIncreaseVector = GetCurrentPositionAlongIncreaseVector();
        float positionDifference = newPositionAlongIncreaseVector - currentPositionAlongIncreaseVector;

        // Меняем прозрачность только в том случае, если персонаж достаточно сдвинулся со своего места
        accumulatedPositionDifference += positionDifference;
        if (Mathf.Abs(accumulatedPositionDifference) < MinPositionDifferenceToChangeOpacity)
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
            RefreshVisibilityValueForObjects();
            lastAppliedOpacityValue = currentOpacityValue;
        }
        
        currentPositionAlongIncreaseVector = newPositionAlongIncreaseVector;
    }

    /// <summary>
    /// Обновить уровень видимости игровых объектов
    /// </summary>
    private void RefreshVisibilityValueForObjects()
    {
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
