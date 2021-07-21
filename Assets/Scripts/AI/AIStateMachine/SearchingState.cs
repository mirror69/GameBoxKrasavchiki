using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchingState : FsmState
{
    private AIMovingObject movingObject;
    private Transform target;

    public override void OnStateEnter()
    {
        Debug.Log($"{name} starting searching");
        movingObject.StartSearching(target);
    }

    public override void OnStateLeave()
    {
    }

    private void Awake()
    {
        movingObject = GetComponentInParent<AIMovingObject>();
    }
    private void Start()
    {
        target = GameManager.Instance.Player.transform;
    }
}
