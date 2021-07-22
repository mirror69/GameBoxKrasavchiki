using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedObstacle : MonoBehaviour, IObstacle
{
    const float collidersPutDownShiftForTransparency = 0.8f;
    
    private Animation obstacleAnimation;
    /// <summary>
    /// Коллайдеры для включения/отключения в зависимости от уровня видимости
    /// </summary>
    private Collider[] opaqueColliders;
    private Collider[] transparentColliders;
    private OpacityChangingParameters opacityChangingParameters;

    private float collidersShiftForTransparency;

    private float currentOpacityValue;
    public float OpacityValue => currentOpacityValue;

    public void Initialize(OpacityChangingParameters opacityChangingParameters)
    {
        this.opacityChangingParameters = opacityChangingParameters;
    }

    public bool IsPassable()
    {
        return currentOpacityValue <= 0;
    }

    /// <summary>
    /// Установить уровень видимости объекта
    /// </summary>
    /// <param name="valueInPercents">Уровень видимости в процентах</param>
    public void SetOpacityValue(float opacityValueInPercents)
    {
        obstacleAnimation[obstacleAnimation.clip.name].time = 
            (1 - opacityValueInPercents * 0.01f) * obstacleAnimation.clip.length;

        SetCollidersState(opacityValueInPercents);

        currentOpacityValue = opacityValueInPercents;
    }

    private void Awake()
    {
        opaqueColliders = gameObject.GetComponentsInChildren<Collider>();

        CreateTransparentColliders();
    }

    private void Start()
    {
        obstacleAnimation = GetComponentInChildren<Animation>();
        obstacleAnimation[obstacleAnimation.clip.name].time = 0;
        obstacleAnimation[obstacleAnimation.clip.name].speed = 0;
        obstacleAnimation.Play();

        currentOpacityValue = OpacityController.MaxOpacityValue;
    }

    private void CreateTransparentColliders()
    {
        // Найдём наибольшую и наименьшую точки коллайдеров. 
        // Коллайдеры, через которые можно видеть, будут точно такими же, как и 
        // исходные, но опущенные вниз так, что персонажи будут возвышаться над ними.
        float topY = float.MinValue;
        float bottomY = float.MaxValue;
        foreach (var item in opaqueColliders)
        {
            float topColliderY = item.bounds.center.y + item.bounds.extents.y;
            float bottomColliderY = item.bounds.center.y - item.bounds.extents.y;
            if (topColliderY > topY)
            {
                topY = topColliderY;
            }
            if (bottomColliderY < bottomY)
            {
                bottomY = bottomColliderY;
            }
        }

        collidersShiftForTransparency = (topY - bottomY) * collidersPutDownShiftForTransparency;

        transparentColliders = new Collider[opaqueColliders.Length];

        // Скопируем каждый коллайдер на новый объект
        for (int i = 0; i < opaqueColliders.Length; i++)
        {
            Collider collider = opaqueColliders[i];
            GameObject obj = new GameObject($"{collider.name}_TransparentCollider");

            obj.transform.position = collider.transform.position - collidersShiftForTransparency * Vector3.up;
            obj.transform.rotation = collider.transform.rotation;
            obj.transform.localScale = collider.transform.lossyScale;
            obj.transform.SetParent(transform);

            if (collider.GetType() == typeof(BoxCollider))
            {
                collider = obj.AddComponent(collider as BoxCollider);
            }
            else if (collider.GetType() == typeof(SphereCollider))
            {
                collider = obj.AddComponent(collider as SphereCollider);
            }
            else if (collider.GetType() == typeof(CapsuleCollider))
            {
                collider = obj.AddComponent(collider as CapsuleCollider);
            }
            else if (collider.GetType() == typeof(MeshCollider))
            {
                collider = obj.AddComponent(collider as MeshCollider);
            }
            else if (collider.GetType() == typeof(WheelCollider))
            {
                collider = obj.AddComponent(collider as WheelCollider);
            }

            transparentColliders[i] = collider;
        }
    }

    /// <summary>
    /// Включить/выключить коллайдеры
    /// </summary>
    /// <param name="colliders">Коллайдеры</param>
    /// <param name="enable">true - включить коллайдеры, false - выключить коллайдеры</param>
    private void SetEnabledColliders(Collider[] colliders, bool enabled)
    {
        foreach (var collider in colliders)
        {
            //collider.enabled = enabled;
            collider.isTrigger = !enabled;
        }        
    }

    /// <summary>
    /// Установить состояние коллайдеров в зависимости от значения видимости
    /// </summary>
    /// <param name="opacityValueInPercents"></param>
    private void SetCollidersState(float opacityValueInPercents)
    {
        SetEnabledColliders(opaqueColliders, 
            opacityValueInPercents >= opacityChangingParameters.FullOpaqueValue);

        SetEnabledColliders(transparentColliders, opacityValueInPercents > 0);
    }
}
