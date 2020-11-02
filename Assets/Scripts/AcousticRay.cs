using System;
using System.Collections.Generic;
using UnityEngine;
using Vector3 = System.Numerics.Vector3;

public class AcousticRay
{
    public Vector3 source
    {
        get;
        set;
    }

    public Vector3 microphonePosition
    {
        get;
        set;
    }

    public List<Vector3> collisionPoints { get; set; } = new List<Vector3>();

    public List<AcousticMaterial> acousticMaterials { get; set; } = new List<AcousticMaterial>();

    public List<double> distances { get; set; } = new List<double>();

    public AcousticRay(Vector3 source, Vector3 microphone)
    {
        this.source = source;
        microphonePosition = microphone;
    }

    public AcousticRay TruncateRay(int position, Vector3 microphonePos)
    {
        var newRay = new AcousticRay(source, microphonePos);
        /*index, number of elements to copy*/
        newRay.collisionPoints = collisionPoints.GetRange(0, position);
        /* TODO: see acoustic material for last acoustic material */
        newRay.acousticMaterials = acousticMaterials.GetRange(0, position);

        return newRay;
    }

    public float GetDistance()
    {
        if (collisionPoints.Count == 0)
            return Vector3.Distance(source, microphonePosition);

        var distance = Vector3.Distance(source,
            collisionPoints[0]);

        for (int index = 0; index < collisionPoints.Count - 1; ++index)
        {
            distance += Vector3.Distance(collisionPoints[index], collisionPoints[index + 1]);
        }

        distance += Vector3.Distance(collisionPoints[collisionPoints.Count - 1], microphonePosition);

        return distance;
    }
}
