using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PathPointData
{
    // Статическое поле со всеми названиями параметров
    public static List<string> allParamNameList = new List<string>();

    [SerializeField]
    private PathPointGenericParameter<float> stopTime = null;
    [SerializeField]
    private PathPointGenericParameter<int> rotationAngle = null;

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
        stopTime = new PathPointGenericParameter<float>(useGlobalValue, 0);
        rotationAngle = new PathPointGenericParameter<int>(useGlobalValue, 0);
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
