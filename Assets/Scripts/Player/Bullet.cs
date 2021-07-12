using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Управление поведением пули
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    [SerializeField] private float bulletStartForce;    
    private Rigidbody bulletRigidbody;
    /// <summary>
    /// Расстояние на которое отлетает пуля
    /// </summary>
    private float bulletLiveMaxDistance;
    /// <summary>
    /// Начальная точка пули
    /// </summary>
    private Vector3 startPosition;

    private void Awake()
    {
        bulletRigidbody = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Передаем начальные параметры пули
    /// </summary>
    /// <param name="startPosition">Откуда вылетат пуля</param>
    /// <param name="rotation">Поворот источника пули</param>
    /// <param name="velocity">Скорость источника пули</param>
    public void SetBulletParameters(Vector3 startPosition, Quaternion rotation, Vector3 velocity, Vector3 targetBulletPoint, int damageMultiplier)
    {
        transform.position = startPosition;
        this.startPosition = transform.position;
        transform.rotation = rotation;
        bulletRigidbody.velocity = velocity;
        this.bulletLiveMaxDistance = (startPosition - new Vector3(0, startPosition.y, 0)- targetBulletPoint).magnitude;
        transform.localScale = transform.localScale * damageMultiplier;
    }

    /// <summary>
    /// Придание движения пули
    /// </summary>
    public void Fire()
    {        
        bulletRigidbody.AddForce(transform.forward * bulletStartForce, ForceMode.Impulse);
    }

    private void Update()
    {
        BulletDistanceCounter();
    }

    private void BulletDistanceCounter()
    {
        if ((transform.position - startPosition).magnitude > bulletLiveMaxDistance) DisableBullet();
    }

    /// <summary>
    /// Метод устранения пули
    /// </summary>
    /// <returns></returns>
    private void DisableBullet()
    {        
        this.gameObject.SetActive(false);
    }

    //private void OnDisable()
    //{
    //    if (disableAfterTime != null) StopCoroutine(disableAfterTime);
    //}
}
