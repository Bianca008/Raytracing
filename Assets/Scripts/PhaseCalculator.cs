using System;
using System.Collections.Generic;
using System.Numerics;

public class PhaseCalculator
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

    private Dictionary<int, List<Complex>> EchogramMagnitudePhase
    {
        get;
        set;
    }

    private Dictionary<int, List<double>> pressures
    {
        get;
        set;
    }

    public PhaseCalculator(Dictionary<int, List<AcousticRay>> rays,
        List<MicrophoneSphere> microphones,
        Dictionary<int, List<double>> pressure)
    {
        Rays = rays;
        pressures = pressure;
        Microphones = microphones;
        EchogramMagnitudePhase = new Dictionary<int, List<Complex>>();
    }

    private void SetEchogram()
    {
        for (int indexMicro = 0; indexMicro < pressures.Count; ++indexMicro)
        {
            List<Complex> values = new List<Complex>();
            for (int indexRay = 0; indexRay < pressures[indexMicro].Count; ++indexRay)
            {
                values.Add(new Complex(pressures[Microphones[indexMicro].Id][indexRay], 0));
            }
            EchogramMagnitudePhase[indexMicro] = values;
        }
    }

    public Dictionary<int, List<Complex>> ComputePhase(double frequency)
    {
        SetEchogram();
        for (int indexMicro = 0; indexMicro < Rays.Count; ++indexMicro)
            for (int indexRay = 0; indexRay < Rays[Microphones[indexMicro].Id].Count; ++indexRay)
            {
                ComputePhase(Microphones[indexMicro].Id, indexRay, frequency);
            }
        return EchogramMagnitudePhase;
    }

    private void ComputePhase(int indexMicro, int indexRay, double frequency, double airSoundSpeed = 343.21)
    {
        if(frequency == 0)
        {
            EchogramMagnitudePhase[indexMicro][indexRay] = new Complex(EchogramMagnitudePhase[indexMicro][indexRay].Real, 0);
            return;
        }

        float distance = Rays[indexMicro][indexRay].Distance;
        double waveLength = airSoundSpeed / frequency;
        double waveNumber = 2 * Math.PI / waveLength;
        double phase = Math.Atan2(-Math.Sin(waveNumber * distance), Math.Cos(waveNumber * distance));

        Complex result = Complex.FromPolarCoordinates(EchogramMagnitudePhase[indexMicro][indexRay].Real, phase);

        EchogramMagnitudePhase[indexMicro][indexRay] = result;
    }
}
