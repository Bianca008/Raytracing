using System;
using System.Collections.Generic;

public class IntensityCalculator
{
    private Dictionary<int, List<AcousticRay>> m_rays
    {
        get;
    }

    private List<MicrophoneSphere> m_microphones
    {
        get;
    }

    public Dictionary<int, List<double>> intensities { get; set; } = new Dictionary<int, List<double>>();

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
        this.m_rays = rays;
        this.m_microphones = microphones;
    }

    public void ComputePower()
    {
        for (int indexMicro = 0; indexMicro < m_rays.Count; ++indexMicro)
        {
            var intensities = new List<double>();
            for (int indexRay = 0; indexRay < m_rays[m_microphones[indexMicro].id].Count; ++indexRay)
            {
                intensities.Add(ComputeRayIntensity(m_microphones[indexMicro].id, indexRay));
            }

            this.intensities[m_microphones[indexMicro].id] = intensities;
        }
    }

    public void TransformIntensitiesToPressure()
    {
        for (int indexMicro = 0; indexMicro < m_rays.Count; ++indexMicro)
        {
            for (int indexRay = 0; indexRay < m_rays[m_microphones[indexMicro].id].Count; ++indexRay)
            {
                intensities[m_microphones[indexMicro].id][indexRay] = PressureConverter.
                    ConvertIntensityToPressure(intensities[m_microphones[indexMicro].id][indexRay]);
            }
        }
    }

    private double ComputeRayIntensity(int indexMicro, int indexRay)
    {
        /*Case for 0 CollisionPoints.*/
        if (m_rays[indexMicro][indexRay].collisionPoints.Count == 0)
        {
            var distance = System.Numerics.Vector3.Distance(
                m_rays[indexMicro][indexRay].source,
                m_rays[indexMicro][indexRay].microphonePosition);
            return Math.Pow(1 / distance, 2) * initialIntensity;
        }

        /*Usual case.*/
        var previousDistance = System.Numerics.Vector3.Distance(
             m_rays[indexMicro][indexRay].source,
             m_rays[indexMicro][indexRay].collisionPoints[0]);
        /*I0*/
        var previousIntensity = Math.Pow(1 / previousDistance, 2) *
                                initialIntensity *
                                Math.Pow(1 - m_rays[indexMicro][indexRay].acousticMaterials[0].AbsorbtionCoefficient, 2);

        for (int indexPosition = 1; indexPosition < m_rays[indexMicro][indexRay].collisionPoints.Count; ++indexPosition)
        {
            var currentDistance = previousDistance + System.Numerics.Vector3.Distance(
                 m_rays[indexMicro][indexRay].collisionPoints[indexPosition],
                 m_rays[indexMicro][indexRay].collisionPoints[indexPosition - 1]);
            var currentIntensity = Math.Pow(previousDistance / currentDistance, 2) * previousIntensity *
                                   Math.Pow(1 - m_rays[indexMicro][indexRay].acousticMaterials[indexPosition].AbsorbtionCoefficient, 2);

            previousDistance = currentDistance;
            previousIntensity = currentIntensity;
        }

        /*In*/
        var lastPos = m_rays[indexMicro][indexRay].collisionPoints.Count - 1;
        return Math.Pow(previousDistance /
                        (previousDistance + System.Numerics.Vector3.Distance(
                        m_rays[indexMicro][indexRay].collisionPoints[lastPos],
                        m_rays[indexMicro][indexRay].microphonePosition)), 2) * previousIntensity;
    }
}
