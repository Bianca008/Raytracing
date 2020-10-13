using System;
using System.Collections.Generic;
using UnityEngine;
using Vector3 = System.Numerics.Vector3;

public class AcousticRay
{
    public Vector3 Source
    {
        get;
        set;
    }

    public Vector3 MicrophonePosition
    {
        get;
        set;
    }

    public List<Vector3> CollisionPoints
    {
        get;
        set;
    }

    public List<AcousticMaterial> AcousticMaterials
    {
        get;
        set;
    }

    public List<double> Distances
    {
        get;
        set;
    }

    public float Distance
    {
        get
        {
            return GetDistance();
        }
    }

    public AcousticRay(Vector3 source, Vector3 microphone)
    {
        Source = source;
        MicrophonePosition = microphone;
        CollisionPoints = new List<Vector3>();
        AcousticMaterials = new List<AcousticMaterial>();
        Distances = new List<double>();
    }

    public AcousticRay TruncateRay(int position, Vector3 microphonePos)
    {
        AcousticRay newRay = new AcousticRay(Source, microphonePos);
        /*index, number of elements to copy*/
        newRay.CollisionPoints = CollisionPoints.GetRange(0, position);
        /* TODO: see acoustic material for last acoustic material */
        newRay.AcousticMaterials = AcousticMaterials.GetRange(0, position);

        return newRay;
    }

    private float GetDistance()
    {
        if (CollisionPoints.Count == 0)
            return Vector3.Distance(Source, MicrophonePosition);

        float distance = Vector3.Distance(Source,
            CollisionPoints[0]);

        for (int index = 0; index < CollisionPoints.Count - 1; ++index)
        {
            distance += Vector3.Distance(CollisionPoints[index], CollisionPoints[index + 1]);
        }

        distance += Vector3.Distance(CollisionPoints[CollisionPoints.Count - 1], MicrophonePosition);

        return distance;
    }
}
