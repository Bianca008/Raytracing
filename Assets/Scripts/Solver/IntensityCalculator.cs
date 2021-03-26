using System;
using System.Collections.Generic;

public class IntensityCalculator
{
    private Dictionary<int, List<AcousticRay>> rays
    {
        get;
    }

    private List<MicrophoneSphere> microphones
    {
        get;
    }

    public Dictionary<int, List<double>> Intensities { get; set; } = new Dictionary<int, List<double>>();

    private double initialIntensity
    {
        get;
    }

    public IntensityCalculator(
        Dictionary<int, List<AcousticRay>> rays,
        List<MicrophoneSphere> microphones,
        float initialPower)
    {
        initialIntensity = initialPower / (4.0 * Math.PI);
        this.rays = rays;
        this.microphones = microphones;
    }

    public void ComputePower()
    {
        for (int indexMicro = 0; indexMicro < rays.Count; ++indexMicro)
        {
            var intensities = new List<double>();
            for (int indexRay = 0; indexRay < rays[microphones[indexMicro].Id].Count; ++indexRay)
            {
                intensities.Add(ComputeRayIntensity(microphones[indexMicro].Id, indexRay));
            }

            this.Intensities[microphones[indexMicro].Id] = intensities;
        }
    }

    public void TransformIntensitiesToPressure()
    {
        for (int indexMicro = 0; indexMicro < rays.Count; ++indexMicro)
        {
            for (int indexRay = 0; indexRay < rays[microphones[indexMicro].Id].Count; ++indexRay)
            {
                Intensities[microphones[indexMicro].Id][indexRay] = PressureConverter.
                    ConvertIntensityToPressure(Intensities[microphones[indexMicro].Id][indexRay]);
            }
        }
    }

    private double ComputeRayIntensity(int indexMicro, int indexRay)
    {
        /*Case for 0 CollisionPoints.*/
        if (rays[indexMicro][indexRay].CollisionPoints.Count == 0)
        {
            var distance = System.Numerics.Vector3.Distance(
                rays[indexMicro][indexRay].Source,
                rays[indexMicro][indexRay].MicrophonePosition);
            return Math.Pow(1 / distance, 2) * initialIntensity;
        }

        /*Usual case.*/
        var previousDistance = System.Numerics.Vector3.Distance(
             rays[indexMicro][indexRay].Source,
             rays[indexMicro][indexRay].CollisionPoints[0]);
        /*I0*/
        var previousIntensity = Math.Pow(1 / previousDistance, 2) *
                                initialIntensity *
                                Math.Pow(1 - rays[indexMicro][indexRay].AcousticMaterials[0].absorbtionCoefficient, 2);

        for (int indexPosition = 1; indexPosition < rays[indexMicro][indexRay].CollisionPoints.Count; ++indexPosition)
        {
            var currentDistance = previousDistance + System.Numerics.Vector3.Distance(
                 rays[indexMicro][indexRay].CollisionPoints[indexPosition],
                 rays[indexMicro][indexRay].CollisionPoints[indexPosition - 1]);
            var currentIntensity = Math.Pow(previousDistance / currentDistance, 2) * previousIntensity *
                                   Math.Pow(1 - rays[indexMicro][indexRay].AcousticMaterials[indexPosition].absorbtionCoefficient, 2);

            previousDistance = currentDistance;
            previousIntensity = currentIntensity;
        }

        /*In*/
        var lastPos = rays[indexMicro][indexRay].CollisionPoints.Count - 1;
        return Math.Pow(previousDistance /
                        (previousDistance + System.Numerics.Vector3.Distance(
                        rays[indexMicro][indexRay].CollisionPoints[lastPos],
                        rays[indexMicro][indexRay].MicrophonePosition)), 2) * previousIntensity;
    }
}
