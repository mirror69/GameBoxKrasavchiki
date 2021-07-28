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

    public Bounds BulletBounds { get; private set; }
    public float DefaultDamageValue => bulletPrefab.DefaultDamageValue;

    private void Awake()
    {
        // ���� ��������� ��� ���� �� �����, �������� ��� � ����� ������ ��������
        if (bulletsContainer == null)
        {
            bulletsContainer = new GameObject($"{name}_bulletsContainer").transform;
        }
        
        // �������� ������ ���� �� ������� ������. ������� ������� ��������� ������, ����� ������, � ����������
        Bullet bullet = Instantiate(bulletPrefab, Vector3.zero, Quaternion.identity);
        Collider bulletCollider = bullet.GetComponent<Collider>();
        BulletBounds = bulletCollider.bounds;
        Destroy(bullet.gameObject);
    }
    private void Start()
    {        
        this.bulletPool = new BulletPool(bulletPrefab, poolCount, bulletsContainer);
    }

    public void Shoot(Vector3 startDirection, Quaternion startRotation, Vector3 startVelocity, Vector3 targetBulletPoint, float damageMultiplier)
    {
        if (!isInheritingSourceVelocity) startVelocity = Vector3.zero;

        bulletPool.EmitBullet(gameObject, startDirection, startRotation, 
            startVelocity, targetBulletPoint, damageMultiplier);
    }

}
