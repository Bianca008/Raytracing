﻿using System;
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

    public AcousticRay(Vector3 source, Vector3 microphone)
    {
        Source = source;
        MicrophonePosition = microphone;
        ColissionPoints = new List<Vector3>();
        AcousticMaterials = new List<AcousticMaterial>();
        GameObject gameObject = new GameObject();
        Intensities = new List<double>();
        Distances = new List<double>();
    }

    public AcousticRay TruncateRay(int position, Vector3 microphonePos)
    {
        AcousticRay newRay = new AcousticRay(Source, microphonePos);
        /*index, number of elements to copy*/
        newRay.ColissionPoints = ColissionPoints.GetRange(0, position);
        /* TODO: see acoustic material for last acoustic material */
        newRay.AcousticMaterials = AcousticMaterials.GetRange(0, position);

        return newRay;
    }

    private float GetDistance()
    {
        if (ColissionPoints.Count == 0)
            return Vector3.Distance(Source, MicrophonePosition);

        float distance = Vector3.Distance(Source,
            ColissionPoints[0]);

        for (int index = 0; index < ColissionPoints.Count - 1; ++index)
        {
            distance += Vector3.Distance(ColissionPoints[index], ColissionPoints[index + 1]);
        }

        return distance;
    }
}
