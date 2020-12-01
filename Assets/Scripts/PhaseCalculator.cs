using System;
using System.Collections.Generic;
using System.Numerics;

public class PhaseCalculator
{
    private Dictionary<int, List<AcousticRay>> m_rays
    {
        get;
    }

    private List<MicrophoneSphere> m_microphones
    {
        get;
    }

    public Dictionary<int, List<Complex>> echogramMagnitudePhase { get; } = new Dictionary<int, List<Complex>>();

    private Dictionary<int, List<double>> pressures
    {
        get;
    }

    public PhaseCalculator(Dictionary<int, List<AcousticRay>> rays,
        List<MicrophoneSphere> microphones,
        Dictionary<int, List<double>> pressure)
    {
        this.m_rays = rays;
        pressures = pressure;
        this.m_microphones = microphones;
    }

    private void SetEchogram()
    {
        for (int indexMicro = 0; indexMicro < pressures.Count; ++indexMicro)
        {
            var values = new List<Complex>();
            for (int indexRay = 0; indexRay < pressures[m_microphones[indexMicro].id].Count; ++indexRay)
            {
                values.Add(new Complex(pressures[m_microphones[indexMicro].id][indexRay], 0));
            }
            echogramMagnitudePhase[m_microphones[indexMicro].id] = values;
        }
    }

    public Dictionary<int, List<Complex>> ComputePhase(double frequency)
    {
        SetEchogram();
        for (int indexMicro = 0; indexMicro < m_rays.Count; ++indexMicro)
            for (int indexRay = 0; indexRay < m_rays[m_microphones[indexMicro].id].Count; ++indexRay)
            {
                ComputePhase(m_microphones[indexMicro].id, indexRay, frequency);
            }
        return echogramMagnitudePhase;
    }

    private void ComputePhase(int indexMicro, int indexRay, double frequency, double airSoundSpeed = 343.21)
    {
        if(frequency == 0)
        {
            echogramMagnitudePhase[indexMicro][indexRay] = new Complex(echogramMagnitudePhase[indexMicro][indexRay].Real, 0);
            return;
        }

        var distance = m_rays[indexMicro][indexRay].GetDistance();
        var waveLength = airSoundSpeed / frequency;
        var waveNumber = 2 * Math.PI / waveLength;
        var phase = Math.Atan2(-Math.Sin(waveNumber * distance), Math.Cos(waveNumber * distance));

        var result = Complex.FromPolarCoordinates(echogramMagnitudePhase[indexMicro][indexRay].Real, phase);

        echogramMagnitudePhase[indexMicro][indexRay] = result;
    }
}
