using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Контроллер для управления уровнем видимости всех объектов на сцене, способных менять свою прозрачность
/// </summary>
public class ObjectsVisibilityController : MonoBehaviour
{
    /// <summary>
    /// Минимальный уровень видимости объектов
    /// </summary>    
    private const float MinVisibilityValue = 0;
    /// <summary>
    /// Максимальный уровень видимости объектов
    /// </summary>
    private const float MaxVisibilityValue = 100;
    /// <summary>
    /// Шаг изменения прозрачности
    /// </summary>
    private const float VisibilityChangeStep = 0.5f;

    [Tooltip("Объект, указывающий направление увеличения уровня видимости")]
    [SerializeField]
    private Transform visibilityIncreaseDirectionTransform = null;
    [Tooltip("Объект-инициатор изменения уровня видимости")]
    [SerializeField]
    private Transform visibilityChangeInitiator = null;   
    [Tooltip("Скорость изменения видимости объектов (в процентах на метр движения)")]
    [SerializeField]
    private float visibilityChangeSpeed = 10;

    /// <summary>
    /// Объекты игры, способные менять уровень своей видимости
    /// </summary>
    private VisibilityChangingObject[] transparencyChangingObjects = null;
    /// <summary>
    /// Текущий уровень видимости
    /// </summary>
    private float currentVisibilityValue = MaxVisibilityValue;

    /// <summary>
    /// Вектор увеличения уровня видимости
    /// </summary>
    private Vector3 visibilityIncreaseVector = Vector3.zero;
    /// <summary>
    /// Текущая позиция объекта-инициатора вдоль вектора увеличения уровня прозрачности
    /// </summary>
    private float currentPositionAlongIncreaseVector = 0;
    /// <summary>
    /// Накопленное значение разницы текущей позиции объекта-инициатора и прошлой позиции, при которой 
    /// уровень видимости был изменен в последний раз
    /// </summary>
    private float accumulatedPositionDifference = 0;

    private void Awake()
    {
        transparencyChangingObjects = FindObjectsOfType<VisibilityChangingObject>();
        visibilityIncreaseVector = visibilityIncreaseDirectionTransform.forward;
        // Выключаем объект, указывающий вектор увеличения видимости объектов, т.к. в игре он не нужен
        visibilityIncreaseDirectionTransform.gameObject.SetActive(false);
    }

    private void Start()
    {
        currentPositionAlongIncreaseVector = GetCurrentPositionAlongIncreaseVector();
    }

    private void Update()
    {
        float newPositionAlongIncreaseVector = GetCurrentPositionAlongIncreaseVector();
        float positionDifference = newPositionAlongIncreaseVector - currentPositionAlongIncreaseVector;
        accumulatedPositionDifference += positionDifference;

        // При возрастании координаты, прозрачность увеличивается, и наоборот.
        float newOpacityValue = currentVisibilityValue + accumulatedPositionDifference * visibilityChangeSpeed;

        // Корректируем, если вышли за допустимые пределы прозрачности.
        // Также в этом случае сбрасываем накопленную разницу координат, чтобы при движении в другом направлении
        // сразу начать менять прозрачность
        if (newOpacityValue > MaxVisibilityValue)
        {
            newOpacityValue = MaxVisibilityValue;
            accumulatedPositionDifference = 0;
        }
        else if (newOpacityValue < MinVisibilityValue)
        {
            newOpacityValue = MinVisibilityValue;
            accumulatedPositionDifference = 0;
        }

        if (Mathf.Abs(currentVisibilityValue - newOpacityValue) >= VisibilityChangeStep)
        {
            currentVisibilityValue = Mathf.Round(newOpacityValue / VisibilityChangeStep) * VisibilityChangeStep;
            RefreshVisibilityValueForObjects();
            // Сбрасываем накопленную разницу координат, чтобы начать копить по-новой до следующего шага изменения
            accumulatedPositionDifference = 0;
        }

        currentPositionAlongIncreaseVector = newPositionAlongIncreaseVector;
    }

    /// <summary>
    /// Обновить уровень видимости игровых объектов
    /// </summary>
    private void RefreshVisibilityValueForObjects()
    {
        foreach (var item in transparencyChangingObjects)
        {
            item.SetVisibilityValue(currentVisibilityValue);
        }
    }

    /// <summary>
    /// Получить текущую позицию объекта-инициатора вдоль вектора увеличения уровня видимости
    /// </summary>
    /// <returns></returns>
    private float GetCurrentPositionAlongIncreaseVector()
    {
        return Vector3.Dot(visibilityChangeInitiator.transform.position, visibilityIncreaseVector);
    }
}
