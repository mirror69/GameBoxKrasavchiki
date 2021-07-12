using System;
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
    private Vector3 targetBulletPoint;
    [SerializeField] float playerSpeed;
    [SerializeField] float rotationSpeed;
    [SerializeField] Transform pistolPosition;
    public Transform testPoint;

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

        // ������������ �������� � ����
        playerForce = (playerSpeed + speedToForceConvertShift) * playerRigidbody.mass * playerRigidbody.drag;
    }

    /// <summary>
    /// �������� ������ �� ���� X, Z
    /// </summary>
    /// <param name="movementVector">������� ����������� ��������</param>
    public void MovePlayer(Vector3 movementVector)
    {
        playerRigidbody.AddForce(movementVector.normalized * playerForce);
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

        /////
        ///
        testPoint.position = targetBulletPoint;
        ///
        /////

        if (playerPlane.Raycast(ray, out hitDistance))
        {
            targetBulletPoint = ray.GetPoint(hitDistance);

            targetRotation = Quaternion.LookRotation(targetBulletPoint - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
    }

    

    /// <summary>
    /// �������� ������� ����������� ��������: ������ ��� �����
    /// </summary>
    /// <returns>1 - ������� �� ������� �������, -1 - ������, 0 - �������� �� ���������</returns>
    public int GetTurnDirection()
    {
        const float minRotationDifferenceForTurn = 1;

        // ���� ������� �������� �������� ��������� � �������, ����������, � ����� ����������� �� ����� ��������������
        float angle1 = transform.rotation.eulerAngles.y;
        float angle2 = targetRotation.eulerAngles.y;
        float difference = angle2 - angle1;

        float differenceAbs = Mathf.Abs(difference);
        // �� ����� ������������ �������, ���� ������� ����� ������ ����� ����.
        if (differenceAbs <= minRotationDifferenceForTurn)
        {
            return 0;
        }

        // ���� ������� �������������, ������ ������� ������� ������ ��������, � �� �������������� �� ������� �������,
        // � ��������� ������ - ������ �������.
        // ������, ���� ������� ����� ������ ������ 180, �� ����� ������������� 
        if (differenceAbs > 180)
        {
            difference += difference > 0 ? -360 : 360;
        }

        return difference > 0 ? 1 : -1;
    }

    public void PlayerShoot(float buttonPressedTime)
    {
        shooting.Shoot(pistolPosition.transform.position, 
                        transform.rotation, 
                        playerRigidbody.velocity, 
                        targetBulletPoint,
                        DamageMultiplier(buttonPressedTime));
    }

    private int DamageMultiplier(float buttonPressedTime)
    {
        if (buttonPressedTime < 1) return 1;
        else return (int)Math.Floor(buttonPressedTime);
    }
}
