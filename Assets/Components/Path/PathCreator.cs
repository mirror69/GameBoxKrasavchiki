using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class PathCreator : MonoBehaviour
{
    [SerializeField, HideInInspector]
    private NPCPath path = null;
    [SerializeField]
    private float defaultPointDistance = 3;

    public NPCPath Path => path;

    public event Action PointAdded;
    public event Action<int> PointDeleted;
    public event Action PathCleared;

    public void RefreshPath()
    {        
        if (path == null)
        {
            path = new NPCPath(transform);
        }
        path.Init(transform);
    }
    public void MovePoint(int i, Vector3 newPosition)
    {     
        path.MovePoint(i, new Vector3(newPosition.x, transform.position.y, newPosition.z));
    }
    public void AddPoint(Vector3 position)
    {
        path.AddPoint(new Vector3(position.x, transform.position.y, position.z));
        PointAdded?.Invoke();
    }
    public bool TryGetPointByRay(Ray worldRay, out Vector3 point)
    {
        Plane pathPlane = new Plane(Vector3.up, transform.position);
        if (pathPlane.Raycast(worldRay, out float hitDistance))
        {
            point = worldRay.GetPoint(hitDistance);
            return true;
        }
        point = Vector3.zero;
        return false;
    }

    public void DeletePoint(int index)
    {
        if (CanDeletePoints())
        {
            path.DeletePoint(index);
            PointDeleted?.Invoke(index);
        }
    }
    public bool CanDeletePoints()
    {
        return path.PointsCount > 1;
    }

    public void AddNextPoint()
    {
        int lastIndex = path.PointsCount - 1;
        Vector3 direction = Vector3.forward;
        if (path.PointsCount > 1)
        {
            direction = path[lastIndex] - path[lastIndex - 1];
            direction = direction.normalized;
        }
        AddPoint(path[lastIndex] + direction * defaultPointDistance);
    }

    public void ClearPath()
    {
        path = new NPCPath(transform);
        RefreshPath();
        PathCleared?.Invoke();
    }

    private void OnEnable()
    {
        RefreshPath();
    }

}
