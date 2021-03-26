using System;
using System.Collections.Generic;
using System.Numerics;

public class PhaseCalculator
{
    private Dictionary<int, List<AcousticRay>> rays
    {
        get;
    }

    private List<MicrophoneSphere> microphones
    {
        get;
    }

    public Dictionary<int, List<Complex>> EchogramMagnitudePhase { get; } = new Dictionary<int, List<Complex>>();

    private Dictionary<int, List<double>> pressures
    {
        get;
    }

    public PhaseCalculator(Dictionary<int, List<AcousticRay>> rays,
        List<MicrophoneSphere> microphones,
        Dictionary<int, List<double>> pressure)
    {
        this.rays = rays;
        pressures = pressure;
        this.microphones = microphones;
    }

    private void SetEchogram()
    {
        for (var indexMicro = 0; indexMicro < pressures.Count; ++indexMicro)
        {
            var values = new List<Complex>();
            for (var indexRay = 0; indexRay < pressures[microphones[indexMicro].Id].Count; ++indexRay)
            {
                values.Add(new Complex(pressures[microphones[indexMicro].Id][indexRay], 0));
            }
            EchogramMagnitudePhase[microphones[indexMicro].Id] = values;
        }
    }

    public Dictionary<int, List<Complex>> ComputePhase(double frequency)
    {
        SetEchogram();
        for (var indexMicro = 0; indexMicro < rays.Count; ++indexMicro)
            for (var indexRay = 0; indexRay < rays[microphones[indexMicro].Id].Count; ++indexRay)
            {
                ComputePhase(microphones[indexMicro].Id, indexRay, frequency);
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

        var distance = rays[indexMicro][indexRay].GetDistance();
        var waveLength = airSoundSpeed / frequency;
        var waveNumber = 2 * Math.PI / waveLength;
        var phase = Math.Atan2(-Math.Sin(waveNumber * distance), Math.Cos(waveNumber * distance));

        var result = Complex.FromPolarCoordinates(EchogramMagnitudePhase[indexMicro][indexRay].Real, phase);

        EchogramMagnitudePhase[indexMicro][indexRay] = result;
    }
}
