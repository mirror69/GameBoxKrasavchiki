using System;
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
