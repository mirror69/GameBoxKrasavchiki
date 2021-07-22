using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AISearchingStrategy))]
public class SearchingState : FsmState
{
    private AIMovingObject movingObject;
    private AISearchingStrategy searchingStrategy;
    private Transform target;

    public override void OnStateEnter()
    {
        Debug.Log($"{name} starting searching");
        searchingStrategy.SetTarget(target);
        searchingStrategy.StartMoving();
    }

    public override void OnStateLeave()
    {
        searchingStrategy.StopMoving();
    }

    public override bool IsPerforming()
    {
        return !searchingStrategy.IsStopped();
    }

    private void Awake()
    {
        movingObject = GetComponentInParent<AIMovingObject>();

        searchingStrategy = GetComponent<AISearchingStrategy>();
        searchingStrategy.Initialize(movingObject);
    }
    private void Start()
    {
        target = GameManager.Instance.Player.transform;
    }
}
