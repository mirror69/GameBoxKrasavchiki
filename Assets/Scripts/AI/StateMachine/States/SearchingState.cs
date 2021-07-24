using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Состояние поиска врагом игрока
/// </summary>
[RequireComponent(typeof(AISearchingStrategy))]
public class SearchingState : FsmState
{
    private AIMovingObject movingObject;
    private AISearchingStrategy searchingStrategy;

    public override void OnStateEnter()
    {        
        Debug.Log($"{movingObject.name} starting searching");

        searchingStrategy.Initialize(movingObject, GameManager.Instance.Player.transform);
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
    }
}
