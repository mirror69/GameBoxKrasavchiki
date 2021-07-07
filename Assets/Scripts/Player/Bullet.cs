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
    private Coroutine disableAfterTime;
    private Rigidbody bulletRigidbody;

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
    public void SetBulletDirection(Vector3 startPosition, Quaternion rotation, Vector3 velocity)
    {
        transform.position = startPosition;
        transform.rotation = rotation;
        bulletRigidbody.velocity = velocity;
    }

    /// <summary>
    /// Придание движения пули
    /// </summary>
    public void Fire()
    {
        disableAfterTime = StartCoroutine(DisableBullet());
        bulletRigidbody.AddForce(transform.forward * bulletStartForce, ForceMode.Impulse);
    }

    /// <summary>
    /// Корутина устранения пули
    /// </summary>
    /// <returns></returns>
    IEnumerator DisableBullet()
    {
        yield return new WaitForSeconds(3f);
        this.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        if (disableAfterTime != null) StopCoroutine(disableAfterTime);
    }
}
