using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedObstacle : MonoBehaviour
{
    private Animation obstacleAnimation;
    /// <summary>
    /// Коллайдеры для включения/отключения в зависимости от уровня видимости
    /// </summary>
    private Collider[] colliders;
    /// <summary>
    /// Параметры изменения видимости
    /// </summary>
    private OpacityChangingStrategy opacityChangingStrategy;

    public void Initialize(OpacityChangingStrategy opacityChangingStrategy)
    {
        this.opacityChangingStrategy = opacityChangingStrategy;
    }

    /// <summary>
    /// Установить уровень видимости объекта
    /// </summary>
    /// <param name="valueInPercents">Уровень видимости в процентах</param>
    public void SetOpacityValue(float opacityValueInPercents)
    {
        obstacleAnimation[obstacleAnimation.clip.name].time = 
            (1 - opacityValueInPercents * 0.01f) * obstacleAnimation.clip.length;
        //obstacleAnimation.Play();
        SetEnableColliders(opacityValueInPercents > 0);
    }

    private void Awake()
    {
        colliders = gameObject.GetComponentsInChildren<Collider>();
    }

    private void Start()
    {
        obstacleAnimation = GetComponentInChildren<Animation>();
        obstacleAnimation[obstacleAnimation.clip.name].time = 0;
        obstacleAnimation[obstacleAnimation.clip.name].speed = 0;
        obstacleAnimation.Play();
    }
    /// <summary>
    /// Включить/выключить коллайдеры
    /// </summary>
    /// <param name="enable">true - включить коллайдеры, false - выключить коллайдеры</param>
    private void SetEnableColliders(bool enable)
    {
        foreach (var collider in colliders)
        {
            collider.enabled = enable;
        }
    }
}
