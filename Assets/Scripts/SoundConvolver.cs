﻿using NWaves.Audio;
using NWaves.Operations;
using NWaves.Signals;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class SoundConvolver
{
    public static Dictionary<int, float[]> convolvedSounds
    {
        get;
        set;
    }

    public static double GetMaxFrequency(AudioSource audioSource)
    {
        return audioSource.clip.frequency / 2.0;
    }

    public static void ConvolveSound(
        AudioSource audioSource,
        Dictionary<int, DiscreteSignal> impulseResponses,
        List<MicrophoneSphere> microphones)
    {
        convolvedSounds = new Dictionary<int, float[]>();

        foreach (var microphone in microphones)
        {
            DiscreteSignal discreteSignal;

            // load
            using (var stream = new FileStream(audioSource.clip.name + ".wav", FileMode.Open))
            {
                var waveFile = new WaveFile(stream);
                discreteSignal = waveFile[Channels.Left];
            }

            NormalizeSampleData(discreteSignal);

            var convolutionResult = Operation.Convolve(discreteSignal,
                impulseResponses[microphone.id]);

            NormalizeSampleData(convolutionResult);

            using (var stream = new FileStream("results/convolutionAttention" +
                                               microphone.id.ToString() +
                                               ".wav", FileMode.Create))
            {
                var waveFile = new WaveFile(convolutionResult);
                convolvedSounds[microphone.id] = convolutionResult.Samples;
                waveFile.SaveTo(stream);
            }
        }
    }

    private static void NormalizeSampleData(DiscreteSignal signal)
    {
        var maxi = signal.Samples.Max();

        for (int index = 0; index < signal.Samples.Length; ++index)
            signal.Samples[index] /= maxi;

        for (int index = 0; index < signal.Samples.Length; ++index)
            signal.Samples[index] *= 0.99f;
    }
}
