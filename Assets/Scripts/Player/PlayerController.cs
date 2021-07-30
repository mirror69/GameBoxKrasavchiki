using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    private const float speedToForceConvertShift = 0.5f;

    private Collider playerCollider;
    private Rigidbody playerRigidbody;
    private Quaternion targetRotation;
    private Camera mainCamera;
    private float playerForce;
    private Vector3 targetBulletPoint;
    [SerializeField] float playerSpeed;
    [SerializeField] float rotationSpeed;
    [SerializeField] Transform pistolPosition;

    public float PlayerSpeed => playerSpeed;
    public Vector3 CurrentVelocity => playerRigidbody.velocity;
    public Vector3 Center => playerCollider.bounds.center;
    public Bounds Bounds => playerCollider.bounds;
    public Vector3 HeadPosition => Center + (Bounds.extents.y - 0.2f) * transform.up;
    public Collider Collider => playerCollider;

    private void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        mainCamera = Camera.main;
        playerCollider = GetComponent<Collider>();

        // Конвертируем скорость в силу
        playerForce = (playerSpeed + speedToForceConvertShift) * playerRigidbody.mass * playerRigidbody.drag;
    }

    /// <summary>
    /// Движение плеера по осям X, Z
    /// </summary>
    /// <param name="movementVector">Целевое направление движения</param>
    public void MovePlayer(Vector3 movementVector)
    {
        playerRigidbody.AddForce(movementVector.normalized * playerForce);
    }

    /// <summary>
    /// Повернуть игрока лицом к заданной точке
    /// </summary>
    /// <param name="lookAtPoint">Точка, на которую должен повернуться игрок</param>
    public void RotatePlayer(Vector3 lookAtPoint)
    {
        lookAtPoint.y = transform.position.y;
        targetRotation = Quaternion.LookRotation(lookAtPoint - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
    }

    /// <summary>
    /// Получить текущее направление поворота: вправо или влево
    /// </summary>
    /// <returns>1 - поворот по часовой стрелке, -1 - против, 0 - персонаж не вращается</returns>
    public int GetTurnDirection()
    {
        const float minRotationDifferenceForTurn = 1;

        // Зная текущее значение поворота персонажа и целевое, определяем, в каком направлении он будет поворачиваться
        float angle1 = transform.rotation.eulerAngles.y;
        float angle2 = targetRotation.eulerAngles.y;
        float difference = angle2 - angle1;

        float differenceAbs = Mathf.Abs(difference);
        // Не будем обрабатывать поворот, если разница между углами очень мала.
        if (differenceAbs <= minRotationDifferenceForTurn)
        {
            return 0;
        }

        // Если разница положительная, значит целевой поворот больше текущего, и мы поворачиваемся по часовой стрелке,
        // в противном случае - против часовой.
        // Однако, если разница между углами больше 180, то нужна корректировка 
        if (differenceAbs > 180)
        {
            difference += difference > 0 ? -360 : 360;
        }

        return difference > 0 ? 1 : -1;
    }

}
