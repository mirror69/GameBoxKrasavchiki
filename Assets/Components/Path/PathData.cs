using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEditor;
using UnityEngine;

[Serializable]
[ExecuteInEditMode]
[RequireComponent(typeof(PathCreator))]
public class PathData : MonoBehaviour
{
    [SerializeField, HideInInspector]
    private PathPointData globalPointsData = new PathPointData();
    [SerializeField, HideInInspector]
    private List<PathPointData> localPointsData = new List<PathPointData>();

    private PathCreator pathCreator;

    public PathPointData GlobalPointsData => globalPointsData;
    public List<PathPointData> LocalPointsData => localPointsData;

    public NPCPath Path => PathCreator.Path;
    public PathCreator PathCreator => pathCreator;

    public PathPointData GetPointData(int index)
    {
        if (index < 0 || index >= LocalPointsData.Count)
        {
            return null;
        }
        return localPointsData[index];
    }

    public void RefreshByPath()
    {
        if (localPointsData.Count > Path.PointsCount)
        {
            for (int i = localPointsData.Count - 1; i >= Path.PointsCount; i--)
            {
                OnPointDeleted(i);
            }
        }
        else if (localPointsData.Count < Path.PointsCount)
        {
            for (int i = localPointsData.Count; i < Path.PointsCount; i++)
            {
                OnPointAdded();
            }
        }      
    }

    [ExecuteInEditMode]
    private void Awake()
    {
        pathCreator = GetComponent<PathCreator>();
        RefreshByPath();
        foreach (var item in localPointsData)
        {
            item.SetGlobalData(GlobalPointsData);
        }
    }

    private void OnEnable()
    {
        RefreshByPath();
        PathCreator.PointAdded += OnPointAdded;
        PathCreator.PointDeleted += OnPointDeleted;
        PathCreator.PathCleared += OnPathCleared;
        Undo.undoRedoPerformed += () => RefreshByPath();
    }

    private void OnDisable()
    {
        PathCreator.PointAdded -= OnPointAdded;
        PathCreator.PointDeleted -= OnPointDeleted;
        PathCreator.PathCleared -= OnPathCleared;
        Undo.undoRedoPerformed -= () => RefreshByPath();
    }

    [ExecuteInEditMode]
    private void OnPointAdded()
    {
        localPointsData.Add(new PathPointData(true, globalPointsData));
    }
    [ExecuteInEditMode]
    private void OnPointDeleted(int index)
    {
        localPointsData.RemoveAt(index);
    }
    [ExecuteInEditMode]
    private void OnPathCleared()
    {
        RefreshByPath();
    }


}
