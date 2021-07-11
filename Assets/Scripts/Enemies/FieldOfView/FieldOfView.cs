using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Поле зрения игровых персонажей
/// </summary>
public class FieldOfView : MonoBehaviour
{
    public enum TargetDetectingState
    {        
        Detected,
        NotDetected
    }

    const float checkTargetInFovDelay = 0.1f;

    [SerializeField]
    private Transform pointOfSight = null;
    [SerializeField]
    private float angle = 0;
    [SerializeField]
    private float distance = 0;
    [SerializeField]
    private float detectionDelay = 0;
    [SerializeField]
    private PlayerController target = null;

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

    private void Start()
    {
        StartSearching();
    }

    private IEnumerator TargetSearching()
    {
        while (true)
        {
            bool targetInFOV = CheckTargetInFov();
            if (targetInFOV)
            {
                if (targetDetectingState == TargetDetectingState.NotDetected)
                {
                    yield return new WaitForSeconds(detectionDelay);
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

    private void StartSearching()
    {
        StartCoroutine(TargetSearching());
    }

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

    private bool CheckTargetInFov()
    {
        // Значения задаются здесь для лёгкой настройки из инспектора в PlayMode 
        // TODO: вынести в Start для оптимизации
        distanceSqr = distance * distance;
        halfAngle = angle / 2;
        pointOfSight.forward = new Vector3(pointOfSight.forward.x, 0, pointOfSight.forward.z).normalized;

        // Ориентируемся на голову персонажа. Будем считать, что если видно голову, то персонаж виден.
        Vector3 targetPosition = target.HeadPosition;

        Vector3 pointOfSightPosition = pointOfSight.position;
        pointOfSightPosition.y = targetPosition.y;

        Vector3 vectorToTarget = targetPosition - pointOfSightPosition;
        // Проверим, не находится ли цель за пределами дальности видимости
        if (Vector3.SqrMagnitude(vectorToTarget) > distanceSqr)
        {
            return false;
        }

        // Проверим, соответствует ли положение цели направлению взгляда
        if (Vector3.Angle(pointOfSight.forward, vectorToTarget) > halfAngle)
        {
            return false;
        }
        
        bool targetInFOV = false;
        if (Physics.Raycast(pointOfSightPosition, vectorToTarget, out RaycastHit hitInfo, distance))
        {
            if (hitInfo.collider == target.Collider)
            {
                targetInFOV = true;
            }
        }
        return targetInFOV;
    }
}
