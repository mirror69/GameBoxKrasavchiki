using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(WeaponController))]
public class PlayerInput : MonoBehaviour
{
    private const float TargetingRayTopY = 50;
    private const float TargetingRayDistanceFromTop = 100;

    [SerializeField]
    private float autoTargetingRadius = 2;
    
    private Vector3 moveDirection;
    private float horizontalInput;
    private float verticalInput;
    private PlayerController playerController;
    private bool isPushingButton = false;
    private Camera mainCamera = null;
    private Vector3 mouseGroundPosition;
    Plane playerMovingPlane;

    private BeamWeaponController weaponController = null;
    private IDamageable currentTarget = null;

    public IDamageable GetTarget()
    {
        return currentTarget;
    }

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        weaponController = GetComponent<BeamWeaponController>();
        mainCamera = Camera.main;
        playerMovingPlane = new Plane(Vector3.up, transform.position + 
            new Vector3(0, weaponController.AttackPoint.position.y, 0));
    }

    private void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        mouseGroundPosition = GetMouseGroundPosition(Input.mousePosition);
        currentTarget = FindNearestTargetInVicinity(mouseGroundPosition, autoTargetingRadius);
        
        if (Input.GetButtonDown("Fire1"))
        {
            weaponController.SetTarget(currentTarget);
            isPushingButton = true;
        }

        if (Input.GetButtonUp("Fire1"))
        {
            weaponController.InterruptAttack();
            isPushingButton = false;
        }

        if (isPushingButton)
        {
            // ≈сли кнопка атаки зажата, делаем попытки атаковать каждый кадр на тот случай,
            // если кулдаун от предыдущей атаки не завершилс€, чтобы сразу после него начать атаку
            weaponController.Attack();
        }
    }

    private void FixedUpdate()
    {
        moveDirection = new Vector3(-horizontalInput, 0, -verticalInput);

        playerController.MovePlayer(moveDirection);

        // ѕри атаке поворачиваемс€ туда, где находитс€ цель.
        // ≈сли цели нет, смотрим на курсор.
        IDamageable target = weaponController.GetTarget();
        Vector3 lookAtPoint = target != null ? target.Transform.position : mouseGroundPosition;
        playerController.RotatePlayer(lookAtPoint);
    }

    /// <summary>
    /// ѕолучить проекцию позиции курсора мыши на плоскость XZ, в которой находитс€ игрок
    /// </summary>
    /// <param name="mousePosition">Ёкранные координаты курсора</param>
    /// <returns></returns>
    private Vector3 GetMouseGroundPosition(Vector3 mousePosition)
    {
        Ray cameraToCursorRay = mainCamera.ScreenPointToRay(mousePosition);
        if (playerMovingPlane.Raycast(cameraToCursorRay, out float hitDistance))
        {
            return cameraToCursorRay.GetPoint(hitDistance);
        }
        return Vector3.zero;
    }

    /// <summary>
    /// Ќайти врага, ближайшего к указанной позиции
    /// </summary>
    /// <param name="point">÷ентр окрестности, в которой осуществл€етс€ поиск</param>
    /// <param name="radius">–адиус окрестности</param>
    /// <returns></returns>
    private IDamageable FindNearestTargetInVicinity(Vector3 point, float radius)
    {
        Vector3 topPoint = point;
        topPoint.y = TargetingRayTopY;

        RaycastHit[] hitInfo = Physics.SphereCastAll(topPoint, radius, Vector3.down,
            TargetingRayDistanceFromTop, GameManager.Instance.EnemyLayers, QueryTriggerInteraction.Ignore);

        if (hitInfo.Length == 0)
        {
            return null;
        }
        
        Transform minDistTransform = hitInfo[0].transform;
        float minSqrDistToTarget = Vector3.SqrMagnitude(minDistTransform.position - point);

        // ≈сли целей в окрестности точки несколько, найдЄм ту, котора€ ближе всего к центру окрестности
        for (int i = 1; i < hitInfo.Length; i++)
        {
            float currentSqrDist = Vector3.SqrMagnitude(hitInfo[i].transform.position - point);
            if (currentSqrDist < minSqrDistToTarget)
            {
                minSqrDistToTarget = currentSqrDist;
                minDistTransform = hitInfo[i].transform;
            }
        }

        return minDistTransform.GetComponent<IDamageable>();
    }

}
