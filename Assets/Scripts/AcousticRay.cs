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

    public List<Vector3> ColissionPoints
    {
        get;
        set;
    }

    public List<AcousticMaterial> AcousticMaterials
    {
        get;
        set;
    }

    public List<double> Intensities
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

    public AcousticRay(Vector3 source)
    {
        Source = source;
        ColissionPoints = new List<Vector3>();
        ColissionPoints.Add(Source);
        AcousticMaterials = new List<AcousticMaterial>();
        GameObject gameObject = new GameObject();
        AcousticMaterials.Add(gameObject.AddComponent<AcousticMaterial>());
        Intensities = new List<double>();
        Distances =new List<double>();
    }

    public AcousticRay TruncateRay(int position, Vector3 microphonePos)
    {
        AcousticRay newRay = new AcousticRay(Source);
        /*index, number of elements to copy*/
        newRay.ColissionPoints = ColissionPoints.GetRange(0, position);
        newRay.ColissionPoints.Add(microphonePos);
        /* TODO: see acoustic material for last acoustic material */
        newRay.AcousticMaterials = AcousticMaterials.GetRange(0, position + 1);

        return newRay;
    }

    private float GetDistance()
    {
        float distance = 0;

        for (int index = 0; index < ColissionPoints.Count - 1; ++index)
        {
            distance += (float)Math.Sqrt(Math.Pow(ColissionPoints[index].X - ColissionPoints[index + 1].X, 2) +
                                  Math.Pow(ColissionPoints[index].Y - ColissionPoints[index + 1].Y, 2) +
                                  Math.Pow(ColissionPoints[index].Z - ColissionPoints[index + 1].Z, 2));
        }

        return distance;
    }
}
