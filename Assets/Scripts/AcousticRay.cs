using System;
using System.Collections.Generic;
using System.Linq;
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
        if (position > collisionPoints.Count || position > acousticMaterials.Count)
            return this;

        var newRay = new AcousticRay(source, microphonePos)
        {
            /*index, number of elements to copy*/
            collisionPoints = collisionPoints.GetRange(0, position),
            acousticMaterials = acousticMaterials.GetRange(0, position)
        };

        return newRay;
    }

    public float GetDistance()
    {
        if (collisionPoints.Count == 0)
            return Vector3.Distance(source, microphonePosition);

        var distance = Vector3.Distance(source,
            collisionPoints[0]);

        distance += collisionPoints.Select((vec, index) =>
            index == collisionPoints.Count - 1 ? 0 : Vector3.Distance(vec, collisionPoints[index + 1])).Sum();

        distance += Vector3.Distance(collisionPoints[collisionPoints.Count - 1], microphonePosition);

        return distance;
    }
}
