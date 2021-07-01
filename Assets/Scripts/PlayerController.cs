using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{    
    private Rigidbody playerRigidbody;
    private Quaternion targetRotation;
    private Transform cameraTransform;
    [SerializeField] float playerSpeed;

    private void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        cameraTransform = Camera.main.transform;
    }

    /// <summary>
    /// Движение плеера по осям X, Z
    /// </summary>
    /// <param name="movementVector">Целевое направление движения</param>
    public void MovePlayer(Vector3 movementVector)
    {
        playerRigidbody.AddForce(movementVector * playerSpeed / Time.deltaTime);
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

            //targetPointDisplay.position = targetPoint; //курсор в игре

            targetRotation = Quaternion.LookRotation(targetPoint - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 7f * Time.deltaTime);
        }
    }

    /// <summary>
    /// Конвертор движения игрока с учетом поворота плеера, предоставляет данные для аниматора относительно текущего поворота плеера, а не глобальных координат
    /// </summary>
    /// <param name="horizontalInput"></param>
    /// <param name="verticalInput"></param>
    /// <returns>Vector2, где первый член это боковое движение, а второй - движение вперед/назад</returns>
    public Vector2 AnimatorSourceData(float horizontalInput, float verticalInput)
    {
        Vector3 move;
        if (cameraTransform != null)
        {
            Vector3 cameraForward = Vector3.Scale(cameraTransform.up, new Vector3(1, 0, 1)).normalized;
            move = verticalInput * cameraForward + horizontalInput * cameraTransform.right;
        }
        else move = verticalInput * Vector3.forward + horizontalInput * Vector3.right;

        if (move.magnitude > 1) move.Normalize();

        Vector3 localMove = transform.InverseTransformDirection(move);

        float turnValue = localMove.x;
        float forwardValue = localMove.z;

        return new Vector2(turnValue, forwardValue);
    }
}
