using System.Collections.Generic;
using UnityEngine;

public class IRayCaster
{
    private int maxDistance;

    public System.Numerics.Vector3 Position
    {
        get;
        set;
    }

    public System.Numerics.Vector3 Direction
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

    public void RayCast(List<List<System.Numerics.Vector3>> linePositions,
        int numberOfRay)
    {
        Ray ray = new Ray(VectorConverter.Convert(Position), VectorConverter.Convert(Direction));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            Direction = VectorConverter.Convert(UnityEngine.Vector3.Reflect(VectorConverter.Convert(Direction), hit.normal));
            Position = VectorConverter.Convert(hit.point);
            ++NumberOfReflections;
            TotalDistance += hit.distance;
            linePositions[numberOfRay].Add(VectorConverter.Convert(hit.point));
        }
        else
        {
            Position += Direction * maxDistance;
            TotalDistance += maxDistance;
        }
    }
}
