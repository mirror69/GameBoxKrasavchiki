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
    /// �������� ������ �� ���� X, Z
    /// </summary>
    /// <param name="movementVector">������� ����������� ��������</param>
    public void MovePlayer(Vector3 movementVector)
    {
        playerRigidbody.AddForce(movementVector * playerSpeed / Time.deltaTime);
    }

    /// <summary>
    /// �������� ������ ����� �� ������������ ��������
    /// </summary>
    /// <param name="mousePosition">������, � ������� �������� ������������ �����</param>
    public void RotatePlayer(Vector3 mousePosition)
    {
        Plane playerPlane = new Plane(Vector3.up, transform.position);

        Ray ray = Camera.main.ScreenPointToRay(mousePosition);

        float hitDistance;

        if (playerPlane.Raycast(ray, out hitDistance))
        {
            Vector3 targetPoint = ray.GetPoint(hitDistance);

            //targetPointDisplay.position = targetPoint; //������ � ����

            targetRotation = Quaternion.LookRotation(targetPoint - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 7f * Time.deltaTime);
        }
    }

    /// <summary>
    /// ��������� �������� ������ � ������ �������� ������, ������������� ������ ��� ��������� ������������ �������� �������� ������, � �� ���������� ���������
    /// </summary>
    /// <param name="horizontalInput"></param>
    /// <param name="verticalInput"></param>
    /// <returns>Vector2, ��� ������ ���� ��� ������� ��������, � ������ - �������� ������/�����</returns>
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
