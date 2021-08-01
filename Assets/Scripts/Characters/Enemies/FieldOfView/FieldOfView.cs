using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ѕоле зрени€ игровых персонажей
/// </summary>
public class FieldOfView : MonoBehaviour
{
    /// <summary>
    /// —осто€ние обнаружени€ цели
    /// </summary>
    public enum TargetDetectingState
    {        
        Detected,
        NotDetected
    }

    /// <summary>
    /// «адержка проверки в секундах (чтобы не каждый кадр провер€ть) 
    /// </summary>
    const float checkTargetInFovDelay = 0.1f;

    [SerializeField]
    private Transform pointOfSight = null;
    [SerializeField]
    private float angle = 0;
    [SerializeField]
    private float distance = 0;
    [SerializeField]
    private float idleDetectionDelay = 1.5f;
    [SerializeField]
    private float alertDetectionDelay = 0;
    [SerializeField]
    private PlayerController target = null;

    private float currentDetectionDelay = 0;
    private float distanceSqr = 0;
    private float halfAngle = 0;
    private TargetDetectingState targetDetectingState = TargetDetectingState.NotDetected;

    public float Distance => distance;
    public float Angle => angle;
    public Transform PointOfSight => pointOfSight;
    public TargetDetectingState DetectingState => targetDetectingState;
    public Vector3 TargetPosition => target.HeadPosition;

    private event Action<Transform> TargetDetected;
    private event Action TargetLost;

    public void RegisterTargetDetectedListener(Action<Transform> listener)
    {
        TargetDetected += listener;
    }
    public void UnregisterTargetDetectedListener(Action<Transform> listener)
    {
        TargetDetected -= listener;
    }
    public void RegisterTargetLostListener(Action listener)
    {
        TargetLost += listener;
    }
    public void UnregisterTargetLostListener(Action listener)
    {
        TargetLost -= listener;
    }

    public void SetIdleDetectionDelay()
    {
        currentDetectionDelay = idleDetectionDelay;
    }
    public void SetAlertDetectionDelay()
    {
        currentDetectionDelay = alertDetectionDelay;
    }

    public void SetEnabled(bool enabled)
    {
        if (FieldOfViewManager.Instance == null)
        {
            return;
        }

        if (enabled)
        {
            FieldOfViewManager.Instance.AddFieldOfView(this);
        }
        else
        {
            FieldOfViewManager.Instance.RemoveFieldOfView(this);
        }
    }

    private void Start()
    {
        SetEnabled(true);
        StartSearching();
    }

    private void OnDestroy()
    {
        SetEnabled(false);
    }

    /// <summary>
    /// ѕоиск цели
    /// </summary>
    /// <returns></returns>
    private IEnumerator TargetSearching()
    {
        while (true)
        {
            bool targetInFOV = CheckTargetInFov();
            if (targetInFOV)
            {
                if (targetDetectingState == TargetDetectingState.NotDetected)
                {
                    yield return new WaitForSeconds(currentDetectionDelay);
                    targetInFOV = CheckTargetInFov();
                    if (targetInFOV)
                    {
                        SetTargetDetectingState(TargetDetectingState.Detected);
                    }
                }
            }
            else if (targetDetectingState == TargetDetectingState.Detected)
            {
                SetTargetDetectingState(TargetDetectingState.NotDetected);
            }

            yield return new WaitForSeconds(checkTargetInFovDelay);
        }
    }

    /// <summary>
    /// Ќачать поиск цели
    /// </summary>
    private void StartSearching()
    {
        StartCoroutine(TargetSearching());
    }

    /// <summary>
    /// ”становить состо€ние обнаружени€ цели
    /// </summary>
    /// <param name="state"></param>
    private void SetTargetDetectingState(TargetDetectingState state)
    {
        targetDetectingState = state;
        if (targetDetectingState == TargetDetectingState.Detected)
        {
            TargetDetected?.Invoke(target.transform);
        }
        else
        {
            TargetLost?.Invoke();
        }
    }

    /// <summary>
    /// ѕроверить, находитс€ ли цель в пределах видимости
    /// </summary>
    /// <returns></returns>
    private bool CheckTargetInFov()
    {
        // «начени€ задаютс€ здесь дл€ лЄгкой настройки из инспектора в PlayMode 
        // TODO: вынести в Start дл€ оптимизации
        distanceSqr = distance * distance;
        halfAngle = angle / 2;
        pointOfSight.forward = new Vector3(pointOfSight.forward.x, 0, pointOfSight.forward.z).normalized;

        // ќриентируемс€ на голову персонажа. Ѕудем считать, что если видно голову, то персонаж виден.
        Vector3 targetPosition = target.HeadPosition;

        Vector3 pointOfSightPosition = pointOfSight.position;
        pointOfSightPosition.y = targetPosition.y;

        Vector3 vectorToTarget = targetPosition - pointOfSightPosition;
        float distanceToTargetSqr = Vector3.SqrMagnitude(vectorToTarget);

        // ѕроверим, не находитс€ ли цель за пределами дальности видимости
        if (distanceToTargetSqr > distanceSqr)
        {
            return false;
        }

        // ѕроверим, соответствует ли положение цели направлению взгл€да
        if (Vector3.Angle(pointOfSight.forward, vectorToTarget) > halfAngle)
        {
            return false;
        }

        // ≈сли столкнулись с каким-либо преп€тствием, то проверим: если оно дальше, чем цель,
        // то цель в зоне видимости
        RaycastHit hitInfo = FieldOfViewManager.GetFovRaycastHit(pointOfSightPosition, vectorToTarget, distance);
        if (hitInfo.collider != null)
        {
            return hitInfo.distance * hitInfo.distance > distanceToTargetSqr;
        }

        // ≈сли не столкнулись c преп€тствием, то цель в пределах видимости
        return true;
    }
}
