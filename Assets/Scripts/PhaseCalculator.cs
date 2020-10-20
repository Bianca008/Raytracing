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

    public Dictionary<int, List<Complex>> Echogram
    {
        get;
        set;
    }

    public PhaseCalculator(Dictionary<int, List<AcousticRay>> rays,
        List<MicrophoneSphere> microphones,
        Dictionary<int, List<Complex>> pressure)
    {
        Rays = rays;
        Echogram = pressure;
        Microphones = microphones;
    }

    public void ComputePhase(int frequency)
    {
        for (int indexMicro = 0; indexMicro < Rays.Count; ++indexMicro)
            for (int indexRay = 0; indexRay < Rays[Microphones[indexMicro].Id].Count; ++indexRay)
            {
                ComputePhase(Microphones[indexMicro].Id, indexRay, frequency);
            }
    }

    private void ComputePhase(int indexMicro, int indexRay, int frequency, double airSoundSpeed = 343.21)
    {
        float distance = Rays[indexMicro][indexRay].Distance;
        double waveLength = airSoundSpeed / frequency;
        double waveNumber = 2 * Math.PI / waveLength;
        double phase = Math.Atan2(-Math.Sin(waveNumber * distance), Math.Cos(waveNumber * distance));

        Complex result = Complex.FromPolarCoordinates(Echogram[indexMicro][indexRay].Real, phase);
        
        Echogram[indexMicro][indexRay] = result;
    }
}
