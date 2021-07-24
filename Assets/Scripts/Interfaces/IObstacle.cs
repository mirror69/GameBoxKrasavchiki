using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public interface IObstacleCollider
//{
//    IObstacle OwnerObstacle;
//}

public interface IObstacle
{
    float OpacityValue { get; }
    void Initialize(OpacityChangingParameters opacityChangingParameters);
    void SetOpacityValue(float opacityValueInPercents);
    bool IsPassable();
}
