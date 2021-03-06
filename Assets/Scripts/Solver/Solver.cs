﻿using NWaves.Signals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine.SceneManagement;
using Echogram = System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<System.Numerics.Complex>>;

public class Solver
{
    public Dictionary<double, Echogram> Echograms { get; set; }

    public List<double> Frequencies { get; set; }

    public Dictionary<int, List<AcousticRay>> Rays { get; set; }

    public List<MicrophoneSphere> Microphones { get; set; }

    public Echogram FrequencyResponse { get; set; }

    public Dictionary<int, DiscreteSignal> ImpulseResponses { get; set; }

    private Vector3 sourcePosition;
    private float initialPower = 1;
    private int numberOfRays = 63000;
    private double frequencyStep = 1024.0;
    private double maxFrequency = 22050.0;
    private int maxDistance = 200;
    private int numberOfReflections = 8;
    private RayGeometry rayGeometryGenerator;

    public Solver(Vector3 sourcePosition)
    {
        this.sourcePosition = sourcePosition;
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

        this.numberOfReflections = numberOfReflections;
        this.maxDistance = maxDistance;
        this.frequencyStep = frequencyStep;
        this.numberOfRays = numberOfRays;
        CreateRays();
        CreateIntersectedRaysWithMicrophones();
        ComputeIntensities();
        ComputeFrequencyResponse();
        ConvolveSound(audioName);
    }

    public void CreateMicrophones()
    {
        if (SceneManager.GetActiveScene().name == "RoomDScene")
        {
            float x = 0;
            float y = 0;
            float z = 0f;
            int n = 8;
            float r = 4f;
            for (int i = 0; i < n; i++)
            {
                Microphones.Add(new MicrophoneSphere(
                    new System.Numerics.Vector3((float)(x + r * Math.Cos(2 * i * Math.PI / n)),
                        y,
                        (float)(z + r * Math.Sin(2 * i * Math.PI / n))), 0.1f));

            }
        }
        else
        {
            Microphones.Add(new MicrophoneSphere(new System.Numerics.Vector3(2, 1.6f, 1.7f), 0.1f));
            Microphones.Add(new MicrophoneSphere(new System.Numerics.Vector3(-1.5f, 1.2f, 1.7f), 0.1f));
            if (SceneManager.GetActiveScene().name != "RoomAscene" && SceneManager.GetActiveScene().name != "RoomDScene")
                Microphones.Add(new MicrophoneSphere(new System.Numerics.Vector3(1f, 2f, 13f), 0.1f));
        }
    }

    private void CreateRays()
    {
        rayGeometryGenerator = new RayGeometry(sourcePosition,
            Microphones,
            numberOfRays,
            numberOfReflections,
            maxDistance);
        rayGeometryGenerator.GenerateRays();
    }

    private List<AcousticRay> RemoveDuplicates(List<AcousticRay> rays)
    {
        var indexRay = 0;
        while (indexRay < rays.Count - 1)
        {
            if (rays[indexRay].CollisionPoints.Count == rays[indexRay + 1].CollisionPoints.Count &&
                rays[indexRay].CollisionPoints.Count == 0)
            {
                rays.RemoveAt(indexRay);
            }
            else
            if (Math.Abs(rays[indexRay].GetDistance() - rays[indexRay + 1].GetDistance()) < 1e-2 &&
                rays[indexRay].CollisionPoints.Count == rays[indexRay + 1].CollisionPoints.Count &&
                rays[indexRay].CollisionPoints.Count > 0)
            {
                var size = rays[indexRay].CollisionPoints.Count;
                var indexPointCompared = 0;
                var ok = false;
                while (indexPointCompared < size && ok == false)
                {
                    double distance = System.Numerics.Vector3.Distance
                    (rays[indexRay].CollisionPoints[indexPointCompared],
                        rays[indexRay + 1].CollisionPoints[indexPointCompared]);
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
            var newRays = rayGeometryGenerator.GetIntersectedRays(microphone);
            newRays.Sort((first, second) => first.GetDistance().CompareTo(second.GetDistance()));
            var raysWithoutDuplicates = RemoveDuplicates(newRays);
            Rays[microphone.Id] = raysWithoutDuplicates;
        }

        var count = 0;
        for (int index = 0; index < Rays.Count; ++index)
            count += Rays[index].Count;
    }

    private void ComputeIntensities()
    {
        var intensityCalculator = new IntensityCalculator(Rays, Microphones, initialPower);
        intensityCalculator.ComputePower();
        intensityCalculator.TransformIntensitiesToPressure();
        var intensities = intensityCalculator.Intensities;

        Frequencies.Clear();
        for (double index = 0; index < maxFrequency; index += maxFrequency / frequencyStep)
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
                if (Echograms[Frequencies[indexFrequency]][microphone.Id].Count != 0)
                {
                    var sumi = Echograms[Frequencies[indexFrequency]][microphone.Id].
                        Aggregate((s, a) => s + a);
                    values.Add(sumi);
                }
            FrequencyResponse[microphone.Id] = values;
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
        SoundConvolver.ConvolvedSounds.Clear();
        Echograms.Clear();
        Frequencies.Clear();
        Rays.Clear();
        Microphones.Clear();
        FrequencyResponse.Clear();
        ImpulseResponses.Clear();
        rayGeometryGenerator.Rays.Clear();
    }
}
