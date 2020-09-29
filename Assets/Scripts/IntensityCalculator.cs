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
        double previousIntensity = InitialIntensity;
        float previousDistance = 1;
        Rays[indexRay].Intensities.Add(InitialIntensity);

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
