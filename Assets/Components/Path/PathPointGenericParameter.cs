using System;
using UnityEngine;

[Serializable]
public class PathPointGenericParameter<T> : PathPointParameter
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
    public PathPointGenericParameter(bool useGlobalValue, T value) : base(useGlobalValue)
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
