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
    [SerializeField] private Transform bulletsContainer;
    [SerializeField] private bool isInheritingSourceVelocity = false;
    private BulletPool bulletPool;


    private void Start()
    {
        this.bulletPool = new BulletPool(bulletPrefab, poolCount, bulletsContainer);
    }

    public void Shoot(Vector3 startDirection, Quaternion startRotation, Vector3 startVelocity, Vector3 targetBulletPoint, int damageMultiplier)
    {
        if (!isInheritingSourceVelocity) startVelocity = Vector3.zero;

        bulletPool.EmitBullet(startDirection, startRotation, 
            startVelocity, targetBulletPoint, damageMultiplier);
    }

}
