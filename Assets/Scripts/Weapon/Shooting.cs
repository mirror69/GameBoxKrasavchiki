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

    public Bounds BulletBounds { get; private set; }
    public float DefaultDamageValue => bulletPrefab.DefaultDamageValue;

    private void Awake()
    {
        // Если контейнер для пуль не задан, создадим его в корне дерева объектов
        if (bulletsContainer == null)
        {
            bulletsContainer = new GameObject($"{name}_bulletsContainer").transform;
        }
        
        // Получить размер пули из префаба нельзя. Поэтому создаем временный объект, берем размер, и уничтожаем
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
