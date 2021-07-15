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
