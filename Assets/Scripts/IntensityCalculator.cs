using System;
using System.Collections.Generic;

public class IntensityCalculator
{
    private List<AcousticRay> Rays
    {
        get;
        set;
    }

    private double InitialIntensity
    {
        get;
        set;
    }

    public List<double> Intensities
    {
        get;
        set;
    }

    public IntensityCalculator(List<AcousticRay> rays, float initialPower)
    {
        InitialIntensity = initialPower / (4.0 * Math.PI);
        Rays = rays;
        Intensities = new List<double>();
    }

    public void ComputePower()
    {
        for (int indexRay = 0; indexRay < Rays.Count; ++indexRay)
        {
            ComputeRayIntensity(indexRay);
        }
    }

    private void ComputeRayIntensity(int indexRay)
    {
        /*Case for 0 CollisionPoints.*/
        if (Rays[indexRay].CollisionPoints.Count == 0)
        {
            float distance = System.Numerics.Vector3.Distance(
                Rays[indexRay].Source,
                Rays[indexRay].MicrophonePosition);
            Intensities.Add(Math.Pow(1 / distance, 2) * InitialIntensity);
            return;
        }

        /*Usual case.*/
        float previousDistance = System.Numerics.Vector3.Distance(
            Rays[indexRay].Source,
            Rays[indexRay].CollisionPoints[0]);
        /*I0*/
        double previousIntensity = Math.Pow(1 / previousDistance, 2) *
                                   InitialIntensity *
                                   Math.Pow(1 - Rays[indexRay].AcousticMaterials[0].AbsorbtionCoefficient, 2);

        for (int indexPosition = 1; indexPosition < Rays[indexRay].CollisionPoints.Count; ++indexPosition)
        {
            float currentDistance = previousDistance + System.Numerics.Vector3.Distance(
                Rays[indexRay].CollisionPoints[indexPosition],
                Rays[indexRay].CollisionPoints[indexPosition - 1]);
            double currentIntensity = Math.Pow(previousDistance / currentDistance, 2) * previousIntensity *
                                Math.Pow(1 - Rays[indexRay].AcousticMaterials[indexPosition].AbsorbtionCoefficient, 2);

            previousDistance = currentDistance;
            previousIntensity = currentIntensity;
        }

        /*In*/
        Intensities.Add(Math.Pow(previousDistance /
            (previousDistance + System.Numerics.Vector3.Distance(
                Rays[indexRay].CollisionPoints[Rays[indexRay].CollisionPoints.Count - 1],
                Rays[indexRay].MicrophonePosition)), 2) * previousIntensity);
    }
}
