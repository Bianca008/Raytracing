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

    public IntensityCalculator(List<AcousticRay> rays, float initialPower)
    {
        InitialIntensity = initialPower / (4.0 * Math.PI);
        Rays = rays;
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
            Rays[indexRay].Intensities.Add(Math.Pow(1 / distance, 2) * InitialIntensity);
            return;
        }

        /*Usual case.*/
        float previousDistance = System.Numerics.Vector3.Distance(
            Rays[indexRay].Source,
            Rays[indexRay].CollisionPoints[0]);
        double previousIntensity = Math.Pow(1 / previousDistance, 2) *
                                   InitialIntensity *
                                   Math.Pow(Rays[indexRay].AcousticMaterials[0].AbsorbtionCoefficient, 2);
        /*I0*/
        Rays[indexRay].Intensities.Add(previousIntensity);

        for (int indexPosition = 1; indexPosition < Rays[indexRay].CollisionPoints.Count; ++indexPosition)
        {
            float currentDistance = previousDistance + System.Numerics.Vector3.Distance(
                Rays[indexRay].CollisionPoints[indexPosition],
                Rays[indexRay].CollisionPoints[indexPosition - 1]);
            double currentIntensity;
            if (indexPosition != Rays[indexRay].CollisionPoints.Count - 1)
            {
                currentIntensity = Math.Pow(previousDistance / currentDistance, 2) * previousIntensity *
                                Math.Pow(Rays[indexRay].AcousticMaterials[indexPosition].AbsorbtionCoefficient, 2);

            }
            else
            {
                currentIntensity = Math.Pow(previousDistance / currentDistance, 2) * previousIntensity;
            }

            Rays[indexRay].Intensities.Add(currentIntensity);

            previousDistance = currentDistance;
            previousIntensity = currentIntensity;
        }
    }
}
