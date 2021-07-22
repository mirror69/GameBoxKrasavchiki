using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PursuingObject : MonoBehaviour
{
    [SerializeField]
    private Transform objToFollow = null;
    [SerializeField]
    private bool followX = true;
    [SerializeField]
    private bool followY = true;
    [SerializeField]
    private bool followZ = true;

    private Vector3 deltaPos = Vector3.zero;

    private void Start()
    {
        deltaPos = transform.position - objToFollow.position;
    }

    void LateUpdate()
    {
        if (objToFollow == null)
        {
            return;
        }
        Vector3 newPosition = transform.position;
        if (followX)
        {
            newPosition.x = objToFollow.position.x + deltaPos.x;
        }
        if (followY)
        {
            newPosition.y = objToFollow.position.y + deltaPos.y;
        }
        if (followZ)
        {
            newPosition.z = objToFollow.position.z + deltaPos.z;
        }
        transform.position = newPosition;
    }

}
