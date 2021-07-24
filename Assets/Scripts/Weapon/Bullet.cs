using UnityEngine;

/// <summary>
/// ���������� ���������� ����
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    [SerializeField] private float bulletStartForce;
    [SerializeField] private float defaultDamageValue;
    [SerializeField] private Transform bulletMesh;

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

    private float currentDamageValue;

    private GameObject owner;

    private void Awake()
    {
        bulletRigidbody = GetComponent<Rigidbody>();
        
        startScale = bulletMesh.transform.localScale;

        currentDamageValue = defaultDamageValue;

    }

    /// <summary>
    /// �������� ��������� ��������� ����
    /// </summary>
    /// <param name="owner">�������� ����</param> 
    /// <param name="startPosition">������ ������� ����</param>
    /// <param name="rotation">������� ��������� ����</param>
    /// <param name="velocity">�������� ��������� ����</param>
    public void SetBulletParameters(GameObject owner, Vector3 startPosition, Quaternion rotation, Vector3 velocity, Vector3 targetBulletPoint, int damageMultiplier)
    {
        this.owner = owner;
        transform.position = startPosition;
        this.startPosition = transform.position;
        transform.rotation = rotation;
        bulletRigidbody.velocity = velocity;
        this.bulletLiveMaxDistance = (startPosition - targetBulletPoint).magnitude;
        bulletMesh.transform.localScale = startScale * damageMultiplier;
        currentDamageValue = defaultDamageValue * damageMultiplier;
        
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
        // ��������� ��������� � ���� � � ���������
        if (owner != null && (other.gameObject == owner || other.gameObject.layer == owner.layer 
            || other.isTrigger))
        {
            return;
        }

        if (other.TryGetComponent(out IDamageable damageableObject))
        {
            damageableObject.ReceiveDamage(currentDamageValue);
        }
        
        DisableBullet();          
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
