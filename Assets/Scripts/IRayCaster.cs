using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IRayCaster
{
    private int maxDistance;

    public Vector3 Position
    {
        get;
        set;
    }

    public Vector3 Direction
    {
        get;
        set;
    }

    public float TotalDistance
    {
        get;
        set;
    }

    public int NumberOfReflections
    {
        get;
        set;
    }

    public IRayCaster(int maxDist)
    {
        maxDistance = maxDist;
    }

    public void RayCast(List<List<Vector3>> linePositions,
        int numberOfRay)
    {
        Ray ray = new Ray(Position, Direction);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            Direction = Vector3.Reflect(Direction, hit.normal);
            Position = hit.point;
            ++NumberOfReflections;
            TotalDistance += hit.distance;
            linePositions[numberOfRay].Add(hit.point);
        }
        else
        {
            Position += Direction * maxDistance;
            TotalDistance += maxDistance;
        }
    }
}
