using UnityEngine;

/// <summary>
/// Управление поведением пули
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    [SerializeField] private float bulletStartForce;
    [SerializeField] private float defaultDamageValue;
    [SerializeField] private Transform bulletMesh;

    private Rigidbody bulletRigidbody;
    /// <summary>
    /// Расстояние на которое отлетает пуля
    /// </summary>
    private float bulletLiveMaxDistance;
    /// <summary>
    /// Начальная точка пули
    /// </summary>
    private Vector3 startPosition;
    /// <summary>
    /// Масштаб по умолчанию из префаба
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
    /// Передаем начальные параметры пули
    /// </summary>
    /// <param name="owner">Владелец пули</param> 
    /// <param name="startPosition">Откуда вылетат пуля</param>
    /// <param name="rotation">Поворот источника пули</param>
    /// <param name="velocity">Скорость источника пули</param>
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
    /// Придание движения пули
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
        // Исключаем попадание в себя и в союзников
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
    /// Метод устранения пули
    /// </summary>
    /// <returns></returns>
    private void DisableBullet()
    {        
        this.gameObject.SetActive(false);
    }
}
