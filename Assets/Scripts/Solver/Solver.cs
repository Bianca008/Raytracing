using NWaves.Signals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Echogram = System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<System.Numerics.Complex>>;

public class Solver
{
    public Dictionary<double, Echogram> Echograms { get; set; }

    public List<double> Frequencies { get; set; }

    public Dictionary<int, List<AcousticRay>> Rays { get; set; }

    public List<MicrophoneSphere> Microphones { get; set; }

    public Echogram FrequencyResponse { get; set; }

    public Dictionary<int, DiscreteSignal> ImpulseResponses { get; set; }

    private Vector3 m_sourcePosition;
    private float m_initialPower = 1;
    private int m_numberOfRays = 63000;
    private double m_frequencyStep = 8192.0;
    private double m_maxFrequency = 22050.0;
    private int m_maxDistance = 200;
    private int m_numberOfReflections = 8;
    private RayGeometry m_rayGeometryGenerator;

    public Solver(Vector3 sourcePosition)
    {
        m_sourcePosition = sourcePosition;
        Microphones = new List<MicrophoneSphere>();
        Frequencies = new List<double>();
        Echograms = new Dictionary<double, Echogram>();
        FrequencyResponse = new Dictionary<int, List<Complex>>();
        ImpulseResponses = new Dictionary<int, DiscreteSignal>();
        Rays = new Dictionary<int, List<AcousticRay>>();
    }

    public void RunSolver(String audioName, int numberOfReflections, int numberOfRays, int maxDistance, int frequencyStep)
    {
        if (numberOfReflections == -1 || maxDistance == -1 || audioName == null)
            return;

        m_numberOfReflections = numberOfReflections;
        m_maxDistance = maxDistance;
        m_frequencyStep = frequencyStep;
        m_numberOfRays = numberOfRays;
        CreateRays();
        CreateIntersectedRaysWithMicrophones();
        ComputeIntensities();
        ComputeFrequencyResponse();
        ConvolveSound(audioName);
    }

    public void CreateMicrophones()
    {
        Microphones.Add(new MicrophoneSphere(new System.Numerics.Vector3(2, 1.6f, 1.7f), 0.1f));
        Microphones.Add(new MicrophoneSphere(new System.Numerics.Vector3(-1.5f, 1.2f, 1.7f), 0.1f));
        Microphones.Add(new MicrophoneSphere(new System.Numerics.Vector3(1f, 2f, 13f), 0.1f));
    }

    private void CreateRays()
    {
        m_rayGeometryGenerator = new RayGeometry(m_sourcePosition,
            Microphones,
            m_numberOfRays,
            m_numberOfReflections,
            m_maxDistance);
        m_rayGeometryGenerator.GenerateRays();
    }

    private List<AcousticRay> RemoveDuplicates(List<AcousticRay> rays)
    {
        var indexRay = 0;
        while (indexRay < rays.Count - 1)
        {
            if (rays[indexRay].collisionPoints.Count == rays[indexRay + 1].collisionPoints.Count &&
                rays[indexRay].collisionPoints.Count == 0)
            {
                rays.RemoveAt(indexRay);
            }
            else
            if (Math.Abs(rays[indexRay].GetDistance() - rays[indexRay + 1].GetDistance()) < 1e-2 &&
                rays[indexRay].collisionPoints.Count == rays[indexRay + 1].collisionPoints.Count &&
                rays[indexRay].collisionPoints.Count > 0)
            {
                var size = rays[indexRay].collisionPoints.Count;
                var indexPointCompared = 0;
                var ok = false;
                while (indexPointCompared < size && ok == false)
                {
                    double distance = System.Numerics.Vector3.Distance
                    (rays[indexRay].collisionPoints[indexPointCompared],
                        rays[indexRay + 1].collisionPoints[indexPointCompared]);
                    if (distance < 0.06 * rays[indexRay].GetDistance())
                    {
                        ok = true;
                        rays.RemoveAt(indexRay);
                    }

                    ++indexPointCompared;
                }
                if (ok == false)
                    ++indexRay;
            }
            else
                ++indexRay;
        }

        return rays;
    }

    private void CreateIntersectedRaysWithMicrophones()
    {
        Rays.Clear();
        foreach (var microphone in Microphones)
        {
            var newRays = m_rayGeometryGenerator.GetIntersectedRays(microphone);

            newRays.Sort((first, second) => first.GetDistance().CompareTo(second.GetDistance()));
            var raysWithoutDuplicates = RemoveDuplicates(newRays);
            Rays[microphone.id] = raysWithoutDuplicates;
        }

        var count = 0;
        for (int index = 0; index < Rays.Count; ++index)
            count += Rays[index].Count;
    }

    private void ComputeIntensities()
    {
        var intensityCalculator = new IntensityCalculator(Rays, Microphones, m_initialPower);
        intensityCalculator.ComputePower();
        intensityCalculator.TransformIntensitiesToPressure();
        var intensities = intensityCalculator.intensities;

        Frequencies.Clear();
        for (double index = 0; index < m_maxFrequency; index += m_maxFrequency / m_frequencyStep)
            Frequencies.Add(index);

        Echograms.Clear();
        foreach (var frequency in Frequencies)
        {
            var phaseCalculator = new PhaseCalculator(Rays, Microphones, intensities);
            Echograms[frequency] = phaseCalculator.ComputePhase(frequency);
        }
    }

    private void ComputeFrequencyResponse()
    {
        FrequencyResponse.Clear();

        foreach (var microphone in Microphones)
        {
            var values = new List<Complex>();
            for (int indexFrequency = 0; indexFrequency < Frequencies.Count; ++indexFrequency)
                if (Echograms[Frequencies[indexFrequency]][microphone.id].Count != 0)
                {
                    var sumi = Echograms[Frequencies[indexFrequency]][microphone.id].
                        Aggregate((s, a) => s + a);
                    values.Add(sumi);
                }
            FrequencyResponse[microphone.id] = values;
        }
    }

    private void ConvolveSound(String audioName)
    {
        ImpulseResponses.Clear();

        foreach (var freqResponse in FrequencyResponse)
        {
            ImpulseResponses[freqResponse.Key] = ImpulseResponseTranformer.Transform(freqResponse.Value);
        }

        SoundConvolver.ConvolveSound(audioName, ImpulseResponses, Microphones);
    }

    public void ResetSolver()
    {
        Echograms.Clear();
        Frequencies.Clear();
        Rays.Clear();
        Microphones.Clear();
        FrequencyResponse.Clear();
        ImpulseResponses.Clear();
        m_rayGeometryGenerator.rays.Clear();
    }
}
