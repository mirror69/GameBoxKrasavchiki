using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool
{
    [SerializeField] private Bullet bulletPrefab { get; }
    [SerializeField] private Transform bulletContainer { get; }

    private List<Bullet> pool;

    public BulletPool(Bullet BulletPrefab, int Count, Transform BulletContainer)
    {
        this.bulletPrefab = BulletPrefab;
        this.bulletContainer = BulletContainer;
        this.CreatePool(Count);
    }

    /// <summary>
    /// ��������� ���� �������� ������ ��������������� ��������
    /// </summary>
    /// <param name="count"></param>
    private void CreatePool(int count)
    {
        this.pool = new List<Bullet>();

        for (int i = 0; i < count; i++)
        {
            this.CreateObject();
        }
    }

    /// <summary>
    /// ������������� ������� ������� � ����������� �� ���������. ���� ����������������� ������� � ����� �����������, 
    /// �� ����� ��������� ����� �������� ������� � ��������� ����������
    /// </summary>
    /// <param name="startPosition"></param>
    /// <param name="startRotation"></param>
    /// <param name="startVelocity"></param>
    /// <param name="isActiveByDefault"></param>
    /// <returns></returns>
    private Bullet CreateObject(Vector3 startPosition = new Vector3(), 
                                Quaternion startRotation = new Quaternion(),
                                Vector3 startVelocity = new Vector3(),
                                bool isActiveByDefault = false)
    {        
        var createdObject = UnityEngine.Object.Instantiate(this.bulletPrefab, startPosition,
            startRotation, this.bulletContainer);

        createdObject.gameObject.SetActive(isActiveByDefault);
        this.pool.Add(createdObject);
        return createdObject;
    }

    /// <summary>
    /// ��������� ���� �� ��� ���������������� �������� � �����
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    public bool HasFreeElement(out Bullet element)
    {
        foreach (var bullet in pool)
        {
            if (!bullet.gameObject.activeInHierarchy)
            {
                element = bullet;
                bullet.gameObject.SetActive(true);
                return true;
            }
        }
        element = null;
        return false;
    }

    /// <summary>
    /// ���� ���� ����������������� ������� � �����, �� �������� ��� ��������� ��������� � 
    /// ��������� ����� Fire();
    /// ���� ��� ������� � ����� ������, ������� ����� ����� �������� ������ � ��������� ����������� 
    /// � ��������� ����� Fire()
    /// </summary>
    /// <param name="startDirection"></param>
    /// <param name="startRotation"></param>
    /// <param name="startVelocity"></param>
    public void EmitBullet(Vector3 startDirection, Quaternion startRotation, Vector3 startVelocity)
    {
        if (this.HasFreeElement(out Bullet element))
        {
            element.SetBulletDirection(startDirection, startRotation, startVelocity);
            element.Fire();            
        }            
        else
        {
            this.CreateObject(startDirection, startRotation, startVelocity, true).Fire();
        }     
    }
}
