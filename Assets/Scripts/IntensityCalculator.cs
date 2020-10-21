using System;
using System.Collections.Generic;

public class IntensityCalculator
{
    private Dictionary<int, List<AcousticRay>> Rays
    {
        get;
        set;
    }

    private List<MicrophoneSphere> Microphones
    {
        get;
        set;
    }

    public Dictionary<int, List<double>> Intensities
    {
        get;
        set;
    }

    private double InitialIntensity
    {
        get;
        set;
    }

    public IntensityCalculator(
        Dictionary<int, List<AcousticRay>> rays,
        List<MicrophoneSphere> microphones,
        float initialPower)
    {
        InitialIntensity = initialPower / (4.0 * Math.PI);
        Rays = rays;
        Intensities = new Dictionary<int, List<double>>();
        Microphones = microphones;
    }

    public void ComputePower()
    {
        for (int indexMicro = 0; indexMicro < Rays.Count; ++indexMicro)
        {
            List<double> intensities = new List<double>();
            for (int indexRay = 0; indexRay < Rays[Microphones[indexMicro].Id].Count; ++indexRay)
            {
                intensities.Add(ComputeRayIntensity(Microphones[indexMicro].Id, indexRay));
            }

            Intensities[Microphones[indexMicro].Id] = intensities;
        }
    }

    public void TransformIntensitiesToPressure()
    {
        for (int indexMicro = 0; indexMicro < Rays.Count; ++indexMicro)
        {
            for (int indexRay = 0; indexRay < Rays[Microphones[indexMicro].Id].Count; ++indexRay)
            {
                Intensities[indexMicro][indexRay] = PressureConverter.
                    ConvertIntensityToPressure(Intensities[indexMicro][indexRay]);
            }
        }
    }

    private double ComputeRayIntensity(int indexMicro, int indexRay)
    {
        /*Case for 0 CollisionPoints.*/
        if (Rays[indexMicro][indexRay].CollisionPoints.Count == 0)
        {
            float distance = System.Numerics.Vector3.Distance(
                Rays[indexMicro][indexRay].Source,
                Rays[indexMicro][indexRay].MicrophonePosition);
            return Math.Pow(1 / distance, 2) * InitialIntensity;
        }

        /*Usual case.*/
        float previousDistance = System.Numerics.Vector3.Distance(
             Rays[indexMicro][indexRay].Source,
             Rays[indexMicro][indexRay].CollisionPoints[0]);
        /*I0*/
        double previousIntensity = Math.Pow(1 / previousDistance, 2) *
                                   InitialIntensity *
                                   Math.Pow(1 - Rays[indexMicro][indexRay].AcousticMaterials[0].AbsorbtionCoefficient, 2);

        for (int indexPosition = 1; indexPosition < Rays[indexMicro][indexRay].CollisionPoints.Count; ++indexPosition)
        {
            float currentDistance = previousDistance + System.Numerics.Vector3.Distance(
                 Rays[indexMicro][indexRay].CollisionPoints[indexPosition],
                 Rays[indexMicro][indexRay].CollisionPoints[indexPosition - 1]);
            double currentIntensity = Math.Pow(previousDistance / currentDistance, 2) * previousIntensity *
                                Math.Pow(1 - Rays[indexMicro][indexRay].AcousticMaterials[indexPosition].AbsorbtionCoefficient, 2);

            previousDistance = currentDistance;
            previousIntensity = currentIntensity;
        }

        /*In*/
        int lastPos = Rays[indexMicro][indexRay].CollisionPoints.Count - 1;
        return Math.Pow(previousDistance /
                        (previousDistance + System.Numerics.Vector3.Distance(
                        Rays[indexMicro][indexRay].CollisionPoints[lastPos],
                        Rays[indexMicro][indexRay].MicrophonePosition)), 2) * previousIntensity;
    }
}
