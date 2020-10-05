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
        if (Rays[indexRay].ColissionPoints.Count == 0)
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
            Rays[indexRay].ColissionPoints[0]);
        double previousIntensity = Math.Pow(1 / previousDistance, 2) * InitialIntensity;
        /*I0*/
        Rays[indexRay].Intensities.Add(previousIntensity);

        for (int indexPosition = 1; indexPosition < Rays[indexRay].ColissionPoints.Count; ++indexPosition)
        {
            float currentDistance = previousDistance + System.Numerics.Vector3.Distance(
                Rays[indexRay].ColissionPoints[indexPosition],
                Rays[indexRay].ColissionPoints[indexPosition - 1]);
            double currentIntensity = Math.Pow(previousDistance / currentDistance, 2) * previousIntensity;
            Rays[indexRay].Intensities.Add(currentIntensity);

            previousDistance = currentDistance;
            previousIntensity = currentIntensity;
        }
    }
}
