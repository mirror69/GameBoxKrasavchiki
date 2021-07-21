using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChasingState : FsmState
{
    private AIMovingObject movingObject;
    private Transform target;
    private FieldOfView fov;

    public override void OnStateEnter()
    {
        Debug.Log($"{name} starting chasing");
        fov.SetAlertDetectionDelay();
        movingObject.StartChasing(target);
    }

    public override void OnStateLeave()
    {
    }

    private void Awake()
    {
        movingObject = GetComponentInParent<AIMovingObject>();
        fov = GetComponentInParent<FieldOfView>();        
    }
    private void Start()
    {
        target = GameManager.Instance.Player.transform;
    }
}
