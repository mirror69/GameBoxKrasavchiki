using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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

    [SerializeField]
    private PathData pathData;
    private Vector3 initialDirection;

    public PathData PathData => pathData;

    public void Initialize(AIMovingObject movingObject)
    {
        BaseInitialize(movingObject);
        initialDirection = movingObject.transform.forward;
    }

    protected override IEnumerator PerformMoving()
    {
        movingObject.SetEnabledAutomaticRotation(true);

        Coroutine rotatingCoroutine;
        bool infiniteRotation = pathData.Path.PointsCount <= 1;
        CurrentPathParam currentPathParam = new CurrentPathParam(pathData.Path, movingObject.Position);
        while (true)
        {
            movingObject.Move(pathData.Path[currentPathParam.PointIndex]);

            PathPointData pointData = pathData.GetPointData(currentPathParam.PointIndex);
            while (!movingObject.IsDestinationReached())
            {
                yield return new WaitForFixedUpdate();

                if (pointData.StopTime == 0)
                {
                    break;
                }
            }

            if (infiniteRotation)
            {
                float angleToInitialDirection = Vector3.SignedAngle(movingObject.transform.forward,
                    initialDirection, Vector3.up);
                float rotationTime = Mathf.Abs(angleToInitialDirection / movingObject.RotationSpeed);
                rotatingCoroutine = StartCoroutine(PerformRotating(angleToInitialDirection, movingObject.RotationSpeed));
                float endTime = Time.time + rotationTime;
                while (Time.time < endTime)
                {
                    yield return new WaitForFixedUpdate();
                }
                this.StopAndNullCoroutine(ref rotatingCoroutine);
            }

            float timeToNextMove = Time.time + pointData.StopTime;
         
            rotatingCoroutine = StartCoroutine(
                PerformRotatingLeftRight(pointData.RotationAngle, movingObject.FovRotationSpeed));
            while (infiniteRotation || Time.time < timeToNextMove)
            {
                yield return new WaitForFixedUpdate();
            }
            this.StopAndNullCoroutine(ref rotatingCoroutine);

            currentPathParam.NextPoint();
        }

    }
}
