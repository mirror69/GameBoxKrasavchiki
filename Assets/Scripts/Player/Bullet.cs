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
    [SerializeField] private float damageValue;
    private Rigidbody bulletRigidbody;
    /// <summary>
    /// ���������� �� ������� �������� ����
    /// </summary>
    private float bulletLiveMaxDistance;
    /// <summary>
    /// ��������� ����� ����
    /// </summary>
    private Vector3 startPosition;
    /// <summary>
    /// ������� �� ��������� �� �������
    /// </summary>
    private Vector3 startScale;

    private void Awake()
    {
        bulletRigidbody = GetComponent<Rigidbody>();
        startScale = transform.localScale;
    }

    /// <summary>
    /// �������� ��������� ��������� ����
    /// </summary>
    /// <param name="startPosition">������ ������� ����</param>
    /// <param name="rotation">������� ��������� ����</param>
    /// <param name="velocity">�������� ��������� ����</param>
    public void SetBulletParameters(Vector3 startPosition, Quaternion rotation, Vector3 velocity, Vector3 targetBulletPoint, int damageMultiplier)
    {
        transform.position = startPosition;
        this.startPosition = transform.position;
        transform.rotation = rotation;
        bulletRigidbody.velocity = velocity;
        this.bulletLiveMaxDistance = (startPosition - new Vector3(0, startPosition.y, 0)- targetBulletPoint).magnitude;
        transform.localScale = startScale * damageMultiplier;
        damageValue *= damageMultiplier;
    }

    /// <summary>
    /// �������� �������� ����
    /// </summary>
    public void BulletMoving()
    {        
        bulletRigidbody.AddForce(transform.forward * bulletStartForce, ForceMode.Impulse);
    }

    private void Update()
    {
        BulletDistanceCounter();
    }

    private void OnTriggerEnter(Collider other)
    {
        IDamageable damageableObject;
        if (other.TryGetComponent<IDamageable>(out damageableObject))
        {
            damageableObject.ReceiveDamage(damageValue);
            DisableBullet();
        }

        if (other.gameObject.layer == 10) DisableBullet(); //Layer 10 = Walls            
    }

    private void BulletDistanceCounter()
    {
        if ((transform.position - startPosition).magnitude > bulletLiveMaxDistance) DisableBullet();
    }

    /// <summary>
    /// ����� ���������� ����
    /// </summary>
    /// <returns></returns>
    private void DisableBullet()
    {        
        this.gameObject.SetActive(false);
    }
}
