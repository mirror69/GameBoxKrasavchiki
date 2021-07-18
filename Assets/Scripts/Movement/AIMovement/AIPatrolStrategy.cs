using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[ExecuteInEditMode]
[RequireComponent(typeof(PathData))]
public class AIPatrolStrategy : AIMovementStrategy
{
    private class CurrentPathParam
    {
        private NPCPath path;
        private int pointIndex;
        private int directionIndexIncrement;

        public int PointIndex => pointIndex;

        public CurrentPathParam(NPCPath path, Vector3 currentPosition)
        {
            this.path = path;
            pointIndex = path.GetNearestPoint(currentPosition);
            directionIndexIncrement = 1;
        }
        public void NextPoint()
        {
            pointIndex += directionIndexIncrement;
            if (directionIndexIncrement > 0)
            {
                if (pointIndex >= path.PointsCount)
                {
                    if (path.IsClosed)
                    {
                        pointIndex = 0;
                    }
                    else
                    {
                        pointIndex = path.PointsCount - 2;
                        directionIndexIncrement = -directionIndexIncrement;
                    }
                }
            }
            else
            {
                if (pointIndex < 0)
                {
                    if (path.IsClosed)
                    {
                        pointIndex = path.PointsCount - 1;
                    }
                    else
                    {
                        pointIndex = 1;
                        directionIndexIncrement = -directionIndexIncrement;
                    }
                }
            }
        }
    }

    private PathData pathData;

    private Coroutine movingCoroutine = null;

    public PathData PathData => pathData;

    public override void StartMoving()
    {
        if (movingCoroutine == null)
        {
            if (pathData.Path.PointsCount == 1)
            {
                movingCoroutine = StartCoroutine(PerformRotating());
            }
            else if(pathData.Path.PointsCount > 1)
            {
                movingCoroutine = StartCoroutine(PerformMoving());
            }            
        }      
    }

    public override void StopMoving()
    {
        if (movingCoroutine != null)
        {
            StopCoroutine(movingCoroutine);
            movingCoroutine = null;
        }
    }

    [ExecuteInEditMode]
    private void Awake()
    {
        pathData = GetComponent<PathData>();
    }

    private IEnumerator PerformRotating()
    {
        CurrentPathParam currentPathParam = new CurrentPathParam(pathData.Path, movingObject.Position);
        PathPointData pointData = pathData.GetPointData(currentPathParam.PointIndex);
        
        // Вначале берем только половину максимального угла, т.к. персонаж смотрит прямо,
        // и нам нужно будет повернуться только наполовину
        // При повороте в обратную сторону будетм брать уже целый угол, т.к. именно на него
        // нужно будет повернуться, чтобы посмотреть в противоположную сторону
        float maxRotationAngle = pointData.RotationAngle * .5f;
        int rotationMultiplier = 1;
        float fullRotation = 0;
        while (true)
        {
            yield return null;

            float rotationDelta = movingObject.FovRotationSpeed * Time.deltaTime;
            movingObject.Rotate(rotationMultiplier * rotationDelta);

            fullRotation += rotationDelta;
            if (fullRotation > maxRotationAngle)
            {
                fullRotation = 0;
                rotationMultiplier = -rotationMultiplier;
                maxRotationAngle = pointData.RotationAngle;
            }
        }
    }
    private IEnumerator PerformMoving()
    {
        CurrentPathParam currentPathParam = new CurrentPathParam(pathData.Path, movingObject.Position);
        while (true)
        {
            movingObject.Move(pathData.Path[currentPathParam.PointIndex]);

            yield return null;

            PathPointData pointData = pathData.GetPointData(currentPathParam.PointIndex);
            while (!movingObject.IsDestinationReached())
            {
                yield return null;
                
                if (pointData.StopTime == 0 && movingObject.GetRemainingDistance() < 0.5f)
                {
                    break;
                }
            }            
            
            float timeToNextMove = Time.time + pointData.StopTime;
            
            float maxRotationAngle = pointData.RotationAngle * .5f;
            int rotationMultiplier = 1;
            float fullRotation = 0;
            while (Time.time < timeToNextMove)
            {
                yield return null;

                float rotationDelta = movingObject.FovRotationSpeed * Time.deltaTime;
                movingObject.Rotate(rotationMultiplier * rotationDelta);
                
                fullRotation += rotationDelta;
                if (fullRotation > maxRotationAngle)
                {
                    fullRotation = 0;
                    rotationMultiplier = -rotationMultiplier;
                    maxRotationAngle = pointData.RotationAngle;
                }
            }

            if (pathData.Path.PointsCount <= 1)
            {
                continue;
            }

            currentPathParam.NextPoint();
        }

    }
}
