using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���������� ���������� ����
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
    /// �������� ��������� ��������� ����
    /// </summary>
    /// <param name="startPosition">������ ������� ����</param>
    /// <param name="rotation">������� ��������� ����</param>
    /// <param name="velocity">�������� ��������� ����</param>
    public void SetBulletDirection(Vector3 startPosition, Quaternion rotation, Vector3 velocity)
    {
        transform.position = startPosition;
        transform.rotation = rotation;
        bulletRigidbody.velocity = velocity;
    }

    /// <summary>
    /// �������� �������� ����
    /// </summary>
    public void Fire()
    {
        disableAfterTime = StartCoroutine(DisableBullet());
        bulletRigidbody.AddForce(transform.forward * bulletStartForce, ForceMode.Impulse);
    }

    /// <summary>
    /// �������� ���������� ����
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
