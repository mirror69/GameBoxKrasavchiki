using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Shooting))]
public class PlayerController : MonoBehaviour
{
    private const float speedToForceConvertShift = 0.5f;

    private Collider playerCollider;
    private Rigidbody playerRigidbody;
    private Quaternion targetRotation;
    private Transform cameraTransform;
    private float playerForce;
    private Shooting shooting;
    [SerializeField] float playerSpeed;
    [SerializeField] float rotationSpeed;

    public float PlayerSpeed => playerSpeed;
    public Vector3 CurrentVelocity => playerRigidbody.velocity;
    public Vector3 Center => playerCollider.bounds.center;
    public Bounds Bounds => playerCollider.bounds;
    public Vector3 HeadPosition => Center + (Bounds.extents.y - 0.2f) * transform.up;
    public Collider Collider => playerCollider;

    private void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        cameraTransform = Camera.main.transform;
        shooting = GetComponent<Shooting>();
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
    /// Вращение плеера вслед за передаваемой позицией
    /// </summary>
    /// <param name="mousePosition">Вектор, в сторону которого поворачивает плеер</param>
    public void RotatePlayer(Vector3 mousePosition)
    {
        Plane playerPlane = new Plane(Vector3.up, transform.position);

        Ray ray = Camera.main.ScreenPointToRay(mousePosition);

        float hitDistance;

        if (playerPlane.Raycast(ray, out hitDistance))
        {
            Vector3 targetPoint = ray.GetPoint(hitDistance);

            targetRotation = Quaternion.LookRotation(targetPoint - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
    }

    /// <summary>
    /// Конвертор движения игрока с учетом поворота плеера, предоставляет данные для аниматора относительно текущего поворота плеера, а не глобальных координат
    /// </summary>
    /// <param name="horizontalInput"></param>
    /// <param name="verticalInput"></param>
    /// <returns>Vector2, где первый член это боковое движение, а второй - движение вперед/назад</returns>
    //public Vector2 AnimatorSourceData(float horizontalInput, float verticalInput)
    //{
    //    Vector3 move;
    //    if (cameraTransform != null)
    //    {
    //        Vector3 cameraForward = Vector3.Scale(cameraTransform.up, new Vector3(1, 0, 1)).normalized;
    //        move = verticalInput * cameraForward + horizontalInput * cameraTransform.right;
    //    }
    //    else move = verticalInput * Vector3.forward + horizontalInput * Vector3.right;

    //    if (move.magnitude > 1) move.Normalize();

    //    Vector3 localMove = transform.InverseTransformDirection(move);

    //    float turnValue = localMove.x;
    //    float forwardValue = localMove.z;

    //    return new Vector2(turnValue, forwardValue);
    //}

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

    public void PlayerShoot()
    {
        shooting.Shoot(transform.position, transform.rotation, playerRigidbody.velocity);
    }
}
