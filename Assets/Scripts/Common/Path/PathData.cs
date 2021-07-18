using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEditor;
using UnityEngine;

[Serializable]
public abstract class PathPointParameter
{
    [SerializeField]
    public bool useGlobalValue;

    public virtual int IntValue { get; set; }
    public virtual float FloatValue { get; set; }

    public PathPointParameter(bool useGlobalValue)
    {
        this.useGlobalValue = useGlobalValue;
    }

    public virtual bool IsFloatValue()
    {
        return false;
    }
    public virtual bool IsIntValue()
    {
        return false;
    }
}

[Serializable]
public class PathPointParameter<T> : PathPointParameter
{
    [SerializeField]
    public T value;

    public override int IntValue
    {
        get => IsIntValue() ? (int)(object)value : 0;
        set => this.value = IsIntValue() ? (T)(object)value : this.value;
    }
    public override float FloatValue
    {
        get => IsFloatValue() ? (float)(object)value : 0;
        set => this.value = IsFloatValue() ? (T)(object)value : this.value;
    }
    public PathPointParameter(bool useGlobalValue, T value) : base(useGlobalValue)
    {
        this.value = value;
    }
    public override bool IsIntValue()
    {
        return typeof(T) == typeof(int);
    }
    public override bool IsFloatValue()
    {
        return typeof(T) == typeof(float);
    }
}

//public class LocalPointData: PathPointData
//{
//    private PathPointData globalPointsData;

//    public float StopTime =>
//        stopTime.useGlobalValue ? globalPointsData.stopTime.value : stopTime.value;
//    public int RotationAngle =>
//        rotationAngle.useGlobalValue ? globalPointsData.rotationAngle.value : rotationAngle.value;
//}

[Serializable]
public class PathPointData
{
    // —татическое поле со всеми названи€ми параметров
    public static List<string> allParamNameList = new List<string>();

    [SerializeField]
    private PathPointParameter<float> stopTime = null;
    [SerializeField]
    private PathPointParameter<int> rotationAngle = null;

    private Dictionary<string, PathPointParameter> paramDictionary = null;

    private PathPointData globalPointsData = null;

    public float StopTime =>
        stopTime.useGlobalValue && globalPointsData != null
        ? globalPointsData.stopTime.value
        : stopTime.value;
    public int RotationAngle =>
        rotationAngle.useGlobalValue && globalPointsData != null
        ? globalPointsData.rotationAngle.value 
        : rotationAngle.value;

    public PathPointData(bool useGlobalValue, PathPointData globalPointsData)
    {
        stopTime = new PathPointParameter<float>(useGlobalValue, 0);
        rotationAngle = new PathPointParameter<int>(useGlobalValue, 0);
        this.globalPointsData = globalPointsData;
        RefreshParamDictionary();
    }
    public PathPointData() : this(false, null)
    {
    }

    public void SetGlobalData(PathPointData globalPointsData)
    {
        this.globalPointsData = globalPointsData;
    }

    public bool TryGetParameter(string key, out PathPointParameter param)
    {
        RefreshParamDictionary();
        return paramDictionary.TryGetValue(key, out param);
    }

    private void RefreshParamDictionary()
    {
        if (paramDictionary == null)
        {
            paramDictionary = new Dictionary<string, PathPointParameter>();
            AddParamName(stopTime, nameof(stopTime));
            AddParamName(rotationAngle, nameof(rotationAngle));
        }
    }

    private void AddParamName(PathPointParameter param, string name)
    {
        paramDictionary[name] = param;
        if (allParamNameList.Contains(name))
        {
            return;
        }
        allParamNameList.Add(name);
    }
}

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
    private NPCPath path;

    public PathPointData GlobalPointsData => globalPointsData;
    public List<PathPointData> LocalPointsData => localPointsData;

    public NPCPath Path => path;
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
        if (localPointsData.Count > path.PointsCount)
        {
            for (int i = localPointsData.Count; i >= path.PointsCount; i--)
            {
                OnPointDeleted(i);
            }
        }
        else if (localPointsData.Count < path.PointsCount)
        {
            for (int i = localPointsData.Count; i < path.PointsCount; i++)
            {
                OnPointAdded();
            }
        }

    }

    [ExecuteInEditMode]
    private void Awake()
    {
        pathCreator = GetComponent<PathCreator>();
        path = pathCreator.Path;
        RefreshByPath();
        foreach (var item in localPointsData)
        {
            item.SetGlobalData(GlobalPointsData);
        }
        path.PointAdded += OnPointAdded;
        path.PointDeleted += OnPointDeleted;
    }

    [ExecuteInEditMode]
    private void OnPointAdded()
    {
        Undo.RecordObject(this, "1");
        localPointsData.Add(new PathPointData(true, globalPointsData));
    }
    [ExecuteInEditMode]
    private void OnPointDeleted(int index)
    {
        Undo.RecordObject(this, "1");
        localPointsData.RemoveAt(index);
    }

}
