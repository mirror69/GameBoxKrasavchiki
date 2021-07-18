using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NPCPath
{
    /// <summary>
    /// Точки маршрута (Поле должно быть сериализуемым, чтобы поддерживать операции Undo)
    /// </summary>
    [SerializeField, HideInInspector]
    private List<Vector3> points;
    [SerializeField, HideInInspector]
    private bool isClosed = false;

    private Transform center;

    public int PointsCount => points.Count;
    public bool IsClosed => isClosed;

    public event Action PointAdded;
    public event Action<int> PointDeleted;
      
    public NPCPath(Transform center)
    {
        points = new List<Vector3>() { Vector3.zero };
        this.center = center;
    }

    public void Init(Transform center)
    {
        this.center = center;
    }

    public Vector3 this[int i] => center.position + points[i];

    public void SetClosed(bool closed)
    {
        isClosed = closed;
    }

    public void AddPoint(Vector3 point)
    {
        points.Add(point - center.position);
        PointAdded?.Invoke();
    }

    public void MovePoint(int i, Vector3 newPosition)
    {
        points[i] = newPosition - center.position;
    }
    public void DeletePoint(int index)
    {
        if (index >= 0 && index < PointsCount)
        {
            points.RemoveAt(index);
            PointDeleted?.Invoke(index);
        }
    }

    public int LoopIndex(int i)
    {
        return i % PointsCount;
    }

    public int GetNearestPoint(Vector3 point)
    {
        int index = 0;
        Vector3 localPoint = point - center.position;
        for (int i = 1; i < PointsCount; i++)
        {
            if (Vector3.SqrMagnitude(localPoint - points[i]) < Vector3.SqrMagnitude(localPoint - points[index]))
            {
                index = i;
            }
        }
        return index;
    }
}
