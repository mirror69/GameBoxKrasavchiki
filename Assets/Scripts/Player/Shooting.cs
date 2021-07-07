using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Основная задача класса: Создать пулл пуль заданного количестваа по заданному префабу и
/// в методе Shoot передавать положение и поворот источника пули.
/// </summary>
public class Shooting : MonoBehaviour
{
    [SerializeField] private int poolCount = 3; //размер пула пуль    
    [SerializeField] private Bullet bulletPrefab; //через инспектор загружаем тип пули со своим поведением
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
