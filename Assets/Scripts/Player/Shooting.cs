using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �������� ������ ������: ������� ���� ���� ��������� ����������� �� ��������� ������� �
/// � ������ Shoot ���������� ��������� � ������� ��������� ����.
/// </summary>
public class Shooting : MonoBehaviour
{
    [SerializeField] private int poolCount = 3; //������ ���� ����    
    [SerializeField] private Bullet bulletPrefab; //����� ��������� ��������� ��� ���� �� ����� ����������
    [SerializeField] private Transform bulletContainer;
    private BulletPool bulletPool;

    private void Start()
    {
        this.bulletPool = new BulletPool(bulletPrefab, poolCount, bulletContainer);
    }

    public void Shoot(Vector3 startDirection, Quaternion startRotation, Vector3 startVelocity)
    {
        bulletPool.EmitBullet(startDirection, startRotation, startVelocity);
    }

}
